using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models.TourViewModels
{
    public class TourViewModel
    {
        public Tour Tour { get; set; }
        public List<ApplicationUser> TourMembers { get; set; }
        public List<PlacementRule> PlacementRules { get; set; }
        public List<StandingsViewModel> Standings { get; set; }
    }
}
