using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Выборы.Classes;

namespace ВыборыTests.Classes
{
    [TestClass]
    public class ElectionTests
    {
        [TestMethod]
        public void GetElectionTest()
        {
            Election election = Election.GetElection("Этого теста не должно существовать ни при каких обстоятельствах");
            Assert.IsNull(election);
        }
    }
}
