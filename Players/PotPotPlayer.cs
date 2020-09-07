using CalamityMod;
using CalamityMod.Buffs.Cooldowns;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using PotPot.Buffs;
using PotPot.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using PotPot.Calamity;
using TIID = Terraria.ID.ItemID;

namespace PotPot.Players
{
    class PotPotPlayer : ModPlayer
    {
        public List<Item> PotPotContent;
        internal List<int> Buffs;
        internal CalamityPlayer CP;
 
        public PotPotPlayer()
        {
            PotPotContent = new List<Item>();
            Buffs = new List<int>();
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "potpotcontent", PotPotContent.ToList() }
            };
        }

        public override void OnEnterWorld(Player player)
        {
            if (!Main.dedServ)
            { 
                PotPot.Instance.MainUI = new PotPotUI();
                PotPot.Instance.MainUI.Activate();
            }
            if ( PotPot.Instance.Calamity != null )
            {
                GetCP();
            }
            
            ApplyBuffs(player);
        }

        public override void PlayerDisconnect(Player player)
        {
            PotPot.Instance.MainUI = null;
        }

        private void GetCP()
        {
            CP = Main.LocalPlayer.GetModPlayer<CalamityPlayer>();
        }

        public override void Load(TagCompound tag)
        {
            if(tag.ContainsKey("potpotcontent"))
            {
                PotPotContent = tag.GetList<Item>("potpotcontent").ToList();
                PotPotContent.RemoveAll(i => i.type == 3930);
            }
        }

        public override void PostUpdateEquips()
        {
            foreach(int i in Buffs)
            {
                if (PotPot.Instance.BuffCallbacks.TryGetValue(i, out Action<PotPotPlayer> action))
                {
                    mod.Logger.Debug("Invoking action for " + i);
                    mod.Logger.Debug(action.ToString());
                    action.Invoke(this);
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if( PotPot.Instance.Calamity != null)
            {
                bool retVal = true;

                retVal = CalamityPreKill(damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
               
                return retVal;
            }
            //draconic elixir buff
            return true;
        }

        public bool CalamityPreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if ( CP != null)
            {
                if( CP.godSlayer && !CP.godSlayerCooldown)
                {
                    if(CP.draconicSurge)
                    {
                        player.AddBuff(PotPot.Instance.Calamity.BuffType("DraconicSurgeCooldown"), 120, true);
                        int additionalTime = 0;
                        for (int i = 0; i < Player.MaxBuffs; i++)
                        {
                            if (player.buffType[i] == BuffID.PotionSickness)
                            {
                                additionalTime = player.buffTime[i];
                                break;
                            }
                        }
                        float potionSicknessTime = 30.0f + (float)Math.Ceiling((double)additionalTime / 60.0);
                        //float potionSicknessTime = player.pStone ? additionalTime * 0.75f: additionalTime;
                        player.AddBuff(BuffID.PotionSickness,CalamityUtils.SecondsToFrames(potionSicknessTime), true);
                    }
                }
                if (CP.silvaSet && CP.silvaCountdown > 0)
                {
                    if (CP.draconicSurge && !CP.draconicSurgeCooldown)
                    {
                        player.AddBuff(ModContent.BuffType<DraconicSurgeCooldown>(), CalamityUtils.SecondsToFrames(60f), true);
                        int additionalTime = 0;
                        for (int n = 0; n < Player.MaxBuffs; n++)
                        {
                            if (player.buffType[n] == BuffID.PotionSickness)
                            {
                                additionalTime = player.buffTime[n];
                                break;
                            }
                        }
                        float potionSicknessTime = 30f + (float)Math.Ceiling((double)additionalTime / 60.0);
                        player.AddBuff(BuffID.PotionSickness, CalamityUtils.SecondsToFrames(potionSicknessTime), true);
                     }   
                }
            }
            return true;
        }

        public void ApplyBuffs(Player player)
        {
            Buffs.Clear();

            foreach (Item i in this.PotPotContent)
            {
                //TODO check for exclusivity here
                Buffs.Add(i.type);
            }
        }


    }
}
