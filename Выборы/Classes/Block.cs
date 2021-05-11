using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Выборы.Classes;

namespace Выборы
{
    /// <summary>
    /// Блок
    /// </summary>
    class Block
    {
        //номер блока
        public int Id { get; private set; }
        //ID пользоателя
        public int User { get; private set; }
        //Дата создания блока
        public DateTime DataCreated { get; private set; }
        //Данные
        public Dictionary<Candidate, int> Data { get; private set; }
        //Хэш блока
        public string Hash { get; private set; }
        //Хэш предыдущего блока
        public string PreviousHash { get; private set; }

        /// <summary>
        /// Конструктор генезис-блока. Создает первый блок.
        /// </summary>
        public Block(Candidate[] candidates, string name)
        {
            Id = 1;
            User = 0;
            DataCreated = DateTime.UtcNow;
            Data = new Dictionary<Candidate, int> { };
            foreach (Candidate candidate in candidates)
            {
                Data.Add(candidate, 0);
            }
            PreviousHash = name;
            Hash = MakeHash();
        }
        /// <summary>
        /// Создание нового блока
        /// </summary>
        /// <param name="user">Идентификатор пользователя</param>
        /// <param name="candidate">кандидат, за которого отдан голос</param>
        /// <param name="block">последний блок</param>
        public Block(int user, Candidate candidate, Block block)
        {
            if (block == null)
            {
                throw new ArgumentException("Пустой блок", nameof(block));
            }

            Id = block.Id + 1;
            User = user;
            DataCreated = DateTime.UtcNow;
            Data = new Dictionary<Candidate, int> { };
            foreach (var data in block.Data)
            {
                Data.Add(data.Key, data.Key == candidate ? data.Value+1 : data.Value);
            }
            PreviousHash = block.Hash;
            Hash = MakeHash();

        }
        /// <summary>
        /// Генерация строки для создания хэша
        /// </summary>
        /// <returns>строка для создания хэша</returns>
        private string GetStringForHash()
        {
            string text = "";
            text += Id.ToString();
            text += User.ToString();
            text += DataCreated.ToString("O");
            text += Data.ToString();
            text += PreviousHash;
            return text;
        }
        /// <summary>
        /// Создание хэша для текущего блока
        /// </summary>
        /// <returns></returns>
        public string MakeHash()
        {
            string hash = GetStringForHash();

            byte[] bytes = Encoding.Unicode.GetBytes(hash);
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
