# MacroMetrics — Project Vision

## Vision statement

MacroMetrics is a read-only data visualisation tool that lets anyone compare major asset classes and economic indicators in inflation-adjusted, ratio form — making it easy to see when things like housing, gold, or equities are historically cheap or expensive relative to wages, inflation, or each other.

## Problem

Most financial charts show raw prices. Raw prices are misleading — a house costing £300,000 today is not comparable to £300,000 in 1990. MacroMetrics removes inflation and income distortion by expressing everything as a ratio, so the user sees *real* relative value over time.

## Target users

- Personal use (developer + family)
- Anyone who wants to understand macro valuation without paying for Bloomberg

## MVP scope

### In scope
- 14 macro metrics sourced from yfinance, FRED (US), and ONS (UK)
- 9 preset ratio cards on a dashboard homepage
- Custom comparison tool: pick any two comparable metrics, see the ratio chart
- 3 standalone indicator cards (CAPE, UK Gilt yield, US Treasury yield)
- All charts show ratio line + long-run average reference line + deviation tooltip (Option C)
- No database — backend fetches and serves data on demand

### Out of scope for MVP
- User accounts, auth, saved views
- Mobile-optimised layout
- Alerts or notifications
- Markets outside UK and US
- Real-time data (daily/weekly refresh acceptable)
- Non-OHLC asset classes (fine art, wine, etc.)

## High-level architecture

```
Frontend (React + RTK Query)
    │
    │  GET /api/metrics
    │  GET /api/metrics/{id}
    │  GET /api/metrics/ratio?numerator=&denominator=
    ▼
Backend (.NET 10 — data aggregation proxy)
    ├── yfinance fetcher  → equities, gold, oil, bitcoin
    ├── FRED fetcher      → US macro data
    └── ONS fetcher       → UK macro data
```

No EF Core or PostgreSQL required for MVP — the backend is a stateless proxy.

## Epics

| Epic | Description |
|---|---|
| [E1 — Data Ingestion & API](../epics/e1-data-ingestion.md) | Fetch, normalise, and expose all metric data *(backend — draft, to be detailed in Phase 6)* |
| [E2 — Dashboard & Preset Ratios](../epics/e2-dashboard-presets.md) | Homepage with 9 curated ratio cards |
| [E3 — Custom Comparison Tool](../epics/e3-custom-comparison.md) | User-driven metric picker + ratio chart |
| [E4 — Standalone Indicator Cards](../epics/e4-indicator-cards.md) | CAPE and bond yield indicator cards |

## Next: Phase 2 — UI/UX Design

With the vision and epic/feature breakdown agreed, the next phase is to design the user-facing product in detail before any code is written.

**Deliverables for Phase 2:**
- ASCII mockup for each page state (homepage, expanded chart, custom comparison, indicators section)
- Mermaid workflow diagram for each key user interaction (e.g. picking a custom comparison)
- Resolution of all open UI/UX questions listed in the feature files
- Sign-off on mockups before user stories are written

Open UI/UX questions to resolve (collected from feature files):
- Date range selector style — slider, preset buttons (5y/10y/20y/Max), or free inputs?
- Preset card grid layout — columns, above-the-fold count, expandable?
- Metric picker style — dropdowns, searchable select, or visual grid?
- Indicator card visual style — same as ratio cards or distinct?
- Warm/cool colour scheme for above/below average — or single colour + threshold line?
