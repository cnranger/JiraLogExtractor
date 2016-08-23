using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraLogExtractor
{
    public class StringParser
    {
        public double ConvertDayToHour(string logtime)
        {
            if (logtime.Contains("m"))
            {
                return 0;
            }

            if (logtime.Contains("d"))
            {
                var noOfDates = double.Parse(logtime.Substring(0, logtime.IndexOf("d")));
                var noOfHours = noOfDates * 6;

                if (logtime.Contains("h"))
                {
                    logtime = logtime.Replace("h", string.Empty);
                    var forePart = logtime.IndexOf("d") + 1;
                    noOfHours += double.Parse(logtime.Substring(forePart, logtime.Length - forePart));
                }

                return noOfHours;
            }
            else
            {
                return double.Parse(logtime.Replace("h", string.Empty));
            }
        }

    }
}
