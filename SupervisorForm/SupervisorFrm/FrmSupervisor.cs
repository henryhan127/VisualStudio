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
                   conn.ConnectionString = Settings.Default.LabDBstring;

                  using(SqlCommand command=new SqlCommand())
                  {
                      command.Connection = conn;
                      command.CommandText = "select Emp_Id,LastName,FirstName from Employees where Account = @Account";
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
               MessageBox.Show(ex.Message);
           }

           try
           {
               using (SqlConnection conn = new SqlConnection())
               {
                   conn.ConnectionString = Settings.Default.LabDBstring;                   
                   string level = "WITH Managers AS (SELECT Emp_ID, FirstName,LastName, ReportsTo, 1 AS level FROM Employees WHERE (ReportsTo IS NULL) UNION ALL SELECT emp.Emp_Id,emp.FirstName, emp.LastName, emp.ReportsTo, mgr.level + 1 FROM Employees AS emp INNER JOIN Managers AS mgr ON emp.ReportsTo = mgr.Emp_Id) SELECT Emp_Id,FirstName, LastName, ReportsTo, level FROM Managers ORDER BY level";                       
                       using (SqlDataAdapter Adapter = new SqlDataAdapter(level,conn))
                       {
                           DataSet Ds = new DataSet();
                           Adapter.Fill(Ds);
                           Ds.Relations.Add("NodeRelation", Ds.Tables[0].Columns["ReportsTo"], Ds.Tables[0].Columns["Emp_Id"]);
                           this.labDBDataSet1.EnforceConstraints = false;
                           this.employeesTableAdapter1.FillByRepotsTo(this.labDBDataSet1.Employees);
                           LabDBDataSet.EmployeesDataTable EmpTable = this.labDBDataSet1.Employees;
                           this.labDBDataSet1.Relations.Add("NodeRelation", EmpTable.Columns["ReportsTo"], EmpTable.Columns["Emp_Id"]);

                           //EmpTable.ChildRelations.Add("NodeRelation", EmpTable.Columns["ReportsTo"], EmpTable.Columns["Emp_Id"]);
                           foreach (DataRow DataRow in EmpTable.Rows)
                           {
                               if (DataRow.IsNull("ReportsTo"))
                               {
                                   TreeNode NewNode = CreateNode(DataRow["FirstName"].ToString(), DataRow["Emp_Id"].ToString());
                                   this.treeView1.Nodes.Add(NewNode);
                                   PopulateSubTree(DataRow, NewNode);
                               }
                           }
                       }                   
               }
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }

           
        }
        private void PopulateSubTree(DataRow DataRow, TreeNode node)
        {

            foreach (DataRow childRow in DataRow.GetChildRows("NodeRelation"))
            {
                TreeNode childNode = CreateNode(childRow["FirstName"].ToString(), childRow["Emp_Id"].ToString());
                node.Nodes.Add(childNode);
                PopulateSubTree(childRow, childNode);
            }
        }
        private TreeNode CreateNode(string FirstName, string Emp_Id)
        {
            TreeNode node = new TreeNode(); ;
            node.Text = FirstName;
            node.Name = Emp_Id;            
            return node;
        }
    }
}
