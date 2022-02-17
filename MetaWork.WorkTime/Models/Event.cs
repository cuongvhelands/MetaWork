
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaWork.WorkTime.Models
{
    public class Event
    {
        public Event()
        {
            this.Start = new EventDateTime()
            {
                TimeZone = "Asia/Ho_Chi_Minh"
            };
            this.End = new EventDateTime()
            {
                TimeZone = "Asia/Ho_Chi_Minh"
            };
        }
        public string Summary { get; set; }
        public string Descrition { get; set; }
        public EventDateTime Start { get; set; }
        public EventDateTime End { get; set; }
        public List<EventAttendee> Attendees { get; set; }
    }
    public class EventDateTime
    {
        public string DateTime { get; set; }
        public string TimeZone { get; set; }
    }
    public class EventAttendee
    { 
        public string Email { get; set; }
    }   
    
  
}