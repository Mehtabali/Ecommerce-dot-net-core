using Discount.Application.Commands;
using Discount.Application.DTOs;
using Discount.Application.Extensions;
using Discount.Application.Mappers;
using Discount.Core.Repositories;
using Grpc.Core;
using MediatR;


namespace Discount.Application.Handlers
{
    public class CreateDiscountHandler(IDiscountRepository discountRepository): IRequestHandler<CreateDiscountCommand, CouponDto>
    {
        private readonly IDiscountRepository _discountRepository = discountRepository;

        public async Task<CouponDto> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            //validate the input
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(request.ProductName))            
                validationErrors["ProductName"] = "Product name must not be empty.";
            if (string.IsNullOrEmpty(request.Description))
                validationErrors["Description"] = "Product DEscription must not be empty.";
            if (request.Amount < 0)
                validationErrors["Amount"] = "Amount must be greater than zero.";
            if(validationErrors.Any())
                throw GrpcErrorHelper.CreateValidationException(validationErrors);
            //map to entity
            var coupon = request.ToEntity();

            var created = await _discountRepository.CreateDiscount(coupon);
            if (!created)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Could not create discount for product: {request.ProductName}"));
            }
            return coupon.ToDto();
        }
    }
}
