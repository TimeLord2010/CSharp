﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.String;

namespace DataStructures {

    public class PList<T> : List<T> {

        //List<T> list = new List<T>();

        public new T this[int i] {
            get => base[FilterIndex(i)];
            set => base[FilterIndex(i)] = value;
        }

        public int FilterIndex(int i) {
            if (Count > 0) {
                while (i < 0) {
                    i += Count;
                }
                while (i > Count) {
                    i -= Count;
                }
            } else {
                throw new IndexOutOfRangeException("List had no elements.");
            }
            return i;
        }

    }

    public class Graph {


        public class NodeItem {
            public NodeItem(int id, string value) {
                Id = id;
                Value = value;
            }
            public int Id;
            public string Value;
            public static implicit operator string(NodeItem nodeItem) {
                return Format("Id: {0} | Value: {1}", nodeItem.Id, nodeItem.Value);
            }
        }
        public class ArcItem {
            public ArcItem(int Idnode1, int Idnode2, string value) {
                node1 = Idnode1;
                node2 = Idnode2;
                Value = value;
            }
            public int node1, node2;
            public string Value;
            public static implicit operator string(ArcItem arcItem) {
                return Format("{0} -> {1} ({2})", arcItem.node1, arcItem.node2, arcItem.Value); ;
            }
        }

        /*public class Step {
            public Graph Graph { get; set; } = new Graph();
            public List<int> Visited() {
                List<int> visited = new List<int>();
                if (Graph.arcs.Count == 0) {
                    for (int i = 0; i < Graph.nodes.Count; i++) {
                        if (Graph.nodes[i].Value.Equals("0")) {
                            visited.Add(Graph.nodes[i].Id);
                        }
                    }
                } else {
                    for (int i = 0; i < Graph.arcs.Count; i++) {
                        if (!visited.Contains(Graph.arcs[i].node1)) {
                            visited.Add(Graph.arcs[i].node1);
                        }
                        if (!visited.Contains(Graph.arcs[i].node2)) {
                            visited.Add(Graph.arcs[i].node2);
                        }
                    }
                }
                return visited;
            }
            public List<int> PreVisitation() {
                List<int> nodes = new List<int>();
                List<int> visited = Visited();
                Graph graph = Graph.FromDatabase();
                for (int i = 0; i < graph.arcs.Count; i++) {
                    if (visited.Contains(graph.arcs[i].node1) && !visited.Contains(graph.arcs[i].node2) && !nodes.Contains(graph.arcs[i].node2)) {
                        nodes.Add(graph.arcs[i].node2);
                    }
                }
                return nodes;
            }
            public List<int> Unvisited() {
                List<int> unvisited = new List<int>();
                for (int i = 0; i < Graph.nodes.Count; i++) {
                    if (Graph.nodes[i].Value.Equals("Inf.")) {
                        unvisited.Add(Graph.nodes[i].Id);
                    }
                }
                return unvisited;
            }
            public static Step StepZero(Graph graph, int OriginId) {
                Step step = new Step();
                for (int i = 0; i < graph.nodes.Count; i++) {
                    step.Graph.nodes.Add(new NodeItem(graph.nodes[i].Id, graph.nodes[i].Id == OriginId ? "0" : "Inf."));
                }
                return step;
            }
        }*/
    }

    /*public class Tree {
        public Tree(int id, Tree father = null, List<Tree> children = null) {
            Father = father;
            Children = children ?? new List<Tree>();
            Id = id;
        }
        public Tree(int id, params int[] childs) {
            Id = id;
            for (int i = 0; i < childs.Length; i++) {
                Children.Add(new Tree(childs[i]));
            }
        }
        public Tree(Graph graph, int Origin) {
            Id = Origin;
            // Checking if origin exists in the graph
            bool found = false;
            for (int i = 0; i < graph.nodes.Count; i++) {
                if (Origin == graph.nodes[i].Id) {
                    found = true;
                }
            }
            if (!found) {
                throw new KeyNotFoundException();
            }
            for (int i = 0; i < graph.arcs.Count; i++) {
                if (graph.arcs[i].node2 == Origin || graph.arcs[i].node1 == graph.arcs[i].node2) {
                    graph.arcs.RemoveAt(i--);
                }
            }
            for (int i = 0; i < graph.arcs.Count; i++) {
                ArcItem a = graph.arcs[i];
                for (int j = 0; j < graph.arcs.Count; j++) {
                    if (i != j && a == graph.arcs[j]) {
                        graph.arcs.RemoveAt(j--);
                        i--;
                    }
                }
            }
        A:
            List<Tree> parents = new List<Tree>() { new Tree(Origin) };
            List<Tree> children = new List<Tree>();
        Operation:
            bool b = false;
            for (int i = 0; i < parents.Count; i++) {
                Tree NParent = parents[i];
                for (int j = 0; j < graph.arcs.Count; j++) {
                    if (NParent.Id == graph.arcs[j].node1) {
                        int id = graph.arcs[j].node2;
                        Tree tree = new Tree(id);
                        tree.Father = NParent;
                        NParent.Children.Add(tree);
                        children.Add(tree);
                        b = true;
                        for (int k = 0; k < graph.arcs.Count; k++) {
                            if (id == graph.arcs[k].node2 && graph.arcs[k].node1 != NParent.Id) {
                                graph.arcs.RemoveAt(k--);
                                goto A;
                            }
                        }
                    }
                }
            }
            if (!b) {
                Father = null;
                Tree a = parents[0];
                Children = a.GetRoot().Children;
            } else if (graph.arcs.Count > 0) {
                if (children.Count > 0) {
                    parents = children;
                    children = new List<Tree>();
                }
                goto Operation;
            }
        }
        public Tree Father;
        public int Id;
        public List<Tree> Children = new List<Tree>();
        public bool HasNode(int id) {
            var parents = new List<Tree>() { GetRoot() };
            var children = new List<Tree>();
        Operation:
            for (int i = 0; i < parents.Count; i++) {
                var NParent = parents[i];
                if (NParent.Id == id) {
                    return true;
                }
                var NChildren = NParent.Children;
                for (int j = 0; j < NChildren.Count; j++) {
                    children.Add(NChildren[j]);
                }
            }
            if (children.Count == 0) {
                return false;
            } else {
                parents = children;
                children = new List<Tree>();
                goto Operation;
            }
        }
        public Tree GetRoot() {
            Tree tree = this;
            while (tree.Father != null) {
                tree = tree.Father;
            }
            return tree;
        }
        public static Tree GetRoot(Tree tree) {
            while (tree.Father != null) {
                tree.Father = tree.Father.Father;
            }
            return tree;
        }
        public static implicit operator string(Tree tree) {
            string result = "";
            List<Tree> parents = new List<Tree>() { tree.GetRoot() };
            List<Tree> children = new List<Tree>();
        Operation:
            for (int i = 0; i < parents.Count; i++) {
                Tree NParent = parents[i];
                result += NParent.Id + ": " + Environment.NewLine;
                List<Tree> NChildren = NParent.Children;
                for (int j = 0; j < NChildren.Count; j++) {
                    result += "\tChild[" + j + "]: " + NChildren[j].Id;
                    children.Add(NChildren[j]);
                }
                result += Environment.NewLine;
            }
            if (children.Count == 0) {
                return result;
            } else {
                parents = children;
                children = new List<Tree>();
                goto Operation;
            }
        }
    }*/

    public class Tree<T> {

        public Tree() { }

        public Tree(T node) {
            Node = node;
        }

        public Tree<T> Father;

        public T Node;

        public readonly List<Tree<T>> Children = new List<Tree<T>>();

        public Tree<T> Clone {
            get {
                Tree<T> copy = new Tree<T>(CloneT == null ? Node : CloneT.Invoke());
                foreach (var child in Children) {
                    copy.Children.Add(child.Clone);
                }
                return copy;
            }
        }

        public Func<T> CloneT;

        public Tree<T> Add(T child) {
            var branch = new Tree<T>(child) {
                Father = this,
                CloneT = CloneT
            };
            Children.Add(branch);
            return branch;
        }

        public void Add(Tree<T> child) {
            if (child == null) return;
            Children.Add(child);
        }

        public Tree<(int, T)> GetDeepth() {
            return GetDeepths(this);
        }

        public Tree<T> GetBranches() {
            return GetChildrenOnly(this);
        }

        static Tree<T> GetChildrenOnly(Tree<T> t) {
            if (t.Children.Count == 0) {
                return t;
            }
            var a = new Tree<T>();
            foreach (var child in t.Children) {
                a.Add(GetChildrenOnly(child));
            }
            return a;
        }

        public Tree<T> Where(Func<T, bool> func) {
            return Where(this, func);
        }

        static Tree<T> Where(Tree<T> tree, Func<T, bool> func) {
            if (!func.Invoke(tree.Node)) {
                return null;
            }
            for (int i = 0; i < tree.Children.Count; i++) {
                var child = tree.Children[i];
                var n = Where(child, func);
                if (n == null) {
                    tree.Children.RemoveAt(i--);
                } else {
                    tree.Children[i] = n;
                }
            }
            return tree;
        }

        static Tree<(int, T)> GetDeepths(Tree<T> tree, int deepth = 0) {
            var a = new Tree<(int, T)> {
                Node = (deepth, tree.Node)
            };
            for (int i = 0; i < tree.Children.Count; i++) {
                var child = tree.Children[i];
                a.Add(GetDeepths(child, deepth + 1));
            }
            return a;
        }

    }

    public class TreeDrawer<T> {

        public TreeDrawer(Tree<T> tree) {
            Tree = tree;
        }

        public int VerticalMargin = 50;
        public int SameFatherChildrenMargin = 50;
        public int DifferentChildrenMargin = 60;
        readonly Tree<T> Tree;

        public Tree<((int, int), T)> GetPositions() {
            var a = new Tree<((int, int), T)>();

            return a;
        }

    }


}