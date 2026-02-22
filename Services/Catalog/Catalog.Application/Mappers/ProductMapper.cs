using Catalog.Application.Commands;
using Catalog.Application.DTOs;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Specifications;

namespace Catalog.Application.Mappers
{
    public static class ProductMapper
    {
        public static ProductResponse ToResponse(this Product product)
        {
            if (product == null) return null;
            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Summary = product.Summary,
                Description = product.Description,
                ImageFile = product.ImageFile,
                Price = product.Price,
                Brand = product.Brand,
                Type = product.Type,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate
            };
        }
        public static Pagination<ProductResponse> ToResponse(this Pagination<Product> pagination)
        {
            return new Pagination<ProductResponse>(pagination.PageIndex, pagination.PageSize, pagination.Count, pagination.Data.Select(p => p.ToResponse()).ToList());
        }

        public static IList<ProductResponse> ToResponseList(this IEnumerable<Product> products) => products.Select(x => x.ToResponse()).ToList();

        public static Product ToEntity(this CreateProductCommand command, ProductBrand brand, ProductType type) =>
            new Product
            {
                Name = command.Name,
                Summary = command.Summary,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Brand = brand,
                Type = type,
                Price = command.Price,
                CreatedDate = DateTimeOffset.UtcNow,
                UpdatedDate = DateTimeOffset.Now
            };
        public static Product ToUpdateEntity(this UpdateProductCommand command, Product existingProduct, ProductBrand brand, ProductType type)
        {
            return new Product
            {
                Id = existingProduct.Id,
                Name = command.Name,
                Description = command.Description,
                Summary = command.Summary,
                ImageFile = command.ImageFile,
                Brand = brand,
                Type = type,
                Price = command.Price,
                CreatedDate = existingProduct.CreatedDate,
                UpdatedDate = DateTimeOffset.UtcNow
            };
        }
        public static ProductDto ToDto(this ProductResponse product)
        {
            if (product == null) return null;
            return new ProductDto
            (
                product.Id,
                product.Name,
                product.Summary,
                product.Description,
                product.ImageFile,
                new BrandDto(product.Brand.Id, product.Brand.Name),
                new TypeDto(product.Type.Id, product.Type.Name),
                product.Price,
                DateTimeOffset.UtcNow
            );
        }
        public static UpdateProductCommand ToCommand(this UpdateProductDto dto, string id)
        {
            return new UpdateProductCommand 
            { 
                Id = id,
                Name = dto.Name,
                Summary = dto.Summary,
                Description = dto.Description,
                ImageFile = dto.ImageFile,
                BrandId = dto.BrandId,
                TypeId = dto.TypeId,
                Price = dto.Price,

            };
        }
    }
}