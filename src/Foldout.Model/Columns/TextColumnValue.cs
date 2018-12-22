namespace Foldout.Model.Columns
{
    public class TextColumnValue : ColumnValue
    {
        public TextColumnValue()
        {

        }

        public TextColumnValue(string text)
        {
            this.Data = text;
        }

        public override object Clone()
        {
            return new TextColumnValue(this.Data.ToString());
        }
    }
}
