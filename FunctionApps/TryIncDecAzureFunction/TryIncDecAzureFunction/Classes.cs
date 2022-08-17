using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryIncDecAzureFunction
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
    public class Reminder
    {
        public string username { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public string reminder { get; set; }
        public Guid userid { get; set; }
    }
    public class Auth
    {
        public Guid userid { get; set; }
        public string username { get; set; }
        public string pwauth { get; set; }
    }
}
