using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models
{
    public class PlacementRule
    {
        public int PlacementRuleId { get; set; }
        public int Place { get; set; }
        public int Points { get; set; }

        public int TourId { get; set; }
        public Tour Tour { get; set; }
    }
}
