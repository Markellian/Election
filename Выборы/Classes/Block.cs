﻿using System;
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
    public class Block: Blocks
    {
        /// <summary>
        /// Конструктор генезис-блока. Создает первый блок.
        /// </summary>
        public Block(Election election)
        {
            User_id = 0;
            DataCreated = election.DateStart.ToUniversalTime();            
            Data = "";
            PreviousHash = election.Name;
            Hash = MakeHash();
        }
        /// <summary>
        /// Создание нового блока
        /// </summary>
        /// <param name="user">Идентификатор пользователя</param>
        /// <param name="candidate">кандидат, за которого отдан голос</param>
        /// <param name="block">последний блок</param>
        public Block(User user, Candidate candidate, Block block)
        {
            if (block == null)
            {
                throw new ArgumentException("Пустой блок", nameof(block));
            }
            User_id = user.Id;
            DataCreated = DateTime.UtcNow;
            Data = candidate.Id.ToString();
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
            text += User_id.ToString();
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
