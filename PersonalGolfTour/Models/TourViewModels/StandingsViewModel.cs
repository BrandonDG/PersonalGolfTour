using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models.TourViewModels
{
    public class StandingsViewModel
    {
        public int? Score { get; set; }
        public ApplicationUser Player { get; set; }
    }
}
