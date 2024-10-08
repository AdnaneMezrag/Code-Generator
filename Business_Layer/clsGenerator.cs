using DataAccess_Layer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer
{
    public class clsGenerator
    {
        private static string _FolderPath;
        public static string FolderPath
        {
            get { return _FolderPath; }
            set {
                _FolderPath = value;
                DirectoryPathForBusinessLayer = value + "\\Business_Layer";
                clsUtil.CreateDirctory(DirectoryPathForBusinessLayer);
                DirectoryPathForDataLayer = value + "\\DataAccess_Layer";
                clsUtil.CreateDirctory(DirectoryPathForDataLayer);
            }
        }

        static string DirectoryPathForBusinessLayer;
        static string DirectoryPathForDataLayer;
        static string FileName;
        private static clsTable _Table;
        public static clsTable Table
        {
            get { return _Table; }
            set
            {
                _Table = value;
                FileName = "cls" + _Table.Name;
                DataAccessAssociated = FileName + "Data";

            }
        }
        public static string DataAccessAssociated;

        public static void PostCredentialsToDataAccessSettings(string DataBase , string UserID , string Password)
        {
            clsDataAccessSettings.GetCredentials(DataBase , UserID , Password);
        }

        //-----------------------------Business Layer---------------------------------
        public static void GenerateBusinessLayerFile()
        {

            string FilePath = DirectoryPathForBusinessLayer+ "\\" +FileName + ".cs";

            string Content = GenerateBusinessHeaderAndAttributes();
            Content += GenerateFirstConstructor();
            Content += GenerateSecondConstructor();
            Content += GenerateAddNew();
            Content += GenerateUpdate();
            Content += GenerateFind();
            Content += GenerateSave();
            Content += GenerateDelete();
            Content += GenerateIsExist();
            Content += GenerateGetAll();

            Content += "\n    }\n}";
            File.WriteAllText(FilePath, Content);

        }



        //Generation Of Attributes
        public static string GenerateBusinessHeaderAndAttributes()
        {
            //Needs Testing
            string Content = $@"using System;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using DataAccess_Layer;

namespace Business_Layer{{
    public class {FileName}
    {{

        public enum enMode {{ AddNew = 0, Update = 1 }};
        public enMode Mode = enMode.AddNew;

";
            //Generate Attributes
            Content += GenerateBusinessAttributes();


            return Content;
        }
        public static string GenerateBusinessAttributes()
        {
            //Needs Testing
            string Content = "";
            Content += GenerateBusinessAttribute(Table.PrimaryAttribute);
            foreach(clsAttribute attribute in Table.NormalAttributes)
            {
                Content += GenerateBusinessAttribute(attribute);
            }
            foreach (clsForeignAttribute attribute in Table.ForeignAttributes)
            {
                Content += GenerateBusinessAttribute(attribute);
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                Content += GenerateBusinessForeignAttribute(foreignAttribute);
            }
            return Content;
        }
        public static string GenerateBusinessAttribute(clsAttribute attribute)
        {
            string Content = $@"        public {clsDataTypesMapping.Search(attribute.DataType)} {attribute.Name} {{set;get;}}
";
            return Content;
        }
        public static string GenerateBusinessForeignAttribute(clsForeignAttribute foreignAttribute)
        {
            string Content = $@"        public cls{foreignAttribute.ParentTableName} {foreignAttribute.ParentTableName} {{set;get}}
";
            return Content;
        }
        //Generation Of Attributes



        //Generation Of Constructors

        //First Constructor For Adding New Records
        public static string GenerateConstructorLine(clsAttribute attribute, string Prefix = "this.")
        {
            string Line = $@"        {Prefix}{attribute.Name} = ";
            if (attribute.CSharpDataType == "int" || attribute.CSharpDataType == "float" || 
                attribute.CSharpDataType ==
                "double" || attribute.CSharpDataType == "decimal" || 
                attribute.CSharpDataType == "short"
                )
            {
                Line += "-1;";
            }
            else if (attribute.CSharpDataType == "string")
            {
                Line += @""""";";
            }
            else if (attribute.CSharpDataType == "DateTime")
            {
                Line += "DateTime.Now;";
            }
            else if (attribute.CSharpDataType == "bool")
            {
                Line += "false;";
            }
            else if (attribute.CSharpDataType == "byte")
            {
                Line += "0;";
            }
            Line += "\n";
            return Line;

        }
        public static string GenerateConstructorLines()
        {
            string Content = "";
            Content += GenerateConstructorLine(Table.PrimaryAttribute);
            foreach (clsAttribute attribute in Table.NormalAttributes)
            {
                Content += GenerateConstructorLine(attribute);
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                Content += GenerateConstructorLine(foreignAttribute);
            }
            return Content;
        }
        public static string GenerateFirstConstructor()
        {
            string Content = $@"    public {FileName} (){{
{GenerateConstructorLines() }

        this.Mode = enMode.AddNew;
}}
" ;
            return Content;
        }




        //Second Constructor For Finding Records
        public static string GenerateAttributeParameter(clsAttribute attribute , string Prefix="")
        {
            
            return Prefix + attribute.CSharpDataType + " " + attribute.Name + ", ";
        }
        public static string GenerateAttributesParameters(bool IncludePK = true , string Prefix = "")
        {
            string AttributesParameters = "";
            if (IncludePK)
            {
                AttributesParameters += GenerateAttributeParameter(Table.PrimaryAttribute);
            }
            foreach (clsAttribute attribute in Table.NormalAttributes)
            {
                AttributesParameters += GenerateAttributeParameter(attribute, Prefix);
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                AttributesParameters += GenerateAttributeParameter(foreignAttribute, Prefix);
            }
            AttributesParameters = AttributesParameters.Remove(AttributesParameters.Length - 2,2);
            return AttributesParameters;
        }
        public static string GenerateConstructorLineForSecondConstructor(clsAttribute attribute)
        {
            return $@"        this.{attribute.Name} = {attribute.Name};
";
        }
        public static string GenerateConstructorLinesForSecondConstructor()
        {
            string Content = "";
            Content += GenerateConstructorLineForSecondConstructor(Table.PrimaryAttribute);
            foreach (clsAttribute attribute in Table.NormalAttributes)
            {
                Content += GenerateConstructorLineForSecondConstructor(attribute);
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                Content += GenerateConstructorLineForSecondConstructor(foreignAttribute);
            }
            return Content;
        }
        public static string GenerateCompostionAttribute(clsForeignAttribute foreignAttribute)
        {
            return $@"        this.{foreignAttribute.ParentTableName} = cls{foreignAttribute.ParentTableName}.Find({foreignAttribute.Name});
";
        }
        public static string GenerateCompositionAttributes()
        {
            string Content = "";
            foreach(clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                Content += GenerateCompostionAttribute(foreignAttribute);
            }
            return Content;
        }
        public static string GenerateSecondConstructor()
        {
            string Content = $@"    private {FileName} ({GenerateAttributesParameters()}){{
{GenerateConstructorLinesForSecondConstructor()}
{GenerateCompositionAttributes()}

        this.Mode = enMode.Update;
}}
";
            return Content;
        }





        //CRUD Operations
        public static string PassAtributesAsParameters(string Prefix = "")
        {
            string Parameters = "";
            foreach(clsAttribute attribute in Table.NormalAttributes)
            {
                Parameters += Prefix + attribute.Name + ", ";
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                Parameters += Prefix + foreignAttribute.Name + ", ";
            }
            Parameters = Parameters.Remove(Parameters.Length-2);
            return Parameters;
        }

        //AddNew
        public static string GenerateAddNew()
        {
            string Content = $@"    private bool _AddNew{Table.Name}(){{
";
            Content += $@"        this.{Table.PrimaryAttribute.Name} = {DataAccessAssociated}.AddNew{Table.Name}(";
            Content += PassAtributesAsParameters("this.") + ");";
            Content += $@"
        return (this.{Table.PrimaryAttribute.Name} != -1);
    }}
";
            return Content;
        }

        //Update
        public static string GenerateUpdate()
        {
            string Content = $@"    private bool _Update{Table.Name}(){{
";
            Content += $@"        return {DataAccessAssociated}.Update{Table.Name}(";
            Content += $"this.{Table.PrimaryAttribute.Name}, " + PassAtributesAsParameters("this.") + ");" + "\n    }\n";
            return Content;
        }

        //Find
        public static string GenerateFind()
        {
            string Content = $@"    public static {FileName} Find({Table.PrimaryAttribute.CSharpDataType} {Table.PrimaryAttribute.Name}){{
";
            foreach (clsAttribute attribute in Table.NormalAttributes)
            {
                Content += GenerateConstructorLine(attribute,attribute.CSharpDataType + " ");
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                Content += GenerateConstructorLine(foreignAttribute , foreignAttribute.CSharpDataType + " ");
            }
            Content += "\n";
            Content += $@"        bool IsFound = {DataAccessAssociated}.Get{Table.Name}InfoBy{Table.PrimaryAttribute.Name}(
            {Table.PrimaryAttribute.Name}, " + PassAtributesAsParameters("ref ") + ");\n\n";

            Content += $@"        if (IsFound){{
            return new {FileName}({Table.PrimaryAttribute.Name}, ";
            Content += PassAtributesAsParameters() + ");}\n";
            Content += "        else{ return null;}\n";

            Content += "    }\n";
            return Content;
        }

        //Save
        public static string GenerateSave()
        {
            string Content = $@"    public bool Save()
    {{
        switch (Mode)
        {{
            case enMode.AddNew:
                if (_AddNew{Table.Name}())
                {{

                    Mode = enMode.Update;
                    return true;
                }}
                else
                {{
                    return false;
                }}

            case enMode.Update:

                return _Update{Table.Name}();

        }}

        return false;
    }}
";
            return Content;
        }

        //Delete
        public static string GenerateDelete()
        {
            string Content = $@"    public bool Delete()
    {{
        return {DataAccessAssociated}.Delete{Table.Name}(this.{Table.PrimaryAttribute.Name}); 
    }}
";
            return Content;
        }

        //Is Exist
        public static string GenerateIsExist()
        {
            string Content = $@"    public static bool Is{Table.Name}Exist({Table.PrimaryAttribute.CSharpDataType} {Table.PrimaryAttribute.Name})
    {{
        return {DataAccessAssociated}.Is{Table.Name}Exist({Table.PrimaryAttribute.Name}); 
    }}
";
            return Content;
        }

        //Read
        public static string GenerateGetAll()
        {
            string Content = $@"    public static DataTable GetAll{Table.Name}()
    {{
        return {DataAccessAssociated}.GetAll{Table.Name}();

    }}
";
            return Content;
        }







        //-----------------------------Data Access Layer---------------------------------
        public static void GenerateDataLayerFile()
        {

            string FilePath = DirectoryPathForDataLayer + "\\" + DataAccessAssociated + ".cs";

            string Content = GenerateDataAccessHeader();
            Content += GenerateAddNewForDataLayer();
            Content += GenerateUpdateForDataLayer();
            Content += GenerateGetInfoByIDForDataLayer();
            Content += GenerateDeleteForDataLayer();
            Content += GenerateIsExistForDataLayer();
            Content += GenerateGetAllForDataLayer();

            Content += "\n    }\n}";
            File.WriteAllText(FilePath, Content);

        }



        //AddNew
        public static string GenerateDataAccessHeader()
        {
            string Content = $@"using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Policy;
using System.ComponentModel;

namespace DataAccess_Layer{{
    public class {DataAccessAssociated}
    {{

";

            return Content;
        }

        public static string GenerateCommandLine(clsAttribute attribute)
        {
            string Content = "";
            if (attribute.IsNullable)
            {
                Content += $@"        if({attribute.Name} == {attribute.DefaultValuee})
        {{
            command.Parameters.AddWithValue(""@{attribute.Name}"", DBNull.Value);
        }}
        else
        {{
            command.Parameters.AddWithValue(""@{attribute.Name}"", {attribute.Name} );
        }}
";
            }
            else
            {
                return $@"        command.Parameters.AddWithValue(""@{attribute.Name}"", {attribute.Name});
";
            }
            return Content;
        }

        public static string GenerateCommandLines(bool IncludePK = true)
        {
            string CommandLines = "";
            if (IncludePK)
            {
                CommandLines += GenerateCommandLine(Table.PrimaryAttribute);
            }
            foreach (clsAttribute attribute in Table.NormalAttributes)
            {
                CommandLines += GenerateCommandLine(attribute);
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                CommandLines += GenerateCommandLine(foreignAttribute);
            }
            return CommandLines;
        }

        public static string GenerateAddNewForDataLayer()
        {
            string Content = $@"    public static int AddNew{Table.Name}(" +
                GenerateAttributesParameters(false) + "){\n";

            Content += $@"        int ID = -1;

        SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
";

            Content += $@"        string query = @""INSERT INTO {Table.Name} ( 
                            {PassAtributesAsParameters()})
                            VALUES ({PassAtributesAsParameters("@")});
                            SELECT SCOPE_IDENTITY();"";

        SqlCommand command = new SqlCommand(query, connection);

";

            Content += GenerateCommandLines(false) + "\n\n";


            Content += $@"        try
        {{
            connection.Open();

            object result = command.ExecuteScalar();

            if (result != null && int.TryParse(result.ToString(), out int insertedID))
            {{
                ID = insertedID;
            }}
        }}

        catch (Exception ex)
        {{
            //Console.WriteLine(""Error: "" + ex.Message);

        }}

        finally
        {{
            connection.Close();
        }}


        return ID;
    }}
";

            return Content;
        }



        //Update
        public static string GenerateUpdateQueryLine(clsAttribute attribute)
        {
            return $@"
{attribute.Name} = @{attribute.Name},";
        }

        public delegate string GenerateLine(clsAttribute attribute);

        public static string GenerateLines(GenerateLine generateLine)
        {
            string CommandLines = "";
            foreach (clsAttribute attribute in Table.NormalAttributes)
            {
                CommandLines += generateLine(attribute);
            }
            foreach (clsForeignAttribute foreignAttribute in Table.ForeignAttributes)
            {
                CommandLines += generateLine(foreignAttribute);
            }
            CommandLines = CommandLines.Remove(CommandLines.Length - 1);
            return CommandLines;
        }

        public static string GenerateUpdateForDataLayer()
        {

            string Content = $@"    public static int Update{Table.Name}(" +
                GenerateAttributesParameters(true) + "){\n";

            Content += $@"        rowsAffected = 0;

        SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
";

            Content += $@"        string query = @""Update {Table.Name} set 
                            {GenerateLines(GenerateUpdateQueryLine)}
                            where {Table.PrimaryAttribute.Name} = @{Table.PrimaryAttribute.Name}"";
                            

        SqlCommand command = new SqlCommand(query, connection);

";

            Content += GenerateCommandLines() + "\n\n";


            Content += $@"        try
        {{
            connection.Open();
            rowsAffected = command.ExecuteNonQuery();

        }}
        catch (Exception ex)
        {{
            //Console.WriteLine(""Error: "" + ex.Message);
            return false;
        }}

        finally
        {{
            connection.Close();
        }}

        return (rowsAffected > 0);
    }}
";

            return Content;
        }



        //Get
        public static string GenerateLineToFillDataReader(clsAttribute attribute)
        {
            string Content = "                    ";
            if (attribute.IsNullable)
            {

                Content += $@"
                    if (reader[""{attribute.Name}""] == DBNull.Value)
                        {attribute.Name} = {attribute.DefaultValuee};
                    else
                        {attribute.Name} = ({attribute.CSharpDataType})reader[""{attribute.Name}""];
";
            }
            else
            {
                Content += $@"{attribute.Name} = ({attribute.CSharpDataType})reader[""{attribute.Name}""];";
            }
            Content += Environment.NewLine;
                
            return Content;
        }

        public static string GenerateGetInfoByIDForDataLayer()
        {
            string Content = $@"    public static bool Get{Table.Name}InfoByID(" +
    GenerateAttributesParameters(true,"ref ") + "){\n";

            Content += $@"        bool isFound = false;

        SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
";

            Content += $@"        string query = ""SELECT * FROM {Table.Name} WHERE {Table.PrimaryAttribute.Name} = @{Table.PrimaryAttribute.Name}"";
                            

        SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue(""@{Table.PrimaryAttribute.Name}"", {Table.PrimaryAttribute.Name});

";

            Content += $@"            try
            {{
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {{

                    // The record was found
                    isFound = true;
                    
{GenerateLines(GenerateLineToFillDataReader)}

                }}
                else
                {{
                    // The record was not found
                    isFound = false;
                }}

                reader.Close();


            }}
            catch (Exception ex)
            {{
                //Console.WriteLine(""Error: "" + ex.Message);
                isFound = false;
            }}
            finally
            {{
                connection.Close();
            }}

            return isFound;
        }}
";

            return Content;
        }



        //Delete
        public static string GenerateDeleteForDataLayer()
        {
            return $@"    public static bool Delete{Table.Name}(int {Table.PrimaryAttribute.Name})
    {{

        int rowsAffected = 0;

        SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

        string query = @""Delete {Table.Name} 
                            where {Table.PrimaryAttribute.Name} = @{Table.PrimaryAttribute.Name}"";

        SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue(""@{Table.PrimaryAttribute.Name}"", {Table.PrimaryAttribute.Name});

        try
        {{
            connection.Open();

            rowsAffected = command.ExecuteNonQuery();

        }}
        catch (Exception ex)
        {{
            // Console.WriteLine(""Error: "" + ex.Message);
        }}
        finally
        {{

            connection.Close();

        }}

        return (rowsAffected > 0);

    }}
";

        }


        //Is Exist
        public static string GenerateIsExistForDataLayer()
        {
            return $@"    public static bool Is{Table.Name}Exist(int {Table.PrimaryAttribute.Name})
    {{
        bool isFound = false;

        SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

        string query = ""SELECT Found=1 FROM {Table.Name} WHERE {Table.PrimaryAttribute.Name} = @{Table.PrimaryAttribute.Name}"";

        SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue(""@{Table.PrimaryAttribute.Name}"", {Table.PrimaryAttribute.Name});

        try
        {{
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            isFound = reader.HasRows;

            reader.Close();
        }}
        catch (Exception ex)
        {{
            //Console.WriteLine(""Error: "" + ex.Message);
            isFound = false;
        }}
        finally
        {{
            connection.Close();
        }}

        return isFound;
    }}
";
        }


        //Read
        public static string GenerateGetAllForDataLayer()
        {
            return $@"    public static DataTable GetAll{Table.Name}()
        {{

            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = ""select * from {Table.Name}"";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {{
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)

                {{
                    dt.Load(reader);
                }}

                reader.Close();


            }}

            catch (Exception ex)
            {{
                // Console.WriteLine(""Error: "" + ex.Message);
            }}
            finally
            {{
                connection.Close();
            }}

            return dt;

        }}
";
        }



        //Data Access Settings
        public static void GenerateDataAccessSettings(string DataBase, string UserID , string Password)
        {
            string Content = $@"using System;

namespace DataAccess_Layer
{{
    static class clsDataAccessSettings
    {{
        public static string ConnectionString = ""Server=.;Database={DataBase};User Id={UserID};Password={Password};"";


    }}
}}";

            string FilePath = DirectoryPathForDataLayer + "\\" + "clsDataAccessSettings" + ".cs";

            File.WriteAllText(FilePath, Content);
        }

        public static void GenerateBusinessAndDataLayers(string DataBase, string UserID, string Password)
        {
            PostCredentialsToDataAccessSettings(DataBase, UserID, Password);
            List<clsTable> Tables = clsTable.FindTables();
            for (int i = 0; i < Tables.Count; i++)
            {
                clsGenerator.Table = Tables[i];
                clsGenerator.GenerateBusinessLayerFile();
                clsGenerator.GenerateDataLayerFile();
            }
            GenerateDataAccessSettings(DataBase,UserID,Password);
        }
    }
}
