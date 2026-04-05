# E1 — Data Ingestion & API

## Summary

The backend fetches time-series data from external public APIs, normalises it into a consistent format, and exposes it to the frontend via a lightweight REST API. No database is required for MVP — data is fetched on demand (or cached in memory).

## Problem solved

All macro metrics live in different formats across different providers (FRED, ONS, yfinance). This epic creates a single abstraction layer so the frontend only ever talks to one API regardless of where the data comes from.

## Data sources

| Provider | What it covers |
|---|---|
| **yfinance** (Python or .NET wrapper) | Gold (`GC=F`), Oil (`BZ=F`), FTSE 100 (`^FTSE`), S&P 500 (`^GSPC`), Bitcoin (`BTC-USD`) |
| **FRED API** (Federal Reserve) | US CPI (`CPIAUCSL`), US Wages (`CES0500000003`), US House Prices (`CSUSHPISA`), CAPE (`CAPE`), US 10yr Yield (`DGS10`) |
| **ONS API** (UK Office for National Statistics) | UK CPI, UK Wages (EARN01), UK House Prices (HPI) |

All sources are free/public — no API keys required for basic access (FRED requires a free key).

## Metric catalogue

| Metric ID | Label | Unit | Source |
|---|---|---|---|
| `uk-house-prices` | UK House Prices | Index (£) | ONS HPI |
| `us-house-prices` | US House Prices | Index | FRED CSUSHPISA |
| `uk-wages` | UK Average Wages | £/year | ONS EARN01 |
| `us-wages` | US Average Hourly Earnings | $/hr | FRED |
| `uk-cpi` | UK CPI | Index | ONS |
| `us-cpi` | US CPI | Index | FRED |
| `gold` | Gold Price | $/oz | yfinance `GC=F` |
| `oil` | Oil Price (Brent) | $/barrel | yfinance `BZ=F` |
| `ftse100` | FTSE 100 | Index pts | yfinance `^FTSE` |
| `sp500` | S&P 500 | Index pts | yfinance `^GSPC` |
| `bitcoin` | Bitcoin | USD | yfinance `BTC-USD` |
| `cape` | Shiller CAPE Ratio | Ratio | FRED `CAPE` |
| `uk-10yr-gilt` | UK 10yr Gilt Yield | % | yfinance / FRED |
| `us-10yr-treasury` | US 10yr Treasury Yield | % | FRED `DGS10` |

Note: `cape`, `uk-10yr-gilt`, and `us-10yr-treasury` are standalone indicators — not available as comparison candidates in the custom comparison tool.

## Features

- [F1.1 — External Data Fetchers](../features/f1.1-data-fetchers.md)
- [F1.2 — Data Normalisation & Alignment](../features/f1.2-data-normalisation.md)
- [F1.3 — Metrics REST API](../features/f1.3-metrics-api.md)
