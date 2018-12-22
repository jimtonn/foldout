using Foldout.Model.Columns;

namespace Foldout.Model.Commands
{
    public class AddColumnCommand : ICommand
    {
        private readonly Column _column;

        public AddColumnCommand(Column column)
        {
            _column = column;
        }

        public void Do(Outline outline)
        {
            outline.AddColumn(_column);
        }

        public ICommand GetReverseCommand()
        {
            return new RemoveColumnCommand(_column);
        }
    }
}
