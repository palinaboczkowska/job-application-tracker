# EF Core: migrations without a host, and seeding without `HasData()`

## Migrations against a class library that has no entry point

`dotnet ef migrations add` needs to construct a `DbContext` to inspect its
model, and normally does that by finding a host (`Program.cs`) that
registers one via DI. `JobApplicationTracker.Domain` deliberately has no
host -- it's a plain class library, referenced by `Api` but not dependent on
it (see [`docs/architecture.md`](./architecture.md)) -- so that path doesn't
exist yet, and by design shouldn't need to.

The standard answer for exactly this situation is
`IDesignTimeDbContextFactory<T>`:
[`JobTrackerDbContextFactory.cs`](../src/JobApplicationTracker.Domain/Data/JobTrackerDbContextFactory.cs)
builds a `JobTrackerDbContext` with a hardcoded local connection string,
used only by `dotnet ef` at design time. The real app never calls this
class -- `Api`'s `Program.cs` configures its own connection string via DI,
from `appsettings.json` or, in tests, from `CustomWebApplicationFactory`'s
in-memory connection. This is what let Session 1 produce a real, working
migration and a fully tested domain model before the `Api` project existed
at all.

## Status stored as text, not an integer

`JobTrackerDbContext.OnModelCreating` applies `.HasConversion<string>()` to
every `ApplicationStatus` column. The tradeoff is a few extra bytes per row
in exchange for a SQLite file that stays legible when opened directly in any
SQLite browser or `sqlite3` CLI -- a row reading `Status: "Interviewing"`
needs no lookup table to understand, `Status: 1` does.

## Seeding at runtime, not via `HasData()`

EF Core's `HasData()` bakes seed rows directly into a migration, which works
well for simple, flat, rarely-changing reference data. It was rejected here
for two concrete reasons specific to this model:

- **The seed data is relational.** Each sample `JobApplication` in
  [`SeedData.cs`](../src/JobApplicationTracker.Domain/Data/SeedData.cs) has
  its own `StatusChange` history -- a real application that went
  Applied → Interviewing → Offer needs three history rows, each with its own
  timestamp and note. `HasData()` supports this only awkwardly (it wants
  scalar snapshots per entity, not a natural object graph with computed
  foreign keys), where a plain C# method just builds the objects directly.
- **Sample data isn't schema.** A migration is a permanent, ordered record
  of how the *shape* of the database changed. Adjusting a seeded company
  name or timestamp is not a schema change and shouldn't need a new
  migration to do it -- `SeedData.EnsureSeeded()` runs once at startup
  (`Api`'s `Program.cs`, right after `context.Database.Migrate()`), checks
  `context.JobApplications.Any()` first so it's a no-op after the first run,
  and can be edited freely.

## The five seeded applications are entirely invented

`Northwind Analytics`, `Contoso Cloud Services`, `Fabrikam Retail Group`,
`Adventure Works Logistics`, and `Tailspin Toys` are the classic
Microsoft-documentation placeholder company names, chosen deliberately: they
read as unambiguously fictional to anyone with any .NET background, on top
of simply not corresponding to real companies. This is sample data for a
portfolio project, not anyone's real job search.
