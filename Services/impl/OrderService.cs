using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechFixBackend.Dtos;
using TechFixBackend._Models;
using TechFixBackend.Repository;

namespace TechFixBackend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IUserRepository userRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }


        public async Task CreateOrderAsync(CreateOrderDto createOrderDto, String id)
        {
            var customer = await _userRepository.GetUserByIdAsync(id);

            if (customer == null)
                throw new Exception("Customer not found.");

            var orderItems = new List<OrderItem>();

            foreach (var item in createOrderDto.Items)
            {
                var product = await _productRepository.GetProductByIdAsync(item.ProductId);

                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found.");

                var vendor = await _userRepository.GetUserByIdAsync(product.VendorId);

                if (vendor == null)
                    throw new Exception($"Vendor with ID {product.VendorId} not found.");

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    VendorId = product.VendorId
                });
            }

            var order = new Order
            {
                CustomerId = id,
                Items = orderItems,
                DeliveryAddress = createOrderDto.DeliveryAddress,
            };

            order.TotalAmount = order.Items.Sum(item => item.TotalPrice);

            await _orderRepository.CreateOrderAsync(order);
        }




        public async Task<(List<GetOrderDetailsDto> orders, long totalOrders)> GetAllOrdersAsync(int pageNumber, int pageSize, string customerId = null)
        {
            var (orders, totalOrders) = await _orderRepository.GetAllOrdersAsync(pageNumber, pageSize, customerId);

            if (orders == null || !orders.Any())
            {
                return (new List<GetOrderDetailsDto>(), 0);
            }

            var orderDtos = new List<GetOrderDetailsDto>();
            foreach (var order in orders)
            {
                var orderItems = new List<GetOrderItemDto>();
                foreach (var item in order.Items)
                {
                    var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {item.ProductId} not found.");
                    }
                    var vendor = await _userRepository.GetUserByIdAsync(product.VendorId);
                    if (vendor == null)
                    {
                        throw new Exception($"Vendor with ID {product.VendorId} not found.");
                    }

                    orderItems.Add(new GetOrderItemDto
                    {
                        ProductId = item.ProductId,
                        Product = new ProductWithVendorDto
                        {
                            Vendor = vendor,
                            ProductName = product.ProductName,
                        },
                        Quantity = item.Quantity,
                        Price = item.Price,
                        TotalPrice = item.TotalPrice,
                        Status = item.Status
                    });
                }
                var customer = await _userRepository.GetUserByIdAsync(order.CustomerId);
                if (customer == null)
                {
                    throw new Exception($"Customer with ID {order.CustomerId} not found.");
                }

                orderDtos.Add(new GetOrderDetailsDto
                {
                    OrderId = order.Id,
                    Customer = customer,
                    DeliveryAddress = order.DeliveryAddress,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                    Items = orderItems,
                    OrderDate = order.OrderDate,
                    DeliveryStatus = order.DeliveryStatus,
                    DispatchedDate = order.DispatchedDate
                });
            }

            return (orderDtos, totalOrders);
        }




        public async Task<GetOrderDetailsDto> GetOrderByIdAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            var customer = await _userRepository.GetUserByIdAsync(order.CustomerId);
            if (customer == null)
            {
                throw new Exception($"Customer with ID {order.CustomerId} not found.");
            }

            var orderItems = new List<GetOrderItemDto>();
            foreach (var item in order.Items)
            {

                var product = await _productRepository.GetProductByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                }


                var vendor = await _userRepository.GetUserByIdAsync(product.VendorId);
                if (vendor == null)
                {
                    throw new Exception($"Vendor with ID {product.VendorId} not found.");
                }

                orderItems.Add(new GetOrderItemDto
                {
                    ProductId = item.ProductId,
                    Product = new ProductWithVendorDto
                    {
                        Vendor = vendor,
                        ProductName = product.ProductName,
                        ProductImageUrl = product.ProductImageUrl

                    },
                    Quantity = item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice,
                    Status = item.Status
                });
            }

            var orderDto = new GetOrderDetailsDto
            {
                OrderId = order.Id,
                Customer = customer,
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = orderItems,
                OrderDate = order.OrderDate,
                DeliveryStatus = order.DeliveryStatus,
                DispatchedDate = order.DispatchedDate
            };

            return orderDto;
        }

        // Updated: Validate ProductId when updating order items.
        public async Task UpdateOrderAsync(string orderId, OrderUpdateDto updateDto)
        {
            // var existingOrder = await GetOrderByIdAsync(orderId);
            // if (existingOrder == null) throw new Exception("Order not found.");

            // if (!string.IsNullOrEmpty(updateDto.DeliveryAddress))
            // {
            //     existingOrder.DeliveryAddress = updateDto.DeliveryAddress;
            // }

            // if (updateDto.Items != null && updateDto.Items.Any())
            // {
            //     foreach (var item in updateDto.Items)
            //     {
            //         // Validate if the product exists before updating the item
            //         var product = await _productRepository.GetProductByIdAsync(item.ProductId);
            //         if (product == null)
            //             throw new Exception($"Product with ID {item.ProductId} not found.");

            //         var existingItem = existingOrder.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            //         if (existingItem != null)
            //         {
            //             if (existingItem.Status == "Processing")
            //             {
            //                 existingItem.Quantity = item.Quantity;
            //             }
            //             else
            //             {
            //                 throw new InvalidOperationException($"Cannot update item '{existingItem.ProductId}' as it is not in 'Processing' status.");
            //             }
            //         }
            //         else
            //         {
            //             existingOrder.Items.Add(new OrderItem
            //             {
            //                 ProductId = item.ProductId,
            //                 Quantity = item.Quantity,
            //                 Price = item.Price
            //             });
            //         }
            //     }
            // }

            // existingOrder.TotalAmount = existingOrder.Items.Sum(i => i.TotalPrice);
            // await _orderRepository.UpdateOrderAsync(existingOrder);
        }

        public async Task CancelRequestOrderAsync(string orderId, RequestCancelOrderDto cancelOrderDto)
        {
            // Fetch the actual Order entity, not the DTO
            var existingOrder = await _orderRepository.GetOrderByIdAsync(orderId);
            if (existingOrder == null) throw new Exception("Order not found.");

            // Initialize the Cancellation property if it's null
            if (existingOrder.Cancellation == null)
            {
                existingOrder.Cancellation = new Cancellation();
            }

            if (!string.IsNullOrEmpty(cancelOrderDto.Reason))
            {
                existingOrder.Cancellation.Reason = cancelOrderDto.Reason;
            }

            // Set the cancellation as requested
            existingOrder.Cancellation.Requested = true;
            existingOrder.Cancellation.Status = "requested";
            existingOrder.Cancellation.RequestedAt = DateTime.UtcNow; // Optionally add the timestamp

            // Persist the updated order entity to the database
            await _orderRepository.UpdateOrderAsync(existingOrder);
        }

        public async Task UpdateOrderStatusAsync(string orderId, string status)
        {
            // var existingOrder = await GetOrderByIdAsync(orderId);
            // if (existingOrder == null) throw new Exception("Order not found.");

            // existingOrder.Status = status;
            // if (status == "Shipped") existingOrder.DispatchedDate = DateTime.UtcNow;

            // await _orderRepository.UpdateOrderAsync(existingOrder);
        }

        public async Task UpdateOrderItemStatusAsync(string orderId, string productId, string status)
        {
            // var existingOrder = await GetOrderByIdAsync(orderId);
            // if (existingOrder == null) throw new Exception("Order not found.");

            // var orderItem = existingOrder.Items.FirstOrDefault(i => i.ProductId == productId);
            // if (orderItem == null) throw new Exception("Order item not found.");

            // orderItem.Status = status;
            // await _orderRepository.UpdateOrderAsync(existingOrder);
        }


        public async Task<List<VendorOrderDto>> GetOrdersByVendorIdAsync(string vendorId)
        {
            var orders = await _orderRepository.GetOrdersByVendorIdAsync(vendorId);
            if (orders == null || orders.Count == 0)
            {
                return null;
            }

            var orderDtos = orders.Select(order => new VendorOrderDto
            {
                OrderId = order.Id.ToString(),
                OrderDate = order.OrderDate,
                Items = order.Items
                            .Where(item => item.VendorId == vendorId)
                            .Select(item => new VendorOrderItemDto
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                TotalPrice = item.Quantity * item.Price
                            }).ToList()
            })
            .Where(orderDto => orderDto.Items.Count > 0)
            .ToList(); 
           
            return orderDtos;
        }


    }
}