using System;
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
        string currentPath  = @"C:\Users\abhij\OneDrive\Mutual_Fund\Current\";
        string archivePath  = @"C:\Users\abhij\OneDrive\Mutual_Fund\Archieve\";
        string errorPath    = @"C:\Users\abhij\OneDrive\Mutual_Fund\Error\";


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
                string messageName = "XML file " + this.fileName + " selected.";
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




                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(this.fileName);

                XmlNodeList xmlNodeList = xmlDocument.SelectNodes("/root/row");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    string fundName = xmlNode["Fund_Name"].InnerText;
                    string amfiCode = xmlNode["AmfiCode"].InnerText;
                    string planName = xmlNode["Plan_Name"].InnerText;
                    string navDate = xmlNode["NAV_Date"].InnerText;
                    string navAmount = xmlNode["NAV_Amount"].InnerText;


                    SqlConnection sqlConnection2 = new SqlConnection(connectionString);
                    sqlConnection2.Open();
                    String query2 = "INSERT INTO dbo.temp_hdfcMF(fund_Name, amfiCode, plan_Name, nav_Date, nav_Amount) VALUES ('" + fundName + "','" + amfiCode + "','" + planName + "','" + navDate + "','" + navAmount + "')";
                    using (SqlCommand sqlCommand2 = new SqlCommand(query2, sqlConnection2))
                    {
                        sqlCommand2.ExecuteNonQuery();
                    }
                    sqlConnection2.Close();





                    // Look for the name in the connectionStrings section.


                }

                string messageBox = "XML file" + this.fileName + "has been successfully imported into database";
                MessageBox.Show(messageBox);

                string currentFile = currentPath + this.fileName;
                string archiveFile = archivePath + this.fileName;
                //System.IO.File.Move(currentFile, archiveFile);

            }
            catch
            {
                //MessageBox.Show("Can't import the selected xml file.");
                //string currentFile = currentPath + this.fileName;
                //string errorFile = errorPath + this.fileName;
                //System.IO.File.Move(currentFile, errorFile);
            }


        }

        
    }
}
