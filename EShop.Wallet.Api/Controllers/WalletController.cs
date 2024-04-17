using EShop.Infrastructure.Command.Wallet;
using EShop.Wallet.DataProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Wallet.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private IWalletService _service;
        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddFunds funds)
        {
            return Accepted(await _service.AddFunds(funds));
        }

        [HttpPost]
        public async Task<IActionResult> Deduct(DeductFunds funds)
        {
            return Accepted(await _service.DeductFunds(funds));
        }
    }
}
