using System.Collections.Generic;

namespace FormalGrammars {
    
        public abstract class GrammarSet {

        public GrammarSet(Grammar grammar) {
            grammar.RemoveLeftRecursion();
            Grammar = grammar;
        }

        public Grammar Grammar { get; }

        public List<(Symbol, HashSet<object>)> AllSets {
            get {
                var sets = new List<(Symbol, HashSet<object>)>();
                foreach (var symbol in Grammar) {
                    sets.Add((symbol, Get(symbol)));
                }
                return sets;
            }
        }

        public abstract HashSet<object> Get(Symbol symbol);
        public abstract HashSet<object> Get(string terminal);

    }
}