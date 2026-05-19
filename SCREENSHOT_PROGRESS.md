# Screenshot Evidence — PR #40: CSS Custom Properties Fix

## Status: ✅ Complete

## What was done

Took Playwright screenshots of the app in both **light** and **dark** themes to prove PR #40's CSS custom property replacements work correctly.

### Key finding: Recharts tooltip crash (dark mode)
Clicking preset cards with a Playwright mouse click triggered a `DefaultTooltipContent` error in Recharts, crashing the app. **Fix:** use a JS `element.click()` call instead — no mouse movement, no hover, no crash.

## Colour verification

### Light theme (`data-theme="light"`)
| Element | Computed colour | Verdict |
|---|---|---|
| `.preset-card__title` | `rgb(71, 85, 105)` (slate-600) | ✅ Visible |
| `.preset-card__value` | `rgb(15, 23, 42)` (near-black) | ✅ Visible |
| `.compare-section__chart-header h3` | `rgb(15, 23, 42)` | ✅ Visible |
| `.compare-section__current` | `rgb(15, 23, 42)` | ✅ Visible |
| `.indicator-card__value` | `rgb(15, 23, 42)` | ✅ Visible |

### Dark theme (`data-theme="dark"`)
| Element | Computed colour | Verdict |
|---|---|---|
| `.preset-card__title` | `rgb(148, 163, 184)` (slate-400) | ✅ Visible |
| `.preset-card__value` | `rgb(248, 250, 252)` (near-white) | ✅ Visible |
| `.compare-section__chart-header h3` | `rgb(248, 250, 252)` | ✅ Visible |
| `.compare-section__current` | `rgb(248, 250, 252)` | ✅ Visible |
| `.indicator-card__value` | `rgb(248, 250, 252)` | ✅ Visible |

**Before fix:** all these elements used hardcoded `#f1f5f9` (near-white), which is invisible on a light background.
**After fix:** CSS custom properties (`var(--text-primary)`, `var(--text-secondary)`) adapt correctly to both themes.

## Screenshot files

Located in `screenshots/proof-final/`:
- `01-hero-light.png` / `01-hero-dark.png`
- `02-presets-light.png` / `02-presets-dark.png`
- `04-compare-light.png` / `04-compare-dark.png`
- `05-indicators-light.png` / `05-indicators-dark.png`

## Scripts used

- `scripts/take-screenshots-v5.js` — final working script (JS click, extended waits)
