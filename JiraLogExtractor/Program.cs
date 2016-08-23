using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JiraLogExtractor.Models;
using JiraLogExtractor.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JiraLogExtractor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigManager();
        
            var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), new ChromeOptions(), TimeSpan.FromMinutes(5));
            driver.Navigate().GoToUrl(config.BaseUri + "/jira/login.jsp");
            driver.Wait();

            var jiraHelper = new JiraHelper();
            jiraHelper.Login(driver);

            var jqlQuery = HttpUtility.UrlEncode("updated >= " + config.Period + " and timespent > 0 and project in" + config.Projects);
            driver.Navigate().GoToUrl(config.BaseUri + "/jira/secure/IssueNavigator!executeAdvanced.jspa?jqlQuery=" + jqlQuery + "&runQuery=true&clear=true");
            driver.Wait();

            var totalCount = int.Parse(driver.FindElementByClassName("results-count-total").Text);

            var lastUpdated = driver.FindElements(By.CssSelector("td[class^='nav updated']")).Select(x => x.Text).ToList();
            var issues = driver.FindElements(By.CssSelector("td[class^='nav issuekey']")).Select(x => x.Text).ToList();

            for (var j = 50; j <= totalCount + 50; j = j + 50)
            {
                driver.Navigate().GoToUrl(config.BaseUri + "/jira/secure/IssueNavigator.jspa?pager/start=" + j);
                driver.Wait();
                lastUpdated.AddRange(driver.FindElements(By.CssSelector("td[class^='nav updated']")).Select(x => x.Text));
                issues.AddRange(driver.FindElements(By.CssSelector("td[class^='nav issuekey']")).Select(x => x.Text));
            }

            var i = 0;
            var jiraItems = new List<JiraItem>();
            foreach (var issue in issues)
            {
                var jiraItem = new JiraItem();
                var date = DateTime.Parse(lastUpdated[i]);
                jiraItem.JiraId = issue;
                jiraItem.LastUpdated = date;
                jiraItem.WorkLog = jiraHelper.GetLog(driver, config.BaseUri, issue, config.Assignees);

                jiraItems.Add(jiraItem);
                i++;
            }

            var logs = jiraItems.SelectMany(x => x.WorkLog).Where(x => x.DateTimeLogged > config.CutoffDate).OrderBy(x=>x.DisplayName);

            var frankLog = logs.Where(x => x.JiraName == "fzhong");
            


        }
    }
}
