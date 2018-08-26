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
        public int EventId { get; set; }
        public string UserId { get; set; }
        public int Place { get; set; }
    }
}
