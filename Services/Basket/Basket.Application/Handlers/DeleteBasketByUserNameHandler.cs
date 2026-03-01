using Basket.Application.Commands;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public record class DeleteBasketByUserNameHandler(IBasketRepository basketRepository) 
        : IRequestHandler<DeleteBasketByUserNameCommand, Unit>
    {
        public readonly IBasketRepository _basketRepository = basketRepository;
        public async Task<Unit> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
        {
            await _basketRepository.DeleteBasket(request.UserName);
            return Unit.Value;
        }
    }
}
