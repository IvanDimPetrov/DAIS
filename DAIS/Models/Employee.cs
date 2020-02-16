using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public DateTime BirthDate { get; set; }

        public string UserName {get; set;}
    }
}
