using System;
using System.Collections.Generic;
using System.IO;
using JiraLogExtractor.Models;

namespace JiraLogExtractor.Output
{
    public class CsvSaver
    {
        public void Save(IEnumerable<WorkLog> logs)
        {
            var lines = new List<string>();
            var seperator = "^";

            lines.Add("JiraId" + seperator + "JiraName" + seperator + "DisplayName" + seperator + "LogTime" + seperator + "LogTimeInHours" + seperator +
                           "DateTimeLogged" + seperator + "Comment");

            foreach (var log in logs)
            {
                var line = log.JiraId + seperator + log.JiraName + seperator + log.DisplayName + seperator + log.LogTime + seperator + log.LogTimeInHours + seperator +
                           log.DateTimeLogged + seperator + log.Comment.Replace(Environment.NewLine, " ");

                lines.Add(line);
            }

            File.WriteAllLines(@"log" + DateTime.Today.ToLongDateString() + ".xls", lines);
        }
    }
}
