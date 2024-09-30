using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechFixBackend._Models;
using TechFixBackend.Dtos;
using TechFixBackend.Repository;


namespace TechFixBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductCatRepository _producuCatsRepository;

        public ProductService(IProductRepository productRepository, IUserRepository userRepository , IProductCatRepository producuCatsRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
            _producuCatsRepository = producuCatsRepository;
        }

        // Retrieves all products with vendor details populated
        public async Task<(List<ProductWithVendorDto> products, long totalProducts)> GetAllProductsAsync(int pageNumber, int pageSize, string search = "")
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var products = await _productRepository.GetProductsAsync(pageNumber, pageSize, search);
            var productsWithVendors = new List<ProductWithVendorDto>();

            foreach (var product in products)
            {
                // Fetch vendor details individually using existing method
                var vendor = await _userRepository.GetUserByIdAsync(product.VendorId);
                var category = await _producuCatsRepository.GetProductCatByIdAsync(product.CategoryId);

                // Map product to include vendor information
                var productWithVendor = new ProductWithVendorDto
                {
                    Id = product.Id,
                    Vendor = vendor, // Populate vendor details
                    ProductName = product.ProductName,
                    ProductDescription = product.ProductDescription,
                    Category = category, // Populate category details
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    ProductStatus = product.ProductStatus,
                    ProductImageUrl = product.ProductImageUrl
                };

                productsWithVendors.Add(productWithVendor);
            }

            var totalProducts = await _productRepository.GetTotalProductsAsync(search);

            return (productsWithVendors, totalProducts);
        }

        // Retrieves a specific product by its ID with vendor details populated
        public async Task<ProductWithVendorDto> GetProductByIdAsync(string productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            if (product == null) return null;

            // Fetch the vendor based on VendorId  using existing method
            var vendor = await _userRepository.GetUserByIdAsync(product.VendorId);
            var category = await _producuCatsRepository.GetProductCatByIdAsync(product.CategoryId);

            // Map product to include vendor information
            return new ProductWithVendorDto
            {
                Id = product.Id,
                Vendor = vendor, // Populate vendor details
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                Category = category, // Populate category details
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                ProductStatus = product.ProductStatus,
                ProductImageUrl = product.ProductImageUrl
            };
        }

        public async Task<Product> CreateProductAsync(ProductCreateDto productDto)
        {
            var vendor = await _userRepository.GetUserByIdAsync(productDto.VendorId);
            if (vendor == null)
            {
                throw new Exception("Vendor not found");
            }

            var category = await _producuCatsRepository.GetProductCatByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            var product = new Product
            {
                VendorId = productDto.VendorId,
                ProductName = productDto.ProductName,
                ProductDescription = productDto.ProductDescription,
                CategoryId = productDto.CategoryId,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                ProductImageUrl = productDto.ProductImageUrl
            };

            await _productRepository.CreateProductAsync(product);
            return product;
        }

        public async Task<bool> UpdateProductAsync(string productId, ProductUpdateDto productDto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(productId);
            if (existingProduct == null) return false;

            var vendor = await _userRepository.GetUserByIdAsync(productDto.VendorId);
            if (vendor == null)
            {
                throw new Exception("Vendor not found");
            }

            var category = await _producuCatsRepository.GetProductCatByIdAsync(productDto.CategoryId);
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            existingProduct.ProductName = productDto.ProductName;
            existingProduct.ProductDescription = productDto.ProductDescription;
            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.Price = productDto.Price;
            existingProduct.StockQuantity = productDto.StockQuantity;
            existingProduct.ProductStatus = productDto.ProductStatus;
            existingProduct.ProductImageUrl = productDto.ProductImageUrl;

            return await _productRepository.UpdateProductAsync(productId, existingProduct);
        }

        public async Task<bool> DeleteProductAsync(string productId)
        {
            return await _productRepository.DeleteProductAsync(productId);
        }
    }
}
