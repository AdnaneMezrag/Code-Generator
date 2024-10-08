using DataAccess_Layer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class clsTable
    {
        public string Name { get; set; }
        public clsAttribute PrimaryAttribute { get; set; }
        public List <clsAttribute> NormalAttributes { get; set; }
        public List <clsForeignAttribute> ForeignAttributes { get; set; }

        public static clsTable FindTable(string TableName)
        {
            clsTable table = new clsTable();
            table.Name = TableName;
            table.NormalAttributes = new List<clsAttribute>();
            table.ForeignAttributes = new List<clsForeignAttribute>();

            DataTable dt = clsData.GetTable(TableName);

            string PK = FindPrimaryKey(TableName);
            DataTable dtForeignKeys = FindForeignKeys(TableName);

            foreach (DataRow dr in dt.Rows)
            {
                clsAttribute Attribute = new clsAttribute();
                clsForeignAttribute ForeignAttribute = new clsForeignAttribute();

                Attribute.FillAttribute((string)dr[1], Convert.ToString(dr[2]), (dr[3].ToString() == "NO" ? false : true));
                int Row = -1;
                if (dr[1].ToString() == PK)
                {
                    table.PrimaryAttribute = Attribute;
                }
                else if ((Row = IsAttributeAForeignKey(dtForeignKeys, dr[1].ToString())) != -1)
                {
                    ForeignAttribute.FillAttribute(dr[1].ToString(), dr[2].ToString(), (dr[3].ToString() == "NO" ? false : true)
                        , dtForeignKeys.Rows[Row][1].ToString());
                    table.ForeignAttributes.Add(ForeignAttribute);
                }
                else
                {
                    table.NormalAttributes.Add(Attribute);
                }
            }
            return table;
        }

        public static DataTable FindForeignKeys(string TableName)
        {
            return clsData.GetForeignKeys(TableName);
        }

        public static string FindPrimaryKey(string TableName)
        {
            return clsData.GetPrimaryKey(TableName);
        }

        public static int IsAttributeAForeignKey(DataTable dtForeignKeys,string Attribute)
        {
            int Row = 0;
            foreach(DataRow dr in dtForeignKeys.Rows)
            {
                if (dr[0].ToString() == Attribute)
                {
                    return Row;
                }
                Row++;
            }
            return -1;
        }

        public static List<string> GetTableNames()
        {
            return clsData.GetTableNames();
        }

        public static List<clsTable> FindTables()
        {
            List<clsTable> Tables = new List<clsTable>();
            List<string> tableNames = GetTableNames();
            string TableName = "";
            for (int i = 0; i < tableNames.Count; i++)
            {
                TableName = tableNames[i].ToString();
                Tables.Add(FindTable(TableName));
            }
            return Tables;
        }
    }
}
