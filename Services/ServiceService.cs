using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Models;
using gentech_services.Repositories;

namespace gentech_services.Services
{
    public class ServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ServiceService(IServiceRepository serviceRepository, ICategoryRepository categoryRepository)
        {
            _serviceRepository = serviceRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<Service?> GetByIdAsync(int serviceId)
        {
            return await _serviceRepository.GetByIdAsync(serviceId);
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _serviceRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _serviceRepository.GetActiveServicesAsync();
        }

        public async Task<IEnumerable<Service>> GetServicesByCategoryAsync(int categoryId)
        {
            return await _serviceRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<IEnumerable<Service>> SearchServicesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetActiveServicesAsync();
            }

            return await _serviceRepository.SearchByNameAsync(searchTerm);
        }

        public async Task<Service> CreateServiceAsync(string name, string description, decimal price, int categoryId)
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found.");
            }

            // Validate price
            if (price < 0)
            {
                throw new InvalidOperationException("Price cannot be negative.");
            }

            var service = new Service
            {
                Name = name,
                Description = description,
                Price = price,
                CategoryID = categoryId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return await _serviceRepository.AddAsync(service);
        }

        public async Task<Service> UpdateServiceAsync(int serviceId, string name, string description, decimal price, int categoryId, bool isActive)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new InvalidOperationException($"Service with ID {serviceId} not found.");
            }

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found.");
            }

            // Validate price
            if (price < 0)
            {
                throw new InvalidOperationException("Price cannot be negative.");
            }

            service.Name = name;
            service.Description = description;
            service.Price = price;
            service.CategoryID = categoryId;
            service.IsActive = isActive;
            service.UpdatedAt = DateTime.Now;

            await _serviceRepository.UpdateAsync(service);
            return service;
        }

        public async Task ActivateServiceAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new InvalidOperationException($"Service with ID {serviceId} not found.");
            }

            service.IsActive = true;
            service.UpdatedAt = DateTime.Now;
            await _serviceRepository.UpdateAsync(service);
        }

        public async Task DeactivateServiceAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new InvalidOperationException($"Service with ID {serviceId} not found.");
            }

            service.IsActive = false;
            service.UpdatedAt = DateTime.Now;
            await _serviceRepository.UpdateAsync(service);
        }

        public async Task DeleteServiceAsync(int serviceId)
        {
            var service = await _serviceRepository.GetByIdAsync(serviceId);
            if (service == null)
            {
                throw new InvalidOperationException($"Service with ID {serviceId} not found.");
            }

            await _serviceRepository.DeleteAsync(service);
        }
    }
}
