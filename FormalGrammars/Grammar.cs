using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FormalGrammars {

        public class Grammar : IEnumerable<Symbol> {

        readonly List<Symbol> Symbols = new List<Symbol>();
        public Symbol Initial {
            get => Symbols.First();
        }

        public static Grammar Parse(string content) {
            var grammar = new Grammar();
            var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines) {
                if (String.IsNullOrWhiteSpace(line)) continue;
                if (!line.Contains("->"))
                    throw new ArgumentException($"Invalid line ({line}).");
                var symbol_rules = line.Split(new string[] { "->" }, 2, StringSplitOptions.None);
                var symbol = Symbol.New(symbol_rules[0], symbol_rules[1]);
                grammar.Symbols.Add(symbol);
            }
            foreach (var symbol in grammar) {
                foreach (var def in symbol) {
                    for (int i = 0; i < def.Count(); i++) {
                        var element = def.ElementAt(i);
                        if (element is Symbol sub_symbol) {
                            def.Elements[i] = grammar.GetSymbol(sub_symbol.Id);
                        }
                    }
                }
            }
            return grammar;
        }

        public void AddNewStartingSymbol (string id) {
            var symbol = Symbol.New( this, id, Symbols[0].Id);
            var def = symbol.ElementAt(0);
            def.Elements[0] = Symbols[0];
            Symbols.Insert(0, symbol);
        }

        public void RemoveLeftRecursion() {
            for (var i = 0; i < Symbols.Count; i++) {
                var symbol = Symbols[i];
                for (var j = 0; j < symbol.Definitions.Count; j++) {
                    var def = symbol.Definitions[j];
                    var first = def.First();
                    if (first is Symbol sub_symbol && sub_symbol.Id == symbol.Id) {
                        RemoveLeftRecursion(symbol, def);
                    }
                }
            }
        }

        public void RemoveLeftRecursion(Symbol symbol, Definition def) {
            symbol.Definitions.Remove(def);
            def.Elements.RemoveAt(0);
            var new_id = symbol.Id + "'";
            var new_symbol = Symbol.New(this, new_id, $"{def}{new_id}|");
            foreach (var d in symbol) {
                d.Add(new_symbol, true);
            }
            Symbols.Add(new_symbol);
        }

        public static Grammar ParseFromFile(string file) {
            return Parse(File.ReadAllText(file, Encoding.UTF8));
        }

        public Symbol GetSymbol(string id) {
            foreach (var symbol in this) {
                if (symbol.Id == id) return symbol;
            }
            throw new KeyNotFoundException($"Id not found for symbol");
        }

        public override string ToString() {
            var a = "";
            foreach (var symbol in Symbols) {
                a += $"{symbol}{Environment.NewLine}";
            }
            return a;
        }

        public IEnumerator<Symbol> GetEnumerator() {
            return Symbols.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Symbols.GetEnumerator();
        }

        public HashSet<string> Terminals {
            get {
                var set = new HashSet<string>();
                foreach (var symbol in this) {
                    set.UnionWith(symbol.Terminals);
                }
                return set;
            }
        }

    }

}