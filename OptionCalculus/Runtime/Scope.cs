using System.Collections.Generic;

namespace OptionCalculus.Runtime {
    public sealed class Scope<V> {
        private class Entry<T> {
            private readonly Dictionary<string, T> bindings;

            public Entry(Entry<T> parent) {
                Parent = parent;
                bindings = new Dictionary<string, T>();
            }

            public Entry<T> Parent { get; }

            public bool Contains(string name) {
                return bindings.ContainsKey(name);
            }

            public T Get(string name) {
                return bindings[name];
            }

            public void Put(string name, T value) {
                bindings[name] = value;
            }
        }

        private Entry<V> head;

        public Scope() {
            head = new Entry<V>(null);
        }

        private Entry<V> findEntryFor(string name) {
            var entry = head;

            while (!entry.Contains(name)) {
                entry = entry.Parent;
                if (entry == null) break;
            }

            return entry;
        }

        public V Lookup(string name) {
            var entry = findEntryFor(name);
            return entry == null ? default(V) : entry.Get(name);
        }

        public void Bind(string name, V value) {
            var entry = findEntryFor(name);

            if (entry == null) {
                head.Put(name, value);
            } else {
                entry.Put(name, value);
            }
        }

        public void Push() {
            head = new Entry<V>(head);
        }

        public void Pop() {
            head = head.Parent;
        }
    }
}