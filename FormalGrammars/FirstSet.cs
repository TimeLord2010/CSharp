using System.Collections.Generic;
using System.Linq;

namespace FormalGrammars {

    public class FirstSet : GrammarSet {

        public FirstSet(Grammar grammar) : base(grammar) {
        }

        public override HashSet<object> Get(Symbol symbol) {
            var set = new HashSet<object>();
            foreach (var def in symbol) {
                var first = def.First();
                if (first is string str /*&& def.Count() > 1*/) {
                    set.Add(str);
                } else if (first is Symbol symb) {
                    if (symb.HasEmptyString) {
                        var to_add = Get(symb);
                        to_add.Remove("");
                        set.UnionWith(to_add);
                        for (var i = 1; i < def.Count(); i++) {
                            var element = def.ElementAt(i);
                            if (element is string s) {
                                set.UnionWith(Get(s));
                            } else if (element is Symbol sy) {
                                set.UnionWith(Get(sy));
                            }
                        }
                    } else {
                        set.UnionWith(Get(symb));
                    }
                }
            }
            return set;
        }

        public override HashSet<object> Get(string s) {
            return new HashSet<object> {
                s
            };
        }
    }

}