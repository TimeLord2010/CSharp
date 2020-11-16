using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper.Attributes {

    public class PrimaryKey : SqlColumn {

        public bool AutoIncrement { get; }

        public PrimaryKey (bool auto_increment = false) {
            AutoIncrement = auto_increment;
        }

    }

}
