using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraLogExtractor
{
    public class JiraItem
    {
        public string JiraId { get; set; }
        public string Project { get; set; }
        public List<WorkLog> WorkLog { get; set; }
        public string Status { set; get; }
        public DateTime LastUpdated { get; set; }
    }
}
