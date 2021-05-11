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
            Chain chain = new Chain(
                new Candidate[]
                    {
                        new Candidate { Name = "Игорь"},
                        new Candidate { Name = "Иван"},
                        new Candidate { Name = "Петр"},
                    },
                "Выборы председателя");
            Assert.IsNotNull(chain);
        }
        [TestMethod()]
        public void ChainAddTest()
        {
            Chain chain = new Chain(new Candidate[] { }, "Выборы председателя");
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