using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalGolfTour.Models.TourViewModels
{
    public class NewTourResultViewModel
    {
        /*
        public List<ApplicationUser> users { get; set; }

        [Display(Name = "Member")]
        public string SelectedUserId { get; set; }

        public IEnumerable<SelectListItem> UserItems
        {
            get { return new SelectList(users, "UserId", "DisplayName"); }
        } */

        public string UserId { get; set; }
        public int Place { get; set; }
    }
}
