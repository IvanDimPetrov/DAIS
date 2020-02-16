using DAIS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.ViewModels
{
    public class CreateVoteViewModel
    {
        [Required]
        [Display(Name = "Birthday Guy")]
        public string Reciever { get; set; }

        public SelectList EmployeeList { get; set; }

        [Required]
        public string Owner { get; set; }

    }
}
