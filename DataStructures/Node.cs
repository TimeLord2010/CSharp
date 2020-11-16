using System.Collections.Generic;
using System.Windows.Documents;

namespace DataStructures {

    public class Node <T, N> {

        public Node (T id, N value) {
            Id = id;
            Value = value;
        }

        public readonly T Id;
        public readonly N Value;

        public override string ToString() {
            return $"({Id}, {Value})";
        }

    }

    public class Node : Node<string, double> {

        public Node (string name, double value = 0) : base(name, value) {}

    }

}