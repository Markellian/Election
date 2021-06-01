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
    public class ControllerTests
    {
        [TestMethod()]
        public void IsLoginValidateTest()
        {
            Assert.AreEqual("", Controller.IsLoginValidate("fsdfwefdskac,w"));
            Assert.AreEqual(Properties.Language.UserWithThisLoginIsRaegistered, Controller.IsLoginValidate("nikita"));
            Assert.AreEqual(Properties.Language.EnterLogin, Controller.IsLoginValidate(""));
            Assert.AreEqual(Properties.Language.LoginLengthMustBe, Controller.IsLoginValidate("qwe"));
        }
        [TestMethod()]
        public void IsPasswordValidateTest()
        {
            Assert.AreEqual("", Controller.IsPassportValidate("1234", "123123"));
            Assert.AreNotEqual("", Controller.IsPassportValidate("134", "123123"));
            Assert.AreNotEqual("", Controller.IsPassportValidate("134", "1232"));
            Assert.AreNotEqual("", Controller.IsPassportValidate("1334", "123"));
            Assert.AreNotEqual("", Controller.IsPassportValidate("0000", "0000000"));
        }
    }
}