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

        private const int _orderTypeId = 1;


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

        public IActionResult Dashboard()
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
                    OrderType = _context.OrderTypes.Single(x => x.OrderTypeID == _orderTypeId),
                    TransactionDateTime = DateTime.Now, 
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
        public async Task<IActionResult> GetItems()
        {

            // Only retrieve items with OrderTypeId = 1
            var items = await _context.Items
                                .Where(item => item.OrderTypeId == _orderTypeId && item.Enabled)
                                .ToListAsync();
            return Json(items);
        }



        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
           
            // Only retrieve items with OrderTypeId = 1
            var menuItems = await _context.Items
                                .Where(item => item.OrderTypeId == _orderTypeId && item.Enabled)
                                .ToListAsync();
            return Json(menuItems);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemTotals()
        {
            try
            {
                var orders = await _context.Orders.Where(x => x.OrderItems.Any(y => y.Item.Enabled && y.Item.OrderTypeId == _orderTypeId)).Include(x => x.OrderItems).ThenInclude(y => y.Item).ToListAsync();
                var itemTotals = new Dictionary<string, ItemTotalViewModel>();

                foreach (var order in orders)
                {
                    
                            foreach (var item in order.OrderItems)
                            {
                                if (!itemTotals.ContainsKey(item.Item.ItemName))
                                {
                                    itemTotals[item.Item.ItemName] = new ItemTotalViewModel
                                    {
                                        ItemName = item.Item.ItemName,
                                        QuantitySold = 0,
                                        TotalRevenue = 0
                                    };
                                }
                                itemTotals[item.Item.ItemName].QuantitySold += item.Quantity;
                                itemTotals[item.Item.ItemName].TotalRevenue += item.Quantity * item.UnitPrice;
                            }
                        
                }
                var finalTotals = itemTotals.Values.Select(v => new
                {
                    itemName = v.ItemName,
                    quantitySold = v.QuantitySold,
                    totalRevenue = v.TotalRevenue
                }).ToList();

                return Json(finalTotals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get item totals.");
                return StatusCode(500, "An error occurred while retrieving item totals.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                // Get all orders with their related order items and item details
                var orders = await _context.Orders
                    .Where(o => o.OrderTypeID == _orderTypeId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Item)
                    .OrderByDescending(o => o.TransactionDateTime)
                    .ToListAsync();

                // Project the data into a new, anonymous object to ensure proper JSON serialization
                var finalOrders = orders.Select(o => new
                {
                    orderId = o.OrderID,
                    amountPaid = o.AmountPaid,
                    transactionDateTime = o.TransactionDateTime.ToString("MM/dd/yyyy hh:mm tt"),
                    orderItems = o.OrderItems.Select(oi => new
                    {
                        itemName = oi.Item.ItemName,
                        quantity = oi.Quantity,
                        unitPrice = oi.UnitPrice
                    }).ToList()
                }).ToList();

                return Json(finalOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders.");
                return StatusCode(500, "An error occurred while retrieving orders.");
            }
        }
    }
}
