using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.Extensions.Logging;
using Shorthand.RealtorBannerGenerator.Core.Contracts;
using Shorthand.RealtorBannerGenerator.Core.Models;

namespace Shorthand.RealtorBannerGenerator.Core.PropertyProviders {
    public class ErikOlssonPropertyProvider : IPropertyProvider {
        private static readonly Regex _urlMatcher = new(@"url\('(?<Url>.*?)'\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly ILogger<ErikOlssonPropertyProvider> _logger;

        public ErikOlssonPropertyProvider(ILogger<ErikOlssonPropertyProvider> logger) {
            _logger = logger;
        }

        public async Task<RealtorProperty> GetRealtorPropertyAsync(string identifier, CancellationToken cancellationToken) {
            _logger.LogInformation($"Trying to get property with identifier {identifier} from ErikOlsson.");

            var configuration = Configuration.Default.WithDefaultLoader();
            var browsingContext = BrowsingContext.New(configuration);

            var document = await browsingContext.OpenAsync($"https://www.erikolsson.se/bostader-till-salu/{WebUtility.UrlEncode(identifier)}", cancellationToken);

            var propertyImageUrl = document.QuerySelector(".background-image-resize").GetAttribute("data-imageurl-large");

            var addressContainer = document.QuerySelector(".property-summary__address-price");
            var streetAddress = addressContainer.QuerySelector("h1").TextContent;
            var city = addressContainer.QuerySelector("h5").TextContent;
            var startingPrice = addressContainer.QuerySelector("h6 .orange")?.TextContent?.Replace("kr", string.Empty).Replace(" ", string.Empty);

            var brokerContainer = document.QuerySelector(".property-summary__container");
            var brokerNameParts = brokerContainer.QuerySelectorAll(".broker__content__name").Select(node => node.TextContent).ToArray();

            var brokerBackgroundNode = brokerContainer.QuerySelector(".broker");
            var brokerBackgroundStyle = brokerBackgroundNode.GetAttribute("style");

            var match = _urlMatcher.Match(brokerBackgroundStyle);
            var brokerImageUrl = match.Success ? "https://www.erikolsson.se" + match.Groups["Url"].Value : null;

            if(propertyImageUrl?.Contains("?") == true)
                propertyImageUrl = propertyImageUrl.Split("?")[0];

            if(brokerImageUrl?.Contains("?") == true)
                brokerImageUrl = brokerImageUrl.Split("?")[0];

            return new RealtorProperty {
                Identifier = identifier,
                City = city,
                Generator = "ErikOlsson",
                RealtorImageUrl = brokerImageUrl,
                RealtorName = $"{brokerNameParts[0]} {brokerNameParts[1]}",
                StartingPrice = startingPrice == null ? null : Int32.Parse(startingPrice),
                StreetAddress = streetAddress,
                PropertyImageUrl = propertyImageUrl
            };
        }
    }
}
