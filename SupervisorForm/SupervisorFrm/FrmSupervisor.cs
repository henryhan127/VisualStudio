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
        internal string Account = "leader";
       
        private void formload()
        {

           try
           {
               using (SqlConnection conn = new SqlConnection())
               {
                   conn.ConnectionString = Settings.Default.LabDBstring_19216833105;

                  using(SqlCommand command=new SqlCommand())
                  {
                      command.Connection = conn;
                      command.CommandText = "select Emp_Id,FirstName,LastName from Employees where Account = @Account";
                      command.Parameters.Add("@Account", SqlDbType.VarChar, 10).Value = Account;
                      conn.Open();
                      using(SqlDataReader datareader1= command.ExecuteReader())
                      {
                          datareader1.Read();
                          string Emp_Id;
                          Emp_Id = datareader1["Emp_Id"].ToString();
                          this.toolStripLabel2.Text = datareader1["FirstName"].ToString() + datareader1["LastName"].ToString();
                          datareader1.Close();
                          command.CommandText = "select Limits_Id from Employee_Limits where Emp_Id =@Emp_Id";
                          command.Parameters.Add("@Emp_Id", SqlDbType.Int).Value = Emp_Id;
                          using (SqlDataReader datareader2 = command.ExecuteReader())
                          {
                              datareader2.Read();
                              string Limits_Id;
                              Limits_Id = datareader2["Limits_Id"].ToString();
                              datareader2.Close();
                              command.CommandText="select Limits_Name from Limits where Limits_Id = @Limits_Id";
                              command.Parameters.Add("@Limits_Id",SqlDbType.Int).Value=Limits_Id;
                              using (SqlDataReader datareader3 = command.ExecuteReader())
                              {
                                  datareader3.Read();
                                  this.toolStripLabel1.Text = datareader3["Limits_Name"].ToString();
                              }
                          }
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
