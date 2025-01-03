using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
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
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(fakeProducts, okResult.Value);
        }

        [TestMethod]
        public async Task GetProducts_ShouldHandleNoProductsGracefully()
        {
            // Arrange
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync())
                             .ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(0, ((List<Product>)okResult.Value).Count);
        }

        [TestMethod]
        public async Task GetProducts_ShouldReturnInternalServerErrorOnGeneralException()
        {
            // Arrange
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync())
                             .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            var statusCodeResult = result as ObjectResult;
            Assert.IsNotNull(statusCodeResult);
            Assert.AreEqual(500, statusCodeResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred: Unexpected error",
                            statusCodeResult.Value);
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
            // Use in-memory database
            var options = new DbContextOptionsBuilder<ProductsContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new ProductsContext(options);
            _context.Products.AddRange(
                new Product { Id = 1, Name = "Product1", Price = 10 },
                new Product { Id = 2, Name = "Product2", Price = 20 }
            );
            _context.SaveChanges();

            _repo = new ProductsRepo(_context, null, null);
        }

        [TestMethod]
        public async Task GetLocalProductsAsync_ShouldReturnProducts()
        {
            // Act
            var result = await _repo.GetLocalProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(p => p.Name == "Product1"));
            Assert.IsTrue(result.Any(p => p.Name == "Product2"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

