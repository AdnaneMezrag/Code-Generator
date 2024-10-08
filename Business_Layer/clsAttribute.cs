using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess_Layer;
using Microsoft.SqlServer.Server;

namespace Business_Layer
{
    public class clsAttribute
    {
        public string Name { get; set; }
        private string _CSharpDataType;
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public string CSharpDataType { get { return _CSharpDataType; }

            set { 
                    _CSharpDataType = value;
                    DefaultValuee = GetDefaultValue();
                }
        
        }
        public string DefaultValuee { get; set; }

        private string GetDefaultValue()
        {
            if (CSharpDataType == "int" || CSharpDataType == "float" ||
    CSharpDataType ==
    "double" || CSharpDataType == "decimal" ||
    CSharpDataType == "short"
    )
            {
                return "-1";
            }
            else if (CSharpDataType == "string")
            {
                return "\"\"";
            }
            else if (CSharpDataType == "DateTime")
            {
                return "DateTime.MaxValue";
            }
            else if (CSharpDataType == "byte")
            {
                return "0";
            }
            return "";
        }

        public void FillAttribute(string Name , string DataType,bool IsNullable)
        {
            this.Name = Name;
            this.DataType = DataType;
            this.IsNullable = IsNullable;
            this.CSharpDataType = clsDataTypesMapping.Search(DataType);
        }

    }
}
