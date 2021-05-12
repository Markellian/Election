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
            Blocks = new List<Block>();
            var genesisBlock = new Block(election);

            Blocks.Add(genesisBlock);
            Last = genesisBlock;
        }

        /// <summary>
        /// Добавление блока в цепочку
        /// </summary>
        /// <param name="user">Пользователь, который голосует</param>
        /// <param name="candidate">Кандидат, за которого голосуют</param>
        public void Add(User user, Candidate candidate)
        {
            if (user == null)
            {
                throw new ArgumentException(Properties.Language.Invalid_user);
            }
            if (candidate == null)
            {
                throw new ArgumentException(Properties.Language.Invalid_condidate);
            }
            Block newBlock = new Block(user, candidate, Last);
            Blocks.Add(newBlock);
            Last = newBlock;
        }         
    }
}
