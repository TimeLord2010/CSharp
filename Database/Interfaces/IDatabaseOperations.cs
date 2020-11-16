using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Interfaces {

    public interface IDatabaseOperations {

        void NonQuery (string q, IEnumerable<(string, object)> parameters);

        void NonQuery (string q, params (string, object)[] parameters);

        void Query (string q, Action<IReader> reader, params (string, object)[] parameters);

        void QueryLoop(string q, Action<IReader> reader, params (string, object)[] parameters);

    }
}
