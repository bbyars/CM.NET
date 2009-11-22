using System;
using System.Data.SqlClient;
using Acme.Web.Properties;

namespace Acme.Web
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string Name
        {
            get
            {
                const string sql = "select name from names where id = 1";
                using (var connection = new SqlConnection(Settings.Default.DBConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        return command.ExecuteScalar().ToString();
                    }
                }
            }
        }
    }
}
