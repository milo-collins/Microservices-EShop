using EShop.Infrastructure.Authentication;
using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Event.User;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // Inject Event Bus into Controller
        private IBusControl _bus;
        private IRequestClient<LoginUser> _loginRequestClient;

        public UserController(IBusControl bus, IRequestClient<LoginUser> loginRequestClient)
        {
            _bus = bus;
            _loginRequestClient = loginRequestClient;

        }

        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateUserDto user)
        {

            // Defines where to send the message. localhost/queue_name
            var uri = new Uri("rabbitmq://localhost/add_user");
            var endPoint = await _bus.GetSendEndpoint(uri);
            // Sends to Handler to be consumed
            await endPoint.Send(new CreateUser()
            {
                Username= user.Username,
                EmailId=user.EmailId,
                Password=user.Password,
                ContactNo=user.ContactNo
            });

            return Accepted("User Created");
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Login([FromForm] LoginUser loginUser)
        {
            var userResponse = await _loginRequestClient.GetResponse<JwtAuthToken>(loginUser);

            return Accepted(userResponse.Message);
        }

    }
}
