using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess_Layer
{
    public static class clsData
    {
        public static DataTable GetTable(string TableName)
        {
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            DataTable dt = new DataTable();

            string query = @"SELECT 
    c.TABLE_NAME, 
    c.COLUMN_NAME, 
    c.DATA_TYPE, 
    c.IS_NULLABLE
	
FROM 
    INFORMATION_SCHEMA.COLUMNS AS c
JOIN 
    INFORMATION_SCHEMA.TABLES AS t
    ON c.TABLE_NAME = t.TABLE_NAME
WHERE 
    t.TABLE_TYPE = 'BASE TABLE'  -- Ensures only tables, not views
    AND t.TABLE_CATALOG = @DataBaseName
	AND t.TABLE_NAME NOT IN ('sysdiagrams')  -- Exclude sysdiagrams and other system tables if needed
	and t.TABLE_NAME = @TableName
ORDER BY 
    c.TABLE_NAME, c.ORDINAL_POSITION;";

            SqlCommand cmd = new SqlCommand(query,Connection);
            cmd.Parameters.AddWithValue("@DataBaseName", clsDataAccessSettings.DataBase);
            cmd.Parameters.AddWithValue("@TableName", TableName);

            try
            {
                Connection.Open();
                SqlDataReader DataReader = cmd.ExecuteReader();
                if (DataReader.HasRows)
                {
                    dt.Load(DataReader);
                }
                DataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return dt;
        }

        public static string GetPrimaryKey(string TableName)
        {
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string PrimaryKey = "";

            string query = @"
SELECT 
    --tc.TABLE_NAME, 
    kcu.COLUMN_NAME
    --,tc.CONSTRAINT_NAME 
FROM 
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
JOIN 
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu
ON 
    tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
WHERE 
    tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND tc.TABLE_NAME NOT IN ('sysdiagrams')  -- Exclude sysdiagrams and other system tables if needed
	and tc.TABLE_NAME = @TableName
ORDER BY 
    tc.TABLE_NAME, kcu.COLUMN_NAME;";

            SqlCommand cmd = new SqlCommand(query, Connection);
            cmd.Parameters.AddWithValue("@TableName", TableName);

            try
            {
                Connection.Open();
                object PK = cmd.ExecuteScalar();
                PrimaryKey = PK.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return PrimaryKey;
        }

        public static DataTable GetForeignKeys(string TableName)
        {
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            DataTable dt = new DataTable();

            string query = @"	SELECT 
    --tc.TABLE_NAME AS ChildTable,
    kcu.COLUMN_NAME AS ChildColumn,
    ccu.TABLE_NAME AS ParentTable
FROM 
    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS rc
JOIN 
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
    ON rc.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
JOIN 
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu
    ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
JOIN 
    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE AS ccu
    ON rc.UNIQUE_CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
	where tc.TABLE_NAME = @TableName
ORDER BY 
    tc.TABLE_NAME, kcu.COLUMN_NAME;";

            SqlCommand cmd = new SqlCommand(query, Connection);
            cmd.Parameters.AddWithValue("@TableName", TableName);

            try
            {
                Connection.Open();
                SqlDataReader DataReader = cmd.ExecuteReader();
                if (DataReader.HasRows)
                {
                    dt.Load(DataReader);
                }
                DataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return dt;
        }

        public static List<string> GetTableNames()
        {
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            List<string> tableNames = new List<string>();

            string query = @"SELECT distinct 
    c.TABLE_NAME 
	
FROM 
    INFORMATION_SCHEMA.COLUMNS AS c
JOIN 
    INFORMATION_SCHEMA.TABLES AS t
    ON c.TABLE_NAME = t.TABLE_NAME
WHERE 
    t.TABLE_TYPE = 'BASE TABLE'  -- Ensures only tables, not views
    AND t.TABLE_CATALOG = @DataBaseName
	AND t.TABLE_NAME NOT IN ('sysdiagrams')  -- Exclude sysdiagrams and other system tables if needed
";

            SqlCommand cmd = new SqlCommand(query, Connection);
            cmd.Parameters.AddWithValue("@DataBaseName", clsDataAccessSettings.DataBase);

            try
            {
                Connection.Open();
                SqlDataReader DataReader = cmd.ExecuteReader();
                while (DataReader.Read())
                {
                    tableNames.Add(DataReader[0].ToString()) ;
                }
                DataReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return tableNames;
        }

        public static bool DoesDatabaseExist(string databaseName)
        {
            string connectionString = $"Server=.;Integrated Security=true;";  // Change for your credentials

            string query = $"SELECT database_id FROM sys.databases WHERE Name = @databaseName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@databaseName", databaseName);

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        // If database_id is returned, the database exists
                        return result != null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        return false;
                    }
                }
            }
        }

        public static bool CheckDatabaseCredentials(string database, string username, string password)
        {
            string connectionString = $"Server=.;Database={database};User Id={username};Password={password};";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return true; // Connection successful, credentials are valid
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Connection failed: {ex.Message}");
                    return false; // Invalid credentials or other connection issues
                }
            }
        }

    }
}
