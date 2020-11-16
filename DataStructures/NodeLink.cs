namespace DataStructures {

    public class NodeLink <NodeId, NodeValue, LinkId, LinkValue> {

        public NodeLink (Node<NodeId, NodeValue> begin, Node<NodeId, NodeValue> end, LinkId id, LinkValue value) {
            Begin = begin;
            End = end;
            Id = id;
        }

        public readonly Node<NodeId, NodeValue> Begin, End;
        public readonly LinkId Id;
        public readonly LinkValue Value;

        public override string ToString() {
            return $"{Begin} --({Id}, {Value})--> {End}";
        }

    }

    public class NodeLink : NodeLink<string, double, string, double> {

        public NodeLink (Node<string, double> begin, Node<string, double> end, string id, double value) : base(begin, end, id, value) {
        }

    }

}