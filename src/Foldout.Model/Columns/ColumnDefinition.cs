using System;
using System.Runtime.CompilerServices;

namespace Foldout.Model.Columns
{
    public abstract class ColumnDefinition
    {
        public Type ColumnValueType { get; protected set;  }

        public string Title { get; internal set; }

        public ColumnDefinition(Type columnValueType, string title)
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

    }

    public abstract class ColumnDefinition<T> : ColumnDefinition where T : ColumnValue 
    {
        public ColumnDefinition(string title = null) : base(typeof(T), title)
        {
        }
    }
}
