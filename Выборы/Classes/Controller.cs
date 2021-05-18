using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    class Controller
    {
        public static User AuthUser(string login, string password)
        {
            return DataBase.GetUser(login, password);
        }

        public static bool AddElection(string name, DateTime start, DateTime end)
        {
            return DataBase.AddElection(name, start, end);
        }
    }
}
