using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDCalculation.Models
{
    public class corporateHolidayClass
    {
        public String CountryId { get; set;}

        public DateTime HolidayDate {get; set;}

        public String HolidayDescription{get;set;}
        public bool isWeekend{get;set;}
    }
}