using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gentech_services.Models;
using gentech_services.Repositories;

namespace gentech_services.Services
{
    public class ProductOrderService
    {
        private readonly IProductOrderRepository _productOrderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ProductService _productService;

        public ProductOrderService(
            IProductOrderRepository productOrderRepository,
            IProductRepository productRepository,
            ProductService productService)
        {
            _productOrderRepository = productOrderRepository;
            _productRepository = productRepository;
            _productService = productService;
        }

        public async Task<ProductOrder?> GetByIdAsync(int productOrderId)
        {
            return await _productOrderRepository.GetByIdAsync(productOrderId);
        }

        public async Task<IEnumerable<ProductOrder>> GetAllProductOrdersAsync()
        {
            return await _productOrderRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ProductOrder>> GetByStatusAsync(string status)
        {
            return await _productOrderRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<ProductOrder>> GetRecentOrdersAsync(int count = 10)
        {
            return await _productOrderRepository.GetRecentOrdersAsync(count);
        }

        public async Task<ProductOrder> CreateProductOrderAsync(
            string fullName,
            string email,
            string phone,
            List<(int ProductID, int Quantity, decimal UnitPrice)> orderItems,
            int? userId = null)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(fullName))
                throw new InvalidOperationException("Customer name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Customer email is required.");

            if (string.IsNullOrWhiteSpace(phone))
                throw new InvalidOperationException("Customer phone is required.");

            if (orderItems == null || !orderItems.Any())
                throw new InvalidOperationException("At least one product must be selected.");

            // Validate all products exist and have sufficient stock
            decimal totalAmount = 0;
            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductID);
                if (product == null)
                    throw new InvalidOperationException($"Product with ID {item.ProductID} not found.");

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for {product.Name}. Available: {product.StockQuantity}, Requested: {item.Quantity}");

                totalAmount += item.UnitPrice * item.Quantity;
            }

            // Create product order
            var productOrder = new ProductOrder
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                TotalAmount = totalAmount,
                Status = "Paid", // Product orders are paid immediately upon payment
                CreatedAt = DateTime.Now,
                ProductOrderItems = new List<ProductOrderItem>()
            };

            // Add product order items
            foreach (var item in orderItems)
            {
                var totalPrice = item.UnitPrice * item.Quantity;
                productOrder.ProductOrderItems.Add(new ProductOrderItem
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = totalPrice
                });
            }

            // Save the order first
            var createdOrder = await _productOrderRepository.AddAsync(productOrder);

            // Reduce stock for each product (stock out) and log to inventory
            int logUserId = userId ?? AuthenticationService.Instance.CurrentUser?.UserID ?? 1;
            foreach (var item in orderItems)
            {
                await _productService.StockOutAsync(
                    item.ProductID,
                    item.Quantity,
                    logUserId,
                    $"Product order #{createdOrder.ProductOrderID}"
                );
            }

            return createdOrder;
        }

        public async Task<ProductOrder> VoidOrderAsync(int productOrderId, int? userId = null)
        {
            var order = await _productOrderRepository.GetByIdAsync(productOrderId);
            if (order == null)
                throw new InvalidOperationException($"Product order with ID {productOrderId} not found.");

            if (order.Status == "Voided")
                throw new InvalidOperationException("Order is already voided.");

            if (order.Status == "Partially Returned" || order.Status == "Fully Returned")
                throw new InvalidOperationException("Cannot void a returned order.");

            // Restore stock for each item and log to inventory
            int logUserId = userId ?? AuthenticationService.Instance.CurrentUser?.UserID ?? 1;
            if (order.ProductOrderItems != null)
            {
                foreach (var item in order.ProductOrderItems)
                {
                    await _productService.StockInAsync(
                        item.ProductID,
                        item.Quantity,
                        logUserId,
                        $"Voided order #{productOrderId}"
                    );
                }
            }

            // Update order status
            order.Status = "Voided";
            await _productOrderRepository.UpdateAsync(order);

            return order;
        }

        public async Task<ProductOrder> ProcessReturnAsync(
            int productOrderId,
            List<(int ProductID, int Quantity)> returnItems,
            int? userId = null)
        {
            var order = await _productOrderRepository.GetByIdAsync(productOrderId);
            if (order == null)
                throw new InvalidOperationException($"Product order with ID {productOrderId} not found.");

            if (order.Status == "Voided")
                throw new InvalidOperationException("Cannot return a voided order.");

            // Validate return items
            if (returnItems == null || !returnItems.Any())
                throw new InvalidOperationException("At least one item must be selected for return.");

            // Restore stock for returned items and log to inventory
            int logUserId = userId ?? AuthenticationService.Instance.CurrentUser?.UserID ?? 1;
            foreach (var returnItem in returnItems)
            {
                var orderItem = order.ProductOrderItems?.FirstOrDefault(oi => oi.ProductID == returnItem.ProductID);
                if (orderItem == null)
                    throw new InvalidOperationException($"Product with ID {returnItem.ProductID} not found in this order.");

                if (returnItem.Quantity > orderItem.Quantity)
                    throw new InvalidOperationException($"Cannot return more items than ordered for product ID {returnItem.ProductID}.");

                await _productService.StockInAsync(
                    returnItem.ProductID,
                    returnItem.Quantity,
                    logUserId,
                    $"Return from order #{productOrderId}"
                );
            }

            // Determine if fully or partially returned
            bool fullyReturned = true;
            if (order.ProductOrderItems != null)
            {
                foreach (var orderItem in order.ProductOrderItems)
                {
                    var returnItem = returnItems.FirstOrDefault(ri => ri.ProductID == orderItem.ProductID);
                    if (returnItem.ProductID == 0 || returnItem.Quantity < orderItem.Quantity)
                    {
                        fullyReturned = false;
                        break;
                    }
                }
            }

            // Update order status
            order.Status = fullyReturned ? "Fully Returned" : "Partially Returned";
            await _productOrderRepository.UpdateAsync(order);

            return order;
        }

        public async Task DeleteProductOrderAsync(int productOrderId)
        {
            var order = await _productOrderRepository.GetByIdAsync(productOrderId);
            if (order == null)
                throw new InvalidOperationException($"Product order with ID {productOrderId} not found.");

            await _productOrderRepository.DeleteAsync(order);
        }
    }
}
