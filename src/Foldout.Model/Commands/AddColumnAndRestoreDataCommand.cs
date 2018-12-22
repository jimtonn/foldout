using System.Collections.Generic;
using Foldout.Model.Columns;

namespace Foldout.Model.Commands
{
    public class AddColumnAndRestoreDataCommand : ICommand
    {
        private readonly Column _column;
        private readonly IDictionary<Row, ColumnValue> _columnValues;

        public AddColumnAndRestoreDataCommand(Column column, IDictionary<Row, ColumnValue> columnValues)
        {
            _column = column;
            _columnValues = columnValues;
        }

        public void Do(Outline outline)
        {
            outline.AddColumn(_column, _columnValues);
        }

        public ICommand GetReverseCommand()
        {
            return new RemoveColumnCommand(_column);
        }
    }
}
