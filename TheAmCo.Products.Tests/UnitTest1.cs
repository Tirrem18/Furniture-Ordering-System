using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
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
            // Arrange
            var fakeProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 100 },
                new Product { Id = 2, Name = "Product2", Price = 200 }
            };
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync())
                             .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(fakeProducts, (List<Product>)okResult.Value);
        }

        [TestMethod]
        public async Task GetProducts_ShouldReturnEmptyListWhenNoProductsExist()
        {
            // Arrange
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync())
                             .ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(0, ((List<Product>)okResult.Value).Count);
        }

        [TestMethod]
        public async Task GetProducts_ShouldHandleExceptionGracefully()
        {
            // Arrange
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync())
                             .ThrowsAsync(new System.Exception("Unexpected error"));

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred: Unexpected error", statusCodeResult.Value);
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

            // Add test data with all required fields
            _context.Products.AddRange(
                new Product
                {
                    Id = 1,
                    Name = "Product1",
                    Description = "Description1",
                    Price = 10,
                    InStock = true,
                    ExpectedRestock = null,
                    CategoryId = 1,
                    CategoryName = "Category1",
                    BrandId = 1,
                    BrandName = "Brand1",
                    Source = "Local"
                },
                new Product
                {
                    Id = 2,
                    Name = "Product2",
                    Description = "Description2",
                    Price = 20,
                    InStock = false,
                    ExpectedRestock = DateTime.UtcNow.AddDays(5),
                    CategoryId = 2,
                    CategoryName = "Category2",
                    BrandId = 2,
                    BrandName = "Brand2",
                    Source = "Local"
                }
            );
            _context.SaveChanges();

            _repo = new ProductsRepo(_context, null, null);
        }

        [TestMethod]
        public async Task GetLocalProductsAsync_ShouldReturnAllProducts()
        {
            // Act
            var result = await _repo.GetLocalProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(p => p.Name == "Product1" && p.Description == "Description1"));
            Assert.IsTrue(result.Any(p => p.Name == "Product2" && p.Description == "Description2"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
     [TestClass]
    public class UnderCuttersServiceTests
    {
        private Mock<HttpMessageHandler> _mockHandler;
        private HttpClient _httpClient;
        private UnderCuttersService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockHandler = new Mock<HttpMessageHandler>();

            // Create HttpClient with mocked handler
            _httpClient = new HttpClient(_mockHandler.Object)
            {
                BaseAddress = new Uri("http://test-undercutters.com/") // Simplified base URL
            };

            _service = new UnderCuttersService(_httpClient, null!); // Pass null for IConfiguration
        }

       [TestClass]
    public class UnderCuttersServiceTests
    {
        private Mock<HttpMessageHandler> _mockHandler;
        private HttpClient _httpClient;
        private UnderCuttersService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockHandler = new Mock<HttpMessageHandler>();

            // Create HttpClient with mocked handler
            _httpClient = new HttpClient(_mockHandler.Object)
            {
                BaseAddress = new Uri("http://test-undercutters.com/") // Simplified base URL
            };

            _service = new UnderCuttersService(_httpClient, "http://test-undercutters.com/");
        }

        [TestMethod]
        public async Task GetProductsAsync_ShouldReturnProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product1", Price = 10, Source = "UnderCutters" },
                new Product { Id = 2, Name = "Product2", Price = 20, Source = "UnderCutters" }
            };

            var jsonResponse = JsonSerializer.Serialize(products);
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            };

            _mockHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>(
                            "SendAsync",
                            ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>()
                        )
                        .ReturnsAsync(responseMessage);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(p => p.Name == "Product1"));
            Assert.IsTrue(result.Any(p => p.Name == "Product2"));
        }

        [TestMethod]
        public async Task GetProductsAsync_ShouldHandleEmptyResponse()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]")
            };

            _mockHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>(
                            "SendAsync",
                            ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>()
                        )
                        .ReturnsAsync(responseMessage);

            // Act
            var result = await _service.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetProductsAsync_ShouldThrowExceptionForServerError()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };

            _mockHandler.Protected()
                        .Setup<Task<HttpResponseMessage>>(
                            "SendAsync",
                            ItExpr.IsAny<HttpRequestMessage>(),
                            ItExpr.IsAny<CancellationToken>()
                        )
                        .ReturnsAsync(responseMessage);

            // Act
            await _service.GetProductsAsync();

            // Assert
            // Exception is expected, no further assertions needed
        }
    }
    
}
}

