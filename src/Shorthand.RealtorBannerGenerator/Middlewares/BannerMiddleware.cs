using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shorthand.RealtorBannerGenerator.Core.Contracts;

namespace Shorthand.RealtorBannerGenerator.Middlewares {
    public class BannerMiddleware {
        private readonly RequestDelegate _next;

        public BannerMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IEnumerable<IBannerGenerator> bannerGenerators) {
            var cancellationToken = httpContext.RequestAborted;
            var request = httpContext.Request;
            var response = httpContext.Response;

            var bannerRequest = BannerRequest.FromHttpRequest(request);

            if(bannerRequest?.Generator == null) {
                await _next(httpContext);
                return;
            }

            var bannerGenerator = bannerGenerators.FirstOrDefault(x => x.GetType().Name.Contains(bannerRequest.Generator, StringComparison.OrdinalIgnoreCase));

            if(bannerGenerator == null) {
                await _next(httpContext);
                return;
            }

            var buffer = await bannerGenerator.GetBannerImageAsync(bannerRequest.PropertyIdentifier, cancellationToken);

            httpContext.Response.ContentType = "image/png";
            await httpContext.Response.BodyWriter.WriteAsync(buffer, cancellationToken);
        }
    }
}
