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

        public readonly Row RootRow;

        public Outline()
        {
           RootRow = new Row(
               this, 
               null, //parent
               Enumerable.Empty<ColumnDefinition>()
               );
        }

        /// <summary>
        /// Adds a column to this outline, with optional data.
        /// Generally, row data will be provided when we are undoing an operation that deleted a column, and we
        /// want to restore all of the old data before we notify listeners about the restored column.
        /// </summary>
        /// <param name="columnDefinition"></param>
        /// <param name="columnValues"></param>
        public void AddColumnDefinition(ColumnDefinition columnDefinition, IDictionary<Row, ColumnValue> columnValues = null)
        {
            if (RootRow.RowValues.ContainsKey(columnDefinition))
            {
                throw new InvalidOperationException("Column already added to outline.");
            }

            foreach(var row in RootRow.EachRowUnder().Concat(new[] { RootRow }))
            {
                row.RowValues.Add(columnDefinition, (ColumnValue)Activator.CreateInstance(columnDefinition.ColumnValueType));
                if (columnValues != null)
                {
                    //todo unit test the scenario where we are supplying data here
                    row.RowValues[columnDefinition] = columnValues[row];
                }
            }
            ColumnAdded?.Invoke(this, new ColumnAddedEventArgs(columnDefinition));
        }

        public void RemoveColumnDefinition(ColumnDefinition columnDefinition)
        {
            if (!RootRow.RowValues.ContainsKey(columnDefinition))
            {
                throw new InvalidOperationException("Column not added to outline.");
            }

            var removedValues = new Dictionary<Row, ColumnValue>();

            foreach (var row in RootRow.EachRowUnder().Concat(new[] { RootRow }))
            {
                removedValues.Add(row, (ColumnValue)row[columnDefinition].Clone());
                row.RowValues.Remove(columnDefinition);
            }
            ColumnRemoved?.Invoke(this, new ColumnRemovedEventArgs(columnDefinition, removedValues));
        }

        public void ChangeColumnTitle(ColumnDefinition columnDefinition, string newTitle)
        {
            var previousTitle = columnDefinition.Title;
            columnDefinition.Title = newTitle;
            ColumnTitleChanged?.Invoke(this, new ColumnTitleChangedEventArgs(columnDefinition, previousTitle, newTitle));
        }

        public IEnumerable<(Row row, int indentLevel)> RowsWithIndent()
        {
            var rowsWithIndents = new Queue<(Row row, int indentLevel)>();
            rowsWithIndents.Enqueue((RootRow, -1));

            Row lastParent = null;
            while(rowsWithIndents.Any())
            {
                var lastParentWithIndent = rowsWithIndents.Dequeue();
                if (lastParentWithIndent.row != RootRow)
                {
                    yield return lastParentWithIndent;
                }

                foreach(var child in lastParentWithIndent.row.Children)
                {
                    rowsWithIndents.Enqueue((child, lastParentWithIndent.indentLevel+1));
                }
            }
        }

        public void RemoveRowAndChildren()
        {

        }

        public void RemoveRowKeepChildren()
        {

        }

        internal void FireRowAdded(RowAddedEventArgs args)
        {
            RowAdded?.Invoke(this, args);
        }

        internal void FireRowDataChanged(RowDataChangedEventArgs args)
        {
            RowDataChanged?.Invoke(this, args);
        }
    }
}
