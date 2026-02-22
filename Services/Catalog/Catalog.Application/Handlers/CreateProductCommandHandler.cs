using Catalog.Application.Commands;
using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers
{
    public class CreateProductCommandHandler(IProductRepository productRepository) : 
                            IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _productRepository = productRepository;
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            //Fetch Brands and Type from Repository
            var brand = await _productRepository.GetBrandByIdAsync(request.BrandId);
            var type = await _productRepository.GetTypesByIdAsync(request.TypeId);
            if(brand == null || type == null)
            {
                throw new ApplicationException("Invalid Brand or Type Specified");
            }
            var productEntity = request.ToEntity(brand, type);
            var newProduct = await _productRepository.CreateProduct(productEntity);
            return newProduct.ToResponse();
        }
    }
}
