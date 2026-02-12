
---

# Booking API – Technical Documentation

## 1. Overview

This project implements a **home availability search API** using a layered architecture:

* **Domain Layer** – Core business rules
* **Application Layer** – Use cases and abstractions
* **Infrastructure Layer** – Repository implementation
* **API Layer** – HTTP endpoints and DTOs
* **Integration Tests** – End-to-end verification

The system allows clients to query which homes are available for a given date range.

---

# 2. Architecture Overview

```
API Layer
   ↓
Application Layer (Use Cases)
   ↓
Domain Layer (Entities)
   ↓
Infrastructure Layer (Repositories)
```

### Design Principles Used

* Separation of concerns
* Clean Architecture concepts
* Dependency inversion (repository abstraction)
* Deterministic test seeding
* Index-based performance optimization

---

# 3. Domain Layer

## `Home` Entity

```csharp
namespace Booking.Domain.Entities;
public sealed class Home
```

### Responsibility

Represents a rentable home and encapsulates availability logic.

### Properties

| Property         | Type                | Description             |
| ---------------- | ------------------- | ----------------------- |
| `Id`             | `int`               | Unique home identifier  |
| `Name`           | `string`            | Display name            |
| `AvailableSlots` | `HashSet<DateOnly>` | Available booking dates |

### Why `HashSet<DateOnly>`?

Fact:

* `HashSet` provides O(1) lookup time for date checks.

Interpretation:

* This makes availability checks efficient even for large date ranges.

---

## Methods

### `IsAvailableFor(DateOnly startDate, DateOnly endDate)`

Checks whether **every date** in the range is available.

Algorithm:

1. Iterate day by day from startDate to endDate.
2. If any date is missing → return false.
3. Otherwise → return true.

Time Complexity:

```
O(N)
Where N = number of days in the range
```

---

### `GetAvailableInRange(DateOnly startDate, DateOnly endDate)`

Returns available dates inside the range.

* Uses `yield return`
* Only returns dates that exist in `AvailableSlots`

---

# 4. Application Layer

## `IHomeRepository`

```csharp
public interface IHomeRepository
```

### Responsibility

Abstracts the data source for retrieving available homes.

```csharp
Task<IReadOnlyCollection<Home>> GetAvailableAsync(DateOnly startDate, DateOnly endDate);
```

This supports:

* In-memory implementation (current)
* Database-backed implementation (future)

---

## `GetAvailableHomesUseCase`

```csharp
public class GetAvailableHomesUseCase
```

### Responsibility

Encapsulates business logic for retrieving available homes.

### Flow

1. Validate date range

    * If startDate > endDate → throw `BadHttpRequestException`
2. Call repository
3. Double-check availability using domain logic
4. Return filtered result

### Important Detail

The repository already intersects dates using an index, but:

Fact:

* The use case still verifies availability using `home.IsAvailableFor`.

Reason:

* Defensive validation in case repository implementation changes.

Opinion:

* This makes the system safer but slightly redundant.

---

# 5. Infrastructure Layer

## `InMemoryHomeRepository`

```csharp
public sealed class InMemoryHomeRepository : IHomeRepository
```

### Purpose

Provides an optimized in-memory implementation using:

* Dictionary for homes
* Inverted index (Date → Home IDs)

---

## Data Structures

```csharp
Dictionary<int, Home> Homes
Dictionary<DateOnly, HashSet<int>> DateIndex
```

### Why DateIndex?

Fact:

* Instead of scanning all homes, the repository intersects home sets per date.

Example:
For range July 15–17:

```
DateIndex[15] = {1,2,3}
DateIndex[16] = {1,3}
DateIndex[17] = {2,3,4}

Intersection:
{1,2,3}
∩ {1,3}
= {1,3}

∩ {2,3,4}
= {3}
```

Only Home 3 remains.

### Performance

Without index:

```
O(H × D)
```

With index:

```
O(D × S)
Where S = average set size
```

This is significantly more scalable.

---

## Deterministic Seeding

`SeedDeterministicHomes()` ensures predictable test behavior.

Seeded homes:

| ID | Name            | Available Dates |
| -- | --------------- | --------------- |
| 1  | Valid Home      | 15,16           |
| 2  | Invalid Home    | 15,17           |
| 3  | Extra Days Home | 15,16,17,18     |
| 4  | Other Dates     | 17              |
| 5  | Invalid Home 2  | 18,20,21        |

This enables precise integration testing.

---

# 6. API Layer

## Endpoint

```csharp
GET /api/available-homes
```

Query parameters:

* `startDate`
* `endDate`

### Flow

1. Parameters bound automatically
2. Use case executed
3. Domain models mapped to DTOs
4. Return `AvailableHomeResponse`

---

## Response Model

```csharp
public record AvailableHomeResponse(string Status, IReadOnlyCollection<HomeDto> Homes);
```

```csharp
public record HomeDto(int HomeId, string HomeName, IReadOnlyCollection<string> AvailableSlots);
```

Example JSON:

```json
{
  "status": "OK",
  "homes": [
    {
      "homeId": 3,
      "homeName": "Extra Days Home",
      "availableSlots": ["2026-07-15","2026-07-16"]
    }
  ]
}
```

---

# 7. Testing Strategy

Testing is done using:

* `WebApplicationFactory`
* Real HTTP calls
* In-memory repository override
* FluentAssertions

These are **integration tests**, not unit tests.

---

## Test Cases Explained

### 1. Returns OK

Verifies:

* Endpoint responds with 200

---

### 2. StartDateAfterEndDate

Expected:

* `BadRequest`


---

### 3. ReturnsAvailableHomes

Verifies:

* Non-empty result for valid range

---

### 4. ReturnsEmptyList_WhenNoHomesAvailable

Range: 18–20 July

Intersection results in empty set.

Verifies:

* API returns 200 with empty collection.

---

### 5. Returns3Homes_When3HomesAvailable

On July 17:

Available homes:

* 2
* 3
* 4

Confirms:

* Index logic works correctly.

---

# 8. Testing Quality Assessment

### What is Good

* Real HTTP testing
* Deterministic data
* No mocking
* Repository replaced cleanly
* Good edge-case coverage

### What Is Missing

1. No unit tests for:

    * `Home.IsAvailableFor`
    * Repository intersection logic

2. No boundary testing:

    * Single-day ranges
    * Very large ranges
    * Non-existent dates

3. No performance testing

---

# 9. How I Would Extend Testing

## Add Unit Tests

### Home Entity

Test:

* Full range available
* Partial missing date
* Single-day range

---

### Repository

Test:

* Date not in index
* Intersect results shrink correctly
* Multiple-day intersection

---

## Add Edge Case Tests

* Same start and end date
* Large date ranges
* Invalid query formats
* Missing parameters

---

## Add Contract Tests

Verify JSON schema:

* Field casing
* Date format correctness
* Status field presence

---

# 10. Summary

This system:

* Uses clean layered architecture
* Implements optimized date intersection
* Separates business logic from infrastructure
* Uses deterministic in-memory data for integration tests
* Validates availability twice (repository + domain)

Performance:

* Efficient for moderate datasets
* Easily extendable to database-backed repository

Testing:

* Strong integration coverage
* Needs more focused unit tests for deeper validation

---

If you want, I can next:

* Generate a UML diagram
* Add performance complexity analysis in more detail
* Refactor this into formal README.md documentation
* Suggest improvements for production readiness
