using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace EasyfisUtilities.Forms.Database
{
    public partial class SysDatabaseForm : Form
    {
        public IndexForm indexForm;

        public SysDatabaseForm(IndexForm index, SqlConnection sqlConnection)
        {
            InitializeComponent();

            indexForm = index;

            using (SqlCommand sqlCommand = new SqlCommand("SELECT name FROM sys.databases WHERE name LIKE '%easyfis_%' AND name NOT IN ('master', 'tempdb', 'model', 'msdb')", sqlConnection))
            {
                using (IDataReader dataReader = sqlCommand.ExecuteReader())
                {
                    List<String> databases = new List<String>();

                    while (dataReader.Read())
                    {
                        databases.Add(dataReader[0].ToString());
                    }

                    comboBoxDatabase.DataSource = databases;
                }
            }

            sqlConnection.Close();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                buttonConnect.Enabled = false;
                buttonCancel.Enabled = false;

                String connectionString = indexForm.SetNewDatabaseConnectionString(comboBoxDatabase.Text);
                SqlConnection sqlConnection = new SqlConnection(connectionString);

                sqlConnection.Open();

                Models.SysConnectionString newConnectionString = new Models.SysConnectionString()
                {
                    ConnectionString = connectionString
                };

                String newJson = new JavaScriptSerializer().Serialize(newConnectionString);
                String connectionStringPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings/SysConnectionString.json");

                File.WriteAllText(connectionStringPath, newJson);

                sqlConnection.Close();

                Close();
                indexForm.Hide();

                Software.SysSoftwareForm sysSoftwareForm = new Software.SysSoftwareForm();
                sysSoftwareForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Easyfis Utilities", MessageBoxButtons.OK, MessageBoxIcon.Error);

                buttonConnect.Enabled = true;
                buttonCancel.Enabled = true;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
