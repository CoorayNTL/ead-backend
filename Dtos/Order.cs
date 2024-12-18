/*
 * File: OrderDtos.cs
 * Project: Healthy Bites
 * Description: This file contains Data Transfer Objects (DTOs) related to order operations in the HealthyBites system.
 *              DTOs are used to transfer data between the client and server layers. They are designed to keep only necessary 
 *              information and omit sensitive or internal fields, ensuring proper abstraction and clean data handling. 
 *              These DTOs are involved in creating orders, updating order statuses, handling cancellations, and retrieving order details.
 * 
 */

namespace HealthyBites.Dtos
{
    public class CreateOrderDto
    {
        // public string CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public string DeliveryAddress { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
        public string Status { get; set; } = "Processing";
    }



    public class OrderStatusUpdateDto
    {
        public string Status { get; set; }
    }


    public class RequestCancelOrderDto
    {
        public string Reason { get; set; } // Optional cancellation reason provided by the customer
    }

    public class CancellationResponseDto
    {
        public string Response { get; set; }
    }

    public class GetOrderItemDto
    {
        public string ProductId { get; set; }
        public ProductWithVendorDto Product { get; set; } // Use ProductWithVendorDto for product details
        public int Quantity { get; set; }
        public float Price { get; set; }
        public float TotalPrice { get; set; }
        public string Status { get; set; }
    }

    public class GetOrderDetailsDto
    {
        public string OrderId { get; set; }
        public User Customer { get; set; }
        public string DeliveryAddress { get; set; }
        public float TotalAmount { get; set; }
        public string Status { get; set; }
        public List<GetOrderItemDto> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime? DispatchedDate { get; set; }


    }

    public class GetCancelOrderDetailsDto
    {
        public string OrderId { get; set; }
        public User Customer { get; set; }
        public string DeliveryAddress { get; set; }
        public float TotalAmount { get; set; }
        public string Status { get; set; }
        public List<GetOrderItemDto> Items { get; set; }
        public DateTime OrderDate { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime? DispatchedDate { get; set; }
        public CancellationDetailsDto Cancellation { get; set; }

    }

    public class VendorOrderDto
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public List<VendorOrderItemDto> Items { get; set; }
    }

    public class VendorOrderItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float TotalPrice { get; set; }
        public string Status { get; set; }
    }
}