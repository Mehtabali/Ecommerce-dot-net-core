using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class UpdateProductCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var existingProduct = await _productRepository.GetProduct(request.Id);
            if (existingProduct is null)
            {
                throw new KeyNotFoundException($"Product with Id {request.Id} not found");
            }
            //Step 1: Fetch Brand and Type
            var brand = await _productRepository.GetBrandByIdAsync(request.BrandId);
            var type = await _productRepository.GetTypesByIdAsync(request.TypeId);
            if (brand is null || type is null)
            {
                throw new ApplicationException("Invalid Brand or Type Specified.");
            }
            //Step 2: Mapper Role
            var updatedProduct = request.ToUpdateEntity(existingProduct, brand, type);

            //Step 3: Update the product
            return await _productRepository.UpdateProduct(updatedProduct);
        }
    }
}
