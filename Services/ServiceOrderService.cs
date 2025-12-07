using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gentech_services.Models;
using gentech_services.Repositories;

namespace gentech_services.Services
{
    public class ServiceOrderService
    {
        private readonly IServiceOrderRepository _serviceOrderRepository;
        private readonly IServiceRepository _serviceRepository;

        public ServiceOrderService(IServiceOrderRepository serviceOrderRepository, IServiceRepository serviceRepository)
        {
            _serviceOrderRepository = serviceOrderRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<ServiceOrder?> GetByIdAsync(int serviceOrderId)
        {
            return await _serviceOrderRepository.GetByIdAsync(serviceOrderId);
        }

        public async Task<IEnumerable<ServiceOrder>> GetAllServiceOrdersAsync()
        {
            return await _serviceOrderRepository.GetAllAsync();
        }

        public async Task<IEnumerable<ServiceOrder>> GetByStatusAsync(string status)
        {
            return await _serviceOrderRepository.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<ServiceOrder>> GetRecentOrdersAsync(int count = 10)
        {
            return await _serviceOrderRepository.GetRecentOrdersAsync(count);
        }

        public async Task<ServiceOrder> CreateServiceOrderAsync(
            string fullName,
            string email,
            string phone,
            DateTime scheduledAt,
            string? deviceDescription,
            string? issueNotes,
            List<(int ServiceID, int Quantity, decimal UnitPrice)> serviceItems)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(fullName))
                throw new InvalidOperationException("Customer name is required.");

            if (string.IsNullOrWhiteSpace(email))
                throw new InvalidOperationException("Customer email is required.");

            if (string.IsNullOrWhiteSpace(phone))
                throw new InvalidOperationException("Customer phone is required.");

            if (scheduledAt < DateTime.Now.Date)
                throw new InvalidOperationException("Scheduled date cannot be in the past.");

            if (serviceItems == null || !serviceItems.Any())
                throw new InvalidOperationException("At least one service must be selected.");

            // Validate all services exist
            foreach (var item in serviceItems)
            {
                var service = await _serviceRepository.GetByIdAsync(item.ServiceID);
                if (service == null)
                    throw new InvalidOperationException($"Service with ID {item.ServiceID} not found.");
            }

            // Create service order
            var serviceOrder = new ServiceOrder
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                ScheduledAt = scheduledAt,
                DeviceDescription = deviceDescription,
                IssueNotes = issueNotes,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                ServiceOrderItems = new List<ServiceOrderItem>()
            };

            // Add service order items
            foreach (var item in serviceItems)
            {
                var totalPrice = item.UnitPrice * item.Quantity;
                serviceOrder.ServiceOrderItems.Add(new ServiceOrderItem
                {
                    ServiceID = item.ServiceID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = totalPrice
                });
            }

            return await _serviceOrderRepository.AddAsync(serviceOrder);
        }

        public async Task<ServiceOrder> UpdateServiceOrderAsync(
            int serviceOrderId,
            string fullName,
            string email,
            string phone,
            DateTime scheduledAt,
            string status,
            string? deviceDescription,
            string? issueNotes)
        {
            var serviceOrder = await _serviceOrderRepository.GetByIdAsync(serviceOrderId);
            if (serviceOrder == null)
                throw new InvalidOperationException($"Service order with ID {serviceOrderId} not found.");

            serviceOrder.FullName = fullName;
            serviceOrder.Email = email;
            serviceOrder.Phone = phone;
            serviceOrder.ScheduledAt = scheduledAt;
            serviceOrder.Status = status;
            serviceOrder.DeviceDescription = deviceDescription;
            serviceOrder.IssueNotes = issueNotes;

            await _serviceOrderRepository.UpdateAsync(serviceOrder);
            return serviceOrder;
        }

        public async Task UpdateStatusAsync(int serviceOrderId, string status)
        {
            var serviceOrder = await _serviceOrderRepository.GetByIdAsync(serviceOrderId);
            if (serviceOrder == null)
                throw new InvalidOperationException($"Service order with ID {serviceOrderId} not found.");

            // Validate status
            var validStatuses = new[] { "Pending", "In Progress", "Completed", "Cancelled" };
            if (!validStatuses.Contains(status))
                throw new InvalidOperationException($"Invalid status: {status}");

            serviceOrder.Status = status;
            await _serviceOrderRepository.UpdateAsync(serviceOrder);
        }

        public async Task DeleteServiceOrderAsync(int serviceOrderId)
        {
            var serviceOrder = await _serviceOrderRepository.GetByIdAsync(serviceOrderId);
            if (serviceOrder == null)
                throw new InvalidOperationException($"Service order with ID {serviceOrderId} not found.");

            await _serviceOrderRepository.DeleteAsync(serviceOrder);
        }
    }
}
