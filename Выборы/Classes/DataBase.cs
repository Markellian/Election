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
                    DateCreated = block.DateCreated,
                    Option_id = block.Option_id,
                    Hash = block.Hash,
                    PreviousHash = block.PreviousHash,
                    Election_id = block.Election_id
                };
            
                db.Blocks.Add(blocks);
                var res = db.SaveChanges();
                return res==1;
            }
        }

        public static Election GetElectionByName(string electionName)
        {
            using(var db = new ElectionsDataBase())
            {
                var elec = (from e in db.Elections where e.Name == electionName select e).ToList().FirstOrDefault();
                return elec == null ? null : new Election(elec);
            }
        }

        /// <summary>
        /// Ищет в базе данных опцию с указанным именем
        /// </summary>
        /// <param name="name">название опции</param>
        /// <returns>в случае успеха возвращает Id опции, иначе -1</returns>
        public static PoolOptions IfExistsOptionByName(string name)
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
        
        public static PoolOptions AddPoolOption(string name)
        {
            using (var db = new ElectionsDataBase())
            {
                PoolOptions poolOptions = new PoolOptions() { Name = name };
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
                        Id = users.Id,
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

        public static bool DeleteUser(int id)
        {
            using (var db = new ElectionsDataBase())
            {
                Users user = db.Users.Find(id);
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

        public static List<Users> GetCandidates()
        {
            using (var db = new ElectionsDataBase())
            {
                return (from u in db.Users where u.Role_id == 3 select u).ToList();
            }
        }
        public static Elections AddInterviewWithOptions(string name, DateTime start, DateTime end, string description, List<string> listOptions)
        {
            if (GetElectionByName(name) != null) return null;
            if (listOptions == null || listOptions.Count == 0) return null;
            using (var db = new ElectionsDataBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {


                        Elections elections = new Elections()
                        {
                            Name = name,
                            DateStart = start,
                            DateEnd = end,
                            Description = description,
                            Voiteing_type_id = 1 //Interview
                        };
                        db.Elections.Add(elections);
                        db.SaveChanges();
                        foreach (var optionName in listOptions)
                        {
                            PoolOptions option = DataBase.IfExistsOptionByName(optionName);
                            if (option == null)
                            {
                                option = new PoolOptions() { Name = optionName };
                                db.PoolOptions.Add(option);
                                db.SaveChanges();
                            }

                            db.ElectionOptions.Add(new ElectionOptions() { Election_id = elections.Id, Option_id = option.Id });
                            db.SaveChanges();
                        }
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
    
        public static Elections AddElectionWithCandidates(string name, DateTime start, DateTime end, string description, List<Candidate> candidates)
        {
            if (GetElectionByName(name) != null) return null;
            if (candidates == null || candidates.Count == 0) return null;
            using (var db = new ElectionsDataBase())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Elections elections = new Elections()
                        {
                            Name = name,
                            DateStart = start,
                            DateEnd = end,
                            Description = description,
                            Voiteing_type_id = 2 //Election
                        };
                        db.Elections.Add(elections);
                        db.SaveChanges();
                        foreach (var candidate in candidates)
                        {
                            db.ElectionOptions.Add(new ElectionOptions() { Election_id = elections.Id, Option_id = candidate.Id });
                            db.SaveChanges();
                        }
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
    
        public static List<Elections> GetElections()
        {
            using (var db = new ElectionsDataBase())
            {
                return (from e in db.Elections select e).ToList();
            }
        }

        public static Elections GetElectionById(int Id)
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
    }
}
