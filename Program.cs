using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(configure => configure.AddConsole());


builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration.GetConnectionString("AppConfig"));
    options.UseFeatureFlags();
});
// Add Azure App Configuration middleware to the container of services.
builder.Services.AddAzureAppConfiguration();
// Add feature management to the container of services.
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<PercentageFilter>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAzureAppConfiguration();

app.MapGet("/feature-flag/Feature1", async (IFeatureManager featureManager) =>
{
    var feature1 = await featureManager.IsEnabledAsync("Feature1");
    var featurePercentage = await featureManager.IsEnabledAsync("PercentageFeature");

    return new
    {
        Feature1Status = $"Feature 1 is '{feature1}'",
        FeaturePercentageStatus = $"PercentageFeature is '{featurePercentage}'"
    };
})
.WithName("FeatureFlagDemo")
.WithOpenApi();

app.Run();