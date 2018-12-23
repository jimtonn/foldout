namespace Foldout.Model.Events
{
    public class RowReparentedEventArgs
    {
        public Row ReparentedRow { get; }
        public Row PreviousParent { get; }
        public Row NewParent { get; }

        public RowReparentedEventArgs(Row reparentedRow, Row previousParent, Row newParent)
        {
            ReparentedRow = reparentedRow;
            NewParent = newParent;
            PreviousParent = previousParent;
        }
    }
}