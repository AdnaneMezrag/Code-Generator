using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class clsForeignAttribute : clsAttribute
    {
        public string ParentTableName;

        public void FillAttribute(string Name, string DataType, bool IsNullable, string ParentTableName)
        {
            base.FillAttribute(Name, DataType, IsNullable);
            this.ParentTableName = ParentTableName;
        }
    }
}
