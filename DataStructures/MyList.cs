using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MyList<T> : IEnumerable<T>, IList<T>, ICollection<T> {

    public MyList(bool is_read_only = false) {
        IsReadOnly = is_read_only;
    }

    public delegate void OnAddHandler(OnAddEventArgs<T> args);
    public event OnAddHandler OnAdd;

    readonly List<T> list = new List<T>();

    public T this[int index] {
        get => list[FilterIndex(index)];
        set => list[FilterIndex(index)] = value;
    }

    public int Count {
        get => list.Count;
    }

    public bool IsReadOnly { get; set; }

    public void Add(T t) {
        if (!_OnAdd(t)) return;
        list.Add(t);
    }

    public bool FilterNegativeIndex = true;
    public bool FilterOverFlowIndex = false;

    public T FirstOr(Func<T, bool> func = null, T t = default(T)) {
        if (func == null) return list.Count == 0 ? t : list.ElementAt(0);
        return list.Any(func) ? list.First(func) : t;
    }

    public void AddRange(IEnumerable<T> collection) {
        list.AddRange(collection);
    }

    public static MyList<T> Cast(IEnumerable<T> collection) {
        MyList<T> a = new MyList<T>();
        foreach (var item in collection) {
            a.Add(item);
        }
        return a;
    }

    public IEnumerator<T> GetEnumerator() {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public int IndexOf(T item) => list.IndexOf(item);

    public void Insert(int index, T item) {
        if (!_OnAdd(item)) return;
        list.Insert(index, item);
    }

    public void RemoveAt(int index) {
        list.RemoveAt(index);
    }

    public void Clear() {
        list.Clear();
    }

    public bool Contains(T item) {
        return list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item) {
        return list.Remove(item);
    }

    bool _OnAdd (T item) {
        if (OnAdd != null) {
            var args = new OnAddEventArgs<T>(item);
            OnAdd(args);
            if (args.Cancel) {
                return false;
            }
        }
        return true;
    }

    int FilterIndex(int index) {
        if (Count == 0) return index;
        while (index < 0 && FilterNegativeIndex) {
            index += Count;
        }
        while (index >= Count && FilterOverFlowIndex) {
            index -= Count;
        }
        return index;
    }

}

public class OnAddEventArgs<T> : EventArgs {

    public OnAddEventArgs(T item) {
        Item = item;
    }

    public T Item { get; set; }
    public bool Cancel { get; set; }

}