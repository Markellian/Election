using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Выборы.Classes
{
    class DataBase
    {
        public static List<Block> GetChain(int election_id)
        {
            List<Block> blockchain;

            using(var db = new ElectionsDataBase())
            {
                var res = from blocks in db.Blocks where blocks.Election_id == election_id select blocks;
                blockchain = new List<Block>(res.Count() * 2);
                foreach (var block in res)
                {
                    blockchain.Add((Block)block);
                }
            }           

            return blockchain;
        }

        public static bool AddBlock(Blocks block)
        {
            using(var db = new ElectionsDataBase())
            {
                db.Blocks.Add(block);
                var res = db.SaveChanges();
                return res==1;
            }
        }

        public static Election GetElection(string electionName)
        {
            Election election;
            using(var db = new ElectionsDataBase())
            {
                election = (Election)(from e in db.Elections where e.Name == electionName select e);
            }
            return election;
        }
    }
}
