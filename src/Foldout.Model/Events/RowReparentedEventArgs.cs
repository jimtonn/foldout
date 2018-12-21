namespace Foldout.Model.Events
{
    public class RowReparentedEventArgs
    {
        public Row Row { get; }

        public RowReparentedEventArgs(Row row)
        {
            Row = row;
        }
    }
}