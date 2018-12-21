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
            var text = new TextColumnDefinition("Content");
            var complete = new TextColumnDefinition("Complete");
            outline.AddColumnDefinition(text);
            outline.AddColumnDefinition(complete);
            var n1 = outline.RootRow.InsertEmptyRow(0);
            var n1_1 = n1.InsertEmptyRow(0);
            var n1_2 = n1.InsertEmptyRow(1);
            var n1_2_1 = n1.InsertEmptyRow(0);

            n1[text].Data = "abc";
            n1[complete].Data = false;
            n1_1[text].Data = "def";
            n1_1[complete].Data = true;

            foreach (var row in outline.RowsWithIndent())
            {
                System.Console.WriteLine($"{new String('\t', row.indentLevel)}{row.row[text].Data}");
            }
        }

    }
}
