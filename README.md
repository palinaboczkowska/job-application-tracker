# job-application-tracker

An ASP.NET Core Web API + Entity Framework Core + SQLite backend with a
Blazor WebAssembly frontend, for tracking job applications. SQLite is a
private, zero-cost, file-based database — no server, no account, no API key,
ever.

## Status

- [x] Session 1 — solution scaffolding, domain model, EF Core + SQLite
      migrations, status-transition business rules, all unit-tested
- [ ] Session 2 — Web API, DTOs, CORS, integration tests
- [ ] Session 3 — Blazor WebAssembly frontend
- [ ] Session 4 — docs, CI, GitHub push

## Quickstart

```bash
dotnet build
dotnet test   # 23 tests, fully offline
```

## License

MIT — see [LICENSE](./LICENSE).
