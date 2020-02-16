using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.Models.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IList<Employee>> GetAllEmployees();

        Task<IList<Employee>> GetEmployeesWithoutVote();

        Task<Employee> GetEmployeeByUserNameAndPassword(string userName, string password);

        Task<IList<Employee>> GetAllUnvotedForVoteEmployees(int voteId);
    }
}
