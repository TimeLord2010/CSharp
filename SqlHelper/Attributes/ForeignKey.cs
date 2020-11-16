using System;

namespace SqlHelper.Attributes {

    public enum ForeignKeyActions {
        Null,
        NoAction,
        Cascasde,
        SetNull,
        SetDefault
    }

    public class ForeignKey : SqlColumn  {

        public ForeignKey (Type type, ForeignKeyActions onDelete = ForeignKeyActions.NoAction, ForeignKeyActions onUpdate = ForeignKeyActions.NoAction) {
            Type = type;
            OnDelete = onDelete;
            OnUpdade = onUpdate;
        }

        public Type Type { get; }
        public ForeignKeyActions OnDelete { get; }
        public ForeignKeyActions OnUpdade { get; }

    }

}