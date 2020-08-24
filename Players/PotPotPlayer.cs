﻿using PotPot.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace PotPot.Players
{
    class PotPotPlayer : ModPlayer
    {
        public IList<Item> PotPotContent;
        public PotPotPlayer()
        {
            PotPotContent = new List<Item>();
            //init potList in Load
        }

        public override TagCompound Save()
        {
            //mod.Logger.Debug("SAVING:[" + PotPotContent + "]");
            return new TagCompound
            {
                { "potpotcontent", PotPotContent }
            };
        }

        public override void OnEnterWorld(Player player)
        {
            if (!Main.dedServ)
            { 
                PotPot.Instance.MainUI = new PotPotUI();
                PotPot.Instance.MainUI.Activate();
            }
        }
        public override void Load(TagCompound tag)
        {
            if(tag.ContainsKey("potpotcontent"))
            {
                PotPotContent = tag.GetList<Item>("potpotcontent");
                //mod.Logger.Debug(PotPotContent);
            }
        }

        public override void ResetEffects()
        {
            
        }

        public override void OnRespawn(Player player)
        {
            ApplyBuffs(player); 
        }

        public void ApplyBuffs(Player player)
        {
            foreach (Item i in this.PotPotContent)
            {
                player.AddBuff(i.buffType, i.buffTime);
                #region test
                /*
                switch (i.buffType)
                {
                    case BuffID.Lifeforce:
                        base.player.lifeForce = true;
                        base.player.statLifeMax2 += base.player.statLifeMax / 5 / 20 * 20;
                        break;
                    case BuffID.Dangersense:
                        base.player.dangerSense = true;
                        break;
                    case BuffID.WaterWalking:
                        base.player.waterWalk = true;
                        break;
                }
                */
                #endregion test
            }
        }

    }
}
