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
        public ICollection<TourEvent> Events { get; set; }
        public ICollection<UserTour> UserTours { get; set; }
        public ICollection<PlacementRule> PlacementRules { get; set; }
    }
}
