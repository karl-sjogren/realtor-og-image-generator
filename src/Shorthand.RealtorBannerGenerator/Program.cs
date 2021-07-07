using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Shorthand.RealtorBannerGenerator.Core.Contracts;
using Shorthand.RealtorBannerGenerator.Core.PropertyProviders;
using Shorthand.RealtorBannerGenerator.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IPropertyProvider, ErikOlssonPropertyProvider>();
builder.Services.AddSingleton<IBannerGenerator, PlaywrightBannerGenerator>();
builder.Services.AddControllers();

var app = builder.Build();

if(app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
}

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/playwright"),
    appBuilder => appBuilder.UseMiddleware<PlaywrightMiddleware>()
);

app.UseStaticFiles();

app.MapControllers();

app.Run();
