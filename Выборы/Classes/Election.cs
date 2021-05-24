using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Выборы.Classes
{
    public class Election: Elections
    {
        public Election(Elections elections)
        {
            Id = elections.Id;
            Name = elections.Name;
            DateStart = elections.DateStart;
            DateEnd = elections.DateEnd;
            Voiteing_type_id = elections.Voiteing_type_id;
            Description = elections.Description;
        }
    }
}
