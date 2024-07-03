using InventoryApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext _dbContext;

        public InventoryController(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            var orders = _dbContext.Orders.ToList();
            return Ok(orders);
        }
    }
}
