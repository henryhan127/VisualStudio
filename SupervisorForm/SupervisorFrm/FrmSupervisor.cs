using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using SupervisorForm.Properties;
namespace SupervisorForm.SupervisorFrm
{
    public partial class FrmSupervisor : Form
    {
        public FrmSupervisor()
        {
            InitializeComponent();
            formload();
        }
        internal string account = "admin";
        private void formload()
        {

           try
           {
               using (SqlConnection conn = new SqlConnection())
               {
                   conn.ConnectionString = Settings.Default.LabDBstring;

                  using(SqlCommand command=new SqlCommand())
                  {
                      command.Connection = conn;
                      command.CommandText = "select Limits_Id from Limits where Limits_Name=@Limits_Name ";
                      command.Parameters.Add("@Limits_Name", SqlDbType.NVarChar, 10).Value = account;
                      conn.Open();
                      using(SqlDataReader datareader= command.ExecuteReader())
                      {
                          datareader.Read();
                          this.toolStripLabel1.Text = datareader["Limits_Id"].ToString();
                      }
                  }

               }
           }
            catch(Exception ex)
           {
               MessageBox.Show(ex.Message );
           }
        }
    }
}
