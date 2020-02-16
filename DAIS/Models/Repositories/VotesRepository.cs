using DAIS.DataBase;
using DAIS.DataBase.Interfaces;
using DAIS.Models.Repositories.Interfaces;
using DAIS.Models.Utilities;
using DAIS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.Models.Repositories
{
    public class VotesRepository : IVotesRepository
    {
        private readonly IDataBaseAccess _context;

        public VotesRepository(IDataBaseAccess context)
        {
            this._context = context;
        }

        public async Task<IList<Vote>> GetAllActiveVotes(string loggedUser)
        {
            var values = new SqlValuesBuilder().Add("LoggedUser", loggedUser).Build();

            var query = $@"SELECT v.* FROM [dbo].[Votes] AS v
                           WHERE IsActive  = 1 AND v.Receiver != @LoggedUser
                           AND v.Id NOT IN (SELECT VoteId FROM EmployeeVoteGift AS ev WHERE ev.EmployeeId = (SELECT TOP(1)Id FROM Employees  AS e WHERE e.UserName = @LoggedUser))";

            var votes = await this._context.ExecuteReader<Vote>(query, values);

            return votes;
        }

        public async Task<IList<Vote>> GetAllSuspendedVotes(string loggedUser)
        {
          
            var values = new SqlValuesBuilder().Add("LoggedUser", loggedUser).Build();

            var query = $@"SELECT * FROM [dbo].[Votes] AS v 
                           WHERE IsActive = 0 AND v.Receiver != @LoggedUser";

            var votes = await this._context.ExecuteReader<Vote>(query, values);

            return votes;
        }

        public async Task<IList<Vote>> GetAllInitiatedFromEmployeeVotes(string loggedUser)
        {       
            var values = new SqlValuesBuilder().Add("LoggedUser", loggedUser).Build();

            string query = $@"SELECT v.* FROM Votes AS v
                              WHERE v.Owner = @LoggedUser";

            var votes = await this._context.ExecuteReader<Vote>(query, values);

            return votes;
        }

        public async Task<IList<Gift>> GetAllGifts()
        {
            string query = $@"SELECT * FROM Gifts";

            var gifts = await this._context.ExecuteReader<Gift>(query);

            return gifts;
        }

        public async Task InsertVote(Vote vote)
        {
            var values = new SqlValuesBuilder().Add("Owner", vote.Owner)
                                                .Add("Receiver", vote.Receiver)
                                                .Add("IsActive", vote.IsActive)
                                                .Build();

            string query = $@"INSERT INTO [dbo].[Votes] (Owner, Receiver, DateCreated, IsActive, OwnerName, ReceiverName)
                              VALUES ( @Owner, 
                                       @Receiver, 
                                       GETDATE(), 
                                       @IsActive, 
                                       (SELECT TOP(1)Name FROM Employees  AS e WHERE e.UserName = @Owner), 
                                       (SELECT TOP(1)Name FROM Employees  AS e WHERE e.UserName = @Receiver)
                                     )";

            await this._context.ExecuteNonQuery(query, values);
        }

        public async Task SuspendVote(int id)
        {

            var values = new SqlValuesBuilder().Add("Id", id).Build();

            var query = $@"UPDATE [dbo].[Votes]
                           SET IsActive = 0
                           WHERE Id = @Id";

            await this._context.ExecuteNonQuery(query, values);
        }

        public async Task VoteForGift(VotingViewModel voteModel)
        {      
            var values = new SqlValuesBuilder().Add("VoteId", voteModel.VoteId)
                                                .Add("GiftId", voteModel.GiftId)
                                                .Add("LoggedUser", voteModel.LoggedUser)
                                                .Build();

            string query = $@"INSERT INTO [dbo].[EmployeeVoteGift] (EmployeeId, VoteId, GiftId)
                              VALUES ((SELECT TOP(1)Id FROM Employees  AS e WHERE e.UserName = @LoggedUser), 
                                       @VoteId, 
                                       @GiftId
                                      )";

            await this._context.ExecuteNonQuery(query, values);
        }

        public async Task<IList<IList<string>>> GetVoteDetails(int id)
        {   
            var values = new SqlValuesBuilder().Add("Id", id).Build();

            string query = $@"SELECT v.ReceiverName, e.UserName, e.Name, g.Name FROM Votes AS v 
                              LEFT JOIN EmployeeVoteGift AS ev 
                              ON v.Id = ev.VoteId
                              LEFT JOIN Employees AS e         
                              ON e.Id = ev.EmployeeId
                              LEFT JOIN Gifts AS g             
                              ON g.Id = ev.GiftId
                              WHERE v.Id = @Id";

            var result = await this._context.ExecuteReader(query, values);

            return result;
        }
    }
}
