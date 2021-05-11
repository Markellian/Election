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
        private Block Last { get; set; }
        //Конструктор для создания цепочки блоков
        public Chain(Candidate[] candidates, string nameElection)
        {
            Blocks = new List<Block>();
            var genesisBlock = new Block(candidates, nameElection);

            Blocks.Add(genesisBlock);
            Last = genesisBlock;
        }

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
            Block newBlock = new Block(user.Id, candidate, Last);
            Blocks.Add(newBlock);
            Last = newBlock;
        }         
    }
}
