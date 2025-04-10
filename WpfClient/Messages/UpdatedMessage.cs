using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Messages
{
    public class UpdatedMessage(MessagesEnum message)
    {
        public MessagesEnum Message { get; } = message;
    }
}
