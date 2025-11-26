using System;

namespace RailwayKiosk
{
    /// <summary>
    /// Model representing a single train record.  Each train has a unique
    /// number, a destination, a scheduled departure time, a type (e.g.
    /// Express, Mail, InterCity), and a current status.  The status field
    /// indicates whether the train is on time, delayed or cancelled.
    /// </summary>
    public class Train
    {
        public string TrainNumber { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
            = DateTime.Now;
        public string TrainType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}