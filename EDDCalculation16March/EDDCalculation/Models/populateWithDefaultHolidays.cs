using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace EDDCalculation.Models
{
    public class populateWithDefaultHolidays
    {

        SqlConnection connection = GetSqlConnection.getSqlConnection(); 
        
        int[] noOfDaysInAMonthArray=new int[12];

        public Boolean isLeapYear(int year)
        {
            if(year%400==0 || (year%4==0 && year%100!=0))
            {
                return true;
            }
            else{return false;}
        }

        public void populate()
        {

            noOfDaysInAMonthArray[0] = noOfDaysInAMonthArray[2] = noOfDaysInAMonthArray[4] = noOfDaysInAMonthArray[6] = noOfDaysInAMonthArray[7] = 31;
            noOfDaysInAMonthArray[9] = noOfDaysInAMonthArray[11] = 31;
            noOfDaysInAMonthArray[3] = noOfDaysInAMonthArray[5] = noOfDaysInAMonthArray[8] = noOfDaysInAMonthArray[10] = 30;

            String query = "Select CurrentYear,FirstDay from HolidaySettings";
            SqlCommand cmd = new SqlCommand(query,connection);

            SqlDataReader dataReader = cmd.ExecuteReader();
            
            dataReader.Read();
            int currentYear=Convert.ToInt32(dataReader["CurrentYear"]);
           if(isLeapYear(currentYear))
           {
               noOfDaysInAMonthArray[1] = 29;
           }
           else
           {
               noOfDaysInAMonthArray[1] = 28;
           }
            
           int FirstDay = Convert.ToInt32(dataReader["FirstDay"])-1;
           dataReader.Close();
           for (int i = 0; i <= 11; i++ )
           {
               int j;
               for (j = 1; j <=noOfDaysInAMonthArray[i];j++ )
               {
                      if((FirstDay+j)%7==6 || (FirstDay+j)%7==0)
                      {
                          String queryInsert = "insert into CorporateHoliday values(@CountryID,@Date,@Description,1)";
                          SqlCommand insertCommand = new SqlCommand(queryInsert,connection);
                         
                          //Using Country as India
                          //insertCommand.AddWithValue("CountryID","in");

                          insertCommand.Parameters.AddWithValue("@CountryID","in");
                          
                          String month =(i+1).ToString();
                          if((i+1)<10)
                          {
                              month = "0" + (i + 1);
                          }
                          String day = j.ToString();
                          if(j<10)
                          {
                              day = "0" + j;
                          }
                          insertCommand.Parameters.AddWithValue("Date",currentYear+"-"+month+"-"+day);
                          insertCommand.Parameters.AddWithValue("Description","Weekend");

                          int rowsInserted = insertCommand.ExecuteNonQuery();
                      }
                      
               }
               FirstDay = (FirstDay + j)%7-1;
              
           }
            
        }
    }
}