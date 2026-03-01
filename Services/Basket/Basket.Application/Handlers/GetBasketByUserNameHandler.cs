using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class GetBasketByUserNameHandler(IBasketRepository basketRepository) : IRequestHandler<GetBasketByUserNameQuery, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository = basketRepository;
        public async Task<ShoppingCartResponse> Handle(GetBasketByUserNameQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.GetBasket(request.UserName);
            if(basket == null)
            {
                return new ShoppingCartResponse(request.UserName)
                {
                    Items = []
                };
            }
            return basket.ToResponse();
                
        }
    }
}
