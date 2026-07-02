using Jenkins_Deploy.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace Jenkins_deploy_Test
{
    public class ProductsControllerTests
    {
        private ProductsController CreateController()
        {
            return new ProductsController(NullLogger<ProductsController>.Instance);
        }

        // ---------------------- GetAll ----------------------

        [Fact]
        public void GetAll_ReturnsOkResult_WithProductList()
        {
            // Positive scenario
            var controller = CreateController();

            var result = controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.NotEmpty(products);
        }

        // ---------------------- GetById ----------------------

        [Fact]
        public void GetById_ExistingId_ReturnsOkWithProduct()
        {
            // Positive scenario
            var controller = CreateController();

            var result = controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var product = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(1, product.Id);  
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNotFound()
        {
            // Negative scenario
            var controller = CreateController();

            var result = controller.GetById(9999);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetById_NegativeOrZeroId_ReturnsBadRequest()
        {
            // Negative scenario
            var controller = CreateController();

            var result = controller.GetById(0);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ---------------------- Create ----------------------

        [Fact]
        public void Create_ValidProduct_ReturnsCreatedAtAction()
        {
            // Positive scenario
            var controller = CreateController();
            var newProduct = new Product { Name = "Monitor", Price = 120.00m };

            var result = controller.Create(newProduct);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var product = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal("Monitor", product.Name);
            Assert.True(product.Id > 0);
        }

        [Fact]
        public void Create_NullProduct_ReturnsBadRequest()
        {
            // Negative scenario
            var controller = CreateController();

            var result = controller.Create(null!);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Create_EmptyName_ReturnsBadRequest()
        {
            // Negative scenario
            var controller = CreateController();
            var newProduct = new Product { Name = "", Price = 10.00m };

            var result = controller.Create(newProduct);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Create_NegativePrice_ReturnsBadRequest()
        {
            // Negative scenario
            var controller = CreateController();
            var newProduct = new Product { Name = "Webcam", Price = -5.00m };

            var result = controller.Create(newProduct);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // ---------------------- Update ----------------------

        [Fact]
        public void Update_ExistingProduct_ReturnsNoContent()
        {
            // Positive scenario
            var controller = CreateController();
            var updatedProduct = new Product { Name = "Keyboard Pro", Price = 35.00m };

            var result = controller.Update(1, updatedProduct);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Update_NonExistingProduct_ReturnsNotFound()
        {
            // Negative scenario
            var controller = CreateController();
            var updatedProduct = new Product { Name = "Ghost Product", Price = 10.00m };

            var result = controller.Update(9999, updatedProduct);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Update_EmptyName_ReturnsBadRequest()
        {
            // Negative scenario
            var controller = CreateController();
            var updatedProduct = new Product { Name = "", Price = 10.00m };

            var result = controller.Update(1, updatedProduct);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void Update_NullProduct_ReturnsBadRequest()
        {
            // Negative scenario
            var controller = CreateController();

            var result = controller.Update(1, null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        // ---------------------- Delete ----------------------

        [Fact]
        public void Delete_ExistingProduct_ReturnsNoContent()
        {
            // Positive scenario
            var controller = CreateController();
            // First create a product to ensure it exists, then delete it.
            var created = controller.Create(new Product { Name = "Temp Item", Price = 5.00m });
            var createdResult = Assert.IsType<CreatedAtActionResult>(created.Result);
            var product = Assert.IsType<Product>(createdResult.Value);

            var result = controller.Delete(product.Id);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_NonExistingProduct_ReturnsNotFound()
        {
            // Negative scenario
            var controller = CreateController();

            var result = controller.Delete(9999);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
