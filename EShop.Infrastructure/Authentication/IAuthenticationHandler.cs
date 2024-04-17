using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Authentication
{
    public interface IAuthenticationHandler
    {
        JwtAuthToken Create(string userId);

    }
}
