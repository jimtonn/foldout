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

    class TestColumnDefinition : ColumnDefinition<TestColumnValue>
    {
        public TestColumnDefinition(string title) : base(title) { }
    }
}
