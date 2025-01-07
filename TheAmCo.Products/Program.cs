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
using Microsoft.Data.SqlClient;
using TheAmCo.Products.Services.DodgeyDealers;



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
    builder.Services.AddSingleton<IDodgyDealersService, DodgyDealersServiceFake>();
}
else
{
    builder.Services.AddHttpClient<IUnderCuttersService, UnderCuttersService>();
    builder.Services.AddHttpClient<IDodgyDealersService, DodgyDealersService>();
}
builder.Configuration.AddEnvironmentVariables();
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
    //builder.Services.AddSingleton<IProductsRepo, ProductRepoFake>();
    builder.Services.AddTransient<IProductsRepo, ProductsRepo>();
    //Test Workflow
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/test-db", async (ProductsContext dbContext) =>
{
    try
    {
        var result = await dbContext.Products.FirstOrDefaultAsync();
        return result != null ? $"ID: {result.Id}" : "No data found in the table.";
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message} | Inner: {ex.InnerException?.Message}";
    }
});

app.MapGet("/test-connection", async () =>
{
    var connectionString = "Server=tcp:thamco.database.windows.net,1433;Initial Catalog=ThAmCo.Products;Persist Security Info=False;User ID=thamcoboss@thamco;Password=Password1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    try
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return "Database connection successful!";
    }
    catch (Exception ex)
    {
        return $"Database connection failed: {ex.Message}";
    }
});


app.Run();
