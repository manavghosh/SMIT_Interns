using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace EDDCalculation.Models
{
    public class GetSqlConnection
    {
        public static SqlConnection getSqlConnection()
        {
            String connString;
            SqlConnection conn=null;
            try
            {
                connString="Data Source=(LocalDB)\\v11.0;AttachDbFilename=\"C:\\Users\\Abhi_Agarwal\\documents\\visual studio 2013\\Projects\\EDDCalculation\\EDDCalculation\\App_Data\\HolidayDatabase.mdf\";Integrated Security=True";
                conn = new SqlConnection(connString);
            }
            catch { }
            return conn;
        }
    }
}