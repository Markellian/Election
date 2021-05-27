using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    public class DataBase
    {
        /// <summary>
        /// Получение цепочки блоков для определенного голосования
        /// </summary>
        /// <param name="election_id">ID голосования</param>
        /// <returns>Цепочку блоков</returns>
        public static List<Block> GetChain(int election_id)
        {
            using(var db = new ElectionsDataBase())
            {
                return (from blocks in db.Blocks where blocks.Election_id == election_id select blocks).ToList();
            }           
        }

        public static bool AddBlock(Block block)
        {
            using (var db = new ElectionsDataBase())
            {
                db.Blocks.Add(block);
                var res = db.SaveChanges();
                return res==1;
            }
        }

        public static Election GetElectionByName(string electionName)
        {
            using(var db = new ElectionsDataBase())
            {
                return (from e in db.Elections where e.Name == electionName select e).ToList().FirstOrDefault();                
            }
        }

        /// <summary>
        /// Ищет в базе данных опцию с указанным именем
        /// </summary>
        /// <param name="name">название опции</param>
        /// <returns>в случае успеха возвращает Id опции, иначе -1</returns>
        public static PoolOption IfExistsOptionByName(string name)
        {
            using (var db = new ElectionsDataBase())
            {
                var options = from o in db.PoolOptions where o.Name == name select o;
                if (options != null)
                {
                    var option = options.ToList().FirstOrDefault();
                    if (option != null) return option;                    
                }
                return null;
            }
        }
        
        public static PoolOption AddPoolOption(string name)
        {
            using (var db = new ElectionsDataBase())
            {
                PoolOption poolOptions = new PoolOption() { Name = name };
                try
                {
                    db.PoolOptions.Add(poolOptions);
                    db.SaveChanges();
                    return poolOptions;
                }
                catch (InvalidOperationException)
                {
                    return null;
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
            User users = new User()
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
            using (var db = new ElectionsDataBase())
            {
                return (from users in db.Users select users).ToList();
            }
        }

        public static List<Role> GetAllRoles()
        {
            List<Role> list = new List<Role>();
            using (var db = new ElectionsDataBase())
            {
                foreach (var role in db.Roles)
                {
                    list.Add(role);
                }
                return list;
            }
        }

        public static bool DeleteUser(int id)
        {
            using (var db = new ElectionsDataBase())
            {
                User user = db.Users.Find(id);
                if (user == null) return false;
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
        }

        public static bool ChangeRole(int userId, int roleId)
        {
            using (var db = new ElectionsDataBase())
            {
                var user = db.Users.Find(userId);
                var role = db.Roles.Find(roleId);
                if (user == null || role == null) return false; //проверка на существование записей
                user.Role_id = roleId;
                db.SaveChanges();
                return true;
            }
        }

        private static string GetHash(string str)
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

        public static List<User> GetCandidates()
        {
            using (var db = new ElectionsDataBase())
            {
                return (from u in db.Users where u.Role_id == 3 select u).ToList();
            }
        }
        public static Election AddInterviewWithOptions(string name, DateTime start, DateTime end, string description, List<string> listOptions)
        {
            if (GetElectionByName(name) != null) return null;
            if (listOptions == null || listOptions.Count == 0) return null;
            using (var db = new ElectionsDataBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Election elections = new Election()
                        {
                            Name = name,
                            DateStart = start,
                            DateEnd = end,
                            Description = description,
                            Voting_type_id = 1 //Interview
                        };
                        db.Elections.Add(elections);
                        db.SaveChanges();
                        foreach (var optionName in listOptions)
                        {
                            PoolOption option = DataBase.IfExistsOptionByName(optionName);
                            if (option == null)
                            {
                                option = new PoolOption() { Name = optionName };
                                db.PoolOptions.Add(option);
                                db.SaveChanges();
                            }

                            db.ElectionOptions.Add(new ElectionOption() { Election_id = elections.Id, Option_id = option.Id });
                            db.SaveChanges();
                        }
                        db.SaveChanges();
                        db.Blocks.Add(new Block(elections));

                        db.SaveChanges();

                        transaction.Commit();
                        return elections;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
            }
            
        }
    
        public static Election AddElectionWithCandidates(string name, DateTime start, DateTime end, string description, List<User> candidates)
        {
            if (GetElectionByName(name) != null) return null;
            if (candidates == null || candidates.Count == 0) return null;
            using (var db = new ElectionsDataBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Election elections = new Election()
                        {
                            Name = name,
                            DateStart = start,
                            DateEnd = end,
                            Description = description,
                            Voting_type_id = 2 //Election
                        };
                        db.Elections.Add(elections);
                        db.SaveChanges();
                        foreach (var candidate in candidates)
                        {
                            db.ElectionOptions.Add(new ElectionOption() { Election_id = elections.Id, Option_id = candidate.Id });
                            db.SaveChanges();
                        }
                        db.SaveChanges();
                        db.Blocks.Add(new Block(elections));
                        db.SaveChanges();
                        transaction.Commit();
                        return elections;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
            }
        }
    
        public static List<Election> GetElections()
        {
            using (var db = new ElectionsDataBase())
            {
                return (from e in db.Elections select e).ToList();
            }
        }

        public static Election GetElectionById(int Id)
        {
            using (var db = new ElectionsDataBase())
            {
                var elections = (from e in db.Elections where e.Id == Id select e).ToList();
                if (elections != null && elections.Count != 0)
                {
                    return elections.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }

        public static List<PoolOption> GetOptions(Election election)
        {
            using (var db = new ElectionsDataBase())
            {
                var res = from po in db.PoolOptions
                          join eo in db.ElectionOptions on po.Id equals eo.Option_id
                          where eo.Election_id == election.Id
                          select po;
                return res.ToList();
            }
        }

        public static List<User> GetCandidates(Election election)
        {
            using (var db = new ElectionsDataBase())
            {
                var res = from u in db.Users
                          join eo in db.ElectionOptions on u.Id equals eo.Option_id
                          where eo.Election_id == election.Id
                          select u;
                return res.ToList(); 
            }
        }

        public static int? IfUserVoted(User user, Election election)
        {
            using (var db = new ElectionsDataBase())
            {
                var res = from b in db.Blocks
                          where b.Election_id == election.Id &&
                                b.User_id == user.Id
                          select b.Option_id;
                return res.FirstOrDefault();
            }
        }

        public static List<Election> GetElections(int userId)
        {
            using (var db = new ElectionsDataBase())
            {
                return (from e in db.Elections
                       join b in db.Blocks on e.Id equals b.Election_id
                       where b.User_id == userId
                       select e).ToList();
            }
        }
    }
}
