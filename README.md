# ShoppingOnline

An e-commerce platform with a .NET 10 backend API and a Next.js frontend, orchestrated locally with .NET Aspire.

## Overview

ShoppingOnline provides core online shopping functionality: user accounts and authentication,
vendors, a product catalog with categories, FIFO-based inventory backed by purchase orders,
shipping addresses (with a default), and order processing with tax-inclusive pricing. An admin
back office (separate from the storefront) manages the catalog, vendors, inventory, purchase
orders, and user accounts.

## Architecture

```
src/
├── backend/
│   ├── api/
│   │   ├── ShoppingOnline.API         # ASP.NET Core Web API (controllers, entry point)
│   │   ├── ShoppingOnline.Database    # EF Core DbContext, migrations, unit of work
│   │   ├── ShoppingOnline.Handler     # Request handlers (business logic, CQRS-style)
│   │   └── ShoppingOnline.Model       # Entities, DTOs, requests/responses
│   └── utilities/
│       ├── ShoppingOnline.Component.Abstractions  # Cross-cutting concerns: auth (Paseto),
│       │                                          # encryption, email, health checks, Swagger,
│       │                                          # service responses, middleware
│       ├── ShoppingOnline.Component.Data          # Base DbContext / unit-of-work abstractions
│       └── ShoppingOnline.ServiceDefault          # Shared Aspire service defaults (telemetry, etc.)
├── dashboard/
│   └── ShoppingOnline.AppHost         # .NET Aspire AppHost for local orchestration
└── frontend/                          # Next.js 16 (React 19) app
```

### Backend

- **.NET 10**, ASP.NET Core Web API
- **Entity Framework Core** for data access, with migrations under `ShoppingOnline.Database/Migrations`
- **Autofac** for dependency injection
- **Mapster** for object mapping
- **Paseto** tokens for authentication/authorization
- **Serilog** for structured logging
- **Swagger** for API documentation
- Domains covered: Accounts, Authorization, Users, Vendors, Products, Product Categories, Stocks
  (inventory lots), Purchase Orders, Shipping Addresses, Orders

### Sales flow

- Customers buy a **product** and quantity - they don't pick a vendor. Fulfillment is allocated
  FIFO across whichever stock lots exist for that product (oldest received lot first, regardless
  of vendor), so overselling is prevented with an atomic conditional stock decrement per lot.
- Every product has an admin-set `SellPrice` and `TaxRatePercent` (default 7%); order totals are
  computed from these, not from a lot's purchase cost.
- Placing an order requires a saved shipping address; the address text is snapshotted onto the
  order so editing/deleting a saved address later never changes a past order.
- New stock only enters the system by **receiving a Purchase Order**: an admin creates a PO
  against a vendor, sends it (emails the vendor), then records what was actually received
  (supports partial/multiple receipts per PO) - each receipt creates a new FIFO lot.
- A confirmation email is sent to the customer after an order is placed; a PO email is sent to
  the vendor when it's sent. Admin accounts cannot place orders or deactivate themselves.

### Admin back office

Available under `/admin` in the frontend to users with the `Admin` role: Products, Vendors,
Categories, Stocks (read-only inventory view), Purchase Orders, and Users (activate/deactivate).

### Dashboard / Orchestration

- **.NET Aspire** `AppHost` project used to run and observe the API and its dependencies locally.

### Frontend

- **Next.js 16**, **React 19**, **TypeScript**
- **Tailwind CSS v4** with `shadcn` components
- See `src/frontend/README.md` and `src/frontend/AGENTS.md` for frontend-specific setup notes.

## Getting Started

### Prerequisites

- .NET SDK `10.0.400` (see `global.json`)
- Node.js and pnpm/npm (see `src/frontend/package.json`)
- Docker (Postgres and pgAdmin are run as containers by the Aspire AppHost)

### Run the backend (via Aspire AppHost)

```bash
dotnet run --project src/dashboard/ShoppingOnline.AppHost
```

This starts Postgres, the API, and the frontend together, and applies EF Core migrations
automatically on startup. A seeded admin account and demo catalog are created on first run:

- Admin login: `admin@nexus.com` / `Admin@12345`

### Run the frontend

```bash
cd src/frontend
pnpm install
pnpm dev
```

## Solution Structure

Open `ShoppingOnline.slnx` in Visual Studio or JetBrains Rider to load all backend projects.
