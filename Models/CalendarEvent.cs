using System;

namespace GraphTutorial.Models
{
    // Represents a Calendar event payload


    public class EventTimeInfo
    {
        public DateTime DateTime { get; set; }

        public string TimeZone { get; set; }

        public EventTimeInfo(DateTime TimeComponent, string TimeZone)
        {
            this.DateTime = TimeComponent;
            this.TimeZone = TimeZone;
        }
    }

    public class Body
    {
        public string ContentType { get; set; }
        public string Content { get; set; }
    }

    public class CalendarEvent
    {
        public string TargetCalendarID { get; set; }
        public string Subject { get; set; }

        public Body Body { get; set; }
        public string StartDateTime { get; set; }
        public string StartTimeZone { get; set; }

        public EventTimeInfo Start { get; set; }
        public EventTimeInfo End { get; set; }

        public string Location { get; set; }

    }
}