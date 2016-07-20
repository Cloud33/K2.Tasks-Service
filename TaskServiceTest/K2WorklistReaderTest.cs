using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tasks.Service;
using System.Configuration;

namespace TaskServiceTest
{
    [TestClass]
    public class K2WorklistReaderTest
    {
        private static K2WorklistReader k2 = new K2WorklistReader(ConfigurationManager.ConnectionStrings["K2DB"].ConnectionString);
        [TestMethod]
        public void GetWorklist()
        {
            int countTot = 0;
            var worklist = k2.GetWorklistItems(@"K2:DENALLIX\Bob", null, null, out countTot, "60", null, null, null);
            Assert.IsFalse(countTot > 0);
        }


    }
}
