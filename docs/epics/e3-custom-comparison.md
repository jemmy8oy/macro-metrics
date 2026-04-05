# E3 — Custom Comparison Tool

## Summary

Below the preset dashboard, the user can build their own comparison by selecting any two metrics from the candidate list as numerator and denominator. The resulting ratio is charted identically to the preset cards (ratio line + long-run average reference line).

## Comparison candidates

All metrics in the catalogue **except** standalone indicators (CAPE, UK Gilt, US Treasury) are available as numerator or denominator:

- UK House Prices
- US House Prices
- UK Wages
- US Wages
- UK CPI
- US CPI
- Gold
- Oil
- FTSE 100
- S&P 500
- Bitcoin

## Behaviour

- Two dropdowns: **Numerator** and **Denominator**
- Validation: numerator and denominator cannot be the same metric
- On selection, chart updates immediately (RTK Query re-fetches if data not cached)
- Date range selector shared with preset chart component
- Chart label auto-generated: e.g. *"Gold / S&P 500"*

## Features

- [F3.1 — Metric Picker UI](../features/f3.1-metric-picker.md)
- [F3.2 — Custom Comparison Chart](../features/f3.2-custom-comparison-chart.md)
