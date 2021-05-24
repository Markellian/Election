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
        public static User AuthUser(string login, string password)
        {
            return DataBase.GetUser(login, password);
        }

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

        public static string AddElection(string name, DateTime start, DateTime end, string description, List<Candidate> candidates)
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

        public static dynamic AddUser(string login, string password, string passportSeries, string passportNumber, string firstName,
                                    string name, string lastName, string Email, string phone, DateTime bith)
        {
            return DataBase.AddUser(login, password, passportSeries+passportNumber, firstName, name, lastName, Email, phone, bith);
        }
    
        public static List<User> GetAllUsers()
        {
            return DataBase.GetAllUsers();
        }  

        public static List<Roles> GetAllRoles()
        {
            return DataBase.GetAllRoles();
        }

        public static List<Candidate> GetCandidates()
        {
            List<Candidate> list = new List<Candidate>();
            var candidates = DataBase.GetCandidates();
            if (candidates == null || candidates.Count == 0) return null;
            foreach (var candidate in candidates)
            {
                list.Add(new Candidate(candidate));
            }

            return list;
        }
    
        public static List<Election> GetElections()
        {
            List<Election> list = new List<Election>();
            var elections = DataBase.GetElections();
            if (elections == null || elections.Count == 0) return null;
            foreach (var election in elections)
            {
                list.Add(new Election(election));
            }
            list.Sort((x,y) => y.Id.CompareTo(x.Id));
            return list;
        }

        public static bool DeleteUser(User user)
        {
            return DataBase.DeleteUser(user.Id);
        }    
    
        public static bool ChahgeRole(User user, Roles role)
        {
            return DataBase.ChangeRole(user.Id, role.Id);
        }
    
        public static Election GetElectionById(int Id)
        {
            return new Election(DataBase.GetElectionById(Id));
        }
    }
}
