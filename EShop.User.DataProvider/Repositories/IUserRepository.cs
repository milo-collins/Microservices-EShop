using EShop.Infrastructure.Command.User;
using EShop.Infrastructure.Event.User;

namespace EShop.User.DataProvider
{
    public interface IUserRepository
    {
        // Takes CreateUser and returns UserCreated
        Task<UserCreated> AddUser(CreateUser user);
        // Fetch user details
        Task<UserCreated> GetUser(CreateUser user);
        Task<UserCreated> GetUserByUsername(string name);
    }
}
