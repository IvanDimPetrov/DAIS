using DAIS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.Models.Repositories.Interfaces
{
    public interface IVotesRepository
    {
        Task<IList<Vote>> GetAllActiveVotes(string loggedUser);

        Task<IList<Vote>> GetAllSuspendedVotes(string loggedUser);

        Task<IList<Vote>> GetAllInitiatedFromEmployeeVotes(string loggedUser);

        Task<IList<Gift>> GetAllGifts();

        Task<IList<IList<string>>> GetVoteDetails(int id);

        Task InsertVote(Vote vote);

        Task SuspendVote(int id);

        Task VoteForGift(VotingViewModel voteModel);
    }


}
