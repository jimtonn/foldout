namespace Foldout.Model.Events
{
    public class RowRemovedEventArgs
    {
        public Row Row { get; }

        public RowRemovedEventArgs(Row row)
        {
            Row = row;
        }
    }
}