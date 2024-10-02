using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechFixBackend.Dtos;
using TechFixBackend.Services;
using System.IdentityModel.Tokens.Jwt;

namespace TechFixBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                // Get the JWT token from the Authorization header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { Message = "Token is missing." });
                }

                // Decode the token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                Console.WriteLine(jwtToken);

                // Extract the user ID from the token
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name");
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID not found in the token." });
                }

                var userId = userIdClaim.Value;
                Console.WriteLine(userId);

                // Pass the user ID to the service method
                await _orderService.CreateOrderAsync(createOrderDto, userId);

                return Ok(new { Message = "Order created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message + " ha hah haaaaaaaah" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOrders(int pageNumber = 1, int pageSize = 10, string customerId = null)
        {
            try
            {
                var (orders, totalOrders) = await _orderService.GetAllOrdersAsync(pageNumber, pageSize, customerId);
                if (orders == null || !orders.Any()) return Ok(new { Message = "No orders found." });

                return Ok(new { totalOrders, orders });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("view/{orderId}")]
        public async Task<IActionResult> ViewOrder(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null) return NotFound(new { Message = "Order not found" });

            return Ok(order);
        }



        [HttpPut("update/{orderId}")]
        public async Task<IActionResult> UpdateOrder(string orderId, [FromBody] OrderUpdateDto updateDto)
        {
            try
            {
                await _orderService.UpdateOrderAsync(orderId, updateDto);
                return Ok(new { Message = "Order updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }




        [HttpPut("request-cancel/{orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId, [FromBody] RequestCancelOrderDto cancelOrderDto)
        {
            try
            {
                await _orderService.CancelRequestOrderAsync(orderId, cancelOrderDto);
                return Ok(new { Message = "Order cancelletion requested successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] OrderStatusUpdateDto statusUpdateDto)
        {
            try
            {
                await _orderService.UpdateOrderStatusAsync(orderId, statusUpdateDto.Status);
                return Ok(new { Message = "Order status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("update-item-status/{orderId}/{productId}")]
        public async Task<IActionResult> UpdateOrderItemStatus(string orderId, string productId, [FromBody] OrderItemStatusUpdateDto statusUpdateDto)
        {
            try
            {
                await _orderService.UpdateOrderItemStatusAsync(orderId, productId, statusUpdateDto.Status);
                return Ok(new { Message = "Order item status updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
