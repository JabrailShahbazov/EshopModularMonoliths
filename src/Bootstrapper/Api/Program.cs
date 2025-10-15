var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarterWIthAssemblies(typeof(CatalogModule).Assembly);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddCatalogModule(builder.Configuration)
                .AddBasketModule(builder.Configuration)
                .AddOrderingModule(builder.Configuration);

var app = builder.Build();

app.MapCarter();

app.UseCatalogModule()
   .UseBasketModule()
   .UseOrderingModule();

app.UseExceptionHandler(options =>
{
    
});

app.Run();