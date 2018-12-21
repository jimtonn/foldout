using Foldout.Model;
using Foldout.Model.Commands;
using NSubstitute;
using Xunit;

namespace Tests.Foldout.Model.Commands
{
    //todo once the command pattern is built out a little more we should add more tests like regarding repeated undos and redos

    public class CommandOutlineTests
    {
        [Fact]
        public void ShouldExecuteTheCommand()
        {
            var command = Substitute.For<ICommand>();
            var outline = new Outline();
            var co = new CommandOutline(outline);
            co.RunCommand(command);
            command.Received(1).Do(outline);
        }

        [Fact]
        public void UndoShouldExecuteTheReverseCommand()
        {
            var command = Substitute.For<ICommand>();
            var reverseCommand = Substitute.For<ICommand>();
            command.GetReverseCommand().Returns(reverseCommand);

            var outline = new Outline();
            var co = new CommandOutline(outline);
            co.RunCommand(command);
            co.Undo();
            reverseCommand.Received(1).Do(outline);
        }

        [Fact]
        public void RedoShouldExecuteUndoneCommand()
        {
            var command = Substitute.For<ICommand>();
            var reverseCommand = Substitute.For<ICommand>();
            var reverseReverseCommand = Substitute.For<ICommand>();
            command.GetReverseCommand().Returns(reverseCommand);
            reverseCommand.GetReverseCommand().Returns(reverseReverseCommand);

            var outline = new Outline();
            var co = new CommandOutline(outline);
            co.RunCommand(command);
            co.Undo();
            co.Redo();
            reverseReverseCommand.Received(1).Do(outline);
        }

        [Fact]
        public void NewCommandShouldClearRedos()
        {
            var command = Substitute.For<ICommand>();
            var reverseCommand = Substitute.For<ICommand>();
            var reverseReverseCommand = Substitute.For<ICommand>();
            command.GetReverseCommand().Returns(reverseCommand);
            reverseCommand.GetReverseCommand().Returns(reverseReverseCommand);

            var outline = new Outline();
            var co = new CommandOutline(outline);
            co.RunCommand(command);
            co.Undo();
            co.RunCommand(Substitute.For<ICommand>());
            co.Redo();
            reverseReverseCommand.Received(0).Do(outline);
        }

    }
}
