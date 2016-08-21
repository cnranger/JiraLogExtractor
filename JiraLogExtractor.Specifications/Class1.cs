using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JiraLogExtractor.Specifications
{
    public class Class1
    {
        [Fact]
        public void TestIt()
        {
            var hours = Program.ConvertDayToHour("3d 4h");
            Assert.Equal(hours, 22);
        }

        [Fact]
        public void TestIt2()
        {
            var hours = Program.ConvertDayToHour("0.5d");
            Assert.Equal(hours, 3);
        }

        [Fact]
        public void TestIt3()
        {
            var hours = Program.ConvertDayToHour("0.5d 3h");
            Assert.Equal(hours, 6);
        }

        [Fact]
        public void TestIt4()
        {
            var hours = Program.ConvertDayToHour("0.5 h");
            Assert.Equal(hours, 0.5);
        }

        [Fact]
        public void TestIt5()
        {
            var hours = Program.ConvertDayToHour("0.5 m");
            Assert.Equal(hours, 0);
        }
    }
}
