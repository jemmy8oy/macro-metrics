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
| [E1 — Data Ingestion & API](../epics/e1-data-ingestion.md) | Fetch, normalise, and expose all metric data |
| [E2 — Dashboard & Preset Ratios](../epics/e2-dashboard-presets.md) | Homepage with 9 curated ratio cards |
| [E3 — Custom Comparison Tool](../epics/e3-custom-comparison.md) | User-driven metric picker + ratio chart |
| [E4 — Standalone Indicator Cards](../epics/e4-indicator-cards.md) | CAPE and bond yield indicator cards |
