using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models.TourViewModels
{
    public class TourViewModel
    {
        public Tour Tour { get; set; }
        public IEnumerable<ApplicationUser> TourMembers { get; set; }
        public IEnumerable<PlacementRule> PlacementRules { get; set; }
        public IEnumerable<StandingsViewModel> Standings { get; set; }
    }
}
