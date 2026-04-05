# CLAUDE.md — web-template

A `dotnet new` monorepo template for .NET 10 + React 19 projects. Every new project you start should be scaffolded from this template.

## Tech Stack

| Layer | Tech |
|---|---|
| Backend | .NET 10, ASP.NET Core Minimal APIs, EF Core + PostgreSQL, AutoMapper, Scalar/OpenAPI |
| Frontend | React 19, TypeScript, Vite (Rolldown), Redux Toolkit + RTK Query |
| Infrastructure | Docker (multi-stage), Helm, Kubernetes (OCI) |

## Repository Structure

```
/
├── backend/                       # .NET solution — 7 projects, Clean Architecture
│   ├── MacroMetrics.WebApi/       # Entry point: routes, DI, OpenAPI
│   ├── MacroMetrics.Services/     # Business logic, AutoMapper profiles
│   ├── MacroMetrics.Abstractions/ # Service interfaces + model interfaces
│   ├── MacroMetrics.Database/     # EF Core DbContext + migrations
│   ├── MacroMetrics.EntityModels/ # Database entities (anemic POCOs)
│   ├── MacroMetrics.DomainModels/ # Rich business-layer objects
│   └── MacroMetrics.DataModels/   # Request/Response DTOs
├── frontend/                      # React + TypeScript Vite app
│   └── src/
│       ├── api/                   # RTK Query: emptyApi, generatedApi (codegen), custom endpoints
│       ├── components/            # Reusable UI components (each with co-located .scss)
│       ├── context/               # React context for UI state
│       ├── data/                  # config.json and static JSON
│       ├── pages/                 # Route-level page components
│       ├── store/                 # Redux store
│       └── styles/                # Global SCSS: design tokens, resets, utilities
├── scripts/init.mjs               # First-run onboarding: generates appsettings + .env
├── helm/                          # Helm chart for Kubernetes deployment
├── deploy.sh                      # Local CD: build, push images, restart K8s deployments
├── docs/specs/                    # Architecture decisions and how-to guides
└── .agents/                       # AI rules and workflows
```

## Key Docs

| Doc | What it covers |
|---|---|
| `docs/specs/backend-architecture.md` | 7-project Clean Architecture layer structure |
| `docs/specs/backend-srp.md` | SRP, orchestrator pattern, service naming |
| `docs/specs/openapi-codegen.md` | OpenAPI → RTK Query codegen workflow |
| `docs/specs/testing-strategy.md` | Unit tests, in-process integration tests, Vitest |
| `docs/specs/sdd-workflow.md` | 7-phase Spec Driven Development process |
| `.agents/rules/project.md` | Coding conventions, branch strategy, GitHub workflow rules |
| `.claude/skills/action-issue/SKILL.md` | How to pick up and implement a GH issue |
| `.claude/skills/respond-to-pr/SKILL.md` | How to respond to PR review comments |

## Running Locally

```bash
# Backend (Terminal 1)
cd backend && dotnet run --project MacroMetrics.WebApi
# API: http://localhost:5000  |  Docs: http://localhost:5000/scalar/v1

# Frontend (Terminal 2)
cd frontend && npm run dev
# App: http://localhost:5173

# Regenerate API client after backend changes
cd frontend && npm run codegen
```

## First-Time Setup

```bash
node scripts/init.mjs   # generates appsettings.Development.json + frontend/.env
cd backend && dotnet ef database update \
  --project MacroMetrics.Database \
  --startup-project MacroMetrics.WebApi
```

## Deployment

```bash
./deploy.sh   # builds ARM64 images, pushes to OCIR, rolling restart in K8s
```

CI runs on PRs only (`ci.yml`). Image builds are manual (`docker-build-push.yml`, `workflow_dispatch`).
