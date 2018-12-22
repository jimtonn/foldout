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
            var col = new TestColumn("Column");
            using (var scope = outline.Monitor())
            {
                outline.AddColumn(col);
                scope.Should().Raise(nameof(Outline.ColumnAdded))
                    .WithSender(outline)
                    .WithArgs<ColumnAddedEventArgs>(args => args.Column == col);
            }
        }

        [Fact]
        public void AddingColumnShouldAddColumnForEachRow()
        {
            var outline = new Outline();
            var col = new TestColumn("Column");

            var n1 = outline.InsertRow(outline.RootRow, 0);
            var n1_1 = outline.InsertRow(n1, 0);

            n1.RowValues.Keys.Should().NotContain(col);
            n1_1.RowValues.Keys.Should().NotContain(col);
            outline.AddColumn(col);
            n1.RowValues.Keys.Should().Contain(col);
            n1_1.RowValues.Keys.Should().Contain(col);
        }

        [Fact]
        public void AddingColumnShouldThrowExceptionIfColumnAlreadyAssociated()
        {
            var outline = new Outline();
            var col = new TestColumn("Column");
            outline.AddColumn(col);

            outline.Invoking(o => o.AddColumn(col)).Should().Throw<InvalidOperationException>().WithMessage("Column already added to outline.");
        }

        [Fact]
        public void RemovingColumnShouldFireEvent()
        {
            var outline = new Outline();
            var col = new TestColumn("Column");
            outline.AddColumn(col);

            using (var scope = outline.Monitor())
            {
                outline.RemoveColumn(col);
                scope.Should().Raise(nameof(Outline.ColumnRemoved))
                    .WithSender(outline)
                    .WithArgs<ColumnRemovedEventArgs>(args => args.Column == col);
            }
        }

        [Fact]
        public void RemovingColumnShouldRemoveColumnFromEachRow()
        {
            var outline = new Outline();
            var col = new TestColumn("Column");
            var n1 = outline.InsertRow(outline.RootRow, 0);
            outline.AddColumn(col);
            var n1_1 = outline.InsertRow(n1, 0);

            n1.RowValues.Keys.Should().Contain(col);
            n1_1.RowValues.Keys.Should().Contain(col);
            outline.RemoveColumn(col);
            n1.RowValues.Keys.Should().NotContain(col);
            n1_1.RowValues.Keys.Should().NotContain(col);
        }

        [Fact]
        public void RemovingColumnShouldThrowExceptionIfColumnNotAssociated()
        {
            var outline = new Outline();
            var col = new TestColumn("Column");

            outline.Invoking(o => o.RemoveColumn(col)).Should().Throw<InvalidOperationException>().WithMessage("Column not added to outline.");
        }

        /*
        [Fact]
        public void EachRowShouldReturnAllRowsUnderRow()
        {
            var outline = new Outline();
            var n1 = outline.RootRow.InsertRow(0);
            var n1_1 = n1.InsertRow(0);
            n1_1.InsertRow(0);
            var n2 = outline.RootRow.InsertRow(0);
            outline.RootRow.EachRowUnder().Count().Should().Be(4);
            n1.EachRowUnder().Count().Should().Be(2);
            n1_1.EachRowUnder().Count().Should().Be(1);
            n2.EachRowUnder().Count().Should().Be(0);
        }*/

        [Fact]
        public void NewOutlineShouldHaveNoRows()
        {
            var outline = new Outline();
            outline.RootRow.Children.Should().BeEmpty();
        }

        /*
        [Fact]
        public void EachRowShouldHandleLargeDepths()
        {
            var outline = new Outline();
            var lastParentRow = outline.RootRow;
            const int rowsToAdd = 1000;
            for (int i=0; i < rowsToAdd; i++)
            {
                lastParentRow = lastParentRow.InsertRow(0);
            }
            outline.RootRow.EachRowUnder().Count().Should().Be(rowsToAdd);
        }*/

        [Fact]
        public void InsertRowShouldCreateRowWithExistingColumns()
        {
            var outline = new Outline();
            var colA = new TestColumn("A");
            var colB = new TestColumn("B");
            var colC = new TestColumn("C");

            outline.AddColumn(colA);
            outline.AddColumn(colB);
            outline.AddColumn(colC);

            var n1 = outline.InsertRow(outline.RootRow, 0);
            var n1_1 = outline.InsertRow(n1, 0);

            n1.RowValues.Keys.Should().Contain(colA);
            n1.RowValues.Keys.Should().Contain(colB);
            n1.RowValues.Keys.Should().Contain(colC);
            n1_1.RowValues.Keys.Should().Contain(colA);
            n1_1.RowValues.Keys.Should().Contain(colB);
            n1_1.RowValues.Keys.Should().Contain(colC);
        }

        [Fact]
        public void InsertRowShouldThrowExceptionForNullParent()
        {
            var outline = new Outline();
            outline.Invoking(o => o.InsertRow(null, 0)).Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-100)]
        public void InsertRowShouldThrowExceptionForPositionLessThan0(int position)
        {
            var outline = new Outline();
            outline.Invoking(o => o.InsertRow(outline.RootRow, position)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(100)]
        public void InsertRowShouldThrowExceptionForPositionGreaterThanNumberOfChildren(int position)
        {
            var outline = new Outline();
            outline.InsertRow(outline.RootRow, 0);
            outline.InsertRow(outline.RootRow, 0);
            outline.InsertRow(outline.RootRow, 0);
            outline.Invoking(o => o.InsertRow(outline.RootRow, position)).Should().Throw<ArgumentOutOfRangeException>();
        }


        [Fact]
        public void InsertRowShouldFireRowAdded()
        {
            var outline = new Outline();

            //insert at 0
            using (var scope = outline.Monitor())
            {
                var newRow = outline.InsertRow(outline.RootRow, 0);
                scope.Should().Raise(nameof(Outline.RowAdded))
                    .WithSender(outline)
                    .WithArgs<RowAddedEventArgs>(args => args.Row == newRow && args.Position == 0);
            }

            //insert at 1
            using (var scope = outline.Monitor())
            {
                var newRow = outline.InsertRow(outline.RootRow, 1);
                scope.Should().Raise(nameof(Outline.RowAdded))
                    .WithSender(outline)
                    .WithArgs<RowAddedEventArgs>(args => args.Row == newRow && args.Position == 1);
            }

            //insert on a row that's not root
            var n1 = outline.InsertRow(outline.RootRow, 0);
            using (var scope = outline.Monitor())
            {
                var newRow = outline.InsertRow(n1, 0);
                scope.Should().Raise(nameof(Outline.RowAdded))
                    .WithSender(outline)
                    .WithArgs<RowAddedEventArgs>(args => args.Row == newRow && args.Position == 0);
            }
        }

        [Fact]
        public void InsertRowShouldSetParentRow()
        {
            var outline = new Outline();
            var n1 = outline.InsertRow(outline.RootRow, 0);
            n1.ParentRow.Should().Be(outline.RootRow);

            var n1_1 = outline.InsertRow(n1, 0);
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
            var col = new TestColumn("Column");
            outline.AddColumn(col);
            var newRow = outline.InsertRow(outline.RootRow, 0);

            using (var scope = outline.Monitor())
            {
                outline.ChangeRowData(newRow, col, "something new" );

                scope.Should().Raise(nameof(Outline.RowDataChanged))
                    .WithSender(outline)
                    .WithArgs<RowDataChangedEventArgs<string>>(args => 
                        args.Row == newRow
                        && args.PreviousData == null
                        && args.NewData == "something new");
            }
        }

        [Fact]
        public void ColumnSetTitleShouldFireChangedEventIfAddedToOutline()
        {
            var outline = new Outline();
            var col = new TestColumn("Column");
            outline.AddColumn(col);

            using (var scope = outline.Monitor())
            {
                outline.ChangeColumnTitle(col, "newtitle");

                scope.Should().Raise(nameof(Outline.ColumnTitleChanged))
                    .WithSender(outline)
                    .WithArgs<ColumnTitleChangedEventArgs>(args => 
                            args.Column == col
                            && args.PreviousTitle == "Column"
                            && args.NewTitle == "newtitle");
            }
        }
    }
}
