using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;

builder.Services.AddCarterWIthAssemblies(catalogAssembly,basketAssembly);

builder.Services.AddMediatRWIthAssemblies(catalogAssembly,basketAssembly);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "EShop Modular Monoliths API",
        Version = "v1",
        Description = "Modular Monolith API with Catalog, Basket, and Ordering modules"
    });
});

builder.Services.AddCatalogModule(builder.Configuration)
                .AddBasketModule(builder.Configuration)
                .AddOrderingModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "EShop API v1");
    options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root (http://localhost:<port>/)
});

app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(options => { });

//module services: catalog, basket, ordering
app.UseCatalogModule()
   .UseBasketModule()
   .UseOrderingModule();

app.Run();