using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PotPot.Calamity
{
    public sealed class CalamityID
    {
        private static CalamityID instance = null;
        private static readonly object padlock = new object();
        private Dictionary<string, int> Item;
        private Dictionary<string, int> Buff;
        private CalamityID()
        {
        }

        public void InitItems()
        {

        }

        public void InitBuffs()
        {

        }

        public static CalamityID Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CalamityID();
                    }
                    return instance;
                }
            }
        }  
    }
}
