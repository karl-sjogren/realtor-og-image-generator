using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shorthand.RealtorBannerGenerator.Core.Contracts;
using Shorthand.RealtorBannerGenerator.Core.PropertyProviders;
using Shorthand.RealtorBannerGenerator.Middlewares;
using Shorthand.RealtorBannerGenerator.Core.BannerGenerators;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IPropertyProvider, ErikOlssonPropertyProvider>();
builder.Services.AddSingleton<IBannerGenerator, PlaywrightBannerGenerator>();
builder.Services.AddSingleton<IBannerGenerator, PuppeteerBannerGenerator>();
builder.Services.AddControllers();

var app = builder.Build();

if(app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/banner"),
    appBuilder => appBuilder.UseMiddleware<BannerMiddleware>()
);

app.UseStaticFiles();

app.MapControllers();

app.Run();
