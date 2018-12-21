namespace Foldout.Core.Events
{
    public abstract class RowDataChangedEventArgs
    {

    }

    public class RowDataChangedEventArgs<T> : RowDataChangedEventArgs
    {
        public Row Row { get; private set; }
        public T PreviousData { get; private set; }
        public T NewData { get; private set; }

        public RowDataChangedEventArgs(Row row, T previousData, T newData)
        {
            Row = row;
            PreviousData = previousData;
            NewData = newData;
        }
    }
}