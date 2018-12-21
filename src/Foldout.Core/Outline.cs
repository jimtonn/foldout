using System;
using System.Linq;
using Foldout.Core.Columns;
using Foldout.Core.Events;

namespace Foldout.Core
{
    public class Outline
    {
        public event EventHandler<ColumnAddedEventArgs> ColumnAdded;
        public event EventHandler<ColumnRemovedEventArgs> ColumnRemoved;
        public event EventHandler<RowAddedEventArgs> RowAdded;
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

        public void AddColumnDefinition(ColumnDefinition columnDefinition)
        {
            if (RootRow.RowValues.ContainsKey(columnDefinition))
            {
                throw new InvalidOperationException("Column already added to outline.");
            }

            foreach(var row in RootRow.EachRowUnder().Concat(new[] { RootRow }))
            {
                row.RowValues.Add(columnDefinition, (ColumnValue)Activator.CreateInstance(columnDefinition.ColumnValueType));
            }
            ColumnAdded?.Invoke(this, new ColumnAddedEventArgs(columnDefinition));
        }

        public void RemoveColumnDefinition(ColumnDefinition columnDefinition)
        {
            if (!RootRow.RowValues.ContainsKey(columnDefinition))
            {
                throw new InvalidOperationException("Column not added to outline.");
            }

            foreach (var row in RootRow.EachRowUnder().Concat(new[] { RootRow }))
            {
                row.RowValues.Remove(columnDefinition);
            }
            ColumnRemoved?.Invoke(this, new ColumnRemovedEventArgs(columnDefinition));
        }

        public void ChangeColumnTitle(ColumnDefinition columnDefinition, string newTitle)
        {
            var previousTitle = columnDefinition.Title;
            columnDefinition.Title = newTitle;
            ColumnTitleChanged?.Invoke(this, new ColumnTitleChangedEventArgs(columnDefinition, previousTitle, newTitle));
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
