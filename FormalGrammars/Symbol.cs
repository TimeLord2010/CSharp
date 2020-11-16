using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormalGrammars {

    public class Symbol : IEnumerable<Definition> {

        public Symbol(Grammar grammar) {
            Grammar = grammar;
        }

        public string Id;
        Grammar Grammar { get; }
        public List<Definition> Definitions = new List<Definition>();

        public bool HasEmptyString {
            get {
                return Definitions.Any(x => x.Contains("") || x.All(y => {
                    if (Regex.IsMatch(y, "[A-Z]+")) {
                        var sym = Grammar.GetSymbol(y);
                        return sym.HasEmptyString;
                    } else {
                        return y.Length == 0;
                    }
                }));
            }
        }

        public static Symbol New( Grammar grammar, string id, string definitions = null) {
            var symbol = new Symbol(grammar);
            if (!Regex.IsMatch(id, "[a-zA-Z]+"))
                throw new ArgumentException($"Symbol ({id}) must contain only letters.");
            symbol.Id = id;
            if (definitions == null) return symbol;
            var sets = definitions.Split('|');
            foreach (var set in sets) {
                var s = new Definition(grammar);
                if (set.Length == 0) {
                    s.Add("");
                }
                foreach (var character in set) {
                    var c = character + "";
                    if (Regex.IsMatch(c, "[A-Z]")) {
                        s.Add(New(grammar, c));
                    } else {
                        s.Add(c);
                    }
                }
                symbol.Definitions.Add(s);
            }
            return symbol;
        }

        public IEnumerator<Definition> GetEnumerator() {
            return Definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Definitions.GetEnumerator();
        }

        public Symbol Clone (Dictionary<string, Symbol> recovered = null) {
            var s = new Symbol(Grammar);
            s.Id = Id;
            s.Definitions = Definitions.Select(x => x.Clone()).ToList();
            return s;
        }

        public HashSet<string> Terminals {
            get {
                var set = new HashSet<string>();
                foreach (var def in this) {
                    set.UnionWith(def.Terminals);
                }
                return set;
            }
        }

        public override string ToString() {
            return $"{Id} -> {string.Join("|", Definitions)}";
        }
    }
    
}