# ShoppingOnline

An e-commerce platform with a .NET 10 backend API and a Next.js frontend, orchestrated locally with .NET Aspire.

## Overview

ShoppingOnline provides core online shopping functionality: user accounts and authentication, vendors, product catalog with categories, stock management, shipping addresses, and order processing.

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
- Domains covered: Accounts, Authorization, Users, Vendors, Products, Product Categories, Stocks, Shipping Addresses, Orders

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

### Run the backend (via Aspire AppHost)

```bash
dotnet run --project src/dashboard/ShoppingOnline.AppHost
```

### Run the frontend

```bash
cd src/frontend
pnpm install
pnpm dev
```

## Solution Structure

Open `ShoppingOnline.slnx` in Visual Studio or JetBrains Rider to load all backend projects.
