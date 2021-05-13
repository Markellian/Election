using Microsoft.VisualStudio.TestTools.UnitTesting;
using Выборы.Classes;

namespace Выборы.Tests
{
    [TestClass]
    public class ElectionTests
    {
        [TestMethod]
        public void GetElectionTest()
        {
            Election election = Election.GetElection("Этого_теста!!не^^^^^должно____существовать ни при каких обстоятельствах");
            Assert.IsNull(election);

            //election = DataBase.
        }
    }
    
}
