using IL.Terraria.ID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace PotPot.Calamity
{
    public static class CalamityID
    {
        public static int Item(string itemname)
        { 
            return PotPot.Instance.Calamity.ItemType(itemname);
        }

        public static int Buff(string buffname)
        {
            return PotPot.Instance.Calamity.BuffType(buffname);
        }
    }
}
