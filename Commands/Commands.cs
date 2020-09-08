using Terraria;
using Terraria.ModLoader;
using PotPot.Players;
using System;
using System.Collections.Generic;
using CalamityMod.Items.Potions;
using Microsoft.VisualBasic;
using PotPot.Utilities;

namespace PotPot.Commands
{
    class Commands : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "PotPot";

        public override string Usage => "/PotPot <Items|Buffs>";

        public override string Description => "Shows ( and prints them to the client.log ) all Item/Buff data that is currently associated with your player set by PotPot.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            PotPotPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PotPotPlayer>();
            switch (args[0].ToLower())
            {
                case "items":
                    int j = 0;
                    foreach (Item i in modPlayer.PotPotContent)
                    {
                        string msg = "[" + j + "] " + i.Name;
                        if(i.Name == "")
                            msg += "<Empty>";
                       
                        Main.NewText(msg, 111, 255, 111);
                        mod.Logger.Info(msg);

                        j++;
                    }
                    break;

                case "buffs":
                    Main.NewText("Vanilla Buffs :");
                    Main.NewText(modPlayer.vb);
                    mod.Logger.Info("Vanilla Buffs :");
                    mod.Logger.Info(modPlayer.vb);

                    if ( PotPot.Instance.Calamity != null )
                    {
                        Main.NewText("Calamity Buffs :");
                        Main.NewText(modPlayer.cb);
                        mod.Logger.Info("Calamity Buffs :");
                        mod.Logger.Info(modPlayer.cb);
                    }
                    break;
            }
        }
    }
}
