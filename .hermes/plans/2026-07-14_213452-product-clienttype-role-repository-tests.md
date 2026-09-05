# Product, ClientType, and Role Repository Integration Tests Implementation Plan

> **For Hermes:** Use subagent-driven-development skill to implement this plan task-by-task.

**Goal:** Add a minimal, valid, non-redundant set of real SQL Server repository integration tests for Product, Role, and ClientType on `market-develop-Saba`.

**Architecture:** Tests will use the existing NUnit `BaseRepositoryTests` fixture, real `MarketDB_Test`, Dapper/repositories, and deployed SSDT stored procedures. Each test will target one reachable behavior at the layer that owns it; direct parameterized SQL will be used for arrangement/verification so one broken repository method cannot create a false result for another.

**Tech Stack:** .NET 8, C#, NUnit 4, Dapper, Microsoft.Data.SqlClient, SQL Server, SSDT/MSBuild, SqlPackage.

---

## 1. Confirmed current state

- Branch: `market-develop-Saba`
- Base commit: `a7d2c3b`
- Existing uncommitted work must be preserved:
  - 11 new Product/Role/ClientType procedure files
  - `Market.Database.sqlproj` additions for those files
- `ProductRepositoryTests`, `RoleRepositoryTests`, and `ClientTypeRepositoryTests` do not exist yet.
- The test fixture clears and reseeds the real test database before every test.
- `Server=.` in `Market.Tests/appsettings.test.json` is currently unreachable.
- Confirmed working SQL target:
  - server: `.\GV1MRA`
  - database: `MarketDB_Test`
- None of the 11 newly added procedures are currently deployed in `MarketDB_Test`.

Do not reset the working tree. Do not commit or push unless the user separately requests it.

---

## 2. Rules for deciding whether a test belongs

A test belongs only when all of the following are true:

1. The input can actually reach the validation/behavior being tested.
2. The behavior is part of the current repository, procedure, table constraint, or an explicit requirement.
3. It exercises a distinct branch or database contract not already proved by another test.
4. It can assert the intended failure precisely enough to avoid false positives.

### Required assertion rules

- Foreign-key violation: assert SQL error `547`.
- Unique violation: assert SQL error `2601` or `2627`.
- Repository guard: assert the exact CLR exception type.
- Soft delete: query `IsDeleted` directly and assert it is `1`.
- Do not accept an arbitrary `SqlException`; missing procedures and wrong parameter names also throw `SqlException`.

### Test isolation rules

- Use a fresh GUID-suffixed name for insert/update/delete arrangements.
- Arrange update/delete rows with direct parameterized SQL.
- Verify insert/update/delete state with direct parameterized SQL.
- Use seeded rows only for read-only lookup tests where IDs/names are deterministic.
- Never make an update test depend on repository Insert, or a delete test depend on repository GetById.

---

## 3. Minimal approved test matrix

### Product — 16 tests

Create: `G11_Market/Market.Tests/ProductRepositoryTests.cs`

| # | Test name | Behavior proved |
|---:|---|---|
| 1 | `Insert_ShouldReturnIdAndCreateActiveProduct` | Valid insert, output ID, field mapping, `IsDeleted = 0` |
| 2 | `Insert_WithUnknownCategory_ShouldThrowForeignKeyViolation` | Product → Category FK; assert SQL `547` |
| 3 | `Insert_WithDuplicateName_ShouldThrowUniqueViolation` | Unique ProductName; assert `2601` or `2627` |
| 4 | `Update_ShouldChangeProductFields` | Correct `@Id`/DTO parameter contract and update behavior |
| 5 | `Delete_ShouldSoftDeleteProduct` | Delete sets `IsDeleted = 1` and does not remove the row |
| 6 | `GetById_ShouldReturnActiveProduct` | `sp_GetProductById` exists and maps an active row |
| 7 | `GetAll_ShouldReturnOnlyActiveProducts` | Active-only list behavior |
| 8 | `GetByName_ShouldReturnMatchingActiveProduct` | Custom exact-name lookup |
| 9 | `GetByName_WhenMissing_ShouldReturnNull` | Defined not-found behavior |
| 10 | `GetByName_WhenWhitespace_ShouldThrowArgumentException` | Explicit whitespace guard |
| 11 | `GetByName_ShouldExcludeDeletedProduct` | Search soft-delete filter |
| 12 | `GetByCategoryId_ShouldReturnOnlyActiveProductsInCategory` | Category filtering plus soft-delete filtering |
| 13 | `GetByCategoryId_WhenZero_ShouldThrowArgumentOutOfRangeException` | The `categoryId <= 0` branch, tested once |
| 14 | `GetByPriceRange_ShouldIncludeBothBoundaries` | Inclusive minimum and maximum |
| 15 | `GetByPriceRange_WhenMinimumNegative_ShouldThrowArgumentOutOfRangeException` | Explicit negative-minimum branch |
| 16 | `GetByPriceRange_WhenMaximumBelowMinimum_ShouldThrowArgumentException` | Explicit invalid-range-order branch |

### Role — 10 tests

Create: `G11_Market/Market.Tests/RoleRepositoryTests.cs`

| # | Test name | Behavior proved |
|---:|---|---|
| 1 | `Insert_ShouldCreateRoleWithNullDescription` | Valid insert, output ID, and nullable Description |
| 2 | `Insert_WithDuplicateName_ShouldThrowUniqueViolation` | Unique role name; assert `2601` or `2627` |
| 3 | `Update_ShouldChangeRoleFields` | Valid update and `UpdateDate` behavior |
| 4 | `Delete_ShouldSoftDeleteRole` | Soft delete sets `IsDeleted = 1` |
| 5 | `GetById_ShouldReturnActiveRole` | Stored-procedure lookup and DTO mapping |
| 6 | `GetAll_ShouldReturnOnlyActiveRoles` | Active-only list behavior |
| 7 | `GetByName_ShouldReturnMatchingActiveRole` | Custom exact-name lookup |
| 8 | `GetByName_WhenMissing_ShouldReturnNull` | Defined not-found behavior |
| 9 | `GetByName_WhenWhitespace_ShouldThrowArgumentException` | One test for null/empty/whitespace guard family |
| 10 | `GetByName_ShouldExcludeDeletedRole` | Search soft-delete filtering |

### ClientType — 6 tests

Create: `G11_Market/Market.Tests/ClientTypeRepositoryTests.cs`

| # | Test name | Behavior proved |
|---:|---|---|
| 1 | `Insert_ShouldCreateClientTypeWithNullDescription` | Valid insert, output ID, nullable Description |
| 2 | `Insert_WithDuplicateName_ShouldThrowUniqueViolation` | Unique ClientType name; assert `2601` or `2627` |
| 3 | `Update_ShouldChangeClientTypeFields` | Valid update |
| 4 | `Delete_ShouldSoftDeleteClientType` | Soft delete sets `IsDeleted = 1` |
| 5 | `GetById_ShouldReturnActiveClientType` | Generic lookup and DTO mapping |
| 6 | `GetAll_ShouldReturnOnlyActiveClientTypes` | Active-only list behavior |

Do not add ClientType `GetByName` tests unless a product requirement first adds `GetByName` to the repository contract.

**Approved core total: 32 tests.**

---

## 4. Tests that must not be written

### Impossible at the normal C# call site

Do not write tests assigning:

- `Product.CategoryId = null`
- `Product.Price = null`
- `Product.Id = null`
- `Role.Id = null`
- `ClientType.Id = null`
- strings such as `"abc"` to numeric DTO properties

Those inputs cannot be represented by the current non-nullable value types, so they cannot reach the repository or SQL validation being claimed.

### Generic inherited behavior already covered elsewhere

Do not repeat these in all three fixtures:

- `GetById(null)`
- `Update(null)`
- `Delete(null)`

They execute the same `BaseRepository<T>` guards for every entity. If the team wants explicit coverage, add one focused base-repository guard fixture rather than nine repeated entity tests.

### Valid values that must not be treated as errors

- `Role.Description = null`
- `ClientType.Description = null`

These columns are nullable. Prove them in successful insert tests.

### Caller-controlled values that do not reach insert/update

Do not write caller-input validation tests for generated/state fields excluded by DTO attributes:

- identity ID
- creation date
- update date
- deletion flag

### Unsupported business rules

Do not expect rejection for these unless the specification/schema is changed first:

- negative Product insertion price — no table CHECK constraint currently rejects it
- blank Product/Role/ClientType insertion name — the tables reject `NULL`, but currently allow whitespace
- positive nonexistent category returning an exception from `GetByCategoryId` — the current search contract returns an empty result

### Duplicate branch coverage

Do not add separate tests for:

- null, empty, and whitespace `GetByName` values; one whitespace test covers the same guard
- zero and negative category IDs; one zero test covers the same `<= 0` branch
- every possible empty price range; the inclusive and invalid-order tests cover the distinct logic
- duplicate name on both insert and update unless the assignment explicitly asks for both; the same unique table constraint owns both paths

---

## 5. Valid but optional tests not included in the core 32

These inputs are reachable, but they are not needed for the minimal non-redundant assignment:

- Required string set to `null!`, asserting SQL error `515`. Nullable-reference annotations are compile-time warnings, not runtime guards, so this is technically reachable. Add only if explicit NOT NULL constraint coverage is required.
- Updating or deleting a nonexistent ID. The new procedures have error paths, but the core CRUD tests already cover the primary contract.
- Duplicate-name update. It repeats the same database unique constraint covered by duplicate insert.
- Maximum string-length behavior. Add only after defining whether overlength values should be rejected or normalized.

---

## 6. Production prerequisites before the tests can be green

### 6.1 Product contract corrections

Modify:

- `G11_Market/Market.DTO/ProductDTO.cs`
- `G11_Market/Market.Database/dbo/Tables/Products.sql`
- `G11_Market/Market.Database/dbo/Stored Procedures/sp_InsertProduct.sql`
- `G11_Market/Market.Database/dbo/Stored Procedures/sp_UpdateProduct.sql`
- `G11_Market/Market.Database/dbo/Stored Procedures/sp_DeleteProduct.sql`

Required corrections:

1. Change the Products default from `IsDeleted = 1` to `IsDeleted = 0`.
2. Add `[IgnoreOnUpdate]` to `ProductDTO.IsDeleted`.
3. Add `[IgnoreOnUpdate]` to `ProductDTO.UpdatedDate`.
4. Change `sp_InsertProduct` to accept `@Id INT OUTPUT`, insert an active product, and assign `SCOPE_IDENTITY()` to `@Id`.
5. Change `sp_UpdateProduct` from `@ProductId` to `@Id`, update only active rows, and let SQL set `UpdatedDate`.
6. Change `sp_DeleteProduct` from `@ProductId` to `@Id`, set `IsDeleted = 1`, and update only active rows.

Expected repository parameters after DTO correction:

```text
Insert: CategoryId, ProductName, Price, Id OUTPUT
Update: Id, CategoryId, ProductName, Price
Delete: Id
```

### 6.2 Role nullability correction

Modify:

- `G11_Market/Market.Services/Interfaces/Repositories/IRoleRepository.cs`
- `G11_Market/Market.Repositories/RoleRepository.cs`

Change `GetByName` from `RoleDTO` to `RoleDTO?`, because `FirstOrDefault()` legitimately returns null. This aligns the public contract with the missing-role test and removes the existing nullable-return warning.

### 6.3 Add the missing ClientType C# repository contract

Create:

- `G11_Market/Market.DTO/ClientTypeDTO.cs`
- `G11_Market/Market.Services/Interfaces/Repositories/IClientTypeRepository.cs`
- `G11_Market/Market.Repositories/ClientTypeRepository.cs`

Modify:

- `G11_Market/Market.Services/Interfaces/IUnitOfWork.cs`
- `G11_Market/Market.Repositories/UnitOfWork.cs`

DTO shape:

```csharp
using Market.Extensions.Attributes;

namespace Market.DTO;

public sealed class ClientTypeDTO
{
    [IgnoreOnInsert]
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public bool IsDeleted { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime CreateDate { get; set; }

    [IgnoreOnInsert]
    [IgnoreOnUpdate]
    public DateTime? UpdateDate { get; set; }
}
```

Interface and repository:

```csharp
public interface IClientTypeRepository : IBaseRepository<ClientTypeDTO>
{
}
```

```csharp
internal sealed class ClientTypeRepository(DbConnection connection)
    : BaseRepository<ClientTypeDTO>(connection), IClientTypeRepository
{
}
```

Add `ClientTypeRepository` to `IUnitOfWork`, initialize a `Lazy<ClientTypeRepository>` in `UnitOfWork`, and expose it through `GetRepository(...)`.

`ClientType` pluralizes to `ClientTypes`, so the generic base repository will call the already-added exact names:

```text
sp_GetClientTypeById
sp_GetAllClientTypes
sp_InsertClientType
sp_UpdateClientType
sp_DeleteClientType
```

### 6.4 Make the local test connection overridable

Modify:

- `G11_Market/Market.Tests/Helpers/ConfigurationManager.cs`

Prefer an environment override without hardcoding Saba's SQL instance into shared source:

```csharp
public static string ConnectionString =>
    Environment.GetEnvironmentVariable("MARKET_TEST_CONNECTION_STRING")
    ?? Configuration.GetConnectionString("MarketDb")
    ?? throw new InvalidOperationException(
        "Connection string 'MarketDb' was not found.");
```

Run local tests with:

```bash
export MARKET_TEST_CONNECTION_STRING='Server=.\GV1MRA;Database=MarketDB_Test;Integrated Security=True;TrustServerCertificate=True;'
```

Keep `appsettings.test.json` as the team default.

---

## 7. Implementation sequence

### Task 1: Preserve scope and establish baseline

**Files:** No project-file edits.

1. Verify `market-develop-Saba` and record `git status --short --branch`.
2. Confirm the existing 11 SQL files and `.sqlproj` edits remain present.
3. Build the solution before adding tests.
4. Do not reset, commit, or push.

Build command:

```bash
MSYS_NO_PATHCONV=1 "/f/Visual Studio/MSBuild/Current/Bin/MSBuild.exe" \
  "G11_Market/G11_Market.sln" \
  /t:Build /p:Configuration=Debug /nologo /v:minimal
```

Expected: build succeeds; only known pre-existing warnings may remain.

### Task 2: Add the Product insert tracer and repair insert contract

**Files:**

- Create: `G11_Market/Market.Tests/ProductRepositoryTests.cs`
- Modify: Product DTO/table/insert procedure files listed in section 6.1

1. Add private parameterized direct-SQL helpers to insert/query Product rows.
2. Write `Insert_ShouldReturnIdAndCreateActiveProduct` first.
3. Run only that test and confirm RED for the current Product insert/output/default mismatch.
4. Correct DTO/table/insert procedure minimally.
5. Build and publish the database project.
6. Rerun the one test and confirm GREEN.
7. Add the unknown-category and duplicate-name insert tests one at a time, verifying their exact SQL error numbers.

### Task 3: Add Product update/delete slices and repair procedures

**Files:**

- Modify: `ProductRepositoryTests.cs`
- Modify: `sp_UpdateProduct.sql`
- Modify: `sp_DeleteProduct.sql`

For each behavior:

1. Arrange a fresh row directly.
2. Write the targeted repository test.
3. Confirm RED against the current parameter/delete bug.
4. Make the minimal procedure correction.
5. Build/publish.
6. Confirm GREEN.

Do not use Product repository Insert as update/delete setup.

### Task 4: Complete Product reads and custom-query tests

**Files:**

- Modify: `ProductRepositoryTests.cs`

Add tests 6–16 from the Product matrix one at a time. Use seeded values only for deterministic read-only cases, such as:

```text
Samsung Monitor: 1200.00
Lenovo Laptop:   2500.00
```

For soft-delete filters, arrange a deleted row directly instead of depending on repository Delete.

### Task 5: Add Role tests and align nullable contract

**Files:**

- Create: `G11_Market/Market.Tests/RoleRepositoryTests.cs`
- Modify: `IRoleRepository.cs`
- Modify: `RoleRepository.cs`

1. Correct `GetByName` return type to `RoleDTO?`.
2. Add direct Role insert/query helpers.
3. Add the 10 approved tests one behavior at a time.
4. Use a fresh row for update/delete tests.
5. Use `Description = null` in the valid insert path.
6. Assert exact unique-constraint numbers for duplicate names.

### Task 6: Add ClientType contract through a compile-failing test

**Files:**

- Create/modify the ClientType DTO/repository/UoW files listed in section 6.3
- Create: `G11_Market/Market.Tests/ClientTypeRepositoryTests.cs`

1. Write the first ClientType insert test against the wished-for API.
2. Run it and confirm RED/compile failure because `ClientTypeDTO` and `ClientTypeRepository` exposure do not yet exist.
3. Add the minimal DTO/interface/repository/UoW contract.
4. Build and publish the database project.
5. Rerun and confirm GREEN.
6. Add the remaining five ClientType tests one at a time.
7. Do not add a custom ClientType `GetByName` method merely to create more tests.

### Task 7: Build and deploy the current database source to the test database

Build:

```bash
MSYS_NO_PATHCONV=1 "/f/Visual Studio/MSBuild/Current/Bin/MSBuild.exe" \
  "G11_Market/Market.Database/Market.Database.sqlproj" \
  /t:Build /p:Configuration=Debug /nologo /v:minimal
```

Publish only to `MarketDB_Test`:

```bash
MSYS_NO_PATHCONV=1 "/f/Visual Studio/Common7/IDE/Extensions/Microsoft/SQLDB/DAC/SqlPackage.exe" \
  /Action:Publish \
  /SourceFile:"C:\Users\SABA\source\repos\Group-11\G11_Market\Market.Database\bin\Debug\Market.Database.dacpac" \
  /TargetConnectionString:"Server=.\GV1MRA;Database=MarketDB_Test;Integrated Security=True;TrustServerCertificate=True;" \
  /p:DropObjectsNotInSource=False
```

Verify the 11 procedure names through `sys.procedures` before running tests. Never publish these changes to a non-test database as part of this task.

### Task 8: Targeted and regression verification

Set the local connection override:

```bash
export MARKET_TEST_CONNECTION_STRING='Server=.\GV1MRA;Database=MarketDB_Test;Integrated Security=True;TrustServerCertificate=True;'
```

Run Product only:

```bash
dotnet test G11_Market/Market.Tests/Market.Tests.csproj \
  --filter 'FullyQualifiedName~Market.Tests.ProductRepositoryTests'
```

Run Role only:

```bash
dotnet test G11_Market/Market.Tests/Market.Tests.csproj \
  --filter 'FullyQualifiedName~Market.Tests.RoleRepositoryTests'
```

Run ClientType only:

```bash
dotnet test G11_Market/Market.Tests/Market.Tests.csproj \
  --filter 'FullyQualifiedName~Market.Tests.ClientTypeRepositoryTests'
```

Run all three together:

```bash
dotnet test G11_Market/Market.Tests/Market.Tests.csproj \
  --filter 'FullyQualifiedName~Market.Tests.ProductRepositoryTests|FullyQualifiedName~Market.Tests.RoleRepositoryTests|FullyQualifiedName~Market.Tests.ClientTypeRepositoryTests'
```

Then run the full regression suite:

```bash
dotnet test G11_Market/Market.Tests/Market.Tests.csproj
```

Final acceptance criteria:

- Exactly 32 approved core tests exist unless an optional case is explicitly approved.
- All 32 targeted tests pass repeatedly.
- Full solution builds.
- Full existing test suite has no new failures.
- SQL failures assert intended numbers/messages rather than arbitrary `SqlException`.
- Each update/delete test uses independent direct-SQL arrangement and verification.
- No impossible non-nullable numeric inputs are tested.
- No unsupported business rule is invented.
- No files outside the listed Product/Role/ClientType/test-infrastructure scope change.
- No commit, push, or non-test database deployment occurs without separate approval.

---

## 8. Expected project files changed by implementation

### Create

- `G11_Market/Market.DTO/ClientTypeDTO.cs`
- `G11_Market/Market.Services/Interfaces/Repositories/IClientTypeRepository.cs`
- `G11_Market/Market.Repositories/ClientTypeRepository.cs`
- `G11_Market/Market.Tests/ProductRepositoryTests.cs`
- `G11_Market/Market.Tests/RoleRepositoryTests.cs`
- `G11_Market/Market.Tests/ClientTypeRepositoryTests.cs`

### Modify

- `G11_Market/Market.DTO/ProductDTO.cs`
- `G11_Market/Market.Services/Interfaces/Repositories/IRoleRepository.cs`
- `G11_Market/Market.Repositories/RoleRepository.cs`
- `G11_Market/Market.Services/Interfaces/IUnitOfWork.cs`
- `G11_Market/Market.Repositories/UnitOfWork.cs`
- `G11_Market/Market.Tests/Helpers/ConfigurationManager.cs`
- `G11_Market/Market.Database/dbo/Tables/Products.sql`
- `G11_Market/Market.Database/dbo/Stored Procedures/sp_InsertProduct.sql`
- `G11_Market/Market.Database/dbo/Stored Procedures/sp_UpdateProduct.sql`
- `G11_Market/Market.Database/dbo/Stored Procedures/sp_DeleteProduct.sql`

The existing 11 new procedure files and their `.sqlproj` entries remain part of the same uncommitted branch work and must be deployed before the tests can run.
