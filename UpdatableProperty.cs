using System;
using System.Collections.Generic;

public class UpdatableProperty <T> {

    public UpdatableProperty (Func<T> getter, Func<DateTime, bool> should_reset = null, T @default = default) {
        if (should_reset == null) should_reset = (_) => false;
        Should_Reset = should_reset;
        Default = @default;
        Getter = getter;
        Restore();
    }

    public Func<DateTime, bool> Should_Reset { get; }
    public Func<T> Getter { get; }
    public T Default { get; }
    public bool IsReseterEnabled { get; set; } = false;
    public DateTime LastUpdated { get; private set; }
    T Original;
    public T Property {
        get {
            //if (IsReseterEnabled && Should_Reset(LastUpdated)) {
            //    Original = Default;
            //}
            //if (EqualityComparer<T>.Default.Equals(Original, Default)) {
            //    Original = Getter();
            //    LastUpdated = DateTime.Now;
            //}
            return Original;
        }
    }
    public void Reset () {
        Original = Default;
    }
    public void Restore () {
        Original = Getter();
        LastUpdated = DateTime.Now;
    }
    public static implicit operator T (UpdatableProperty<T> updatable) {
        return updatable.Property;
    }
}