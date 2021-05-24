using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    public class Candidate: User
    {
        public Candidate(Users user)
        {
            Id = user.Id;
            Passport = user.Passport;
            Name = user.Name;
            First_name = user.First_name;
            Last_name = user.Last_name;
            Email = user.Email;
            Phone = user.Phone;
            Birthday = user.Birthday;
            Role_id = user.Role_id;

        }
    }
}
