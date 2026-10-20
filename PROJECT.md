# PortfolioApp — Project Guide for AI Agents

This document explains what this project is, how it is structured, and the conventions used, so any AI agent (or developer) can understand it quickly and make correct changes.

## Overview

`PortfolioApp` is a single-page freelance-oriented portfolio website for **Bhupendra Verma** (.NET Developer). It is a standard **ASP.NET Core MVC** application targeting **.NET 8**.

- The page layout and most content live in one Razor view (`Index.cshtml`).
- The site is positioned for **freelance clients**: hero/about copy focus on business value, there is an "Impact Highlights" metrics band, a "Services Offered" section, and an in-page contact form.
- The **projects section is database-driven**: project entries are stored in PostgreSQL and read at request time.
- **Contact inquiries** are saved to PostgreSQL and (optionally) emailed via SMTP; the owner reads them in the admin inbox.
- **No user registration.** The only protected area is a simple admin login (password in `appsettings`) used to manage projects and read the inbox.
- Styling and interactivity are plain CSS and vanilla JavaScript (no frameworks, no build step, no npm).

## Tech Stack

- **C# / .NET 8** (SDK: `net8.0`)
- **ASP.NET Core MVC** (Razor views, tag helpers)
- **Entity Framework Core 9** with the **Npgsql** provider → **PostgreSQL 17**
- **Cookie authentication** (simple admin password login)
- **CSS3** with custom properties (`--var`) for theming
- **Vanilla JavaScript** (theme toggle, typing effect, scroll reveal, tab navigation, copy-to-clipboard)
- Google Fonts: Space Grotesk, Inter, JetBrains Mono (loaded via `<link>` in `_Layout.cshtml`)

## Directory Structure

```
PortfolioApp/
├── Program.cs                     # App entry point: DI, DbContext, cookie auth, middleware, routing
├── PortfolioApp.csproj            # Project file (net8.0, nullable + implicit usings enabled)
├── PortfolioApp.sln               # Solution file
├── appsettings.json               # Logging, AllowedHosts, connection string, admin password, SMTP
├── appsettings.Development.json   # Dev config (connection string + admin password override)
├── README.md                      # End-user run instructions
├── PROJECT.md                     # This document (AI-agent guide)
├── CHANGELOG.md                   # Change history (must be updated on every change)
├── database/
│   ├── schema.sql                 # PostgreSQL DDL — create tables manually (no EF migrations)
│   └── seed.sql                   # Optional seed data (Owl Cafe project)
├── Controllers/
│   ├── HomeController.cs          # Index (loads active projects, renders portfolio) + Error
│   ├── AccountController.cs       # Admin Login/Logout (simple password, cookie auth)
│   ├── ProjectsController.cs      # Authorized CRUD at /Projects (conventional routing)
│   ├── ContactController.cs       # Public POST /Contact/Send — saves inquiry + optional SMTP email
│   └── ContactMessagesController.cs # Authorized inbox: list, mark-read, delete
├── Data/
│   └── AppDbContext.cs            # EF Core context (Projects + ContactMessages)
├── Models/
│   ├── Project.cs                 # Entity matching the Projects table
│   └── ContactMessage.cs          # Entity matching the ContactMessages table
├── ViewModels/
│   ├── HomeViewModel.cs           # Projects list for the home page
│   ├── LoginViewModel.cs          # Admin login model
│   └── ContactFormViewModel.cs    # Contact form model (incl. honeypot field)
├── Views/
│   ├── _ViewImports.cshtml        # @using PortfolioApp + tag helper registration
│   ├── _ViewStart.cshtml          # Sets default layout = "_Layout"
│   ├── Home/
│   │   ├── Index.cshtml           # THE portfolio page (all sections; projects loop over Model.Projects)
│   │   └── Error.cshtml           # Standalone error page (Layout = null)
│   ├── Account/
│   │   └── Login.cshtml           # Admin login page
│   ├── Projects/
│   │   ├── Index.cshtml           # Manage projects list (links to Inbox)
│   │   ├── Create.cshtml          # Add project form
│   │   ├── Edit.cshtml            # Edit project form
│   │   ├── Delete.cshtml          # Delete confirmation
│   │   └── _ProjectForm.cshtml    # Shared create/edit form partial
│   ├── ContactMessages/
│   │   └── Index.cshtml           # Admin inbox
│   └── Shared/
│       └── _Layout.cshtml         # HTML shell: fonts, site.css, site.js, @RenderBody()
├── wwwroot/
│   ├── css/site.css               # All styling; dark/light theme + admin/form styles
│   ├── js/site.js                 # Theme toggle, typing effect, scroll reveal, tabs, copy
│   └── images/avatar-illustration.jpg  # Illustration used in the contact section
├── Properties/
│   └── launchSettings.json        # Run profiles: http (5253), https (7253), IIS Express
└── bin/ obj/ .vs/                 # Build artifacts (ignored)
```

## Database Setup (manual — no EF migrations)

The app connects to PostgreSQL but does **not** create or migrate the schema. Do this once:

1. Create the database:
   ```sql
   CREATE DATABASE portfolioapp;
   ```
2. Create the tables:
   ```bash
   psql -U postgres -d portfolioapp -f database/schema.sql
   ```
3. (Optional) Add seed data:
   ```bash
   psql -U postgres -d portfolioapp -f database/seed.sql
   ```
4. Set the connection string, admin password, and (optionally) SMTP settings in `appsettings.Development.json`.

If the schema changes, update `database/schema.sql` and the matching entities (`Project`, `ContactMessage`) together.

### Projects table

| Column | Type | Notes |
|---|---|---|
| `Id` | SERIAL PK | |
| `Title` | VARCHAR(200) | Required |
| `Description` | TEXT | Shown under the title |
| `Problem` | TEXT | Business problem the project solves |
| `Solution` | TEXT | How the project solves it |
| `Features` | TEXT | Newline-separated; each line becomes a bullet |
| `TechStack` | TEXT | Comma-separated; each item becomes a chip |
| `Url` | VARCHAR(500) | Optional "Visit Project" link |
| `ImageUrl` | VARCHAR(500) | Reserved for future use |
| `DisplayOrder` | INTEGER | Lower shows first |
| `IsActive` | BOOLEAN | Only active projects appear on the public site |
| `CreatedAt` | TIMESTAMPTZ | |
| `UpdatedAt` | TIMESTAMPTZ | |

### ContactMessages table

| Column | Type | Notes |
|---|---|---|
| `Id` | SERIAL PK | |
| `Name` | VARCHAR(200) | From the contact form |
| `Email` | VARCHAR(200) | From the contact form |
| `Message` | TEXT | Project details |
| `CreatedAt` | TIMESTAMPTZ | |
| `IsRead` | BOOLEAN | Toggled in the admin inbox |

> **Upgrade an existing DB**: `ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "Problem" TEXT; ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "Solution" TEXT;` (also in `schema.sql`).

## Request Flow

1. A request hits the server and is processed by the middleware pipeline in `Program.cs` (HTTPS redirection, static files, routing, authentication, authorization).
2. The default route `{controller=Home}/{action=Index}/{id?}` maps `/` to `HomeController.Index`.
3. `Index` (async) loads active projects (`IsActive == true`, ordered by `DisplayOrder`) from PostgreSQL via `AppDbContext` and passes a `HomeViewModel` to the view.
4. `_ViewStart.cshtml` wraps it in `Views/Shared/_Layout.cshtml` (unless the view overrides `Layout`).
5. `_Layout.cshtml` loads the page title (`ViewData["Title"]`), the Google Fonts, `site.css`, and `site.js`.

## Admin Area

- **Login**: `/Account/Login` — enter the password from `Admin:Password` in appsettings. On success a cookie is issued.
- **Manage projects**: `/Projects` (`ProjectsController`, `[Authorize]`) — list, create, edit, delete. Uses conventional routing (`/Projects`, `/Projects/Create`, `/Projects/Edit/{id}`, `/Projects/Delete/{id}`).
- **Inbox**: `/ContactMessages` (`ContactMessagesController`, `[Authorize]`) — read, mark-read, and delete contact-form inquiries. Linked from the Projects page.
- **Logout**: button on the manage pages posts to `/Account/Logout`.
- All admin views reuse `_Layout` and the `admin-*` / `form-*` styles in `site.css`.

## Contact Form & Email

- The contact form on the home page (`#contactForm`) submits via AJAX (`site.js`) to `POST /Contact/Send` with an antiforgery token.
- `ContactController.Send` validates the model (plus a hidden **honeypot** field to block bots), saves the inquiry to `ContactMessages`, then tries to send an SMTP email.
- SMTP is optional: if `Smtp:Host` or `Smtp:ToEmail` is empty, the message is saved to the database only (logged, no error surfaced to the visitor). Configure `Smtp:*` in appsettings to enable delivery.

## Key Conventions

- **Page sections** in `Index.cshtml` are `<section>` elements with IDs: `hero`, `impact`, `about`, `experience`, `skills`, `services`, `projects`, `education`, `contact`. Each section starts with a comment banner (`<!-- ===================== NAME ===================== -->`).
- **Tab navigation**: `<nav class="tabs">` tabs carry `data-target` attributes pointing at section IDs (`services` has its own tab). `site.js` scrolls to the target and uses an `IntersectionObserver` to highlight the active tab.
- **Theme system**: `_Layout.cshtml` sets `<html data-theme="dark">`. `site.css` defines CSS variables in `:root` (and a `[data-theme="light"]` override). `site.js` toggles `data-theme` and persists the choice to `localStorage`.
- **Scroll reveal**: any element with the class `reveal` fades/slides in when scrolled into view (via `IntersectionObserver` in `site.js`).
- **Chips**: skill/stack tags use the `chip` class (e.g. `skills` and `projects` sections).
- **Typing effect**: `#typedText` cycles through phrases defined in `site.js` (`phrases` array).
- **Copy buttons**: contact rows call `copyText(text, button)` from `site.js`.
- **Email escaping**: in `.cshtml`, literal `@` characters are escaped as `@@` (e.g. `bhupendraverma7388@@gmail.com`). Do not remove the double `@`.

## How to Run

Prerequisites: **.NET 8 SDK**, a running **PostgreSQL 17** server, and the database set up (see "Database Setup" above, including the connection string + admin password in `appsettings.Development.json`).

```bash
dotnet restore
dotnet run
```

Then open the URL shown in the console. The `https` profile uses `https://localhost:7253` and `http://localhost:5253` (see `Properties/launchSettings.json`).

## Editing Rules (Gotchas)

- **Public content**: edit `Views/Home/Index.cshtml`, `wwwroot/css/site.css`, or `wwwroot/js/site.js`. Most sections (hero, impact, about, services) are hardcoded HTML.
- **Projects content**: lives in PostgreSQL, managed via `/Projects` (or directly in the DB). To change the project schema, update BOTH `database/schema.sql` and `Models/Project.cs` together — the app does not auto-migrate.
- **Contact inquiries**: live in the `ContactMessages` table, read at `/ContactMessages`. SMTP delivery is optional — empty `Smtp:*` values just disable email (DB save still happens).
- **Connection string / admin password / SMTP**: stored in `appsettings.Development.json` (`ConnectionStrings:DefaultConnection`, `Admin:Password`, `Smtp:*`). Never commit a real password.
- **ViewData["Title"]**: `Index.cshtml` sets it to `"Home"`; `_Layout.cshtml` renders `<title>@ViewData["Title"] - Bhupendra Verma</title>`.
- **Static assets**: referenced with `~/` or absolute `/` paths. Cache-busting is automatic via `asp-append-version="true"`.
- **Images**: the contact section references `/images/avatar-illustration.jpg` (an absolute path, not `~/images/...`).
- **Do not edit** `bin/`, `obj/`, or `.vs/` — they are build/generated artifacts.
- **Middleware order** in `Program.cs` matters: static files, routing, authentication, and authorization are configured explicitly; `UseAuthentication()` must come before `UseAuthorization()`. The error handler (`/Home/Error`) only runs in non-Development environments.
- **EF Core is query-only here**: no migrations are used. Use the same table/column names EF expects (PascalCase, matching the `Project` entity), or the queries will fail.

## IMPORTANT: Change History Requirement

**Every change made to this project MUST be recorded in `CHANGELOG.md`.**

Whenever code is added, modified, or removed — regardless of who makes the change (human or AI agent) — a new entry must be appended to `CHANGELOG.md` at the top of the file, above the previous entries, using this format:

```markdown
## YYYY-MM-DD — Short summary of the change

### Changed
- `path/to/file`: what was changed and why.

### Added
- `path/to/file`: what was added.

### Fixed
- `path/to/file`: what was fixed.
```

Rules:
- Use the current date (`YYYY-MM-DD`) and a concise, descriptive title.
- List every file touched, with a one-line description of what/why.
- Group under `### Changed` / `### Added` / `### Fixed` as applicable.
- Do not delete or rewrite existing entries.
- If a change is trivial (e.g. whitespace), still log it briefly.
