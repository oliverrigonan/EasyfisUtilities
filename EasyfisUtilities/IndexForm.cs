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

namespace EasyfisUtilities
{
    public partial class IndexForm : Form
    {
        public IndexForm()
        {
            InitializeComponent();

            String serverConnectionPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings/SysServerConnectionSettings.json");

            String json;
            using (StreamReader trmRead = new StreamReader(serverConnectionPath)) { json = trmRead.ReadToEnd(); }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Models.SysServerConnectionSettings sysServerConnectionSettings = javaScriptSerializer.Deserialize<Models.SysServerConnectionSettings>(json);

            textBoxServer.Text = sysServerConnectionSettings.Server;
            comboBoxAuthentication.Text = sysServerConnectionSettings.Authentication;
            textBoxUser.Text = sysServerConnectionSettings.User;

            if (sysServerConnectionSettings.RememberPassword == false)
            {
                textBoxPassword.Text = "";
            }
            else
            {
                textBoxPassword.Text = sysServerConnectionSettings.Password;
            }

            checkBoxRememberPassword.Checked = sysServerConnectionSettings.RememberPassword;
        }

        private void comboBoxAuthentication_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAuthentication.Text == "Windows Authentication")
            {
                textBoxUser.Text = "";
                textBoxPassword.Text = "";

                textBoxUser.Enabled = false;
                textBoxPassword.Enabled = false;
                checkBoxRememberPassword.Enabled = false;
            }
            else
            {
                textBoxUser.Enabled = true;
                textBoxPassword.Enabled = true;
                checkBoxRememberPassword.Enabled = true;
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                buttonConnect.Enabled = false;
                buttonCancel.Enabled = false;

                textBoxServer.Enabled = false;
                comboBoxAuthentication.Enabled = false;
                textBoxUser.Enabled = false;
                textBoxPassword.Enabled = false;
                checkBoxRememberPassword.Enabled = false;

                String server = textBoxServer.Text;
                String database = "master";
                String user = textBoxUser.Text;
                String password = textBoxPassword.Text;
                String connectionString = "";

                if (comboBoxAuthentication.Text == "Windows Authentication")
                {
                    connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";Persist Security Info=True;Integrated Security=true";
                }
                else
                {
                    connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";Persist Security Info=True;User ID=" + user + ";Password=" + password;
                }

                SqlConnection sqlConnection = new SqlConnection(connectionString);

                sqlConnection.Open();

                Models.SysServerConnectionSettings newServerConnectionSettings = new Models.SysServerConnectionSettings()
                {
                    Server = server,
                    Authentication = comboBoxAuthentication.Text,
                    User = user,
                    Password = password,
                    RememberPassword = checkBoxRememberPassword.Checked
                };

                String newJson = new JavaScriptSerializer().Serialize(newServerConnectionSettings);
                String serverConnectionPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings/SysServerConnectionSettings.json");

                File.WriteAllText(serverConnectionPath, newJson);

                Forms.Database.SysDatabaseForm sysDatabaseForm = new Forms.Database.SysDatabaseForm(this, sqlConnection);
                sysDatabaseForm.ShowDialog();

                buttonConnect.Enabled = true;
                buttonCancel.Enabled = true;

                textBoxServer.Enabled = true;
                comboBoxAuthentication.Enabled = true;

                if (comboBoxAuthentication.Text == "Windows Authentication")
                {
                    textBoxUser.Text = "";
                    textBoxPassword.Text = "";

                    textBoxUser.Enabled = false;
                    textBoxPassword.Enabled = false;
                    checkBoxRememberPassword.Enabled = false;
                }
                else
                {
                    textBoxUser.Enabled = true;
                    textBoxPassword.Enabled = true;
                    checkBoxRememberPassword.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Easyfis Utilities", MessageBoxButtons.OK, MessageBoxIcon.Error);

                buttonConnect.Enabled = true;
                buttonCancel.Enabled = true;

                textBoxServer.Enabled = true;
                comboBoxAuthentication.Enabled = true;

                if (comboBoxAuthentication.Text == "Windows Authentication")
                {
                    textBoxUser.Text = "";
                    textBoxPassword.Text = "";

                    textBoxUser.Enabled = false;
                    textBoxPassword.Enabled = false;
                    checkBoxRememberPassword.Enabled = false;
                }
                else
                {
                    textBoxUser.Enabled = true;
                    textBoxPassword.Enabled = true;
                    checkBoxRememberPassword.Enabled = true;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public String SetNewDatabaseConnectionString(String database)
        {
            String server = textBoxServer.Text;
            String user = textBoxUser.Text;
            String password = textBoxPassword.Text;
            String connectionString = "";

            if (comboBoxAuthentication.Text == "Windows Authentication")
            {
                connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";Persist Security Info=True;Integrated Security=true";
            }
            else
            {
                connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";Persist Security Info=True;User ID=" + user + ";Password=" + password;
            }

            return connectionString;
        }
    }
}
