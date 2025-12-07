using gentech_services.Models;
using gentech_services.Repositories;

namespace gentech_services.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly InventoryLogService _inventoryLogService;

        public ProductService(IProductRepository productRepository, InventoryLogService inventoryLogService)
        {
            _productRepository = productRepository;
            _inventoryLogService = inventoryLogService;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _productRepository.GetActiveProductsAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _productRepository.GetLowStockProductsAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _productRepository.GetByCategoryAsync(categoryId);
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<Product?> GetProductBySKUAsync(string sku)
        {
            return await _productRepository.GetBySKUAsync(sku);
        }

        public async Task<Product> CreateProductAsync(
            string name,
            string? description,
            decimal price,
            string sku,
            int stockQuantity,
            int lowStockLevel,
            int categoryId,
            int createdBy)
        {
            // Validate SKU uniqueness
            if (await _productRepository.SKUExistsAsync(sku))
            {
                throw new InvalidOperationException($"Product with SKU '{sku}' already exists.");
            }

            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                SKU = sku,
                StockQuantity = stockQuantity,
                LowStockLevel = lowStockLevel,
                CategoryID = categoryId,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var createdProduct = await _productRepository.AddAsync(product);

            // Log initial stock
            if (stockQuantity > 0)
            {
                await _inventoryLogService.CreateLogAsync(
                    createdProduct.ProductID,
                    "Stock In",
                    stockQuantity,
                    0,
                    stockQuantity,
                    createdBy,
                    "Initial stock"
                );
            }

            return createdProduct;
        }

        public async Task<Product> UpdateProductAsync(
            int productId,
            string name,
            string? description,
            decimal price,
            string sku,
            int lowStockLevel,
            int categoryId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found.");
            }

            // Check SKU uniqueness if changed
            if (product.SKU != sku && await _productRepository.SKUExistsAsync(sku))
            {
                throw new InvalidOperationException($"Product with SKU '{sku}' already exists.");
            }

            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.SKU = sku;
            product.LowStockLevel = lowStockLevel;
            product.CategoryID = categoryId;
            product.UpdatedAt = DateTime.Now;

            await _productRepository.UpdateAsync(product);
            return product;
        }

        public async Task<Product> StockInAsync(int productId, int quantity, int userId, string? reason = null)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Stock in quantity must be greater than 0.");
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found.");
            }

            int previousQuantity = product.StockQuantity;
            product.StockQuantity += quantity;
            product.UpdatedAt = DateTime.Now;

            await _productRepository.UpdateAsync(product);

            // Log the stock in
            await _inventoryLogService.CreateLogAsync(
                productId,
                "Stock In",
                quantity,
                previousQuantity,
                product.StockQuantity,
                userId,
                reason
            );

            return product;
        }

        public async Task<Product> StockOutAsync(int productId, int quantity, int userId, string? reason = null)
        {
            if (quantity <= 0)
            {
                throw new InvalidOperationException("Stock out quantity must be greater than 0.");
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found.");
            }

            if (product.StockQuantity < quantity)
            {
                throw new InvalidOperationException($"Insufficient stock. Current stock: {product.StockQuantity}, requested: {quantity}");
            }

            int previousQuantity = product.StockQuantity;
            product.StockQuantity -= quantity;
            product.UpdatedAt = DateTime.Now;

            await _productRepository.UpdateAsync(product);

            // Log the stock out
            await _inventoryLogService.CreateLogAsync(
                productId,
                "Stock Out",
                quantity,
                previousQuantity,
                product.StockQuantity,
                userId,
                reason
            );

            return product;
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found.");
            }

            // Soft delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.Now;

            await _productRepository.UpdateAsync(product);
        }

        public async Task<Product> RestoreProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException($"Product with ID {productId} not found.");
            }

            product.IsActive = true;
            product.UpdatedAt = DateTime.Now;

            await _productRepository.UpdateAsync(product);
            return product;
        }
    }
}
