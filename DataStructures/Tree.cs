using System;
using System.Collections.Generic;

namespace DataStructures {

    public class Tree<T> {

        public Tree() { }

        public Tree(T node) {
            Node = node;
        }

        public Tree<T> Father;
        public bool AllowNull = true;
        public T Node;

        public List<Tree<T>> Children = new List<Tree<T>>();

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
            if (!AllowNull && child == null) {
                throw new ArgumentNullException();
            }
            var branch = new Tree<T>(child) {
                Father = this,
                CloneT = CloneT,
                AllowNull = AllowNull
            };
            Children.Add(branch);
            return branch;
        }

        public void Add(Tree<T> child) {
            if (child == null) return;
            Children.Add(child);
        }

        public Tree<(int, T)> GetDeepths() {
            return GetDeepths(this);
        }

        public int GetDeepth() {
            int max_deepth = 0;
            GetDeepths().ForEach((tree) => {
                if (tree.Node.Item1 > max_deepth) {
                    max_deepth = tree.Node.Item1;
                }
            });
            return max_deepth;
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

        void ForEach(Action<Tree<T>> action) {
            ForEach(this, action);
        }

        static void ForEach(Tree<T> tree, Action<Tree<T>> action) {
            foreach (var child in tree.Children) {
                action(child);
                foreach (var child_child in child.Children) {
                    ForEach(child_child, action);
                }
            }
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

}