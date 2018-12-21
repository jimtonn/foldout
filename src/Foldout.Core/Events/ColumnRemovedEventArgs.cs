using Foldout.Core.Columns;

namespace Foldout.Core.Events
{
    public class ColumnRemovedEventArgs
    {
        public ColumnDefinition ColumnDefinition { get; }

        public ColumnRemovedEventArgs(ColumnDefinition columnDefinition)
        {
            ColumnDefinition = columnDefinition;
        }
    }
}