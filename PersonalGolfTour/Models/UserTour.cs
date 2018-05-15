using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models
{
    public class UserTour
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int TourId { get; set; }
        public Tour Tour { get; set; }
    }
}
