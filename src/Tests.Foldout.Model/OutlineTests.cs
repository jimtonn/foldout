using System;
using System.Collections.Generic;
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
        public void AddingColumnShouldFillInProvidedData()
        {
            var outline = new Outline();
            var colA = new TestColumn("Column A");
            outline.AddColumn(colA);
            var colB = new TestColumn("Column B");

            var n1 = outline.InsertRow(outline.RootRow, 0);
            var n1_1 = outline.InsertRow(n1, 0);

            outline.AddColumn(colB, new Dictionary<Row, object>
            {
                { n1_1, "abc" }
            });

            n1_1.RowValues[colB].Should().Be("abc");
        }

        [Fact]
        public void AddingColumnShouldNotFireEventsForFilledInRowData()
        {
            //when we add a column and supply data, usually it's because we are undoing the deletion of
            //a column. the re-addition will fire the column added event, and anyone who is listening can
            //repaint the entire column if they are presenting a UI. there doesn't seem to be a need
            //to indicate which row+columns specifically received non-default data.

            var outline = new Outline();
            var colA = new TestColumn("Column A");
            outline.AddColumn(colA);
            var colB = new TestColumn("Column B");

            var n1 = outline.InsertRow(outline.RootRow, 0);
            var n1_1 = outline.InsertRow(n1, 0);

            using (var scope = outline.Monitor())
            {
                outline.AddColumn(colB, new Dictionary<Row, object>
                {
                    { n1_1, "abc" }
                });

                scope.Should().NotRaise(nameof(Outline.RowAdded));
            }
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

        [Fact]
        public void NewOutlineShouldHaveNoRows()
        {
            var outline = new Outline();
            outline.RootRow.Children.Should().BeEmpty();
        }

        [Fact]
        public void InsertingRowShouldCreateRowWithExistingColumns()
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
        public void InsertingRowShouldThrowExceptionForNullParent()
        {
            var outline = new Outline();
            outline.Invoking(o => o.InsertRow(null, 0)).Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-100)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(100)]
        public void InsertingRowShouldThrowExceptionForOutOfRangePosition(int position)
        {
            var outline = new Outline();
            outline.InsertRow(outline.RootRow, 0);
            outline.InsertRow(outline.RootRow, 0);
            outline.InsertRow(outline.RootRow, 0);
            outline.Invoking(o => o.InsertRow(outline.RootRow, position)).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void InsertingRowShouldFireRowAdded()
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
        public void InsertingRowShouldSetParentRow()
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
        public void ChangingRowValueDataShouldFireChangedEvent()
        {
            var outline = new Outline();
            var col = new TestColumn("Column");
            outline.AddColumn(col);
            var newRow = outline.InsertRow(outline.RootRow, 0);

            using (var scope = outline.Monitor())
            {
                outline.ChangeRowData(newRow, col, "something new");

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

        [Fact]
        public void RemovingRowShouldThrowExceptionIfRowHasChildren()
        {
            var outline = new Outline();
            var row = outline.InsertRow(outline.RootRow, 0);
            outline.InsertRow(row, 0);
            outline.Invoking(o => o.RemoveRow(row)).Should().Throw<InvalidOperationException>()
                .WithMessage("Row cannot be removed because it has children.");
        }

        [Fact]
        public void RemovingRowShouldThrowExceptionForRootRow()
        {
            var outline = new Outline();
            outline.Invoking(o => o.RemoveRow(outline.RootRow)).Should().Throw<InvalidOperationException>()
                .WithMessage("Root row cannot be removed.");
        }

        [Fact]
        public void RemovingRowShouldThrowExceptionIfRowNotFoundInOutline()
        {
            //this situation could happen if you try to remove a row twice

            var outline = new Outline();
            var row = outline.InsertRow(outline.RootRow, 0);
            outline.RemoveRow(row);
            outline.Invoking(o => o.RemoveRow(row)).Should().Throw<InvalidOperationException>()
                .WithMessage("Row not found. It may have already been removed.");
        }

        [Fact]
        public void RemovingRowShouldRemoveRow()
        {
            var outline = new Outline();
            var row = outline.InsertRow(outline.RootRow, 0);
            outline.RootRow.Children.Count.Should().Be(1);
            outline.RemoveRow(row);
            outline.RootRow.Children.Count.Should().Be(0);
        }

        [Fact]
        public void RemovingRowShouldFireEvent()
        {
            var outline = new Outline();
            var row = outline.InsertRow(outline.RootRow, 0);

            using (var scope = outline.Monitor())
            {
                outline.RemoveRow(row);
                scope.Should().Raise(nameof(Outline.RowRemoved))
                    .WithSender(outline)
                    .WithArgs<RowRemovedEventArgs>(args =>
                        args.Row == row);
            }
        }

        [Fact]
        public void ReparentingRowShouldReparentRow()
        {
            var outline = new Outline();
            var n1 = outline.InsertRow(outline.RootRow, 0);
            outline.InsertRow(n1, 0);
            outline.InsertRow(n1, 1);

            var n2 = outline.InsertRow(outline.RootRow, 1);

            n1.Children.Should().NotContain(n2);
            n2.ParentRow.Should().NotBe(n1);
            outline.ReparentRow(n2, n1, 1);

            n1.Children[1].Should().Be(n2);
            n2.ParentRow.Should().Be(n1);
        }

        [Fact]
        public void ReparentingRowShouldThrowExceptionForRootRow()
        {
            var outline = new Outline();
            var n1 = outline.InsertRow(outline.RootRow, 0);

            outline.Invoking(o => o.ReparentRow(outline.RootRow, n1, 0)).Should().Throw<InvalidOperationException>().WithMessage("Root row cannot be reparented.");
        }

        [Fact]
        public void ReparentingRowShouldThrowExceptionForNullRow()
        {
            var outline = new Outline();
            outline.Invoking(o => outline.ReparentRow(null, outline.RootRow, 0)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReparentingRowShouldThrowExceptionForNullParent()
        {
            var outline = new Outline();
            var n1 = outline.InsertRow(outline.RootRow, 0);
            outline.Invoking(o => outline.ReparentRow(n1, null, 0)).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReparentingRowShouldFireEvent()
        {
            var outline = new Outline();
            var n1 = outline.InsertRow(outline.RootRow, 0);
            var n2 = outline.InsertRow(outline.RootRow, 1);

            using (var scope = outline.Monitor())
            {
                outline.ReparentRow(n2, n1, 0);
                scope.Should().Raise(nameof(Outline.RowReparented))
                    .WithSender(outline)
                    .WithArgs<RowReparentedEventArgs>(args =>
                        args.ReparentedRow == n2
                        && args.PreviousParent == outline.RootRow
                        && args.NewParent == n1
                        );
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-2)]
        [InlineData(-100)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(100)]
        public void ReparentingRowShouldThrowExceptionForOutOfRangePosition(int position)
        {
            var outline = new Outline();
            var n1 = outline.InsertRow(outline.RootRow, 0);
            outline.InsertRow(n1, 0);
            outline.InsertRow(n1, 0);
            outline.InsertRow(n1, 0);
            var n2 = outline.InsertRow(outline.RootRow, 1);
            outline.Invoking(o => outline.ReparentRow(n2, n1, position)).Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
