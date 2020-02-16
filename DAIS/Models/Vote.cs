using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.Models
{
    public class Vote
    {
        public int Id { get; set; }

        public string Owner { get; set; }

        public string OwnerName { get; set; }

        public string Receiver { get; set; }

        public string ReceiverName { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}
