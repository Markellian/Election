using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    class DataBase
    {
        public static List<Block> GetChain(int election_id)
        {
            List<Block> blockchain;

            using(var db = new ElectionsDataBase())
            {
                var res = from blocks in db.Blocks where blocks.Election_id == election_id select blocks;
                blockchain = new List<Block>(res.Count() * 2);
                foreach (var block in res)
                {
                    blockchain.Add((Block)block);
                }
            }           

            return blockchain;
        }

        public static bool AddBlock(Block block)
        {
            using (var db = new ElectionsDataBase())
            {
                Blocks blocks = new Blocks()
                {
                    User_id = block.User_id,
                    DataCreated = block.DataCreated,
                    Data = block.Data,
                    Hash = block.Hash,
                    PreviousHash = block.PreviousHash,
                    Election_id = block.Election_id
                };
            
                db.Blocks.Add(blocks);
                var res = db.SaveChanges();
                return res==1;
            }
        }

        public static Election GetElection(string electionName)
        {
            Election election;
            using(var db = new ElectionsDataBase())
            {
                var elec = (from e in db.Elections where e.Name == electionName select e).ToList().First();
                election = new Election(elec);
            }
            return election;
        }

        public static bool AddElection(string name, DateTime start, DateTime end)//Election election)
        {
            
            Elections elections = new Elections(){
                Name = name,// election.Name,
                DateStart = start,// election.DateStart,
                DateEnd = end,// election.DateEnd
            };

            using (var db = new ElectionsDataBase())
            {
                try
                {
                    db.Elections.Add(elections);
                    db.SaveChanges();
                    return true;
                }
                catch (InvalidOperationException)
                {
                    return false;
                }
            }
        }

        public static User GetUser(string login, string password)
        {
            User user = null;
            using(var db = new ElectionsDataBase())
            {
                var pas = GetHash(password);
                var res0 = (from u in db.Users where u.Login == login && u.Password == pas  select u).ToList();
                if (res0.Count() != 0)
                {
                    var res = res0.First(); 
                    user = new User()
                    {
                        Id = res.Id,
                        Passport = res.Passport,
                        Name = res.Name,
                        First_name = res.First_name,
                        Last_name = res.Last_name,
                        Email = res.Email,
                        Phone = res.Phone,
                        Birthday = res.Birthday,
                        Role_id = res.Role_id,
                        Login = login
                    };
                }
            }
            return user;
        }

        public static string GetHash(string str)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            byte[] result = new SHA256Managed().ComputeHash(bytes);

            string hashString = string.Empty;
            foreach (byte x in result)
            {
                hashString += String.Format("{0:x2}", x);
            }

            return hashString;
        }
    }
}
