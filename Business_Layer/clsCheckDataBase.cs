using DataAccess_Layer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class clsCheckDataBase
    {
        public static bool DoesDatabaseExist(string DataBase)
        {
            return clsData.DoesDatabaseExist(DataBase);
        }

        public static bool CheckDatabaseCredentials(string DataBase,string UserName , string Password) { 
            return clsData.CheckDatabaseCredentials(DataBase, UserName, Password);
        }
    }
}
