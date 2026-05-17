// Playwright screenshot script for macro-metrics PR review
// Usage: node scripts/take-screenshots.js
// Prerequisites: Both backend (port 5257) and frontend (port 5173) must be running

const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

const BASE_URL = 'http://localhost:5173';
const OUTPUT_DIR = path.join(__dirname, '../screenshots');

async function main() {
  if (!fs.existsSync(OUTPUT_DIR)) {
    fs.mkdirSync(OUTPUT_DIR, { recursive: true });
  }

  const browser = await chromium.launch({
    args: ['--no-sandbox', '--disable-setuid-sandbox', '--disable-dev-shm-usage'],
  });

  const context = await browser.newContext({
    viewport: { width: 1440, height: 900 },
  });

  const page = await context.newPage();

  console.log('Navigating to app...');
  await page.goto(BASE_URL, { waitUntil: 'networkidle', timeout: 30000 });

  // 1. Full page hero / landing view
  console.log('Taking: 01-hero.png');
  await page.screenshot({ path: `${OUTPUT_DIR}/01-hero.png`, fullPage: false });

  // 2. Scroll to and screenshot the Presets section (preset ratio cards)
  console.log('Taking: 02-presets-section.png');
  await page.evaluate(() => {
    const el = document.querySelector('#presets') || document.querySelector('[data-section="presets"]');
    if (el) el.scrollIntoView({ behavior: 'instant' });
  });
  // Wait for any charts to finish rendering
  await page.waitForTimeout(2000);
  await page.screenshot({ path: `${OUTPUT_DIR}/02-presets-section.png`, fullPage: false });

  // 3. Full-page scroll to capture everything (sticky nav visible)
  console.log('Taking: 03-full-page.png');
  await page.evaluate(() => window.scrollTo(0, 0));
  await page.waitForTimeout(500);
  await page.screenshot({ path: `${OUTPUT_DIR}/03-full-page.png`, fullPage: true });

  // 4. Click a preset card and screenshot the Compare section
  console.log('Taking: 04-compare-prepopulated.png');
  const firstCard = await page.$('[data-testid="preset-card"]') || await page.$('.preset-card') || await page.$('.presetCard');
  if (firstCard) {
    await firstCard.click();
    await page.waitForTimeout(2000);
  } else {
    // Try to scroll to compare directly
    await page.evaluate(() => {
      const el = document.querySelector('#compare') || document.querySelector('[data-section="compare"]');
      if (el) el.scrollIntoView({ behavior: 'instant' });
    });
    await page.waitForTimeout(1000);
  }
  await page.screenshot({ path: `${OUTPUT_DIR}/04-compare-prepopulated.png`, fullPage: false });

  // 5. Scroll to Indicators section
  console.log('Taking: 05-indicators-section.png');
  await page.evaluate(() => {
    const el = document.querySelector('#indicators') || document.querySelector('[data-section="indicators"]');
    if (el) el.scrollIntoView({ behavior: 'instant' });
  });
  await page.waitForTimeout(2000);
  await page.screenshot({ path: `${OUTPUT_DIR}/05-indicators-section.png`, fullPage: false });

  await browser.close();

  const files = fs.readdirSync(OUTPUT_DIR).filter(f => f.endsWith('.png'));
  console.log(`\nDone. Screenshots saved to ${OUTPUT_DIR}:`);
  files.forEach(f => console.log(`  - ${f} (${Math.round(fs.statSync(`${OUTPUT_DIR}/${f}`).size / 1024)}KB)`));
}

main().catch(err => {
  console.error('Screenshot error:', err.message);
  process.exit(1);
});
