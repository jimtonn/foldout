using System.Collections.Generic;
using Foldout.Model.Columns;

namespace Foldout.Model.Commands
{
    public class AddColumnAndRestoreDataCommand : ICommand
    {
        private readonly ColumnDefinition _columnDefinition;
        private readonly IDictionary<Row, ColumnValue> _columnValues;

        public AddColumnAndRestoreDataCommand(ColumnDefinition columnDefinition, IDictionary<Row, ColumnValue> columnValues)
        {
            _columnDefinition = columnDefinition;
            _columnValues = columnValues;
        }

        public void Do(Outline outline)
        {
            outline.AddColumnDefinition(_columnDefinition, _columnValues);
        }

        public ICommand GetReverseCommand()
        {
            return new RemoveColumnCommand(_columnDefinition);
        }
    }
}
