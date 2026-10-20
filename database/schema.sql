-- PortfolioApp database schema
-- PostgreSQL 17
-- Run this file to create the database tables used by the project.
--   psql -U postgres -d portfolioapp -f database/schema.sql
-- The database itself (portfolioapp) must be created first:
--   CREATE DATABASE portfolioapp;

-- ===================== PROJECTS =====================
-- Drives the "#projects" section of the portfolio.
-- A project is shown on the public site only when "IsActive" is true.
CREATE TABLE IF NOT EXISTS "Projects" (
    "Id"           SERIAL PRIMARY KEY,
    "Title"        VARCHAR(200)        NOT NULL,
    "Description"  TEXT,
    "Problem"      TEXT,
    "Solution"     TEXT,
    "Features"     TEXT,
    "TechStack"    TEXT,
    "Url"          VARCHAR(500),
    "ImageUrl"     VARCHAR(500),
    "DisplayOrder" INTEGER             NOT NULL DEFAULT 0,
    "IsActive"     BOOLEAN             NOT NULL DEFAULT TRUE,
    "CreatedAt"    TIMESTAMPTZ         NOT NULL DEFAULT NOW(),
    "UpdatedAt"    TIMESTAMPTZ
);

COMMENT ON TABLE "Projects" IS 'Portfolio project entries shown in the projects section.';

-- ===================== CONTACT MESSAGES =====================
-- Stores inquiries submitted through the contact form on the home page.
CREATE TABLE IF NOT EXISTS "ContactMessages" (
    "Id"        SERIAL PRIMARY KEY,
    "Name"      VARCHAR(200)        NOT NULL,
    "Email"     VARCHAR(200)        NOT NULL,
    "Message"   TEXT                NOT NULL,
    "CreatedAt" TIMESTAMPTZ         NOT NULL DEFAULT NOW(),
    "IsRead"    BOOLEAN             NOT NULL DEFAULT FALSE
);

COMMENT ON TABLE "ContactMessages" IS 'Contact form inquiries. Read via the admin inbox at /ContactMessages.';

-- ===================== UPGRADE NOTES =====================
-- If the "Projects" table already exists (created before the Problem/Solution columns),
-- run the following instead of re-running the CREATE above:
--
--   ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "Problem"  TEXT;
--   ALTER TABLE "Projects" ADD COLUMN IF NOT EXISTS "Solution" TEXT;
--
