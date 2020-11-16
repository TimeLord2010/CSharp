using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diagrams {

    class DiagramPositionRetriver<T> {

        public DiagramPositionRetriver(Tree<T> t) {
            Tree = t;
        }

        public int NodeWidth = 50;
        public int VerticalMargin = 50;
        public int DiferentFatherHorizontalMargin = 60;
        public int SameFatherHorizontalMargin = 50;
        Tree<T> Tree;

        public Tree<((int, int), T)> GetPositions(out int width) {
            var a = GetPosition(Tree, 0, 0, out width);
            width += NodeWidth;
            return a;
        }

        Tree<((int x, int y) position, T tree)> GetPosition(Tree<T> tree, int x, int y, out int width) {
            var a = new Tree<((int x, int y) pos, T)>(((x, y), tree.Node));
            if (tree.Children.Count == 0) {
                a.Node.pos.x += NodeWidth / 2;
                width = NodeWidth;
                return a;
            }
            int OriginalX = x;
            int lastWidth = -1;
            for (int i = 0; i < tree.Children.Count; i++) {
                var child = tree.Children[i];
                var pos = GetPosition(child, x, y + VerticalMargin + NodeWidth, out int width2);
                a.Children.Add(pos);
                if (i < tree.Children.Count - 1) {
                    int add = lastWidth == -1 ? NodeWidth : lastWidth;
                    x = pos.Node.position.x + SameFatherHorizontalMargin + ((add/2) + (width2/2));
                } else {
                    x = pos.Node.position.x + width2/2;
                }
                lastWidth = width2;
            }
            width = x - OriginalX;
            a.Node.pos.x = OriginalX + (width / 2);
            a.Node.pos.y = y;
            return a;
        }

    }

}
/*
class DiagramPositions:

	def __init__(self, father, *args):
		self.father = father
		self.children = list(args)

class DiagramPositionRetriver (list):

	verticalSpacing = 50
	horizontalSpacing = 50

	def __init__(self, father):
		self.father = father
		self.children = []

	def append (self, child):
		if not isinstance(child, DiagramPositionRetriver):
			child = DiagramPositionRetriver(child)
		self.children.append(child)

	def __getitem__(self, index):
		return self.children[index]

	@property
	def positions (self) -> ((float,float),[float]):
		children = []
		last_width = 0
		for child in self.children:
			x = child.width / 2
			if len(children) > 0:
				x += last_width / 2
			last_width = child.width
			children.append(x)
		children = [child + (i * self.horizontalSpacing) for i, child in enumerate(children)]
		father = (sum(children), self.verticalSpacing)
		return (father, children)

	@property
	def width (self) -> float:
		return None

	@property
	def heigth (self) -> float:
		return None
*/
