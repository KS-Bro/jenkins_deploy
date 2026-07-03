using Microsoft.AspNetCore.Mvc;

namespace Jenkins_Deploy.Controllers
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        // Static in-memory store so it persists across requests within the app's lifetime.
        private static readonly List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Keyboard", Price = 25.50m },
            new Product { Id = 2, Name = "Mouse", Price = 15.00m },
            new Product { Id = 3, Name = "Monitor", Price = 75.00m },
            new Product { Id = 4, Name = "RAM", Price = 35.00m },
            new Product { Id = 5, Name = "Hard Disc", Price = 115.00m },
            new Product { Id = 6, Name = "Cabinet", Price = 15.00m },
            new Product { Id = 7, Name = "Speaker", Price = 32.00m },
            new Product { Id = 8, Name = "MotherBoard", Price = 132.00m },
            new Product { Id = 8, Name = "Power Supply Unit", Price = 12.00m }
        };

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetAll()
        {
            return Ok(_products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public ActionResult<Product> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Id must be a positive integer.");
            }

            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound($"Product with id {id} was not found.");
            }

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        public ActionResult<Product> Create([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product payload cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest("Product name is required.");
            }

            if (product.Price < 0)
            {
                return BadRequest("Product price cannot be negative.");
            }

            product.Id = _products.Count == 0 ? 1 : _products.Max(p => p.Id) + 1;
            _products.Add(product);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product payload cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                return BadRequest("Product name is required.");
            }

            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing == null)
            {
                return NotFound($"Product with id {id} was not found.");
            }

            existing.Name = product.Name;
            existing.Price = product.Price;

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing == null)
            {
                return NotFound($"Product with id {id} was not found.");
            }

            _products.Remove(existing);
            return NoContent();
        }
    }
}
