using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Foldout.Model.Columns;
using Foldout.Model.Events;

namespace Foldout.Model {
    public class Row
    {
        public Row ParentRow { get; internal set; }

        public readonly Dictionary<Column, object> RowValues = new Dictionary<Column, object>();

        public object this[Column column] => RowValues[column];

        public IReadOnlyList<Row> Children => _children.AsReadOnly();
        internal readonly List<Row> _children = new List<Row>();

        internal Row() { }

        public T GetValue<T>(Column column) => (T)RowValues[column];

        public override bool Equals(object obj)
        {
            return ReferenceEquals(obj, this);
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }
    }
}