# E2 — Dashboard & Preset Ratios

## Summary

The homepage presents a curated set of ratio cards — each showing a meaningful macro comparison (e.g. UK house prices relative to wages) as a time-series chart. Each chart shows the raw ratio line and a long-run average reference line so the user can immediately see whether the metric is historically cheap or expensive.

## Preset ratio catalogue

| Card title | Numerator | Denominator | Reads as |
|---|---|---|---|
| UK Affordability | UK House Prices | UK Wages | Years of wages to buy a house |
| US Affordability | US House Prices | US Wages | Years of wages to buy a house |
| UK Real House Prices | UK House Prices | UK CPI | Inflation-adjusted UK property |
| Real Gold | Gold | US CPI | Inflation-adjusted gold price |
| Gold vs Equities | Gold | S&P 500 | Relative value: gold vs stocks |
| Real S&P 500 | S&P 500 | US CPI | Inflation-adjusted US equities |
| UK Property vs Gold | UK House Prices | Gold | Property priced in gold |
| Real Oil | Oil | US CPI | Inflation-adjusted oil price |
| Bitcoin vs Gold | Bitcoin | Gold | Crypto vs hard asset |

## Chart design

- X-axis: time (date range selector, default last 30 years or max available)
- Y-axis: ratio value (not raw prices)
- Reference line: long-run average of the ratio over the full available history
- Tooltip: date + current ratio value + % above/below long-run average
- Colour coding: above average = warm, below = cool (subtle)

## Features

- [F2.1 — Homepage Layout & Navigation](../features/f2.1-homepage-layout.md)
- [F2.2 — Ratio Chart Component](../features/f2.2-ratio-chart.md)
- [F2.3 — Preset Ratio Card Grid](../features/f2.3-preset-ratio-grid.md)
