using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TheAmCo.Products.Services.UnderCutters;
using TheAmCo.Products.Data.Products;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Products.Services.ProductsRepo;
using Polly;
using Polly.Extensions.Http;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth:Authority"];
        options.Audience = builder.Configuration["Auth:Audience"];
    });
builder.Services.AddAuthorization();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IUnderCuttersService, UnderCuttersServiceFake>();
}
else
{
    builder.Services.AddHttpClient<IUnderCuttersService, UnderCuttersService>();
}

builder.Services.AddDbContext<ProductsContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = System.IO.Path.Join(path, "products.db");
        options.UseSqlite($"Data Source={dbPath}");
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        Console.WriteLine($"Database Path: {dbPath}");
    }
    else
    {
        var cs = builder.Configuration.GetConnectionString("ProductsContext");
        options.UseSqlServer(cs, sqlServerOptionsAction: sqlOptions =>
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(1.5),
                errorNumbersToAdd: null
            )
        );
    }
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IProductsRepo, ProductRepoFake>();
}
else
{
    builder.Services.AddTransient<IProductsRepo, ProductsRepo>();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var env = services.GetRequiredService<IWebHostEnvironment>();
    if (env.IsDevelopment())
    {
        var context = services.GetRequiredService<ProductsContext>();
        try
        {
            ProductsInitialiser.SeedTestData(context).Wait();
        }
        catch (Exception e)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogDebug("Seeding test data failed: " + e.Message);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(5, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
