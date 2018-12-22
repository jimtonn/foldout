using System;
using System.Collections.Generic;
using Foldout.Model;
using Foldout.Model.Columns;

namespace Foldout.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var outline = new Outline();
            var text = new TextColumn("Content");
            var complete = new TextColumn("Complete");
            outline.AddColumn(text);
            outline.AddColumn(complete);
            var n1 = outline.InsertRow(outline.RootRow, 0, new Dictionary<Column, ColumnValue> { { text, new TextColumnValue("this is a row") } });
            var n1_1 = outline.InsertRow(n1, 0, new Dictionary<Column, ColumnValue> { { text, new TextColumnValue("this is a row too") } });
            var n1_2 = outline.InsertRow(n1, 1, new Dictionary<Column, ColumnValue> { { text, new TextColumnValue("something") } });
            var n1_2_1 = outline.InsertRow(n1_2, 0, new Dictionary<Column, ColumnValue> { { text, new TextColumnValue("this is a cool row") } });
            var n2 = outline.InsertRow(outline.RootRow, 1, new Dictionary<Column, ColumnValue> { { text, new TextColumnValue("this is the bottom row") } });

            foreach (var row in outline.RowsWithIndent())
            {
                System.Console.WriteLine($"{new String('\t', row.indentLevel)}{row.row[text].Data}");
            }
        }

    }
}
