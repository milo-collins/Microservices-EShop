using EShop.Infrastructure.Command.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Wallet.DataProvider.Repositories
{
    public interface IWalletRepository
    {
        Task<bool> AddFunds(AddFunds funds);
        Task<bool> DeductFunds(DeductFunds funds);
    }
}
