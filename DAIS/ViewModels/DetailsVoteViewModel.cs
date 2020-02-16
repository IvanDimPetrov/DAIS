using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.ViewModels
{
    public class DetailsVoteViewModel
    {
        public string ReceiverName { get; set; }

        public Dictionary<string, List<string>> VotingDetails { get; set; }

        public Dictionary<string, int> GiftsStatistic { get; set; }
    }
}
