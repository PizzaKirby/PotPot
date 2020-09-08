using Terraria;
using Terraria.ModLoader;
using PotPot.Players;
using System;
using System.Collections.Generic;
using CalamityMod.Items.Potions;
using Microsoft.VisualBasic;
using PotPot.Utilities;
using Steamworks;
using IL.Terraria.ID;
using Terraria.ID;

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
                       
                        Main.NewText(msg, 111, 111, 111);
                        mod.Logger.Info(msg);

                        j++;
                    }
                    break;

                case "buffs":
                    foreach(int i in modPlayer.Buffs)
                    {
                        string msg = "[" + i + "] ";

                        if (ModContent.GetModItem(i) != null)
                            msg += ModContent.GetModItem(i)?.Name;
                        else
                            msg += Lang.GetItemName(i); 

                        Main.NewText(msg, 111, 111, 111);
                        mod.Logger.Debug(msg);
                    }
                    break;
            }
        }
    }
}
