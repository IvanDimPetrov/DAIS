using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAIS.Models.Utilities
{
    public class SqlValuesBuilder
    {
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public SqlValuesBuilder Add(string name, object value)
        {
            this._values.Add(name, value);

            return this;
        }

        public Dictionary<string, object> Build()
        {
            return this._values;
        }
    }
}
