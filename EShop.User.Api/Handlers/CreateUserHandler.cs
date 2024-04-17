using EShop.Infrastructure.Command.User;
using EShop.User.DataProvider;
using MassTransit;

namespace EShop.User.Api.Handlers
{
    public class CreateUserHandler : IConsumer<CreateUser>
    {
        // Inject Service
        private IUserService _service;
        public CreateUserHandler(IUserService service)
        {
            _service = service;
        }

        public async Task Consume(ConsumeContext<CreateUser> context)
        {
            var createdUser = await _service.AddUser(context.Message);
        }
    }
}
