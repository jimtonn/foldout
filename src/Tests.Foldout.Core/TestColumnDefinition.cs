using System;
using System.Collections.Generic;
using System.Text;
using Foldout.Core;
using Foldout.Core.Columns;

namespace Tests.Foldout.Core
{
    class TestColumnValue : ColumnValue
    {

    }

    class TestColumnDefinition : ColumnDefinition<TestColumnValue>
    {
        public TestColumnDefinition(string title) : base(title) { }
    }
}
