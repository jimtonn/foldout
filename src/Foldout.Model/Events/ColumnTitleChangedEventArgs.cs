using Foldout.Model.Columns;

namespace Foldout.Model.Events
{
    public class ColumnTitleChangedEventArgs
    {
        public ColumnDefinition ColumnDefinition { get; }
        public string PreviousTitle { get; }
        public string NewTitle { get; }

        public ColumnTitleChangedEventArgs(ColumnDefinition columnDefinition, string previousTitle, string newTitle)
        {
            ColumnDefinition = columnDefinition;
            PreviousTitle = previousTitle;
            NewTitle = newTitle;
        }
    }
}