using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Shorthand.RealtorBannerGenerator {
    public record BannerRequest {
        public string PropertyIdentifier { get; init; }
        public string Generator { get; init; }

        public static BannerRequest FromHttpRequest(HttpRequest request) {
            var path = request.Path;

            if(string.IsNullOrWhiteSpace(path)) {
                return null;
            }

            var pathParts = path.ToString().Split("/", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return new BannerRequest {
                PropertyIdentifier = pathParts.Skip(2).FirstOrDefault(),
                Generator = pathParts.Skip(1).FirstOrDefault()
            };
        }
    }
}
