using Microsoft.VisualStudio.TestTools.UnitTesting;
using Выборы.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes.Tests
{
    [TestClass()]
    public class ChainTests
    {
        [TestMethod()]
        public void ChainConstructorTest()
        {
            Chain chain = new Chain(new Election("Выборы на пост главного дворника", DateTime.Parse("10.10.2019 20:00:00"), DateTime.Parse("10.12.2019 20:00:00")));
            Assert.IsNotNull(chain);
        }
        [TestMethod()]
        public void ChainAddTest()
        {
            Chain chain = new Chain(new Election("Выборы на пост главного дворника", DateTime.Parse("10.10.2019 20:00:00"), DateTime.Parse("10.12.2019 20:00:00")));
            try
            {
                chain.Add(null, new Candidate());
                Assert.Fail();
            } catch (ArgumentException ex)
            {
                Assert.AreEqual(Properties.Language.Invalid_user, ex.Message);
            } catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                chain.Add(new User(), null);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(Properties.Language.Invalid_condidate, ex.Message);
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            try
            {
                chain.Add(new User(), new Candidate());
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
    }
}