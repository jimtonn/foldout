using Foldout.Model.Columns;

namespace Foldout.Model.Events
{
    public class ColumnTitleChangedEventArgs
    {
        public Column Column { get; }
        public string PreviousTitle { get; }
        public string NewTitle { get; }

        public ColumnTitleChangedEventArgs(Column column, string previousTitle, string newTitle)
        {
            Column = column;
            PreviousTitle = previousTitle;
            NewTitle = newTitle;
        }
    }
}