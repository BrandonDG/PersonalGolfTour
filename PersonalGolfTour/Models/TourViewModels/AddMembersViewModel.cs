using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models.TourViewModels
{
    public class AddMembersViewModel
    {
        public Tour Tour { get; set; }
        public IEnumerable<SelectListItem> MemberList { get; set; }
        public IEnumerable<String> SelectedMembers { get; set; }
    }
}
