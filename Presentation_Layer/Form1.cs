using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Business_Layer;

namespace Presentation_Layer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to generate business and data access layers?",
                "Confirmation",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            string folderPath = "";
            using (var folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Get the selected folder path
                    folderPath = folderDialog.SelectedPath;
                    // Do something with the folder path
                }
            }
            if(folderPath == "")
            {
                MessageBox.Show("Please choose a folder to save the generated files in",
                    "Choose Folder",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string DataBase = tbDatabaseName.Text , UserID = tbUserID.Text ,
                Password = tbPassword.Text;

            if (!clsCheckDataBase.DoesDatabaseExist(DataBase))
            {
                MessageBox.Show($"The Database with name '{DataBase}' Dosen't Exist",
                    "Choose Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!clsCheckDataBase.CheckDatabaseCredentials(DataBase, UserID, Password))
            {
                MessageBox.Show($"Invalid UserID/Password",
    "Invalid Credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clsGenerator.FolderPath = folderPath;

            clsGenerator.GenerateBusinessAndDataLayers(DataBase,UserID,Password);

            MessageBox.Show($"Your Files Are Successfully Generated",
"Generation Succeeded", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
