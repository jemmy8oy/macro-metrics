# E4 — Standalone Indicator Cards

## Summary

Some metrics are already ratios or rates (CAPE, bond yields) and cannot be meaningfully used as a numerator or denominator in a comparison. They are displayed as standalone indicator cards showing the current value plus historical context.

## Indicators

| Indicator | Value shown | Context |
|---|---|---|
| Shiller CAPE Ratio | Current P/E multiple | Long-run average (~17×) + % above/below |
| UK 10yr Gilt Yield | Current yield (%) | Historical range + current position |
| US 10yr Treasury Yield | Current yield (%) | Historical range + current position |

## Card design

- Headline number (large, current value)
- Sparkline: last 30 years of history
- Annotation: long-run average reference + deviation label (e.g. *"42% above historical average"*)
- No date range picker needed — context is fixed

## Features

- [F4.1 — Indicator Card Component](../features/f4.1-indicator-card.md)
- [F4.2 — Indicators Section](../features/f4.2-indicators-section.md)
