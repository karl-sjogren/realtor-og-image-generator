using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Shorthand.RealtorBannerGenerator.Core.Contracts;

namespace Shorthand.RealtorBannerGenerator.Core.BannerGenerators {
    /// <summary>
    /// This PuppeteerSharp generator is a bit faster and can automaticly download
    /// Chromium or Firefox on startup. The API is not as nice as Playwrights though.
    /// Needs to be registered as a Singleton.
    /// </summary>
    public sealed class PuppeteerBannerGenerator : IBannerGenerator, IAsyncDisposable {
        private readonly ILogger<PuppeteerBannerGenerator> _logger;

        private Browser _browser;

        public PuppeteerBannerGenerator(ILogger<PuppeteerBannerGenerator> logger) {
            _logger = logger;
        }

        /// <summary>
        /// Start a browser and populate the _browser field.
        /// This speeds up generation by 15-20 seconds by reusing the browser instance.
        /// </summary>
        private async Task StartBrowserAsync() {
            if(_browser == null) {
                var browserFetcher = new BrowserFetcher();

                // Check if the browser is already downloaded, if not then download it.
                if(browserFetcher.LocalRevisions().Contains(BrowserFetcher.DefaultChromiumRevision)) {
                    _logger.LogInformation("Downloading Chromium for Puppeteer.");
                    await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                }

                _logger.LogInformation("Starting new Chromium context for Puppeteer.");
                _browser = await Puppeteer.LaunchAsync(new LaunchOptions());
            }
        }

        public async Task<Memory<byte>> GetBannerImageAsync(string identifier, CancellationToken cancellationToken) {
            await StartBrowserAsync();

            using var page = await _browser.NewPageAsync();
            await page.GoToAsync("https://localhost:5001/erikolsson.html?identifier=" + identifier);

            // Wait for window.documentReady to be set to true (needs to be handled by the page after all images have loaded)
            await page.WaitForExpressionAsync("window.documentReady === true", new WaitForFunctionOptions { Polling = WaitForFunctionPollingOption.Raf });

            // Puppeteer starts with a too small viewport so we need to resize it
            await page.SetViewportAsync(new ViewPortOptions { Width = 1500, Height = 1500 });

            // Get the main element and create a PNG screenshot of it
            var mainElement = await page.QuerySelectorAsync("main");
            using var screenshotStream = await mainElement.ScreenshotStreamAsync(new ScreenshotOptions { Type = ScreenshotType.Png });

            using var ms = new MemoryStream();
            await screenshotStream.CopyToAsync(ms, cancellationToken);

            return ms.ToArray().AsMemory();
        }

        public async ValueTask DisposeAsync() {
            if(_browser != null) {
                await _browser.DisposeAsync();
            }
        }
    }
}
