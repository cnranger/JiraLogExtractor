using System;
using System.Collections.Generic;

namespace JiraLogExtractor.Models
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
