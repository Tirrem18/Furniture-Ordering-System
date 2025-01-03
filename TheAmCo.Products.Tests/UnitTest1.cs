using Castle.Core.Configuration;
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
    public class ProductsContextTests
    {
        private ProductsContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Use an in-memory database for testing
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
            // Act
            await ProductsInitialiser.SeedTestData(_context);

            // Assert
            Assert.AreEqual(2, _context.Products.Count());
            Assert.IsTrue(_context.Products.Any(p => p.Name == "Product Placeholder 1"));
            Assert.IsTrue(_context.Products.Any(p => p.Name == "Product Placeholder 2"));
        }

        [TestMethod]
        public async Task ProductsInitialiser_ShouldNotSeedIfDataExists()
        {
            // Arrange
            var existingProduct = new Product { Id = 3, Name = "Existing Product", Price = 20.00m };
            _context.Products.Add(existingProduct);
            await _context.SaveChangesAsync();

            // Act
            await ProductsInitialiser.SeedTestData(_context);

            // Assert
            Assert.AreEqual(1, _context.Products.Count()); // Should remain 1
            Assert.IsTrue(_context.Products.Any(p => p.Name == "Existing Product"));
        }

        [TestMethod]
        public void ProductsContext_ShouldCreateDatabaseSchema()
        {
            // Act
            _context.Database.EnsureCreated();

            // Assert
            Assert.IsTrue(_context.Database.CanConnect());
        }
    }  
    
}


