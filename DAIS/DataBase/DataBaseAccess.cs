using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DAIS.DataBase.Interfaces;

namespace DAIS.DataBase
{
    public class DataBaseAccess : IDataBaseAccess
    {
        private readonly SqlConnection _connection;
        
        public DataBaseAccess (string connectionString)
        {
            this._connection = new SqlConnection(connectionString);
        }

        public async Task<IList<T>> ExecuteReader<T>(string query, IDictionary<string, object> values = null) where T : class, new()
        {
            var result = new List<T>();

            var type = typeof(T);

            var propsInfo = type.GetProperties(BindingFlags.Public|BindingFlags.Instance);


            using (this._connection)
            {
                this._connection.Open();

                using (var command = new SqlCommand(query, this._connection))
                {
                    this.AddParameters(command, values);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var fieldCount = reader.FieldCount;

                            var currentT = new T();

                            for (var i = 0; i < fieldCount; i++)
                            {
                                var prop = propsInfo.FirstOrDefault(x => x.Name.Equals(reader.GetName(i), StringComparison.OrdinalIgnoreCase) && x.PropertyType == reader.GetFieldType(i));

                                if (prop != null)
                                {
                                    prop.SetValue(currentT, reader[i]);
                                }

                            }

                            result.Add(currentT);
                        }
                    }
                };
            }

            return result;
        }

        public async Task<IList<IList<string>>> ExecuteReader(string query, IDictionary<string, object> values = null)
        {
            var result = new List<IList<string>>();

            using (this._connection)
            {
                this._connection.Open();

                using (var command = new SqlCommand(query, this._connection))
                {
                    this.AddParameters(command, values);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var fieldCount = reader.FieldCount;

                            var currentRow = new List<string>();

                            for (var i = 0; i < fieldCount; i++)
                            {
                                currentRow.Add(reader[i].ToString());
                            }

                            result.Add(currentRow);
                        }
                    }
                }
            };

            return result;
        }

        public async Task ExecuteNonQuery(string query, IDictionary<string, object> values = null)
        {
            using(this._connection)
            {
                this._connection.Open();

                using (var command = new SqlCommand(query, this._connection))
                {
                    this.AddParameters(command, values);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private void AddParameters(SqlCommand command, IDictionary<string, object> values)
        {
            if (values != null)
            {
                foreach (var kvp in values)
                {
                    command.Parameters.AddWithValue(kvp.Key, kvp.Value);
                }
            }
        }
    }
}
