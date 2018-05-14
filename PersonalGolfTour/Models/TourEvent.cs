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
        // I apparently can't use Dictionaries, so I'll have to think of something later.
        //public Dictionary<string, int> PlaceResults { get; set; }
        //public Dictionary<string, int> ScoreResults { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
    }
}
