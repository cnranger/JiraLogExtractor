using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JiraLogExtractor
{
    public class Program
    {
        private const string BaseUri = "https://projects.fnz.com";

        static List<string> assignees = new List<string> { "fzhong", "wbu", "fxu", "fezhong", "zhang", "jazhang", "sgao", "fqin", "lchen", "sfan" };
        static string projects = "(\"Aviva Singapore\",\"Singapore Life\",\"Mercer\",\"Reorient\", \"ANZTK\")";

        public static void Main(string[] args)
        {
            var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), new ChromeOptions(), TimeSpan.FromMinutes(5));
            //   driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMinutes(-5));

            driver.Navigate().GoToUrl(BaseUri + "/jira/login.jsp");
            //driver.Manage().Window.Maximize();
            driver.Wait();

            Login(driver);

            var jqlQuery = HttpUtility.UrlEncode("updated >= startOfWeek(-1) and timespent > 0 and project in" + projects);
            driver.Navigate().GoToUrl(BaseUri + "/jira/secure/IssueNavigator!executeAdvanced.jspa?jqlQuery=" + jqlQuery + "&runQuery=true&clear=true");
            driver.Wait();

            var totalCount = int.Parse(driver.FindElementByClassName("results-count-total").Text);

            var lastUpdated = driver.FindElements(By.CssSelector("td[class^='nav updated']")).Select(x => x.Text).ToList();
            var issues = driver.FindElements(By.CssSelector("td[class^='nav issuekey']")).Select(x => x.Text).ToList();

            for (int j = 50; j <= totalCount + 50; j = j + 50)
            {
                driver.Navigate().GoToUrl("https://projects.fnz.com/jira/secure/IssueNavigator.jspa?pager/start=" + j);
                driver.Wait();
                lastUpdated.AddRange(driver.FindElements(By.CssSelector("td[class^='nav updated']")).Select(x => x.Text));
                issues.AddRange(driver.FindElements(By.CssSelector("td[class^='nav issuekey']")).Select(x => x.Text));
            }

            var i = 0;

            DateTime date;

            var jiraItems = new List<JiraItem>();
            foreach (var issue in issues)
            {
                var jiraItem = new JiraItem();
                date = DateTime.Parse(lastUpdated[i]);
                jiraItem.JiraId = issue;
                jiraItem.LastUpdated = date;
                jiraItem.WorkLog = GetLog(driver, issue);

                jiraItems.Add(jiraItem);
                i++;
            }

            var logs = jiraItems.SelectMany(x => x.WorkLog).Where(x => x.DateTimeLogged >= DateTime.Parse("8 Aug 2016"));
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

            //var logs = jiraItems.SelectMany(x => x.WorkLog)
            //    .Where(x => Assignees.Contains(x.JiraName) & x.DateTimeLogged >= DateTime.Today);

        }

        private static List<WorkLog> GetLog(ChromeDriver driver, string jiraId)
        {
            //var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), new ChromeOptions(), TimeSpan.FromMinutes(5));

            driver.Navigate().GoToUrl(BaseUri + "/jira/browse/" + jiraId + "?page=com.atlassian.jira.plugin.system.issuetabpanels:worklog-tabpanel");
            driver.Wait();

            var loggers = driver.FindElements(By.CssSelector("a[id^='worklogauthor']"));
            var durations = driver.FindElements(By.CssSelector("dd[class='worklog-duration']"));
            var comments = driver.FindElements(By.CssSelector("dd[class='worklog-comment']"));
            var timeLogged = driver.FindElements(By.CssSelector("span[class='date']"));

            int i = 0;
            var logs = new List<WorkLog>();
            foreach (var logger in loggers)
            {
                if (assignees.Contains(logger.GetAttribute("rel").Trim()))
                {
                    logs.Add(new WorkLog()
                    {
                        JiraId = jiraId,
                        DisplayName = logger.Text,
                        JiraName = logger.GetAttribute("rel"),
                        LogTime = durations[i].Text,
                        LogTimeInHours = ConvertDayToHour(durations[i].Text),
                        Comment = comments[i].Text,
                        DateTimeLogged = DateTime.Parse(timeLogged[i].Text)
                    });
                }
                i++;
            }

            return logs;
        }

        public static double ConvertDayToHour(string logtime)
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

        private static void Login(ChromeDriver driver)
        {
            IWebElement userName = driver.FindElement(By.Id("login-form-username"));
            userName.SendKeys("fzhong");

            IWebElement password = driver.FindElement(By.Id("login-form-password"));
            password.SendKeys("Passwor01");

            driver.FindElement(By.Id("login-form-submit")).Click();

            driver.Wait();
        }
    }
}
