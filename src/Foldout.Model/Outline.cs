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
        /// <param name="rowValues"></param>
        public void AddColumn(Column column, IDictionary<Row, object> rowValues = null)
        {
            if (RootRow.RowValues.ContainsKey(column))
            {
                throw new InvalidOperationException("Column already added to outline.");
            }

            foreach(var row in EachRow.Concat(new[] { RootRow }))
            {
                row.RowValues.Add(column, column.DefaultValue);
                if (rowValues != null && rowValues.TryGetValue(row, out object value))
                {
                    row.RowValues[column] = value;
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

            var removedValues = new Dictionary<Row, object>();

            foreach (var row in EachRow.Concat(new[] { RootRow }))
            {
                removedValues.Add(row, row[column]);
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

        public Row InsertRow(Row parent, int position, IDictionary<Column, object> initialData = null)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            if (position < 0 || position > parent.Children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            var row = new Row
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
                    row.RowValues[col] = col.DefaultValue;
                }
            }

            RowAdded?.Invoke(this, new RowAddedEventArgs(row, position));
            return row;
        }

        public void ChangeRowData<T>(Row row, Column column, T value)
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
                
                foreach(var child in lastParentWithIndent.row.Children)
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
        internal IEnumerable<Row> EachRow
        {
            get
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
        }

        /// <summary>
        /// Removes the specified row from the outline. A row must have 0 children before
        /// it can be removed. Callers are responsible for reparenting or removing any
        /// children prior to a move.
        /// </summary>
        public void RemoveRow(Row row)
        {
            if (row == RootRow)
            {
                throw new InvalidOperationException("Root row cannot be removed.");
            }

            if (row.Children.Any())
            {
                throw new InvalidOperationException("Row cannot be removed because it has children.");
            }

            if (!row.ParentRow.Children.Contains(row))
            {
                throw new InvalidOperationException("Row not found. It may have already been removed.");
            }

            row.ParentRow._children.Remove(row);
            RowRemoved?.Invoke(this, new RowRemovedEventArgs(row));
        }

        public void ReparentRow(Row row, Row newParent, int position)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (newParent == null)
            {
                throw new ArgumentNullException(nameof(newParent));
            }

            if (row == RootRow)
            {
                throw new InvalidOperationException("Root row cannot be reparented.");
            }

            row.ParentRow._children.Remove(row);

            if (position < 0 || position > newParent.Children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            newParent._children.Insert(position, row);

            var previousParent = row.ParentRow;
            row.ParentRow = newParent;

            RowReparented?.Invoke(this, new RowReparentedEventArgs(row, previousParent, newParent));
        }
    }
}
