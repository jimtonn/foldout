using Foldout.Core.Events;
using System;

namespace Foldout.Core.Columns
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
    }

    public abstract class ColumnDefinition<T> : ColumnDefinition where T : ColumnValue 
    {
        public ColumnDefinition(string title = null) : base(typeof(T), title)
        {
        }
    }
}
