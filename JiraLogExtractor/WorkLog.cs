using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraLogExtractor
{
    public class WorkLog
    {
        public string JiraId { get; set; }
        public string JiraName { get; set; }
        public string DisplayName { get; set; }
        public string LogTime { get; set; }
        public double LogTimeInHours { get; set; }
        public string Comment { get; set; }
        public DateTime DateTimeLogged { get; set; }
    }
}
