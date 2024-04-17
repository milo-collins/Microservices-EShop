using EShop.Infrastructure.Command.Wallet;
using EShop.Wallet.DataProvider.Services;
using MassTransit;

namespace EShop.Wallet.Api.Handlers
{
    public class AddFundsConsumer : IConsumer<AddFunds>
    {
        private IWalletService _service;
        public AddFundsConsumer(IWalletService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<AddFunds> context)
        {
            var isAdded = await _service.AddFunds(context.Message);

            if (isAdded)
            {
                await Task.CompletedTask;
            }
            else
            {
                throw new Exception("New Funds not added");
            }
        }
    }
}
