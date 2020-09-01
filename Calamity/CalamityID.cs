using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

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

        public void AddItem(ModItem item)
        {
            Item.Add(item.Name, PotPot.Instance.Calamity.ItemType(item.Name));
        }

        public void AddBuff(ModBuff buff)
        {
            Buff.Add(buff.Name, PotPot.Instance.Calamity.BuffType(buff.Name));
        }

        public int GetBID(string buffname)
        {
            if (Buff.TryGetValue(buffname, out int retval))
                return retval;
            return -1;
        }

        public int GetIID(string itemname)
        {
            if(Item.TryGetValue(itemname, out int retval))
                return retval;
            return -1;
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
