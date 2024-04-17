using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.EventBus
{
    // Maps Rabbit MQ Config
    public class RabbitMqOption
    {
        public string ConnectionString { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
