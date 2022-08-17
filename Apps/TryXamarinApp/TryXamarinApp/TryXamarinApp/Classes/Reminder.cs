using System;
using System.Collections.Generic;
using System.Text;

namespace TryXamarinApp.Classes
{
    public class Reminder
    {
        public string username { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public string reminder { get; set; }
        public Guid userid { get; set; }
    }
}
