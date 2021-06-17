using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    public class DataBase
    {
        /// <summary>
        /// Преверяет на возможность подключения к БД
        /// </summary>
        /// <returns>если null, то ошибок нет. иначе - сообщение об ошибке</returns>
        public static string TryConnect()
        {
            using (var db = new ElectionsDataBase())
            {
                try
                {
                    db.Database.Connection.Open();
                    db.Database.Connection.Close();
                    return null;
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

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
        /// <summary>
        /// Добавить блок
        /// </summary>
        /// <param name="block">блок</param>
        /// <returns>true - успех, false - неудача</returns>
        public static bool AddBlock(Block block)
        {
            using (var db = new ElectionsDataBase())
            {
                db.Blocks.Add(block);
                var res = db.SaveChanges();
                return res==1;
            }
        }
        /// <summary>
        /// Получить голосование по названию
        /// </summary>
        /// <param name="electionName">название голосования</param>
        /// <returns>Election в случае успеха, иначе - null</returns>
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
        /// <summary>
        /// Получить User по логину и паролю
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <returns>User в случае успеха, иначе - null</returns>
        public static User GetUser(string login, string password)
        {
            User user = null;
            using(var db = new ElectionsDataBase())
            {
                try
                {
                    var pas = GetHash(password);
                    var res0 = (from u in db.Users where u.Login == login && u.Password == pas select u).ToList();
                    if (res0.Count() != 0)
                    {
                        user = res0.FirstOrDefault();
                    }
                }
                catch (EntityException)
                {
                    return null;
                }
            }
            return user;
        }
        /// <summary>
        /// Есть ли пользователь с таким логином
        /// </summary>
        /// <param name="login">логин</param>
        /// <returns>true - если существует пользователь с таким логином, иначе - false</returns>
        public static bool IfExistsUserByLogin(string login)
        {
            using (var db = new ElectionsDataBase())
            {
                return db.Users.Where(l => l.Login == login).FirstOrDefault() == null ? false : true;
            }
        }
        /// <summary>
        /// Существует ли пользователь с такими паспортными данными
        /// </summary>
        /// <param name="passport">паспорт</param>
        /// <returns>true - если существует пользователь с таким паспортом, иначе - false</returns>
        public static bool IfExistsUserByPassport(string passport)
        {
            using (var db = new ElectionsDataBase())
            {
                return db.Users.Where(p => p.Passport == passport).FirstOrDefault() == null ? false : true;
            }
        }
        /// <summary>
        /// Добавить пользователя
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <param name="passport">паспорт</param>
        /// <param name="firstName">фамилия</param>
        /// <param name="name">имя</param>
        /// <param name="lastName">отчество</param>
        /// <param name="email"><почта/param>
        /// <param name="phone">телефон</param>
        /// <param name="bith">дата рождения</param>
        /// <returns>User в случае успеха, иначе - null</returns>
        public static User AddUser(string login, string password, string passport, string firstName,
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
                        return users;
                    }
                    else return null;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// получить список всех пользователей
        /// </summary>
        /// <returns>список пользователей</returns>
        public static List<User> GetAllUsers()
        {
            using (var db = new ElectionsDataBase())
            {
                return (from users in db.Users select users).ToList();
            }
        }
        /// <summary>
        /// Получить все роли
        /// </summary>
        /// <returns>список ролей</returns>
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
        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="id">id пользователя</param>
        /// <returns>true - успех, false - неудача</returns>
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
        /// <summary>
        /// Изменить роль пользователя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="roleId">Id роли</param>
        /// <returns>true - успех, false - неудача</returns>
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
        /// <summary>
        /// создать хэш-строку
        /// </summary>
        /// <param name="str">исходная строка</param>
        /// <returns>хэш-строка</returns>
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
        /// <summary>
        /// Получить пользователей с ролью "кандидат"
        /// </summary>
        /// <returns>список пользователей (кандидатов)</returns>
        public static List<User> GetCandidates()
        {
            using (var db = new ElectionsDataBase())
            {
                return (from u in db.Users where u.Role_id == 3 select u).ToList();
            }
        }
        /// <summary>
        /// Добавить опрос с опциями
        /// </summary>
        /// <param name="name">название голосования</param>
        /// <param name="start">дата начала голосования</param>
        /// <param name="end">дата окончания голосования</param>
        /// <param name="description">описание</param>
        /// <param name="listOptions">список опций</param>
        /// <returns>созданное голосование в случае успеха, иначе - null</returns>
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
        /// <summary>
        /// Добавить выборы с опциями
        /// </summary>
        /// <param name="name">название голосования</param>
        /// <param name="start">дата начала голосования</param>
        /// <param name="end">дата окончания голосования</param>
        /// <param name="description">описание голосования</param>
        /// <param name="candidates">список опций</param>
        /// <returns>созданное голосование в случае успеха, иначе - null</returns>
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
        /// <summary>
        /// Получить все голосования. 
        /// </summary>
        /// <returns>список голосований</returns>
        public static List<Election> GetElections()
        {
            using (var db = new ElectionsDataBase())
            {
                try
                {
                    return (from e in db.Elections select e).ToList();
                }
                catch (EntityException)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// получить голосование по ID
        /// </summary>
        /// <param name="Id">id голосования</param>
        /// <returns>Election в случае успеха, иначе - null</returns>
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
        /// <summary>
        /// получить опции опроса
        /// </summary>
        /// <param name="election">голосование</param>
        /// <returns>список опций</returns>
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
        /// <summary>
        /// Получить опции выборов
        /// </summary>
        /// <param name="election">голосование</param>
        /// <returns>список кандидатов(User)</returns>
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
        /// <summary>
        /// голосовал ли пользователь
        /// </summary>
        /// <param name="user">пользователь</param>
        /// <param name="election">голосование</param>
        /// <returns>ID опции, если голосовал. иначе - null</returns>
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
        /// <summary>
        /// получить голосования, в которыз участвовал пользователь
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <returns>список голосований</returns>
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
