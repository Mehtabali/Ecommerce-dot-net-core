using Microsoft.AspNetCore.Mvc;
using MediatR;
using Basket.Application.DTOs;
using Basket.Application.Queries;
using Basket.Application.Commands;
namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        
        //Get: api/v1/basket/{userName}
        [HttpGet("{userName}")]
        public async Task<ActionResult<ShoppingCartDto>> GetBasket(string userName)
        {
            var query = new GetBasketByUserNameQuery(userName);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        //Post: api/v1/basket
        [HttpPost]
        public async Task<ActionResult<ShoppingCartDto>> CreateOrUpdateBasket([FromBody] CreateShoppingCartCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        //Delete: api/v1/basket/{userName}
        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            var command = new DeleteBasketByUserNameCommand(userName);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
