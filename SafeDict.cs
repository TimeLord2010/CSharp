using System.Collections.Generic;

class SafeDict<K, V> : Dictionary<K, V> {

    public SafeDict() {}
    public SafeDict(V defaultValue) {
        DefaultValue = defaultValue;
    }

    public V DefaultValue;

    public new V this[K key] {
        get {
            if (!ContainsKey(key)) {
                return DefaultValue;
            }
            return this[key];
        }
        set {
            if (ContainsKey(key)) {
                this[key] = value;
            } else {
                Add(key, value);
            }
        }
    }

}