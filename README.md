# How to use Azure App Configuration with Feature Management in ASP.NET Core

Full article on [Implement Feature flags in dotnet in 5 mins](https://vkishan.hashnode.dev/implement-feature-flags-in-dotnet-in-5-mins)

```
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration.GetConnectionString("AppConfig"))
            .ConfigureRefresh(refresh =>
            {
                refresh.Register("TestApp:Settings:Message")
                       .SetCacheExpiration(TimeSpan.FromSeconds(10));
            });
    // Load all feature flags with no label
    options.UseFeatureFlags();
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(configure => configure.AddConsole());

// Add Azure App Configuration middleware to the container of services.
builder.Services.AddAzureAppConfiguration();

// Add feature management to the container of services.
builder.Services.AddFeatureManagement();

var app = builder.Build();

//var appConfigVal= app.Configuration["TestApp:Settings:Message"];
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/feature-flag/Feature1", async (IConfigurationRefresherProvider refresherProvider, IFeatureManager featureManager) =>
{
    var feature1=await featureManager.IsEnabledAsync("Feature1");

    foreach (var refresher in refresherProvider.Refreshers)
        {
            _ = refresher.TryRefreshAsync();
        }
    return new {
        FeatureFlag=$"Feature 1 is '{feature1}'",
        MessageFromAzAppConfig=app.Configuration["TestApp:Settings:Message"]
    };
})
.WithName("FeatureFlagDemo")
.WithOpenApi();

app.Run();
```