using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace JiraLogExtractor
{
    public enum PeriodEnum
    {
        startOfWeek,
        startOfMonth
    }

    public class ConfigManager
    {
        private List<string> _assignees;
        private string _baseUri;
        private string _projects;
        private string _period;

        public List<string> Assignees
        {
            get
            {
                if (_assignees == null)
                {
                    var assigneesString = ConfigurationManager.AppSettings["Assignees"];
                    _assignees = assigneesString.Split(new[] { ",", string.Empty }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                return _assignees;
            }
        }

        public string Projects
        {
            get
            {
                if (_projects == null)
                {
                    var projectString = ConfigurationManager.AppSettings["Projects"];
                    _projects = ConvertToJiraSearchString(projectString);
                }

                return _projects;
            }
        }

        public string BaseUri 
        {
            get
            {
                if (_baseUri == null)
                {
                    _baseUri = ConfigurationManager.AppSettings["BaseUri"];
                }
                return _baseUri;
            }
        }

        public string Period
        {
            get
            {
                if (_period == null)
                {
                    _period = ConfigurationManager.AppSettings["Period"] + "(" +
                              ConfigurationManager.AppSettings["PeriodOffset"] + ")";
                }
                return _period;
            }
        }

        public DateTime CutoffDate
        {
            get
            {
                var offset = int.Parse(ConfigurationManager.AppSettings["PeriodOffset"]);

                if (PeriodEnum.startOfWeek.ToString() == ConfigurationManager.AppSettings["Period"])
                {
                    var dayOfWeek   = (int) DateTime.Today.DayOfWeek;
                    if (dayOfWeek == 0) dayOfWeek = 7; 
                    return DateTime.Today.AddDays((offset - 1)*7 - dayOfWeek);
                }
                else
                {
                    var month = (int)DateTime.Today.Month;
                    var offSetMonth = month +1 + offset; 
                    return DateTime.Parse(1 + " " + offSetMonth + " " + DateTime.Today.Year);
                }
            }
        }

        private string ConvertToJiraSearchString(string projects)
        {
            // "(\"Aviva Singapore\",\"Singapore Life\",\"Mercer\",\"Reorient\", \"ANZTK\")";
            var projectList = projects.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var stringWithQuotations = string.Join(",", projectList.Select(x => "\"" + x.Trim() + "\""));
            return "(" + stringWithQuotations + ")";
        }
    }
}
