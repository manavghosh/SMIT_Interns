using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDCalculation.Models
{
    interface iSqlOperations
    {
        void insertHoliday();
        void deleteHoliday();
        corporateHolidayClass readEntry();
        void updateHoliday();
    }
}
