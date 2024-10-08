using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public static class clsDataTypesMapping
    {
        public static Dictionary<string, string> sqlToCSharpTypes = new Dictionary<string, string>
        {
            { "bigint", "long" },
            { "binary", "byte[]" },
            { "bit", "bool" },
            { "char", "string" },
            { "date", "DateTime" },
            { "datetime", "DateTime" },
            { "datetime2", "DateTime" },
            { "datetimeoffset", "DateTimeOffset" },
            { "decimal", "decimal" },
            { "float", "double" },
            { "image", "byte[]" },
            { "int", "int" },
            { "money", "decimal" },
            { "nchar", "string" },
            { "ntext", "string" },
            { "numeric", "decimal" },
            { "nvarchar", "string" },
            { "real", "float" },
            { "smalldatetime", "DateTime" },
            { "smallint", "short" },
            { "smallmoney", "decimal" },
            { "sql_variant", "object" },
            { "text", "string" },
            { "time", "TimeSpan" },
            { "tinyint", "byte" },
            { "uniqueidentifier", "Guid" },
            { "varbinary", "byte[]" },
            { "varchar", "string" },
            { "xml", "string" }
        };

        public static string Search(string Key)
        {
            if (sqlToCSharpTypes.ContainsKey(Key))
            {
                return sqlToCSharpTypes[Key];
            }
            return "";
        }
    }
}
