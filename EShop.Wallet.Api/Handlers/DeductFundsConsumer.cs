using EShop.Infrastructure.Command.Wallet;
using EShop.Wallet.DataProvider.Services;
using MassTransit;

namespace EShop.Wallet.Api.Handlers
{
    public class DeductFundsConsumer : IConsumer<DeductFunds>
    {
        private IWalletService _service;
        public DeductFundsConsumer(IWalletService service)
        {
            _service = service;
        }
        public async Task Consume(ConsumeContext<DeductFunds> context)
        {
            var isDeducted = await _service.DeductFunds(context.Message);

            if (isDeducted)
            {
                await Task.CompletedTask;
            }
            else
            {
                throw new Exception("Funds not deducted");
            }
        }
    }
}
