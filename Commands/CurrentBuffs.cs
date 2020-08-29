using PotPot.Players;
using Terraria;
using Terraria.ModLoader;
using PotPot.Buffs;

namespace PotPot.Commands
{
    class CurrentBuffs : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "CurrentBuffs";

        public override string Usage => "/CurrentBuffs";

        public override string Description => "Shows all Buffs that are currently associated with your player set by PotPot.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            PotPotPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PotPotPlayer>();

        }
    }
}
