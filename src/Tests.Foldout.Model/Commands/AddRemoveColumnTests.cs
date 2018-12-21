using FluentAssertions;
using Foldout.Model;
using Foldout.Model.Commands;
using Foldout.Model.Events;
using Xunit;

namespace Tests.Foldout.Model.Commands
{
    public class AddRemoveColumnTests
    {
        [Fact]
        public void AddColumnCommandShouldAddColumnToOutline()
        {
            var outline = new Outline();
            var co = new CommandOutline(outline);
            var colDef = new TestColumnDefinition("column");

            var columnCount = outline.RootRow.RowValues.Count;

            using (var scope = outline.Monitor())
            {
                co.RunCommand(new AddColumnCommand(colDef));

                scope.Should().Raise(nameof(Outline.ColumnAdded))
                    .WithSender(outline)
                    .WithArgs<ColumnAddedEventArgs>(args => args.ColumnDefinition == colDef);
            }

            outline.RootRow.RowValues.Count.Should().Be(columnCount + 1);
        }

        [Fact]
        public void RemoveColumnCommandShouldRemoveColumnFromOutline()
        {
            var outline = new Outline();
            var co = new CommandOutline(outline);
            var colDef = new TestColumnDefinition("column");
            outline.AddColumnDefinition(colDef);

            var columnCount = outline.RootRow.RowValues.Count;

            using (var scope = outline.Monitor())
            {
                co.RunCommand(new RemoveColumnCommand(colDef));

                scope.Should().Raise(nameof(Outline.ColumnRemoved))
                    .WithSender(outline)
                    .WithArgs<ColumnRemovedEventArgs>(args => args.ColumnDefinition == colDef);
            }

            outline.RootRow.RowValues.Count.Should().Be(columnCount - 1);
        }
    }
}
