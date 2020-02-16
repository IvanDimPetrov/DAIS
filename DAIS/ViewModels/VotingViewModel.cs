using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.ViewModels
{
    public class VotingViewModel
    {
        [Required]
        public int? VoteId { get; set; }

        public SelectList Gifts { get; set; }

        [Required(ErrorMessage = "Please select a gift")]
        [Display(Name = "Select a Gift")]
        public int? GiftId { get; set; }

        [Required]
        public string LoggedUser { get; set; }

    }
}
