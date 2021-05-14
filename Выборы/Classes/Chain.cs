using System;
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
        private List<Block> Blocks { get; set; }
        //Последний блок
        private Block Last { get; set; }
        //Конструктор для создания цепочки блоков
        public Chain(Election election)
        {
            Blocks = DataBase.GetChain(election.Id);
            if (Blocks.Count() == 0)
            {
                Blocks.Add(new Block(election));
            }
            Last = Blocks.Last();          
            
        }

               
        /// <summary>
        /// Добавляет новый блок в цепочку
        /// </summary>
        /// <param name="user">Пользователь, который инициирует добавление</param>
        /// <param name="candidate">Кандидат, за которого отдается голос</param>
        /// <returns>true - добавление прошло успешно. false - ошибка добавления</returns>
        public bool Add(User user, Candidate candidate)
        {
            if (user == null || candidate == null)
            {
                return false;
            }
            Block newBlock = new Block(user, candidate, Last);
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
