# job-application-tracker

An ASP.NET Core Web API + Entity Framework Core + SQLite backend with a
Blazor WebAssembly frontend, for tracking job applications. SQLite is a
private, zero-cost, file-based database — no server, no account, no API key,
ever.

## Status

- [x] Session 1 — solution scaffolding, domain model, EF Core + SQLite
      migrations, status-transition business rules, all unit-tested
- [x] Session 2 — Web API (controllers), DTOs, CORS, OpenAPI,
      `WebApplicationFactory` integration tests. Verified live with real
      `curl` requests: 201 with Location on create, 200 on a valid status
      transition, 409 on an invalid one, 204 + 404 on delete
- [x] Session 3 — Blazor WebAssembly frontend calling the real API over CORS.
      Verified live in a browser: seeded list renders, create a new
      application, walk it through status changes, an invalid backward
      transition shows a friendly message (not a crash), delete it again
- [ ] Session 4 — docs, CI, GitHub push

## Quickstart

```bash
dotnet build
dotnet test                                   # 29 tests, fully offline
dotnet run --project src/JobApplicationTracker.Api
# then, in another terminal:
curl http://localhost:5097/api/job-applications
```

## License

MIT — see [LICENSE](./LICENSE).
