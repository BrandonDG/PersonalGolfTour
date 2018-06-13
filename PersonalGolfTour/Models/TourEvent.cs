using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models
{
    public class TourEvent
    {
        public int TourEventId { get; set; }
        public string TourEventName { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }

        public int TourId { get; set; }
        public Tour Tour { get; set; }

        public List<TourResult> TourResults { get; set; } = new List<TourResult>();
    }
}
