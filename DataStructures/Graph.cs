using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures {

    public class Graph <NodeId, NodeValue, LinkId, LinkValue> {

        readonly List<Node<NodeId, NodeValue>> Nodes = new List<Node<NodeId, NodeValue>>();
        readonly List<NodeLink<NodeId, NodeValue, LinkId, LinkValue>> NodeLinks = new List<NodeLink<NodeId, NodeValue, LinkId, LinkValue>>();

        public void AddNode(Node<NodeId, NodeValue> node) {
            Nodes.Add(node);
        }

        public Node<NodeId, NodeValue> AddNode (NodeId id, NodeValue value = default) {
            if (HasNode(id)) {
                throw new Exception($"Couldn't add node because there is already a node with the same id in the graph.");
            }
            var node = new Node<NodeId, NodeValue>(id, value);
            Nodes.Add(node);
            return node;
        }

        //public void AddLink (NodeId from, NodeId to, LinkId linkId = default, LinkValue value = default, bool add_node_if_not_found = false) {
        //    var link = new NodeLink<NodeId, NodeValue, LinkId, LinkValue>(
        //        GetNode(from, add_if_not_found: add_node_if_not_found), 
        //        GetNode(to, add_if_not_found: add_node_if_not_found), 
        //        linkId, 
        //        value);
        //    NodeLinks.Add(link);
        //}

        public void AddLink (NodeLink<NodeId, NodeValue, LinkId, LinkValue> link) {
            NodeLinks.Add(link);
            if (!HasNode(link.Begin.Id)) {
                AddNode(link.Begin);
            }
            if (!HasNode(link.End.Id)) {
                AddNode(link.End);
            }
        }

        public Node<NodeId, NodeValue> GetNode (NodeId id, NodeValue value = default, bool add_if_not_found = false) {
            var result = Nodes.Where(x => EqualityComparer<NodeId>.Default.Equals(x.Id, id));
            if (result.Count() == 0) {
                return add_if_not_found? AddNode(id, value) : null;
            } else {
                return result.ElementAt(0);
            }
        }

        public void ForEachLink (Action<NodeLink<NodeId, NodeValue, LinkId, LinkValue>> action) {
            NodeLinks.ForEach(action);
        }

        public void ForEachNode (Action<Node<NodeId, NodeValue>> action) {
            Nodes.ForEach(action);
        }

        public IEnumerable<NodeLink<NodeId, NodeValue, LinkId, LinkValue>> GetLinks () {
            return NodeLinks.Select(x => x);
        }

        public IEnumerable<Node<NodeId, NodeValue>> GetNodes () {
            return Nodes.Select(x => x);
        }

        public int LinkCount {
            get => NodeLinks.Count;
        }

        public int NodeCount {
            get => Nodes.Count;
        }

        public bool HasNode (NodeId id) => 
            Nodes.Where(x => EqualityComparer<NodeId>.Default.Equals(x.Id, id)).Count() > 0;

        public override string ToString() {
            string a = "";
            foreach (var link in NodeLinks) {
                a += $"{link}{Environment.NewLine}";
            }
            return a;
        }

    }

    public class Graph<NodeId, NodeValue> : Graph<NodeId, NodeValue, string, double> {

        //public void AddLink (NodeId from, NodeId to, bool Add) {

        //}

    }

}