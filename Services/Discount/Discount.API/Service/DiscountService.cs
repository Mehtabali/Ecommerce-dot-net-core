using Discount.Application.Commands;
using Discount.Application.Mappers;
using Discount.Application.Queries;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;
using System.Runtime.CompilerServices;

namespace Discount.API.Service
{
    public class DiscountService(IMediator mediatr) : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IMediator _mediatr = mediatr;
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var query = new GetDiscountQuery(request.ProductName);
            var couponDto = await _mediatr.Send(query);
            return couponDto.ToModel();
        }
        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var cmd = request.Coupon.ToCreateCommand();
            var dto = await _mediatr.Send(cmd);
            return dto.ToModel();
        }
        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var cmd = request.Coupon.ToUpdateCommand();
            var dto = await _mediatr.Send(cmd);
            return dto.ToModel();
        }
        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var cmd = new DeleteDiscountCommand(request.ProductName);
            var deleted = await _mediatr.Send(cmd);
            return new DeleteDiscountResponse
            {
                Success = deleted
            };
        }
    }
}
