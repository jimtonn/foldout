using System.Collections.Generic;
using Foldout.Model.Columns;

namespace Foldout.Model.Commands
{
    public class RemoveColumnCommand : ICommand
    {
        private readonly Column _column;
        private Dictionary<Row, object> _removedValues;

        public RemoveColumnCommand(Column column)
        {
            _column = column;
        }

        public void Do(Outline outline)
        {
            _removedValues = new Dictionary<Row, object>();

            foreach (var row in outline.EachRow)
            {
                _removedValues.Add(row, row[_column]);
            }

            outline.RemoveColumn(_column);
        }

        public ICommand GetReverseCommand()
        {
            return new AddColumnAndRestoreDataCommand(_column, _removedValues);
        }
    }
}
