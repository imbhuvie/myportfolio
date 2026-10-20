# Changelog

> **REQUIRED READING**: Every change made to this project must be recorded here.
> For instructions and the exact entry format, see the "IMPORTANT: Change History Requirement" section in `PROJECT.md`.
> New entries go at the **top** of this file, above older ones.

## 2026-08-16 — Updated PROJECT.md / README.md for freelance refactor

### Changed
- `PROJECT.md`: Documented the freelance focus (impact metrics, services section, contact form), new controllers/models/views (`ContactController`, `ContactMessagesController`, `ContactFormViewModel`, `ContactMessage`, inbox at `/ContactMessages`), the `Smtp` appsettings block, the `ContactMessages` table and `Problem`/`Solution` columns in the schema docs, and the contact-form/SMTP conventions.
- `README.md`: Added contact-form and inbox sections, SMTP setup note, and the `ALTER TABLE` upgrade steps for existing databases.

### Note
- Applied the `database/schema.sql` upgrade to the local dev DB (added `Problem`/`Solution` to `Projects`, created `ContactMessages`, updated the Owl Cafe row) and verified end-to-end: home page renders all new sections, `/Contact/Send` saves inquiries (200 success JSON), admin login + `/ContactMessages` inbox work.

## 2026-08-16 — Freelance-focused content + services + contact form

### Changed
- `Views/Home/Index.cshtml`: Refocused the site from job-seeking to freelance client acquisition — new hero headline/description/terminal ("accepting freelance projects"), business-value About copy ("Software that pays for itself"), renumbered section eyebrows, added a `services` tab.
- `wwwroot/js/site.js`: Updated typing-effect phrases, made the hero gutter line count dynamic, added an AJAX contact-form handler (POST `/Contact/Send`).
- `wwwroot/css/site.css`: New styles for the impact grid, services grid, project Problem/Solution panels, contact form, and inbox metadata.
- `Controllers/ProjectsController.cs` / `Views/Projects/_ProjectForm.cshtml`: Create/Edit now map the new `Problem` and `Solution` fields.
- `appsettings.json` / `appsettings.Development.json`: Added `Smtp` config block (placeholders).

### Added
- `Views/Home/Index.cshtml`: New "Impact / Key Metrics" section below the hero (`~70%`, `1.4+ yrs`, `End-to-End`, `24h`); new "Services Offered" section (`services.json`) with API Development, DB Optimization & Migration, Legacy System Automation, Desktop to Web Conversion; projects now render **Problem → Solution → Features** panels; new IDE-themed contact form (Name, Email, Project Details) with "Send Project Brief →" CTA.
- `Models/Project.cs`: Added `Problem` and `Solution` string properties.
- `database/schema.sql`: Added `Problem`/`Solution` columns to `Projects` and a new `ContactMessages` table (plus `ALTER TABLE` upgrade notes for existing DBs).
- `database/seed.sql`: Rewrote the Owl Cafe row with Problem/Solution copy.
- `Models/ContactMessage.cs`, `ViewModels/ContactFormViewModel.cs`: New contact-inquiry entity and form model (incl. honeypot field).
- `Controllers/ContactController.cs`: Public `Send` action — validates, saves to `ContactMessages`, attempts SMTP delivery (skips gracefully when not configured), returns JSON.
- `Controllers/ContactMessagesController.cs`: Authorized inbox (list, mark-read, delete) at `/ContactMessages`.
- `Views/ContactMessages/Index.cshtml`: Inbox UI; "Inbox" link added to the Projects admin page.
- `Data/AppDbContext.cs`: Added `ContactMessages` DbSet.

### Note (future work)
- **VibeIndia** (social chat app) copy drafted but NOT shipped yet — planned as a second project row once the app is buildable. Draft: title "VibeIndia — Social Chat App"; Problem: "Social apps die in the first week when messages lag, feeds feel heavy, and the backend can't scale with the audience."; Solution: "A real-time social chat application with a .NET backend — Web API for data, a real-time messaging layer for live conversations, and a PostgreSQL schema engineered for scale."; features: real-time messaging, Web API endpoints, scalable PostgreSQL data model; stack: ASP.NET Core Web API, SignalR, PostgreSQL, EF Core. Add it via the admin `/Projects` or an INSERT into `Projects`.

## 2026-08-15 — Fixed ambiguous admin routing

### Fixed
- `Controllers/ProjectsController.cs`: Removed the class-level `[Route("Admin/Projects")]` (now commented out) that made every action match the same template and caused an `AmbiguousMatchException`. The controller now uses conventional routing; admin URLs are `/Projects`, `/Projects/Create`, `/Projects/Edit/{id}`, `/Projects/Delete/{id}`.

### Changed
- `PROJECT.md`, `README.md`: Updated admin URL references from `/Admin/Projects` to `/Projects`.

## 2026-08-15 — PostgreSQL-backed projects with admin CRUD

### Added
- `database/schema.sql`: PostgreSQL DDL for the `Projects` table (run manually against a `portfolioapp` database).
- `database/seed.sql`: Optional seed row restoring the "Owl Cafe" project.
- `Models/Project.cs`: EF Core entity matching the `Projects` table.
- `Data/AppDbContext.cs`: EF Core DbContext (`Projects` DbSet) using the Npgsql provider.
- `ViewModels/HomeViewModel.cs`: Holds the projects list for the home page.
- `ViewModels/LoginViewModel.cs`: Password-only model for the admin login.
- `Controllers/AccountController.cs`: Simple password login/logout using cookie authentication (`Admin:Password` from appsettings).
- `Controllers/ProjectsController.cs`: Authorized CRUD at `Admin/Projects` (Index, Create, Edit, Delete).
- `Views/Account/Login.cshtml`: Admin login page.
- `Views/Projects/`: Admin views (Index, Create, Edit, Delete, `_ProjectForm` partial).
- Admin styles in `wwwroot/css/site.css` (`admin-*`, `form-*`, `btn-sm` classes).

### Changed
- `Program.cs`: Registered `AppDbContext` (Npgsql), cookie authentication, and `app.UseAuthentication()`.
- `Controllers/HomeController.cs`: `Index` is now async and loads active projects ordered by `DisplayOrder`.
- `Views/Home/Index.cshtml`: Projects section renders from the database via `Model.Projects`; empty state shows the "more projects" line.
- `appsettings.json` / `appsettings.Development.json`: Added `ConnectionStrings:DefaultConnection` (placeholder password) and `Admin:Password` (placeholder).

### Fixed
- (none)

## 2026-08-15 — Added project documentation for AI agents

### Added
- `PROJECT.md`: Full project guide (overview, tech stack, directory structure, request flow, conventions, run instructions, editing gotchas) written so any AI agent can understand the project quickly.
- `CHANGELOG.md`: This file. Created to satisfy the requirement that every code change is recorded with references.
