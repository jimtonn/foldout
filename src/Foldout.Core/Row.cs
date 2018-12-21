using System;
using System.Collections.Generic;
using System.Linq;
using Foldout.Core.Columns;
using Foldout.Core.Events;

namespace Foldout.Core {
    public class Row
    {
        /// <summary>
        /// Each row has a reference to the containing outline, mostly so it can fire events at the outline level.
        /// </summary>
        private readonly Outline _outline;

        public Row ParentRow { get; private set; }

        public readonly Dictionary<ColumnDefinition, ColumnValue> RowValues;

        //todo do we actually need this accessor?
        public ColumnValue this[ColumnDefinition columnDefinition] => RowValues[columnDefinition];

        public IReadOnlyList<Row> Children => _children.AsReadOnly();
        private readonly List<Row> _children = new List<Row>();

        public Row(Outline outline, Row parent, IEnumerable<ColumnDefinition> columnDefinitions)
        {
            _outline = outline;
            ParentRow = parent;

            RowValues = new Dictionary<ColumnDefinition, ColumnValue>();
            foreach(var col in columnDefinitions)
            {
                RowValues.Add(col, (ColumnValue)Activator.CreateInstance(col.ColumnValueType));
            }
        }

        /// <summary>
        /// Produces an enumerable containing each row beneath this row.
        /// This will probably not be used for display but for making updates to the entire outline, such as
        /// column changes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Row> EachRowUnder()
        {
            var rows = new Stack<Row>();
            foreach (var child in Children)
            {
                rows.Push(child);
            }

            while (rows.Any())
            {
                var row = rows.Pop();
                yield return row;
                foreach (var child in row.Children)
                {
                    rows.Push(child);
                }
            }
        }

        public Row InsertEmptyRow(int position)
        {
            var row = new Row(this._outline, this, this.RowValues.Keys);
            _children.Insert(position, row);

            _outline.FireRowAdded(new RowAddedEventArgs(row, position));
            return row;
        }

        public T GetValue<T>(ColumnDefinition columnDefinition) => (T)RowValues[columnDefinition].Data;

        public void SetValue<T>(ColumnDefinition columnDefinition, T value)
        {
            var previousData = GetValue<T>(columnDefinition);
            RowValues[columnDefinition].Data = value;
            _outline.FireRowDataChanged(new RowDataChangedEventArgs<T>(this, previousData, value));
        }
    }
}