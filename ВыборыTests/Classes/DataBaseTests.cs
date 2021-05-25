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
    public class DataBaseTests
    {
        [TestMethod()]
        public void IfOptionExistsTest()
        {
            Assert.AreEqual(1, DataBase.IfExistsOptionByName("Тест.0"));
            Assert.AreEqual(-1, DataBase.IfExistsOptionByName(""));
        }

        [TestMethod()]
        public void AddElectionWithOptionsTest()
        {
            List<string> l = new List<string>() {"зеленый", "голубой", "красный" };
            DataBase.AddInterviewWithOptions("любимый цвет3", DateTime.Parse("2/2/18"), DateTime.Parse("2/2/20"), "Выбираем самый любимый цвет. Если нет любимого, то голосуйте за первый вариант", l);
        }
    }
}