using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper.Attributes {

    public class Identity : Attribute {

        public Identity(int seed, int increment) {
            Seed = seed;
            Increment = increment;
        }

        public int Seed { get; set; }
        public int Increment { get; set; }

    }
}
