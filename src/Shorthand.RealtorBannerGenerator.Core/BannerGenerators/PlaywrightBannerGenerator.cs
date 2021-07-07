using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using Shorthand.RealtorBannerGenerator.Core.Contracts;

namespace Shorthand.RealtorBannerGenerator.Core.BannerGenerators {
    /// <summary>
    /// This Playwrite generator needs the required browsers first being
    /// installed using Playwright CLI. It is also a bit slower then Puppeteer
    /// but the API is way nicer and more powerful. It also supports Webkit/Safari.
    /// Needs to be registered as a Singleton.
    /// </summary>
    public sealed class PlaywrightBannerGenerator : IBannerGenerator, IAsyncDisposable {
        private readonly ILogger<PlaywrightBannerGenerator> _logger;

        private IPlaywright _playwright;
        private IBrowser _browser;

        public PlaywrightBannerGenerator(ILogger<PlaywrightBannerGenerator> logger) {
            _logger = logger;
        }

        /// <summary>
        /// Start a browser and then create new contexts for each banner generation.
        /// This speeds up generation by 15-20 seconds.
        /// </summary>
        private async Task<IBrowserContext> GetBrowserContextAsync() {
            if(_playwright == null) {
                _logger.LogInformation("Starting new Chromium context for Playwright.");
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync();
            }

            var options = new BrowserNewContextOptions();
            return await _browser.NewContextAsync(options);
        }

        public async Task<Memory<byte>> GetBannerImageAsync(string identifier, CancellationToken cancellationToken) {
            await using var browser = await GetBrowserContextAsync();

            var page = await browser.NewPageAsync();
            await page.GotoAsync("https://localhost:5001/erikolsson.html?identifier=" + identifier);

            // Wait for "Document ready" to be written to the console (needs to be handled by the page after all images have loaded)
            await page.WaitForConsoleMessageAsync(new PageWaitForConsoleMessageOptions { Predicate = message => message.Text.Contains("Document ready") });

            // Get the main element and create a PNG screenshot of it
            var mainElement = await page.QuerySelectorAsync("main");
            var screenshotBytes = await mainElement.ScreenshotAsync(new ElementHandleScreenshotOptions { Type = ScreenshotType.Png });

            return screenshotBytes.AsMemory();
        }

        public async ValueTask DisposeAsync() {
            if(_browser != null) {
                await _browser.DisposeAsync();
            }

            _playwright?.Dispose();
        }
    }
}
