-- Optional seed data: restores the "Owl Cafe" project that was previously hardcoded.
--   psql -U postgres -d portfolioapp -f database/seed.sql
-- Note: "Features" is newline-separated; each line becomes a bullet point.

INSERT INTO "Projects" ("Title", "Description", "Problem", "Solution", "Features", "TechStack", "Url", "ImageUrl", "DisplayOrder", "IsActive")
VALUES (
    'Owl Cafe',
    'A production-grade restaurant management web app built with ASP.NET Core MVC (.NET 8) — a premium public site, a full admin dashboard, and a complete table reservation system, all in one solution.',
    'Restaurant owners manage menu changes, table bookings, and customer questions across phone calls, paper notes, and spreadsheets — double-bookings and stale information quietly cost real revenue.',
    'One platform: a public site, an admin dashboard, and a table-reservation system with real-time availability. Menu, bookings, gallery, and site settings live in a single place, auto-seeded on first run.',
    E'Dynamic menu, gallery, about & reservation pages
Table reservations with availability checks
Full admin CRUD: menu, tables, bookings, gallery
Role-based auth for Admin / Customer
SEO-ready dynamic site settings
Auto-seeded database on first run',
    'ASP.NET Core MVC (.NET 8),PostgreSQL + EF Core 8,ASP.NET Identity,AutoMapper,FluentValidation,Serilog,Bootstrap 5,Docker',
    NULL,
    NULL,
    1,
    TRUE
);

-- ===================== SEEDING EXISTING DATABASES =====================
-- If the "Projects" table already exists but lacks the Problem/Solution columns:
--   ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "Problem"  TEXT;
--   ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "Solution" TEXT;
-- Then update the existing Owl Cafe row (instead of a duplicate INSERT):
--   UPDATE "Projects" SET "Problem" = '...', "Solution" = '...' WHERE "Title" = 'Owl Cafe';
