using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormalGrammars {

    public class Definition : IEnumerable<string> {

        public Definition (Grammar grammar) {
            Grammar = grammar;
        }

        Grammar Grammar { get; }
        public List<string> Elements = new List<string>();

        public HashSet<string> Terminals {
            get {
                var set = new HashSet<string>();
                set.UnionWith(Elements.Where(x => x is string str && str.Length > 0).Select(x => x.ToString()));
                return set;
            }
        }

        public IEnumerable<Symbol> SymbolElements {
            get => Elements.Where(x => Regex.IsMatch(x, "[A-Z]+")).Select(x => Grammar.GetSymbol(x));
        }

        public void Add(string a, bool safe_add = false) {
            if (!ValidateStringAdd(a, safe_add)) return;
            Elements.Add(a);
        }

        public void Add(Symbol s, bool safe_add = false) {
            if (!ValidateSymbolAdd(safe_add)) return;
            Elements.Add(s.Id);
        }

        public void Insert (int index, string input, bool safe_insert = false) {
            if (!ValidateStringAdd(input, safe_insert)) return;
            Elements.Insert(index, input);
        }

        public void Insert (int index, Symbol symbol, bool safe_insert = false) {
            if (!ValidateSymbolAdd(safe_insert)) return;
            Elements.Insert(index, symbol.Id);
        }

        bool ValidateStringAdd (string a, bool safe_add) {
            if (Contains("") || (Elements.Count > 0 && a == "")) {
                if (safe_add) return false;
                throw new Exception($"The empty string must the the only element in the definiion.");
            }
            return true;
        }

        bool ValidateSymbolAdd (bool safe_add) {
            if (Contains("")) {
                if (safe_add) return false;
                throw new Exception($"The empty string must the the only element in the definiion.");
            }
            return true;
        }

        public bool Contains(string a) {
            return Terminals.Contains(a);
        }

        public IEnumerator<string> GetEnumerator() {
            return Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Elements.GetEnumerator();
        }

        public Definition Clone() {
            var list = new List<string>();
            foreach (var element in this) {
                if (element is string str) {
                    list.Add(str);
                }
            }
            return new Definition(Grammar) {
                Elements = list
            };
        }

        public override string ToString() {
            return string.Join("", Elements);
        }

    }

}