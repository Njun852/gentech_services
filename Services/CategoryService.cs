using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gentech_services.Models;
using gentech_services.Repositories;

namespace gentech_services.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category?> GetByIdAsync(int categoryId)
        {
            return await _categoryRepository.GetByIdAsync(categoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string type)
        {
            return await _categoryRepository.GetByTypeAsync(type);
        }

        public async Task<IEnumerable<Category>> GetServiceCategoriesAsync()
        {
            return await _categoryRepository.GetByTypeAsync("Service");
        }

        public async Task<IEnumerable<Category>> GetProductCategoriesAsync()
        {
            return await _categoryRepository.GetByTypeAsync("Product");
        }

        public async Task<Category> CreateCategoryAsync(string name, string type)
        {
            // Validate type
            if (type != "Product" && type != "Service")
            {
                throw new InvalidOperationException("Type must be either 'Product' or 'Service'.");
            }

            // Check if category name already exists for this type
            if (await _categoryRepository.NameExistsAsync(name, type))
            {
                throw new InvalidOperationException($"Category '{name}' already exists for type '{type}'.");
            }

            var category = new Category
            {
                Name = name,
                Type = type
            };

            return await _categoryRepository.AddAsync(category);
        }

        public async Task<Category> UpdateCategoryAsync(int categoryId, string name, string type)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found.");
            }

            // Validate type
            if (type != "Product" && type != "Service")
            {
                throw new InvalidOperationException("Type must be either 'Product' or 'Service'.");
            }

            // Check if new name already exists for this type (excluding current category)
            var existingCategory = await _categoryRepository.GetByNameAndTypeAsync(name, type);
            if (existingCategory != null && existingCategory.CategoryID != categoryId)
            {
                throw new InvalidOperationException($"Category '{name}' already exists for type '{type}'.");
            }

            category.Name = name;
            category.Type = type;

            await _categoryRepository.UpdateAsync(category);
            return category;
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found.");
            }

            // Note: This might fail if there are products/services using this category
            // due to foreign key constraints. This is intentional to preserve data integrity.
            await _categoryRepository.DeleteAsync(category);
        }
    }
}
