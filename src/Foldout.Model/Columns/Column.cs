using System;
using System.Runtime.CompilerServices;

namespace Foldout.Model.Columns
{
    public abstract class Column
    {
        public Type ColumnValueType { get; protected set;  }

        public string Title { get; internal set; }

        public Column(Type columnValueType, string title)
        {
            ColumnValueType = columnValueType;
            Title = title;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(obj, this);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public object DefaultValue;
    }

    public abstract class Column<T> : Column
    {
        public Column(string title = null) : base(typeof(T), title)
        {
        }
    }
}
