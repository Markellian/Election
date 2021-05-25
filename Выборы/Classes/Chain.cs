﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    /// <summary>
    /// Цепочка блоков
    /// </summary>
    public class Chain
    {
        //Цепочка блоков
        public List<Block> Blocks { get; set; }
        //Последний блок
        private Block Last { get; set; }
        //Конструктор для создания цепочки блоков
        public Chain(Election election)
        {
            try
            {
                List<Block> blocks = DataBase.GetChain(election.Id);
                Blocks = new List<Block>(blocks.Count() * 2);

                var genesisBlock = blocks.Find((b) => b.PreviousHash.Trim() == election.Name);
                Blocks.Add(genesisBlock);
                string hash = genesisBlock.Hash;
                do
                {
                    Block block = blocks.Find((b) => b.PreviousHash == hash);
                    if (block != null && block.MakeHash() == block.Hash)
                    {
                        Blocks.Add(block);
                        hash = block.Hash;
                    }
                    else
                    {
                        hash = null;
                    }

                } while (hash != null);
                Last = Blocks.Last();
            }
            catch (NullReferenceException)
            {
                return;
            }
        }

               
        /// <summary>
        /// Добавляет новый блок в цепочку
        /// </summary>
        /// <param name="user">Пользователь, который инициирует добавление</param>
        /// <param name="candidate">Кандидат, за которого отдается голос</param>
        /// <returns>true - добавление прошло успешно. false - ошибка добавления</returns>
        public bool Add(User user, int option_id)
        {
            if (user == null)
            {
                return false;
            }
            Block newBlock = new Block(user, Last, option_id);
            var res = DataBase.AddBlock(newBlock);
            if (res)
            {
                Blocks.Add(newBlock);
                Last = newBlock;
                return true;
            }
            else
            {
                return false;
            }            
        }      
    
    }
}
