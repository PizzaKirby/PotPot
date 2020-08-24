using Terraria;
using Terraria.ModLoader;

namespace PotPot.Buffs
{
    public class PotPotDummyBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("dummy");
            Description.SetDefault(" ");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
        }
    }
}
