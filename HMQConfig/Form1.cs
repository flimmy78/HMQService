using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BekUtils.Database;

namespace HMQConfig
{
    public partial class Form1 : Form
    {
        private IDataProvider dbProvider = null;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionStr = string.Empty;
            connectionStr = @"Data Source=192.168.0.62;Initial Catalog = master;User ID = sa;PWD = ustbzy";
            dbProvider = DataProvider.CreateDataProvider(DataProvider.DataProviderType.SqlDataProvider, connectionStr);

            DataSet ds = dbProvider.RetriveDataSet("select name from master.dbo.sysdatabases;");
            if (null != ds)
            {
                string d = ds.Tables[0].Rows[0][0].ToString();
                int k = 0;
            }
        }
    }
}
