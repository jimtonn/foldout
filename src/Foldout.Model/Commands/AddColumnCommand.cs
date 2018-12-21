using Foldout.Model.Columns;

namespace Foldout.Model.Commands
{
    public class AddColumnCommand : ICommand
    {
        private readonly ColumnDefinition _columnDefinition;

        public AddColumnCommand(ColumnDefinition columnDefinition)
        {
            _columnDefinition = columnDefinition;
        }

        public void Do(Outline outline)
        {
            outline.AddColumnDefinition(_columnDefinition);
        }

        public ICommand GetReverseCommand()
        {
            return new RemoveColumnCommand(_columnDefinition);
        }
    }
}
