using System;

namespace Foldout.Model.Columns
{
    //todo can/should this be made generic?
    public abstract class ColumnValue : ICloneable
    {
        public object Data { get; set; }

        public abstract object Clone();
    }
}
