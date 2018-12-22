using Foldout.Model.Columns;

namespace Foldout.Model.Events
{
    public class ColumnAddedEventArgs
    {
        public Column Column { get; }

        public ColumnAddedEventArgs(Column column)
        {
            Column = column;
        }
    }
}
