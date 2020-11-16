using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper.Attributes {

    public class SqlColumn : Attribute {

        public bool Nullable { get; set; }
        public object Default { get; set; }

    }
}
