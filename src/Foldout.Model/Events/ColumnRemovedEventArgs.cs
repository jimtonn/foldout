using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foldout.Model.Columns;

namespace Foldout.Model.Events
{
    public class ColumnRemovedEventArgs
    {
        public ColumnDefinition ColumnDefinition { get; }

        /// <summary>
        /// Contains a clone of the value that each row had for the given column prior to its removal.
        /// Note that this can contain large amounts of data (which may be necessary to store for undo/redo
        /// operations), and references to these eventargs should be removed when possible.
        /// </summary>
        public IReadOnlyDictionary<Row, ColumnValue> RowValues { get; }

        public ColumnRemovedEventArgs(ColumnDefinition columnDefinition, IDictionary<Row, ColumnValue> rowValues)
        {
            ColumnDefinition = columnDefinition;
            RowValues = new ReadOnlyDictionary<Row, ColumnValue>(rowValues);
        }
    }
}