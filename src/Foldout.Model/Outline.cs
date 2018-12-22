using System;
using System.Collections.Generic;
using System.Linq;
using Foldout.Model.Columns;
using Foldout.Model.Events;

namespace Foldout.Model
{
    public class Outline
    {
        //The philosophy of outline-level events is that they should
        //broadcast changes to the outline that would have to be updated in a visual
        //representation. The presentation layer should probably, in fact, listen to these
        //events and do updates from them.
        public event EventHandler<ColumnAddedEventArgs> ColumnAdded;
        public event EventHandler<ColumnRemovedEventArgs> ColumnRemoved;
        public event EventHandler<RowAddedEventArgs> RowAdded;
        public event EventHandler<RowRemovedEventArgs> RowRemoved;
        public event EventHandler<RowReparentedEventArgs> RowReparented;
        public event EventHandler<RowDataChangedEventArgs> RowDataChanged;
        public event EventHandler<ColumnTitleChangedEventArgs> ColumnTitleChanged;

        public readonly Row RootRow = new Row();

        /// <summary>
        /// Adds a column to this outline, with optional data.
        /// Generally, row data will be provided when we are undoing an operation that deleted a column, and we
        /// want to restore all of the old data before we notify listeners about the restored column.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="columnValues"></param>
        public void AddColumn(Column column, IDictionary<Row, ColumnValue> columnValues = null)
        {
            if (RootRow.RowValues.ContainsKey(column))
            {
                throw new InvalidOperationException("Column already added to outline.");
            }

            foreach(var row in EachRow().Concat(new[] { RootRow }))
            {
                row.RowValues.Add(column, (ColumnValue)Activator.CreateInstance(column.ColumnValueType));
                if (columnValues != null)
                {
                    //todo unit test the scenario where we are supplying data here
                    row.RowValues[column] = columnValues[row];
                }
            }
            ColumnAdded?.Invoke(this, new ColumnAddedEventArgs(column));
        }

        public void RemoveColumn(Column column)
        {
            if (!RootRow.RowValues.ContainsKey(column))
            {
                throw new InvalidOperationException("Column not added to outline.");
            }

            var removedValues = new Dictionary<Row, ColumnValue>();

            foreach (var row in EachRow().Concat(new[] { RootRow }))
            {
                removedValues.Add(row, (ColumnValue)row[column].Clone());
                row.RowValues.Remove(column);
            }
            ColumnRemoved?.Invoke(this, new ColumnRemovedEventArgs(column, removedValues));
        }

        public void ChangeColumnTitle(Column column, string newTitle)
        {
            var previousTitle = column.Title;
            column.Title = newTitle;
            ColumnTitleChanged?.Invoke(this, new ColumnTitleChangedEventArgs(column, previousTitle, newTitle));
        }

        public IEnumerable<Column> Columns => RootRow.RowValues.Keys;

        public Row InsertRow(Row parent, int position, IDictionary<Column, ColumnValue> initialData = null)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (position < 0 || position > parent.Children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            var row = new Row()
            {
                ParentRow = parent
            };
            parent._children.Insert(0, row);

            //todo unit test data assignment here
            foreach (var col in Columns)
            {
                if (initialData != null && initialData.TryGetValue(col, out var initialValue))
                {
                    row.RowValues[col] = initialValue;
                }
                else
                {
                    row.RowValues[col] = (ColumnValue)Activator.CreateInstance(col.ColumnValueType);
                }
            }

            RowAdded?.Invoke(this, new RowAddedEventArgs(row, position));
            return row;
        }

        public void ChangeRowData<T>(Row row, Column column, T value) where T : ColumnValue
        {
            var previousValue = row.GetValue<T>(column);
            row.RowValues[column] = value;
            RowDataChanged?.Invoke(this, new RowDataChangedEventArgs<T>(row, previousValue, value));
        }

        public IEnumerable<(Row row, int indentLevel)> RowsWithIndent()
        {
            var rowsWithIndents = new Stack<(Row row, int indentLevel)>();
            rowsWithIndents.Push((RootRow, -1));

            while(rowsWithIndents.Any())
            {
                var lastParentWithIndent = rowsWithIndents.Pop();
                
                foreach(var child in lastParentWithIndent.row.Children.Reverse())
                {
                    rowsWithIndents.Push((child, lastParentWithIndent.indentLevel+1));
                }

                if (lastParentWithIndent.row != RootRow)
                {
                    yield return lastParentWithIndent;
                }
            }
        }

        /// <summary>
        /// Yields all rows in the outline, excluding the RootRow.
        /// Does not necessarily return in order. This can be used for internal
        /// operations like adding a column to each row.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<Row> EachRow()
        {
            var rowsWithIndents = new Stack<Row>();
            rowsWithIndents.Push(RootRow);

            while (rowsWithIndents.Any())
            {
                var lastParentWithIndent = rowsWithIndents.Pop();

                foreach (var child in lastParentWithIndent.Children)
                {
                    rowsWithIndents.Push(child);
                }

                if (lastParentWithIndent != RootRow)
                {
                    yield return lastParentWithIndent;
                }
            }
        }

        public void RemoveRowAndChildren()
        {

        }

        public void RemoveRowKeepChildren()
        {

        }
    }
}
