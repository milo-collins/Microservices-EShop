using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Infrastructure.Common
{
    public class IdempotentConsumer
    {
        public Guid? MessageId { get; set; }
        public string ConsumerName { get; set; }
    }
}
