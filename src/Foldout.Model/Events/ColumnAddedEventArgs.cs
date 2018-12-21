using Foldout.Model.Columns;

namespace Foldout.Model.Events
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
