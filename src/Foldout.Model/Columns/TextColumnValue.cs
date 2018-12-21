namespace Foldout.Model.Columns
{
    public class TextColumnValue : ColumnValue
    {
        public override object Clone()
        {
            return new TextColumnValue { Data = this.Data };
        }
    }
}
