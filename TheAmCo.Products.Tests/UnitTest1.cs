using Castle.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ThAmCo.Products.Controllers;
using ThAmCo.Products.Services.ProductsRepo;
using TheAmCo.Products.Data.Products;
using TheAmCo.Products.Services.DodgeyDealers;
using TheAmCo.Products.Services.UnderCutters;

namespace TheAmCo.Products.Tests
{
    [TestClass]
    public class ProductsControllerTests
    {
        private Mock<IProductsRepo> _mockProductsRepo;
        private ProductsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockProductsRepo = new Mock<IProductsRepo>();
            _controller = new ProductsController(_mockProductsRepo.Object);
        }

        [TestMethod]
        public async Task GetProducts_ShouldReturnOkWithProducts()
        {
            var fakeProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 100 },
                new Product { Id = 2, Name = "Product2", Price = 200 }
            };
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(fakeProducts);

            var result = await _controller.GetProducts();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(fakeProducts, (List<Product>)okResult.Value);
        }

        [TestMethod]
        public async Task GetProducts_ShouldReturnEmptyListWhenNoProductsExist()
        {
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(new List<Product>());

            var result = await _controller.GetProducts();

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(0, ((List<Product>)okResult.Value).Count);
        }

        [TestMethod]
        public async Task GetProducts_ShouldHandleExceptionGracefully()
        {
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync()).ThrowsAsync(new System.Exception("Unexpected error"));

            var result = await _controller.GetProducts();

            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred: Unexpected error", statusCodeResult.Value);
        }

        [TestMethod]
        public void Controller_ShouldBeInstanceOfControllerBase()
        {
            Assert.IsInstanceOfType(_controller, typeof(ControllerBase));
        }

        [TestMethod]
        public async Task GetProducts_ShouldReturnProductsWithPrices()
        {
            var fakeProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 100 },
                new Product { Id = 2, Name = "Product2", Price = 200 }
            };
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync()).ReturnsAsync(fakeProducts);

            var result = await _controller.GetProducts();
            var okResult = result as OkObjectResult;
            var returnedProducts = okResult?.Value as List<Product>;

            Assert.IsNotNull(returnedProducts);
            Assert.IsTrue(returnedProducts.All(p => p.Price > 0));
        }
    }

    [TestClass]
    public class ProductsRepoTests
    {
        private ProductsContext _context;
        private ProductsRepo _repo;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ProductsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ProductsContext(options);

            _context.Products.AddRange(
                new Product { Id = 1, Name = "Product1", Description = "Description1", Price = 10, InStock = true, Source = "Local" },
                new Product { Id = 2, Name = "Product2", Description = "Description2", Price = 20, InStock = false, Source = "Local" }
            );
            _context.SaveChanges();

            _repo = new ProductsRepo(_context, null, null);
        }

        [TestMethod]
        public async Task GetLocalProductsAsync_ShouldReturnAllProducts()
        {
            var result = await _repo.GetLocalProductsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public async Task GetLocalProductsAsync_ShouldReturnProductNames()
        {
            var result = await _repo.GetLocalProductsAsync();

            Assert.IsTrue(result.Any(p => p.Name == "Product1"));
            Assert.IsTrue(result.Any(p => p.Name == "Product2"));
        }

        [TestMethod]
        public async Task GetLocalProductsAsync_ShouldIncludePrices()
        {
            var result = await _repo.GetLocalProductsAsync();

            Assert.IsTrue(result.All(p => p.Price > 0));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }

    [TestClass]
    public class ProductsContextTests
    {
        private ProductsContext _context;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ProductsContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ProductsContext(options);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task ProductsInitialiser_ShouldSeedTestData()
        {
            await ProductsInitialiser.SeedTestData(_context);

            Assert.AreEqual(2, _context.Products.Count());
        }

        [TestMethod]
        public async Task ProductsInitialiser_ShouldNotDuplicateData()
        {
            await ProductsInitialiser.SeedTestData(_context);
            await ProductsInitialiser.SeedTestData(_context);

            Assert.AreEqual(2, _context.Products.Count());
        }

        [TestMethod]
        public async Task ProductsContext_ShouldCreateDatabase()
        {
            _context.Database.EnsureCreated();

            Assert.IsTrue(_context.Database.CanConnect());
        }

        [TestMethod]
        public async Task ProductsContext_ShouldContainSeededProductNames()
        {
            await ProductsInitialiser.SeedTestData(_context);

            Assert.IsTrue(_context.Products.Any(p => p.Name == "Product Placeholder 1"));
        }
    }
    [TestClass]
public class WebApplicationBuilderTests
{
    [TestMethod]
    public void Services_ShouldContainAuthentication()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new string[] { });

        // Act
        builder.Services.AddAuthentication();

        // Assert
        var serviceProvider = builder.Services.BuildServiceProvider();
        var authService = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationService>();
        Assert.IsNotNull(authService);
    }

    [TestMethod]
    public void Environment_ShouldBeDevelopmentOrProduction()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new string[] { });

        // Act
        var env = builder.Environment;

        // Assert
        Assert.IsTrue(env.IsDevelopment() || env.IsProduction());
    }
}
[TestClass]
public class DatabaseInitializationTests
{
    [TestMethod]
    public void ProductsContext_ShouldInitializeWithSQLiteInDevelopment()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProductsContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        // Act
        using var context = new ProductsContext(options);

        // Assert
        Assert.IsNotNull(context);
        Assert.IsTrue(context.Database.CanConnect());
    }

    [TestMethod]
    public async Task ProductsContext_ShouldSeedDataCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ProductsContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        using var context = new ProductsContext(options);
        await ProductsInitialiser.SeedTestData(context);

        // Act
        var products = context.Products.ToList();

        // Assert
        Assert.AreEqual(2, products.Count);
        Assert.IsTrue(products.Any(p => p.Name == "Product Placeholder 1"));
        Assert.IsTrue(products.Any(p => p.Name == "Product Placeholder 2"));
    }
}
[TestClass]
public class ServiceRegistrationTests
{
    [TestMethod]
    public void ShouldRegisterProductsRepoInServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register dependencies for ProductsRepo
        var options = new DbContextOptionsBuilder<ProductsContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        services.AddSingleton(new ProductsContext(options));
        services.AddSingleton<IUnderCuttersService>(new Mock<IUnderCuttersService>().Object);
        services.AddSingleton<IDodgyDealersService>(new Mock<IDodgyDealersService>().Object);
        services.AddTransient<IProductsRepo, ProductsRepo>();

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var repo = serviceProvider.GetService<IProductsRepo>();

        // Assert
        Assert.IsNotNull(repo);
        Assert.IsInstanceOfType(repo, typeof(ProductsRepo));
    }

    [TestMethod]
    public void ShouldRegisterProductsContextInServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Register ProductsContext
        var options = new DbContextOptionsBuilder<ProductsContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        services.AddSingleton(new ProductsContext(options));

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetService<ProductsContext>();

        // Assert
        Assert.IsNotNull(context);
        Assert.IsInstanceOfType(context, typeof(ProductsContext));
    }

    [TestMethod]
    public void ShouldRegisterUnderCuttersServiceInServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IUnderCuttersService>(new Mock<IUnderCuttersService>().Object);

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<IUnderCuttersService>();

        // Assert
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public void ShouldRegisterDodgyDealersServiceInServices()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IDodgyDealersService>(new Mock<IDodgyDealersService>().Object);

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetService<IDodgyDealersService>();

        // Assert
        Assert.IsNotNull(service);
    }


    [TestMethod]
    public void ShouldRegisterAuthentication()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder(new string[] { });
        builder.Services.AddAuthentication();

        // Act
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        var authService = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationService>();
        Assert.IsNotNull(authService);
    }
}

}
