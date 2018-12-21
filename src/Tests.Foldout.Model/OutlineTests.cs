using System;
using System.Linq;
using FluentAssertions;
using Foldout.Model;
using Foldout.Model.Events;
using Xunit;

namespace Tests.Foldout.Model
{
    public class OutlineTests
    {
        [Fact]
        public void AddingColumnShouldFireEvent()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");
            using (var scope = outline.Monitor())
            {
                outline.AddColumnDefinition(colDef);
                scope.Should().Raise(nameof(Outline.ColumnAdded))
                    .WithSender(outline)
                    .WithArgs<ColumnAddedEventArgs>(args => args.ColumnDefinition == colDef);
            }
        }

        [Fact]
        public void AddingColumnShouldAddColumnForEachRow()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");

            var n1 = outline.RootRow.InsertEmptyRow(0);
            var n1_1 = n1.InsertEmptyRow(0);

            n1.RowValues.Keys.Should().NotContain(colDef);
            n1_1.RowValues.Keys.Should().NotContain(colDef);
            outline.AddColumnDefinition(colDef);
            n1.RowValues.Keys.Should().Contain(colDef);
            n1_1.RowValues.Keys.Should().Contain(colDef);
        }

        [Fact]
        public void AddingColumnShouldThrowExceptionIfColumnAlreadyAssociated()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");
            outline.AddColumnDefinition(colDef);

            outline.Invoking(o => o.AddColumnDefinition(colDef)).Should().Throw<InvalidOperationException>().WithMessage("Column already added to outline.");
        }

        [Fact]
        public void RemovingColumnShouldFireEvent()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");
            outline.AddColumnDefinition(colDef);

            using (var scope = outline.Monitor())
            {
                outline.RemoveColumnDefinition(colDef);
                scope.Should().Raise(nameof(Outline.ColumnRemoved))
                    .WithSender(outline)
                    .WithArgs<ColumnRemovedEventArgs>(args => args.ColumnDefinition == colDef);
            }
        }

        [Fact]
        public void RemovingColumnShouldRemoveColumnFromEachRow()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");
            var n1 = outline.RootRow.InsertEmptyRow(0);
            outline.AddColumnDefinition(colDef);
            var n1_1 = n1.InsertEmptyRow(0);

            n1.RowValues.Keys.Should().Contain(colDef);
            n1_1.RowValues.Keys.Should().Contain(colDef);
            outline.RemoveColumnDefinition(colDef);
            n1.RowValues.Keys.Should().NotContain(colDef);
            n1_1.RowValues.Keys.Should().NotContain(colDef);
        }

        [Fact]
        public void RemovingColumnShouldThrowExceptionIfColumnNotAssociated()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");

            outline.Invoking(o => o.RemoveColumnDefinition(colDef)).Should().Throw<InvalidOperationException>().WithMessage("Column not added to outline.");
        }

        [Fact]
        public void EachRowShouldReturnAllRowsUnderRow()
        {
            var outline = new Outline();
            var n1 = outline.RootRow.InsertEmptyRow(0);
            var n1_1 = n1.InsertEmptyRow(0);
            n1_1.InsertEmptyRow(0);
            var n2 = outline.RootRow.InsertEmptyRow(0);
            outline.RootRow.EachRowUnder().Count().Should().Be(4);
            n1.EachRowUnder().Count().Should().Be(2);
            n1_1.EachRowUnder().Count().Should().Be(1);
            n2.EachRowUnder().Count().Should().Be(0);
        }

        [Fact]
        public void NewOutlineShouldHaveNoRows()
        {
            var outline = new Outline();
            outline.RootRow.EachRowUnder().Should().BeEmpty();
        }

        [Fact]
        public void EachRowShouldHandleLargeDepths()
        {
            var outline = new Outline();
            var lastParentRow = outline.RootRow;
            const int rowsToAdd = 1000;
            for (int i=0; i < rowsToAdd; i++)
            {
                lastParentRow = lastParentRow.InsertEmptyRow(0);
            }
            outline.RootRow.EachRowUnder().Count().Should().Be(rowsToAdd);
        }

        [Fact]
        public void InsertNewEmptyRowShouldCreateRowWithExistingColumnDefinitions()
        {
            var outline = new Outline();
            var colDefA = new TestColumnDefinition("A");
            var colDefB = new TestColumnDefinition("B");
            var colDefC = new TestColumnDefinition("C");

            outline.AddColumnDefinition(colDefA);
            outline.AddColumnDefinition(colDefB);
            outline.AddColumnDefinition(colDefC);

            var n1 = outline.RootRow.InsertEmptyRow(0);
            var n1_1 = n1.InsertEmptyRow(0);

            n1.RowValues.Keys.Should().Contain(colDefA);
            n1.RowValues.Keys.Should().Contain(colDefB);
            n1.RowValues.Keys.Should().Contain(colDefC);
            n1_1.RowValues.Keys.Should().Contain(colDefA);
            n1_1.RowValues.Keys.Should().Contain(colDefB);
            n1_1.RowValues.Keys.Should().Contain(colDefC);
        }

        [Fact]
        public void InsertNewEmptyRowShouldFireRowAdded()
        {
            var outline = new Outline();

            //insert at 0
            using (var scope = outline.Monitor())
            {
                var newRow = outline.RootRow.InsertEmptyRow(0);
                scope.Should().Raise(nameof(Outline.RowAdded))
                    .WithSender(outline)
                    .WithArgs<RowAddedEventArgs>(args => args.Row == newRow && args.Position == 0);
            }

            //insert at 1
            using (var scope = outline.Monitor())
            {
                var newRow = outline.RootRow.InsertEmptyRow(1);
                scope.Should().Raise(nameof(Outline.RowAdded))
                    .WithSender(outline)
                    .WithArgs<RowAddedEventArgs>(args => args.Row == newRow && args.Position == 1);
            }

            //insert on a row that's not root
            var n1 = outline.RootRow.InsertEmptyRow(0);
            using (var scope = outline.Monitor())
            {
                var newRow = n1.InsertEmptyRow(0);
                scope.Should().Raise(nameof(Outline.RowAdded))
                    .WithSender(outline)
                    .WithArgs<RowAddedEventArgs>(args => args.Row == newRow && args.Position == 0);
            }
        }

        [Fact]
        public void InsertNewEmptyRowShouldSetParentRow()
        {
            var outline = new Outline();
            var n1 = outline.RootRow.InsertEmptyRow(0);
            n1.ParentRow.Should().Be(outline.RootRow);

            var n1_1 = n1.InsertEmptyRow(0);
            n1_1.ParentRow.Should().Be(n1);
        }

        [Fact]
        public void RootRowShouldHaveNullParent()
        {
            var outline = new Outline();
            outline.RootRow.ParentRow.Should().BeNull();
        }

        [Fact]
        public void RowValueDataChangeShouldFireChangedEvent()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");
            outline.AddColumnDefinition(colDef);
            var newRow = outline.RootRow.InsertEmptyRow(0);

            using (var scope = outline.Monitor())
            {
                newRow.SetValue(colDef, "something new");

                scope.Should().Raise(nameof(Outline.RowDataChanged))
                    .WithSender(outline)
                    .WithArgs<RowDataChangedEventArgs<string>>(args => 
                        args.Row == newRow
                        && args.PreviousData == null
                        && args.NewData == "something new");
            }
        }

        [Fact]
        public void ColumnDefinitionSetTitleShouldFireChangedEventIfAddedToOutline()
        {
            var outline = new Outline();
            var colDef = new TestColumnDefinition("Column");
            outline.AddColumnDefinition(colDef);

            using (var scope = outline.Monitor())
            {
                outline.ChangeColumnTitle(colDef, "newtitle");

                scope.Should().Raise(nameof(Outline.ColumnTitleChanged))
                    .WithSender(outline)
                    .WithArgs<ColumnTitleChangedEventArgs>(args => 
                            args.ColumnDefinition == colDef
                            && args.PreviousTitle == "Column"
                            && args.NewTitle == "newtitle");
            }
        }
    }
}
