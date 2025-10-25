var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerDocumentation(builder.Environment);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;

builder.Services.AddCarterWIthAssemblies(catalogAssembly,basketAssembly);

builder.Services.AddMediatRWIthAssemblies(catalogAssembly,basketAssembly);

builder.Services.AddMassTransitWithAssemblies(catalogAssembly,basketAssembly);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCatalogModule(builder.Configuration)
                .AddBasketModule(builder.Configuration)
                .AddOrderingModule(builder.Configuration);

var app = builder.Build();

app.UseSwaggerDocumentation();

app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(options => { });

//module services: catalog, basket, ordering
app.UseCatalogModule()
   .UseBasketModule()
   .UseOrderingModule();

app.Run();