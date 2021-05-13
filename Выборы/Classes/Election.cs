using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    public class Election: Elections
    {
        public Election(string name, DateTime dateStart, DateTime dateEnd)
        {
            Name = name;
            DateStart = dateStart;
            DateEnd = dateEnd;
        }
        public static Election GetElection(string name)
        {
            return DataBase.GetElection(name);
        }
    }
}
