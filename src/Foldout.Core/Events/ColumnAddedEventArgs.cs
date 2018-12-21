using Foldout.Core.Columns;

namespace Foldout.Core.Events
{
    public class ColumnAddedEventArgs
    {
        public ColumnDefinition ColumnDefinition { get; }

        public ColumnAddedEventArgs(ColumnDefinition columnDefinition)
        {
            ColumnDefinition = columnDefinition;
        }
    }
}
