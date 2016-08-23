using System;
using System.Configuration;
using Xunit;

namespace JiraLogExtractor.Specifications
{
    public class ConfigManagerSpecs
    {
        [Fact]
        public void SingleProject()
        {
            var config = new ConfigManager();
            
            Assert.Equal("\"Aviva Singapore\",\"Singapore Life\",\"Mercer\",\"Reorient\",\"ANZTK\")", config.Projects);
        }
    }

    public class When_start_of_date_offset_by_one_month
    {
        [Fact]
        public void Should_get_the_first_day_of_the_month_as_cutoff_date()
        {
            ConfigurationManager.AppSettings["Period"] = "startOfMonth";
            ConfigurationManager.AppSettings["PeriodOffset"] = "-1";
            
            var config = new ConfigManager();

            var d = config.CutoffDate;


            Assert.Equal(DateTime.Parse("1 Aug 2016"), d);
        }
    }

    public class When_start_of_date_offset_by_more_than_one_month
    {
        [Fact]
        public void Should_get_the_first_day_of_the_month_as_cutoff_date()
        {
            ConfigurationManager.AppSettings["Period"] = "startOfMonth";
            ConfigurationManager.AppSettings["PeriodOffset"] = "-3";

            var config = new ConfigManager();

            var d = config.CutoffDate;

            Assert.Equal(DateTime.Parse("1 June 2016"), d);
        }
    }

}
