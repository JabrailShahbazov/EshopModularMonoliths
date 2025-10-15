using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddCarterWIthAssemblies(typeof(CatalogModule).Assembly);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCatalogModule(builder.Configuration)
                .AddBasketModule(builder.Configuration)
                .AddOrderingModule(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(options => { });

//module services: catalog, basket, ordering
app.UseCatalogModule()
   .UseBasketModule()
   .UseOrderingModule();

app.Run();