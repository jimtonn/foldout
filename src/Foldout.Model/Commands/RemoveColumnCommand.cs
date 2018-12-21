using System.Collections.Generic;
using Foldout.Model.Columns;

namespace Foldout.Model.Commands
{
    public class RemoveColumnCommand : ICommand
    {
        private readonly ColumnDefinition _columnDefinition;
        private Dictionary<Row, ColumnValue> _removedValues;

        public RemoveColumnCommand(ColumnDefinition columnDefinition)
        {
            _columnDefinition = columnDefinition;
        }

        public void Do(Outline outline)
        {
            _removedValues = new Dictionary<Row, ColumnValue>();

            foreach (var row in outline.RootRow.EachRowUnder())
            {
                _removedValues.Add(row, (ColumnValue)row[_columnDefinition].Clone());
            }

            outline.RemoveColumnDefinition(_columnDefinition);
        }

        public ICommand GetReverseCommand()
        {
            return new AddColumnAndRestoreDataCommand(_columnDefinition, _removedValues);
        }
    }
}
