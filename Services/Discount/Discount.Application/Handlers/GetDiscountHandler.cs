using Discount.Application.DTOs;
using Discount.Application.Queries;
using Discount.Core.Repositories;
using MediatR;
using Discount.Application.Mappers;
using Discount.Application.Extensions;
using Grpc.Core;

namespace Discount.Application.Handlers
{
    public class GetDiscountHandler(IDiscountRepository discountRepository) : IRequestHandler<GetDiscountQuery, CouponDto>
    {
        private readonly IDiscountRepository _discountRepository = discountRepository;

        public async Task<CouponDto> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
        {
            //validate the input
            if (string.IsNullOrEmpty(request.productName))
            {
                var validationErrors = new Dictionary<string, string>()
                {
                    {"ProductName", "Product name must not be empty." }
                };
                throw GrpcErrorHelper.CreateValidationException(validationErrors);
            }
            //Fetch from repo
            var coupon = await _discountRepository.GetDiscount(request.productName);
            if (coupon == null)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Could not create discount for product: {request.productName}"));
            }
            return coupon.ToDto();
        } 
    }
}
