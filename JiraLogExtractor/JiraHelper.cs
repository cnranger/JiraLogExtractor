using System;
using System.Collections.Generic;
using JiraLogExtractor.Models;
using JiraLogExtractor.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace JiraLogExtractor
{
    public class JiraHelper
    {
        public void Login(ChromeDriver driver)
        {
            IWebElement userName = driver.FindElement(By.Id("login-form-username"));
            userName.SendKeys("fzhong");

            IWebElement password = driver.FindElement(By.Id("login-form-password"));
            password.SendKeys("Passwor01");

            driver.FindElement(By.Id("login-form-submit")).Click();

            driver.Wait();
        }

        public List<WorkLog> GetLog(ChromeDriver driver, string baseUri, string jiraId, List<string> assignees)
        {
            //var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), new ChromeOptions(), TimeSpan.FromMinutes(5));

            driver.Navigate().GoToUrl(baseUri + "/jira/browse/" + jiraId + "?page=com.atlassian.jira.plugin.system.issuetabpanels:worklog-tabpanel");
            driver.Wait();

            var loggers = driver.FindElements(By.CssSelector("a[id^='worklogauthor']"));
            var durations = driver.FindElements(By.CssSelector("dd[class='worklog-duration']"));
            var comments = driver.FindElements(By.CssSelector("dd[class='worklog-comment']"));
            var timeLogged = driver.FindElements(By.CssSelector("span[class='date']"));

            int i = 0;
            var logs = new List<WorkLog>();
            var stringParser = new StringParser();
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
                        LogTimeInHours = stringParser.ConvertDayToHour(durations[i].Text),
                        Comment = comments[i].Text,
                        DateTimeLogged = DateTime.Parse(timeLogged[i].Text)
                    });
                }
                i++;
            }

            return logs;
        }

    }
}
