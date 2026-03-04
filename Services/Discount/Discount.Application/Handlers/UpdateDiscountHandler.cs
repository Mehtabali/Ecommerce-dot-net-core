using Discount.Application.Commands;
using Discount.Application.DTOs;
using Discount.Application.Extensions;
using Discount.Core.Repositories;
using Grpc.Core;
using MediatR;
using Discount.Application.Mappers;

namespace Discount.Application.Handlers
{
    public class UpdateDiscountHandler(IDiscountRepository discountRepository) : IRequestHandler<UpdateDiscountCommand, CouponDto>
    {
        private readonly IDiscountRepository _discountRepository = discountRepository;

        public async Task<CouponDto> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            //validate the input
            var validationErrors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(request.ProductName))
                validationErrors["ProductName"] = "Product name must not be empty.";
            if (string.IsNullOrEmpty(request.Description))
                validationErrors["Description"] = "Product DEscription must not be empty.";
            if (request.Amount < 0)
                validationErrors["Amount"] = "Amount must be greater than zero.";
            if (validationErrors.Any())
                throw GrpcErrorHelper.CreateValidationException(validationErrors);
            //map to entity
            var coupon = request.ToEntity();

            var created = await _discountRepository.UpdateDiscount(coupon);
            if (!created)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Could not update discount for product: {request.ProductName}"));
            }
            return coupon.ToDto();
        }

    }
}
