using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.DataBase.Interfaces
{
    public interface IDataBaseAccess
    {
        Task<IList<T>> ExecuteReader<T>(string query, IDictionary<string, object> values = null) where T : class, new();

        Task<IList<IList<string>>> ExecuteReader(string query, IDictionary<string, object> values = null);

        Task ExecuteNonQuery(string query, IDictionary<string, object> values = null);
    }
}
