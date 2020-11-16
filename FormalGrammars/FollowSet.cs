using System;
using System.Collections.Generic;
using System.Linq;

namespace FormalGrammars {
    
    public class FollowSet : GrammarSet {

        public FollowSet(Grammar grammar) : base(grammar) {
            foreach (var symbol in Grammar) {
                Sets.Add(symbol, new HashSet<object>());
            }
            First_Set = new FirstSet(Grammar);
            foreach (var symbol in Grammar) {
                var set = Sets[symbol];
                if (symbol == Grammar.Initial) {
                    set.Add($"$");
                }
                foreach (var def in symbol) {
                    for (var i = 0; i < def.Count(); i++) {
                        Rule2(def, i);
                        Rule3(symbol, def, i);
                        //if (def.Count() > 2) {
                        //    Rule2(def, 1);
                        //    Rule3(symbol, def, 1);
                        //} else if (def.Count() == 2) {
                        //    Rule2(def, 2);
                        //    Rule3(symbol, def, 2);
                        //}
                    }
                }
            }
        }

        readonly Dictionary<Symbol, HashSet<object>> Sets = new Dictionary<Symbol, HashSet<object>>();
        readonly FirstSet First_Set;

        public override HashSet<object> Get(Symbol symbol) {
            return Sets[symbol];
        }

        public override HashSet<object> Get(string terminal) {
            throw new NotImplementedException();
        }

        void Rule2(Definition def, int i) {
            var x = def.ElementAt(i);
            if (x is Symbol s1) {
                var x_follows = Sets[s1];
                for (var j = i + 1; j < def.Count(); j++) {
                    var beta = def.ElementAt(j);
                    if (beta is Symbol sym) {
                        var fs = First_Set.Get(sym);
                        fs.Remove("");
                        x_follows.UnionWith(fs);
                        if (!sym.HasEmptyString) {
                            break;
                        }
                    } else if (beta is string str && str.Length > 0) {
                        var fs = First_Set.Get(str);
                        x_follows.UnionWith(fs);
                        break;
                    }
                }
            }
        }

        void Rule3(Symbol sym, Definition def, int i) {
            var x = def.ElementAt(i);
            if (x is Symbol s1) {
                var x_follows = Sets[s1];
                bool has_empty = true;
                for (var j = i + 1; j < def.Count(); j++) {
                    var beta = def.ElementAt(j);
                    if ((beta is Symbol sym1 && !sym1.HasEmptyString) || (beta is string str && str.Length > 0)) {
                        has_empty = false;
                        break;
                    }
                }
                if (has_empty) {
                    var a_follow = Sets[sym].Where(y => y is string str && str.Length > 0);
                    x_follows.UnionWith(a_follow);
                    //foreach (var def1 in sym) {
                    //    foreach (var item in def1.OfType<string>().Where(y => y.Length > 0)) {
                    //        x_follows.Add(item);
                    //    }
                    //}
                }
            }
        }

    }

}