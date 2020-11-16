using System;
using System.Collections.Generic;
using System.Linq;

public static class FuncUtils {

    public static bool ContainsAny<T>(IEnumerable<T> collection, params T[] ts) {
        foreach (var t in ts) {
            if (collection.Contains(t)) {
                return true;
            }
        }
        return false;
    }

    public static bool ContainsAll<T>(IEnumerable<T> collection, params T[] ts) {
        foreach (var t in ts) {
            if (!collection.Contains(t)) {
                return false;
            }
        }
        return true;
    }

    public static bool Any<T>(IEnumerable<T> collection, Func<T, T, bool> comparator, params T[] ts) {
        foreach (var t in ts) {
            foreach (var element in collection) {
                if (comparator.Invoke(t, element)) {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool AnyEquals<T>(T t, params T[] ts) {
        return Any(ts, (x1, x2) => x1.Equals(x2), t);
    }

}