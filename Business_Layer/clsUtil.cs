using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class clsUtil
    {
        public static bool CreateDirctory(string path)
        {

            // Check if the directory already exists
            if (!Directory.Exists(path))
            {
                // Create the directory
                Directory.CreateDirectory(path);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
