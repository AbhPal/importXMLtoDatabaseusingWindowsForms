﻿using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;

namespace importDatafromXMLtoDatabaseusingWindowsForms
{
    public partial class ifxtd : Form
    {
        private string fileName     = "";
        private string fileNameOnly = "";

        //private bool accepted = false;        // Not required
        readonly string currentPath  = @"C:\Users\abhij\OneDrive\Mutual_Fund\Current\";
        readonly string archivePath  = @"C:\Users\abhij\OneDrive\Mutual_Fund\Archieve\";
        readonly string errorPath    = @"C:\Users\abhij\OneDrive\Mutual_Fund\Error\";


        public ifxtd()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an XML file";
            openFileDialog.Filter = "XML File|*.xml";
            openFileDialog.InitialDirectory = currentPath;
            DialogResult dialogResult = openFileDialog.ShowDialog();
            if(dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.fileName = openFileDialog.FileName;
                fileNameOnly = this.fileName.Replace(currentPath, "");
                string messageName = "XML file " + fileNameOnly + " selected.";
                MessageBox.Show(messageName);
            }

        }

        private void btnImport_Click(object sender, EventArgs e)
        {


            //MessageBox.Show(xmlDocument);



            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["invcon"].ConnectionString;

                SqlConnection sqlConnection1 = new SqlConnection(connectionString);
                sqlConnection1.Open();
                string query1 = "TRUNCATE TABLE Investment.dbo.temp_hdfcMF";
                using (SqlCommand sqlCommand1 = new SqlCommand(query1, sqlConnection1))
                {
                    sqlCommand1.ExecuteNonQuery();
                }
                sqlConnection1.Close();

                progressBar1.Minimum = 0;



                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(this.fileName);

                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/root/row");
                progressBar1.Minimum = 0;
                progressBar1.Maximum = xmlNodeList.Count;

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    string fundName     = xmlNode["Fund_Name"].InnerText;
                    string amfiCode     = xmlNode["AmfiCode"].InnerText;
                    string planName     = xmlNode["Plan_Name"].InnerText;
                    string navDate      = xmlNode["NAV_Date"].InnerText;
                    string navAmount    = xmlNode["NAV_Amount"].InnerText;


                    SqlConnection sqlConnection2 = new SqlConnection(connectionString);
                    sqlConnection2.Open();
                    String query2 = "INSERT INTO dbo.temp_hdfcMF(fund_Name, amfiCode, plan_Name, nav_Date, nav_Amount) VALUES ('" + fundName + "','" + amfiCode + "','" + planName + "','" + navDate + "','" + navAmount + "')";
                    using (SqlCommand sqlCommand2 = new SqlCommand(query2, sqlConnection2))
                    {
                        sqlCommand2.ExecuteNonQuery();
                        progressBar1.Value++;
                    }
                    sqlConnection2.Close();

                    // Look for the name in the connectionStrings section.
                }

                string messageBox = "XML file" + fileNameOnly + "has been successfully imported into database";
                MessageBox.Show(messageBox);

                string currentFile      = currentPath + fileNameOnly;
                string archiveFile      = archivePath + fileNameOnly;
                System.IO.File.Move(currentFile, archiveFile);

            }
            catch
            {
                MessageBox.Show("Can't import the selected " + fileNameOnly + "xml file.");
                string currentFile      = currentPath + fileNameOnly;
                string errorFile        = errorPath + fileNameOnly;
                System.IO.File.Move(currentFile, errorFile);
            }


        }

        
    }
}
