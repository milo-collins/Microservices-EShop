using EShop.Infrastructure.Event.User;
using EShop.User.DataProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using System.Drawing;

namespace EShop.User.Query.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService= userService;
        }

        [HttpGet]
        public async Task<UserCreated> GetUserByUsername(string username)
        {
            var user = await _userService.GetUserByUsername(username);
            return user;
        }
    }
}
