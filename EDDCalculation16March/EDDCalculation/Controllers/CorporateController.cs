using EDDCalculation.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace EDDCalculation.Controllers
{
    public class CorporateController : Controller
    {
        //
        // GET: /Corporate/
        public ActionResult Index()
        {
            SqlConnection connectionSql = GetSqlConnection.getSqlConnection();
            connectionSql.Open();

            String query = "Select * from CountryDB";
            SqlCommand command = new SqlCommand(query,connectionSql);
            List<String> listOfCountries = new List<string>();
            SqlDataReader dataReader = command.ExecuteReader();
            while(dataReader.Read())
            {
                listOfCountries.Add(dataReader["CountryName"].ToString());
            }
            dataReader.Dispose();
            connectionSql.Close();
            return View(listOfCountries);
        }
        /*
        public string getNew()
        {
            SqlConnection connectionSql = GetSqlConnection.getSqlConnection();
            connectionSql.Open();
            String query = "select * from CorporateHoliday";
            SqlCommand cmd = new SqlCommand(query, connectionSql);

            SqlDataReader dataReader = cmd.ExecuteReader();
            List<eventListClass> list = new List<eventListClass>();
            while (dataReader.Read())
            {
                eventListClass obj = new eventListClass();
                DateTime date = Convert.ToDateTime(dataReader["HolidayDate"]);

                obj.start = date.ToString();
                obj.backgroundColor = "#ff0000";
                obj.title = dataReader["HolidayDescription"].ToString();
                obj.allDay = "true";

                list.Add(obj);
            }
            connectionSql.Close();
            // connectionSql.Dispose();
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonString = javaScriptSerializer.Serialize(list);

            return jsonString;
        }
        */
        public string getDatabaseEvents()
        {
            SqlConnection connectionSql = GetSqlConnection.getSqlConnection();

            connectionSql.Open();
            String query = "select * from CorporateHoliday";
            SqlCommand cmd = new SqlCommand(query, connectionSql);

            SqlDataReader dataReader = cmd.ExecuteReader();
            List<eventListClass> list = new List<eventListClass>();
            while (dataReader.Read())
            {
                eventListClass obj = new eventListClass();
                DateTime date=Convert.ToDateTime(dataReader["HolidayDate"]);
                obj.start = date.ToString();
                obj.title = dataReader["HolidayDescription"].ToString();
                obj.allDay = "true";
                obj.backgroundColor = dataReader["HolidayColor"].ToString();
                list.Add(obj);
            }
            connectionSql.Close();
            dataReader.Dispose();
            var javaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonString = javaScriptSerializer.Serialize(list);

            return jsonString;
        }
        public bool AddEventToDatabase(string eventTitle,string eventStart,string eventEnd,string eventColor)
        {
            SqlConnection connectionSql = GetSqlConnection.getSqlConnection();
            connectionSql.Open();

            SqlTransaction transaction = connectionSql.BeginTransaction();
            String query = "insert into CorporateHoliday values('in',@date,@holidayDescription,@isWeekend,@holidayColor);";
            SqlCommand cmd = new SqlCommand(query, connectionSql,transaction);

            cmd.Parameters.AddWithValue("@date", eventStart);
            cmd.Parameters.AddWithValue("@holidayDescription", eventTitle);
            cmd.Parameters.AddWithValue("@holidayColor",eventColor);
            if (String.Compare(eventTitle, "saturday", true) == 0 || String.Compare(eventTitle, "sunday", true) == 0)
            {
                cmd.Parameters.AddWithValue("@isWeekend", 1);
            }
            else{
                 cmd.Parameters.AddWithValue("@isWeekend",0);
            }

            int rowsInserted =cmd.ExecuteNonQuery();
            transaction.Commit();
            connectionSql.Close();
            if (rowsInserted > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteEventFromCorporateDatabase(string eventTitle, string eventStart,string eventColor)
        {
            SqlConnection connection = GetSqlConnection.getSqlConnection();
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();

            String query = "delete from CorporateHoliday where HolidayDate=@eventDate and HolidayDescription=@eventDescription and HolidayColor=@eventColor";

            SqlCommand command = new SqlCommand(query,connection,transaction);
            command.Parameters.AddWithValue("@eventDate",eventStart);
            command.Parameters.AddWithValue("@eventDescription",eventTitle);
            command.Parameters.AddWithValue("@eventColor", eventColor);

            int rowsDeleted = command.ExecuteNonQuery();

            transaction.Commit();
            connection.Close();
            if (rowsDeleted > 0)
            {
                return true;
            }
            else {
                return false;
            }
        }
        
        public bool UpdateHolidayDatabase(string newEventTitle,string eventStart,string oldEventTitle,string oldEventColor,string newEventColor)
        {
            SqlConnection connection = GetSqlConnection.getSqlConnection();
            connection.Open();

            string query = "update CorporateHoliday set HolidayDescription=@newDescription, HolidayColor=@holidayColor where  HolidayDate=@eventDate AND HolidayDescription=@eventTitle AND HolidayColor=@eventColor";

            SqlTransaction transaction = connection.BeginTransaction();

            SqlCommand command = new SqlCommand(query,connection,transaction);
            

            command.Parameters.AddWithValue("@newDescription",newEventTitle);
            command.Parameters.AddWithValue("@eventDate", eventStart);
            command.Parameters.AddWithValue("@eventTitle", oldEventTitle);
            command.Parameters.AddWithValue("@holidayColor",newEventColor);
            command.Parameters.AddWithValue("@eventColor", oldEventColor);

            int rowsUpdated = command.ExecuteNonQuery();
            try { transaction.Commit(); }
            catch { }

            connection.Close();
            if(rowsUpdated>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult CalendarPlanner()
        {
            SqlConnection connection = GetSqlConnection.getSqlConnection();
            connection.Open();

            String query = "Select * from CorporateHoliday where HolidayDate Like @currentYear";
            SqlCommand command = new SqlCommand(query,connection);

            command.Parameters.AddWithValue("@currentYear", (DateTime.Today.Year)+"%");

            SqlDataReader dataReader = command.ExecuteReader();

            List<eventListClass> list = new List<eventListClass>();

            while(dataReader.Read())
            {
                eventListClass holidayObj = new eventListClass();
                holidayObj.title = dataReader["HolidayDescription"].ToString();
                CultureInfo culture = new CultureInfo("fr-FR");
                holidayObj.start = dataReader["HolidayDate"].ToString();
                list.Add(holidayObj);
            }
            dataReader.Dispose();
            connection.Close();
            return View(list);
        }

        public ActionResult ImportFromPreviousYear()
        {
            SqlConnection connection = GetSqlConnection.getSqlConnection();
            connection.Open();

            int previousYear = DateTime.Today.Year-1; 

            String query = "Select * from CorporateHoliday where HolidayDate Like @previousYear or HolidayDate like @currentYear";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("previousYear",previousYear+"%");
            command.Parameters.AddWithValue("@currentYear", (DateTime.Today.Year) + "%");
            SqlDataReader dataReader = command.ExecuteReader();

            List<eventListClass> list = new List<eventListClass>();

            while (dataReader.Read())
            {
                eventListClass holidayObj = new eventListClass();
                holidayObj.title = dataReader["HolidayDescription"].ToString();

                CultureInfo culture = new CultureInfo("fr-FR");
                holidayObj.start = dataReader["HolidayDate"].ToString();
                list.Add(holidayObj);
            }
            dataReader.Dispose();
            connection.Close();
            //return RedirectToAction("CalendarPlanner", list);
            return View(list);
        }
        
	}
}