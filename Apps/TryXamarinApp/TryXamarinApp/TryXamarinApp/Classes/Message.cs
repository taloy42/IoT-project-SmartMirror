using System;
using System.Collections.Generic;
using System.Text;

namespace TryXamarinApp.Classes
{
    public class Message
    {
        public DateTime timestamp { get; set; }
        public string sender { get; set; }
        public Guid senderID { get; set; }
        public string reciever { get; set; }
        public Guid recieverID { get; set; }
        public string message { get; set; }
    }
}
