using Foldout.Model.Columns;

namespace Tests.Foldout.Model
{
    class TestColumnValue : ColumnValue
    {
        public override object Clone()
        {
            return new TestColumnValue() { Data = this.Data };
        }
    }

    class TestColumn : Column<TestColumnValue>
    {
        public TestColumn(string title) : base(title) { }
    }
}
