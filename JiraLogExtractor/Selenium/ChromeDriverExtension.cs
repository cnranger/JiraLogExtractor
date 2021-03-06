﻿using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace JiraLogExtractor.Selenium
{
    public static class ChromeDriverExtension
    {
        public static void Wait(this ChromeDriver driver, int timeOut = 120)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeOut));
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }
    }
}
