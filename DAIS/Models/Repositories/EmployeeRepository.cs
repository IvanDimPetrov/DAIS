using DAIS.DataBase;
using System.Collections.Generic;
using System;
using System.Linq;
using DAIS.Models.Utilities;
using DAIS.DataBase.Interfaces;
using System.Threading.Tasks;
using DAIS.Models.Repositories.Interfaces;

namespace DAIS.Models.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDataBaseAccess _context;

        public EmployeeRepository(IDataBaseAccess context)
        {
            this._context = context;
        }

        public async Task<IList<Employee>> GetAllEmployees()
        {
            var query = @"SELECT * FROM Employees";

            var employees = await this._context.ExecuteReader<Employee>(query);

            return employees;
        }

        public async Task<Employee> GetEmployeeByUserNameAndPassword(string userName, string password)
        { 
            var values = new SqlValuesBuilder().Add("UserName", userName)
                                                .Add("Password", password)
                                                .Build();


            string query = $@"SELECT * FROM Employees AS e
                              WHERE e.UserName = @UserName AND e.Password = @Password";

            var employee = await this._context.ExecuteReader<Employee>(query, values);

            return employee.FirstOrDefault();
        }

        public async Task<IList<Employee>> GetEmployeesWithoutVote()
        {
            string query = $@"SELECT e.* FROM Employees AS e
                              LEFT JOIN Votes AS v
                              ON e.UserName = v.Receiver
                              WHERE v.IsActive IS NULL";

            var employees = await this._context.ExecuteReader<Employee>(query);

            return employees;
        }

        public async Task<IList<Employee>> GetAllUnvotedForVoteEmployees(int voteId)
        {
           
            var values = new SqlValuesBuilder().Add("VoteId", voteId).Build();

            string query = $@"SELECT * FROM Employees AS e
                              WHERE e.Id  NOT IN (SELECT evg.EmployeeId 
					                              FROM EmployeeVoteGift AS evg 
					                              WHERE evg.VoteId = @VoteId)";

            var employees = await this._context.ExecuteReader<Employee>(query, values);

            return employees;
        }

    }
}
