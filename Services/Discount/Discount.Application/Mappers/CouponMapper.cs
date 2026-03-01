using Discount.Application.DTOs;
using Discount.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Discount.Application.Mappers
{
    public static class CouponMapper
    {
        public static CouponDto ToDto(this Coupon coupon)
        {
            return new CouponDto
            (
                coupon.Id,
                coupon.ProductName,
                coupon.Description,
                coupon.Amount
             );            
        }
    }
}
