using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models
{
    public class TourResult
    {
        public int TourResultId { get; set; }
        public int Place { get; set; }

        //public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int TourEventId { get; set; }
        public TourEvent TourEvent { get; set; }
    }
}
