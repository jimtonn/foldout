namespace Foldout.Core.Events
{
    public class RowAddedEventArgs
    {
        /// <summary>
        /// The row that was inserted.
        /// </summary>
        public Row Row { get; private set; }

        /// <summary>
        /// Position under the parent where the row was inserted.
        /// </summary>
        public int Position { get; private set; }

        public RowAddedEventArgs(Row row, int position)
        {
            Row = row;
            Position = position;
        }
    }
}