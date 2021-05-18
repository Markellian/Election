using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public static string IsLoginValidate(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                return "Введите логин";
            }
            if (login.Length < 6)
            {
                return "Логин должен быть не короче 6 символов";
            }
            if (DataBase.IfExistsUserByLogin(login))
            {
                return "Пользователь с таким логином уже зарегистрирован";
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
                return "Неверный формат email: email@email.email";
            }
            return "";
        }

        public static string IsPhoneValidate(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return "Введите телефон";
            }
            if (!(phone.Length == 11 || phone=="") || !int.TryParse(phone, out int p))
            {
                return "Указан неверный номер телфона";
            }
            return "";
        }

        public static dynamic AddUser(string login, string password, string passportSeries, string passportNumber, string firstName,
                                    string name, string lastName, string Email, string phone, DateTime bith)
        {
            return DataBase.AddUser(login, password, passportSeries+passportNumber, firstName, name, lastName, Email, phone, bith);
        }
    }
}
