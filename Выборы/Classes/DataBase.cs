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
            using(var db = new ElectionsDataBase())
            {
                var elec = (from e in db.Elections where e.Name == electionName select e).ToList().FirstOrDefault();
                return elec == null ? null : new Election(elec);
            }
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

        public static bool IfExistsUserByLogin(string login)
        {
            using (var db = new ElectionsDataBase())
            {
                return db.Users.Where(l => l.Login == login).FirstOrDefault() == null ? false : true;
            }
        }
        public static bool IfExistsUserByPassport(string passport)
        {
            using (var db = new ElectionsDataBase())
            {
                return db.Users.Where(p => p.Passport == passport).FirstOrDefault() == null ? false : true;
            }
        }

        public static dynamic AddUser(string login, string password, string passport, string firstName,
                                    string name, string lastName, string email, string phone, DateTime bith)
        {
            Users users = new Users()
            {
                Login = login, 
                Password = GetHash(password),
                Passport = passport,
                Name = name,
                First_name = firstName,
                Last_name = lastName == "" ? null : lastName,
                Email = email,
                Phone = phone == "" ? null: phone,
                Birthday = bith.ToUniversalTime(),
                Role_id = 2

            };

            using (var db = new ElectionsDataBase())
            {
                db.Users.Add(users);
                try
                {


                    if (db.SaveChanges() == 1)
                    {
                        return new User()
                        {
                            Login = login,
                            Password = password,
                            Passport = passport,
                            Name = name,
                            First_name = firstName,
                            Last_name = lastName,
                            Email = email,
                            Phone = phone,
                            Birthday = bith.ToUniversalTime(),
                            Role_id = 2
                        };
                    }
                    else return null;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException e)
                {
                    return e.Message;
                }
            }
        }

        public static List<User> GetAllUsers()
        {
            List<User> list = new List<User>();
            using (var db = new ElectionsDataBase())
            {
                foreach (var users in db.Users)
                {
                    User user = new User()
                    {
                        Login = users.Login,
                        Passport = users.Passport.Insert(4, " "),
                        Name = users.Name,
                        First_name = users.First_name,
                        Last_name = users.Last_name,
                        Email = users.Email,
                        Phone = users.Phone,
                        Birthday = users.Birthday,
                        Roles = users.Roles
                    };
                    list.Add(user);
                }
            }
            return list;
        }

        public static List<Roles> GetAllRoles()
        {
            List<Roles> list = new List<Roles>();
            using (var db = new ElectionsDataBase())
            {
                foreach (var role in db.Roles)
                {
                    list.Add(role);
                }
                return list;
            }
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
