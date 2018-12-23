using System.Collections.Generic;
using System.Linq;

namespace Foldout.Model.Commands
{
    /// <summary>
    /// A wrapper for an Outline that keeps track of history, enables undo and redo operations,
    /// and provides more complex operations that are built from the basic outline operations.
    /// </summary>
    public class CommandOutline
    {
        public Outline Outline { get; private set; }

        private readonly Stack<ICommand> _executedCommands = new Stack<ICommand>();
        private readonly Stack<ICommand> _undoneCommands = new Stack<ICommand>();

        //todo broadcast events indicating what was undone/redone and when

        public CommandOutline(Outline outline)
        {
            Outline = outline;
        }

        public void RunCommand(ICommand command)
        {
            command.Do(Outline);
            _executedCommands.Push(command);
            _undoneCommands.Clear();
        }

        public void Undo()
        {
            if (_executedCommands.Any())
            {
                var lastCommand = _executedCommands.Pop();
                var reverseCommand = lastCommand.GetReverseCommand();
                reverseCommand.Do(Outline);
                _undoneCommands.Push(reverseCommand);
            }
        }

        public void Redo()
        {
            if (_undoneCommands.Any())
            {
                var lastCommand = _undoneCommands.Pop();
                var reverseCommand = lastCommand.GetReverseCommand();
                reverseCommand.Do(Outline);
                _executedCommands.Push(reverseCommand);
            }
        }
    }
}
