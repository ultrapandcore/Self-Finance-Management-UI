using Auth0.AspNetCore.Authentication;
using Polly;
using Polly.Contrib.WaitAndRetry;
using SelfFinanceManagerUI.Helpers;
using SelfFinanceManagerUI.Helpers.Constants;
using SelfFinanceManagerUI.Services;
using SelfFinanceManagerUI.Services.Interfaces;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddSingleton<TokenProvider>();

    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IOperationService, OperationService>();

    builder.Services.AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"];
        options.ClientId = builder.Configuration["Auth0:ClientId"];
        options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
        options.Scope = "openid profile email";
    }).WithAccessToken(options =>
    {
        options.Audience = builder.Configuration["Auth0:Audience"];
        options.UseRefreshTokens = true;
    });

    builder.Services.AddScoped<AccessTokenHandler>();
    builder.Services.AddHttpClient(ApiConstants.ClientName, client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["BaseAddress"]);
    })
    .AddTransientHttpErrorPolicy(policyBuilder =>
        policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)))
    .AddHttpMessageHandler<AccessTokenHandler>();

    var app = builder.Build();
    
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRequestLocalization(GetLocalizationOptions());
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    app.UseSerilogRequestLogging();
    app.Run();

    RequestLocalizationOptions GetLocalizationOptions()
    {
        var cultures = builder.Configuration.GetSection("Cultures")
            .GetChildren().ToDictionary(x => x.Key, x => x.Value);

        var supportedCultures = cultures.Keys.ToArray();
        var localizationOptions = new RequestLocalizationOptions()
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        return localizationOptions;
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}