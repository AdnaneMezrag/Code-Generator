using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_Layer
{
    public static class clsDataAccessSettings
    {
        public static string DataBase, UserID, Password;
        public static string ConnectionString = $"Server=.;Database={DataBase};User ID={UserID};Password={Password};";
    
        public static void GetCredentials(string DataBase , string UserID , string Password)
        {
            clsDataAccessSettings.DataBase = DataBase ;
            clsDataAccessSettings.UserID = UserID ;
            clsDataAccessSettings.Password = Password ;
            ConnectionString = $"Server=.;Database={DataBase};User ID={UserID};Password={Password};";
        }
    
    }
}
