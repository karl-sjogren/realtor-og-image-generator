using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shorthand.RealtorBannerGenerator.Core.Contracts;

namespace Shorthand.RealtorBannerGenerator.Middlewares {
    public class PlaywrightMiddleware {
        private readonly RequestDelegate _next;

        public PlaywrightMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IBannerGenerator bannerGenerator) {
            var cancellationToken = httpContext.RequestAborted;
            var request = httpContext.Request;
            var response = httpContext.Response;

            var bannerRequest = BannerRequest.FromHttpRequest(request);

            if(bannerRequest == null) {
                await _next(httpContext);
                return;
            }

            var buffer = await bannerGenerator.GetBannerImageAsync(bannerRequest.PropertyIdentifier, cancellationToken);

            httpContext.Response.ContentType = "image/png";
            await httpContext.Response.BodyWriter.WriteAsync(buffer, cancellationToken);
        }
    }
}
