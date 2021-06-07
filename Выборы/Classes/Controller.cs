using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    public class Controller
    {
        /// <summary>
        /// Авторизировать пользователя
        /// </summary>
        /// <param name="login">логин пользователя</param>
        /// <param name="password">пароль пользователя</param>
        /// <returns>возвращает User в случае успеха авторизации, иначе - null</returns>
        public static User AuthUser(string login, string password)
        {
            return DataBase.GetUser(login, password);
        }
        /// <summary>
        /// Добавить Опрос
        /// </summary>
        /// <param name="name">название</param>
        /// <param name="start">дата начала</param>
        /// <param name="end">дата конца</param>
        /// <param name="description">описание</param>
        /// <param name="options">список опций</param>
        /// <returns>сообщение о результате</returns>
        public static string AddInterview(string name, DateTime start, DateTime end, string description, List<string> options)
        {
            if (DataBase.GetElectionByName(name) != null)
            {
                return Properties.Language.ElectionWithThisNameIsExists;
            }
            if (DataBase.AddInterviewWithOptions(name, start, end, description, options) == null)
            {
                return Properties.Language.FailedToCreateElection;
            }
            else
            {
                return Properties.Language.ElectionСreated;
            }
        }
        /// <summary>
        /// Добавить выборы
        /// </summary>
        /// <param name="name">название</param>
        /// <param name="start">дата начала</param>
        /// <param name="end">дата окончания</param>
        /// <param name="description">описание</param>
        /// <param name="candidates">список кандидатов</param>
        /// <returns></returns>
        public static string AddElection(string name, DateTime start, DateTime end, string description, List<User> candidates)
        {
            if (DataBase.GetElectionByName(name) != null)
            {
                return Properties.Language.ElectionWithThisNameIsExists;
            }
            if (DataBase.AddElectionWithCandidates(name, start, end, description, candidates) == null)
            {
                return Properties.Language.FailedToCreateElection;
            }
            else
            {
                return Properties.Language.ElectionСreated;
            }
        }
        /// <summary>
        /// Проверяет логин на корректность
        /// </summary>
        /// <param name="login">логин</param>
        /// <returns>сообщение об ошибке. пустая строка - успех</returns>
        public static string IsLoginValidate(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                return Properties.Language.EnterLogin;
            }
            if (login.Length < 6)
            {
                return Properties.Language.LoginLengthMustBe;
            }
            if (DataBase.IfExistsUserByLogin(login))
            {
                return Properties.Language.UserWithThisLoginIsRaegistered;
            }            
            return "";
        }
        /// <summary>
        /// Проверяет пароль на корректность
        /// </summary>
        /// <param name="password">пароль</param>
        /// <returns>сообщение об ошибке. пустая строка - успех</returns>
        public static string IsPasswordValidate(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return "Введите пароль";
            }
            if (password.Length < 6)
            {
                return "Пароль должен быть не короче 6 символов";
            }
            return "";
        }
        /// <summary>
        /// Проверяет поспортные данные на корректность
        /// </summary>
        /// <param name="series">серия паспорта</param>
        /// <param name="number">номер паспорта</param>
        /// <returns>сообщение об ошибке. пустая строка - успех</returns>
        public static string IsPassportValidate(string series, string number)
        {
            if (string.IsNullOrEmpty(series) || string.IsNullOrEmpty(number))
            {
                return "Введите серию и номер паспорта";
            }

            if (series.Length != 4 || !int.TryParse(series, out int a))
            {
                return "Некорректная серия паспотра. Серия паспорта состоит из 4 цифр";
            }
            if (number.Length != 6 || !int.TryParse(number, out int b))
            {
                return "Некорректный номер паспотра. Номер паспорта состоит из 6 цифр";
            }
            if (DataBase.IfExistsUserByPassport(series + number))
            {
                return "Пользователь с такими паспортными данными уже зарегистрирован";
            }
            return "";
        }
        /// <summary>
        /// Проверяет почту на корректность
        /// </summary>
        /// <param name="email">почти</param>
        /// <returns>сообщение об ошибке. пустая строка - успех</returns>
        public static string IsEmailValidate(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "Введите email";
            }
            if (!Regex.IsMatch(email, ".+@.+\\..+"))
            {
                return "Неверный формат email";
            }
            return "";
        }
        /// <summary>
        /// Проверяет телефон на корректность
        /// </summary>
        /// <param name="phone">номер телефона</param>
        /// <returns>сообщение об ошибке. пустая строка - успех</returns>
        public static string IsPhoneValidate(string phone)
        {
            if (phone != "")
            {
                if (!(phone.Length == 11 && Int64.TryParse(phone, out long p))) 
                {
                    return "Номер телефона должен состоять из 11 цифр";
                }
            }            
            return "";
        }
        /// <summary>
        /// Зарегистрировать пользователя
        /// </summary>
        /// <param name="login">логин</param>
        /// <param name="password">пароль</param>
        /// <param name="passportSeries">серия паспорта</param>
        /// <param name="passportNumber">номер паспорта</param>
        /// <param name="firstName">фамилия</param>
        /// <param name="name">имя</param>
        /// <param name="lastName">ютчество</param>
        /// <param name="Email">почта</param>
        /// <param name="phone">телефон</param>
        /// <param name="bith">дата рождения</param>
        /// <returns>возвращает User в случае успеха, иначе - null</returns>
        public static User AddUser(string login, string password, string passportSeries, string passportNumber, string firstName,
                                    string name, string lastName, string Email, string phone, DateTime bith)
        {
            return DataBase.AddUser(login, password, passportSeries+passportNumber, firstName, name, lastName, Email, phone, bith);
        }
        /// <summary>
        /// получение всех пользователей
        /// </summary>
        /// <returns>список пользователей</returns>
        public static List<User> GetAllUsers()
        {
            return DataBase.GetAllUsers();
        }  
        /// <summary>
        /// Получение всех ролей
        /// </summary>
        /// <returns>список ролей</returns>
        public static List<Role> GetAllRoles()
        {
            return DataBase.GetAllRoles();
        }
        /// <summary>
        /// Получить всех пользователей с правами кандидатов
        /// </summary>
        /// <returns>список пользователей(только кандидаты)</returns>
        public static List<User> GetCandidates()
        {
            var candidates = DataBase.GetCandidates();
            if (candidates == null || candidates.Count == 0) return null;
            return candidates;
        }
        /// <summary>
        /// Получить все голосования
        /// </summary>
        /// <returns>список голосования</returns>
        public static List<Election> GetElections()
        {
            var elections = DataBase.GetElections();
            if (elections == null || elections.Count == 0) return null;
            elections.Sort((x,y) => y.Id.CompareTo(x.Id));
            return elections;
        }
        /// <summary>
        /// Удалить пользователя
        /// </summary>
        /// <param name="user">удаляемый пользователель</param>
        /// <returns>true - успех, false - неудача</returns>
        public static bool DeleteUser(User user)
        {
            return DataBase.DeleteUser(user.Id);
        }    
        /// <summary>
        /// изменить роль пользователя
        /// </summary>
        /// <param name="user">пользователь, роль которого надо изменить</param>
        /// <param name="role">какую роль установить</param>
        /// <returns>true - успех, false - неудача</returns>
        public static bool ChahgeRole(User user, Role role)
        {
            return DataBase.ChangeRole(user.Id, role.Id);
        }
        /// <summary>
        /// получить голосование по ID
        /// </summary>
        /// <param name="Id">ID голосования</param>
        /// <returns>Election в сулчае успеха, иначе - null</returns>
        public static Election GetElectionById(int Id)
        {
            return DataBase.GetElectionById(Id);
        }
        /// <summary>
        /// Получить опции голосования
        /// </summary>
        /// <param name="election">голосование</param>
        /// <returns>список опций</returns>
        public static List<Option> GetOptions(Election election)
        {
            Chain chain = new Chain(election);
            List<Option> options = new List<Option>();
            if (election.Voting_type_id == 1)
            {
                var list = DataBase.GetOptions(election);
                foreach (var option in list)
                {
                    options.Add(new Option() { Name = option.Name, Voites = 0, Id = option.Id });
                }
            }
            if (election.Voting_type_id == 2)
            {
                var list = DataBase.GetCandidates(election);
                foreach (var candidate in list)
                {
                    options.Add(new Option() { Name = candidate.ToString(), Voites = 0, Id = candidate.Id });
                }
            }

            foreach (Block block in chain.Blocks.Skip(1))
            {
                Option option = options.Find((o) => o.Id == block.Option_id);
                option.Voites++;
            }
            
            return options;
        }
        /// <summary>
        /// Проголосовать. Добавляет в цепочку блоков новый блок.
        /// </summary>
        /// <param name="user">проголосовавший пользователь</param>
        /// <param name="election">олосование</param>
        /// <param name="option">оция, за которую голосуют</param>
        /// <returns>true - успех, false - неудача</returns>
        public static bool Vote(User user, Election election, Option option)
        {
            Chain chain = new Chain(election);
            return chain.Add(user, option.Id);
        }

        /// <summary>
        /// Проверяет, голосовал ли пользователь в голосовании
        /// </summary>
        /// <param name="user">пользователь</param>
        /// <param name="election">голосование</param>
        /// <returns>id опции. в случае неудачи - null</returns>
        public static int? IfUserVoted(User user, Election election)
        {
            return DataBase.IfUserVoted(user, election);
        }
        /// <summary>
        /// Получить голосования, где учавствовал пользователь
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <returns>список голосований</returns>
        public static List<Election> GetElections(int userId)
        {
            var list = DataBase.GetElections(userId);
            if (list != null && list.Count != 0)
            {
                List<Election> elections = new List<Election>();
                foreach (var l in list)
                {
                    if (!elections.Contains(l)) elections.Add(l);
                }
                return elections;
            }
            return null;
                
        }
    }
}
