using Foldout.Core.Columns;

namespace Foldout.Core.Events
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