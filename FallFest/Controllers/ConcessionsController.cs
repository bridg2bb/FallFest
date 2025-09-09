using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq; // Added for the Where() method
using FallFest.Data;
using FallFest.Models;
using Microsoft.EntityFrameworkCore;

namespace FallFest.Controllers
{
    public class ConcessionsController : Controller
    {
        private readonly ILogger<ConcessionsController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        // Constructor with Dependency Injection
        public ConcessionsController(
            ILogger<ConcessionsController> logger,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        // The main action to render the view
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrder([FromBody] OrderRequestViewModel orderData)
        {
            if (orderData == null || orderData.OrderItems == null || orderData.OrderItems.Count == 0)
            {
                _logger.LogError("Invalid order data received.");
                return BadRequest("Invalid order data.");
            }

            try
            {
                var newOrder = new Order
                {
                    AmountPaid = orderData.AmountPaid,
                    AmountReturned = orderData.AmountReturned,
                    OrderType = _context.OrderTypes.Single(x => x.OrderTypeID == 1),
                    TransactionDateTime = DateTime.UtcNow, // Use UTC for consistency
                    OrderID = Guid.NewGuid()
                };

                _context.Orders.Add(newOrder);

                // 3. Process each item in the order
                foreach (var itemRequest in orderData.OrderItems)
                {
                    var item = await _context.Items.FirstOrDefaultAsync(i => i.ItemName == itemRequest.Name);
                    if (item == null)
                    {
                        return BadRequest($"Item '{itemRequest.Name}' not found in the lookup table.");
                    }

                    var orderItem = new OrderItem
                    {
                        OrderID = newOrder.OrderID,
                        ItemID = item.ItemID,
                        Quantity = itemRequest.Quantity,
                        UnitPrice = itemRequest.Price
                    };
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Order saved successfully with ID: {OrderId}", newOrder.OrderID);
                return Ok(new { success = true, message = "Order saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save order.");
                return StatusCode(500, "An error occurred while saving the order.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
           
            // Only retrieve items with OrderTypeId = 1
            var menuItems = await _context.Items
                                .Where(item => item.OrderTypeId == 1)
                                .ToListAsync();
            return Json(menuItems);
        }
    }
}
