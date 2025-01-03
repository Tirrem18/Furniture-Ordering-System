using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThAmCo.Products.Controllers;
using ThAmCo.Products.Services.ProductsRepo;

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
        public void GetRestrictedData_ShouldReturnOkResult()
        {
            // Act
            var result = _controller.GetRestrictedData();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Users can Order Products after implementation", 
                            ((dynamic)okResult.Value).Message);
        }

        [TestMethod]
        public async Task GetProducts_ShouldReturnOkResultWithProducts()
        {
            // Arrange
            var fakeProducts = new List<string> { "Product1", "Product2" };
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync())
                             .ReturnsAsync(fakeProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            CollectionAssert.AreEqual(fakeProducts, okResult.Value as List<string>);
        }

        [TestMethod]
        public async Task GetProducts_ShouldReturnBadRequestOnInvalidOperationException()
        {
            // Arrange
            _mockProductsRepo.Setup(repo => repo.GetProductsAsync())
                             .ThrowsAsync(new InvalidOperationException("Invalid operation"));

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.IsNotNull(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Invalid operation", ((dynamic)badRequestResult.Value).Error);
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
}
