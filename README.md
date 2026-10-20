# Bhupendra Verma — Freelance Portfolio (ASP.NET Core MVC)

A single-page, freelance-focused portfolio site built as an ASP.NET Core MVC (.NET 8) project. The projects section and contact inquiries are backed by **PostgreSQL**; the contact form can also email you via SMTP.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 17 running locally

## One-time database setup

The app connects to PostgreSQL but does not create tables — set them up manually:

```sql
CREATE DATABASE portfolioapp;
```

```bash
psql -U postgres -d portfolioapp -f database/schema.sql
# optional seed data (the "Owl Cafe" project):
psql -U postgres -d portfolioapp -f database/seed.sql
```

Upgrading an existing database? Run the `ALTER TABLE` statements at the bottom of `database/schema.sql` to add the `Problem`/`Solution` columns.

Then open `appsettings.Development.json` and set:
- `ConnectionStrings:DefaultConnection` → your postgres credentials
- `Admin:Password` → the password for the admin login
- `Smtp:Host` / `Smtp:Username` / `Smtp:Password` → to receive contact-form emails (optional; messages are still saved to the database without it)

## Run it

```bash
dotnet restore
dotnet run
```

Then open the URL shown in the console (e.g. `https://localhost:7253`).

## Admin

- Login at `/Account/Login` (password from `Admin:Password`).
- Manage projects at `/Projects` — add, edit, or delete portfolio projects (Problem/Solution format).
- Read contact-form inquiries at `/ContactMessages` (linked from the projects page).

## Public site

- **Impact Highlights** — key metrics band below the hero (`~70%` automation, `1.4+ yrs`, end-to-end, `24h`).
- **Services Offered** — API Development, DB Optimization & Migration, Legacy Automation, Desktop-to-Web.
- **Projects** — DB-driven, rendered as Problem → Solution → Features.
- **Contact form** — IDE-themed form that saves inquiries and optionally emails them to you.

## Project layout

- `Controllers/HomeController.cs` — serves the `Index` view (projects loaded from PostgreSQL).
- `Controllers/ProjectsController.cs` — authorized CRUD for projects (`/Projects`).
- `Controllers/ContactController.cs` — public contact-form endpoint (DB + optional SMTP).
- `Controllers/ContactMessagesController.cs` — admin inbox (`/ContactMessages`).
- `Controllers/AccountController.cs` — simple password login.
- `Data/AppDbContext.cs` — EF Core context (Npgsql provider).
- `database/schema.sql` — PostgreSQL DDL (run manually; no EF migrations).
- `Views/Home/Index.cshtml` — the portfolio page markup (hero, impact, about, skills, services, projects, education, contact).
- `Views/Projects/` — admin pages to manage projects.
- `Views/ContactMessages/Index.cshtml` — admin inbox.
- `Views/Shared/_Layout.cshtml` — shared HTML shell: fonts, `site.css`, and `site.js`.
- `wwwroot/css/site.css` — all page styling (dark/light theme + admin + form styles).
- `wwwroot/js/site.js` — theme toggle, typing effect, scroll reveal, tab nav, copy-to-clipboard, contact-form AJAX.
- `wwwroot/images/avatar-illustration.jpg` — the illustration used in the contact section.

## Notes

- See `PROJECT.md` for a full AI-agent guide (structure, conventions, database schema).
- Every code change must be logged in `CHANGELOG.md`.
