using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models
{
    public class Tour
    {
        public int TourId { get; set; }
        public string TourName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Colour { get; set; }
        // I apparently can't use Dictionaries, so I'll have to think of something later.
        //public Dictionary<int, int> RuleSet { get; set; }
        //public Dictionary<string, int> Standings { get; set; }
        public List<TourEvent> Events { get; set; }
        public List<UserTour> UserTours { get; set; } = new List<UserTour>();
    }
}
