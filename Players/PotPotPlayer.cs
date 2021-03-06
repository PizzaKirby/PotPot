﻿using CalamityMod;
using CalamityMod.CalPlayer;
using PotPot.Calamity;
using PotPot.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using ThoriumMod;

namespace PotPot.Players
{
    class PotPotPlayer : ModPlayer
    {
        public List<Item> PotPotContent;
        internal List<int> Buffs;
 
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
            ApplyBuffs();
        }

        public override void PlayerDisconnect(Player player)
        {
            PotPot.Instance.MainUI = null;
            Buffs.Clear();
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
            foreach (int i in Buffs)
            {
                if (PotPot.Instance.BuffCallbacks.TryGetValue(i, out Action<PotPotPlayer> action))
                { 
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
            if ( Main.LocalPlayer.GetModPlayer<CalamityPlayer>() != null)
            {
                if( Main.LocalPlayer.GetModPlayer<CalamityPlayer>().godSlayer && !Main.LocalPlayer.GetModPlayer<CalamityPlayer>().godSlayerCooldown)
                {
                    if(Main.LocalPlayer.GetModPlayer<CalamityPlayer>().draconicSurge)
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
                        player.AddBuff(BuffID.PotionSickness,CalamityUtils.SecondsToFrames(potionSicknessTime), true);
                    }
                }
                if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>().silvaSet && Main.LocalPlayer.GetModPlayer<CalamityPlayer>().silvaCountdown > 0)
                {
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>().draconicSurge && !Main.LocalPlayer.GetModPlayer<CalamityPlayer>().draconicSurgeCooldown)
                    {
                        player.AddBuff(PotPot.Instance.Calamity.BuffType("DraconicSurgeCooldown"), CalamityUtils.SecondsToFrames(60f), true);
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

        public void ApplyBuffs()
        {
            Buffs.Clear();
            bool HasMeleeImbue = false;
            bool HasThrowImbue = false;

            foreach (Item i in this.PotPotContent)
            {
                if (i.Name == "")
                    continue;

                if (i.Name.Contains("Campfire"))
                {
                    Buffs.Add(ItemID.Campfire);
                    continue;
                }
                else if (i.buffType == BuffID.WellFed)
                {
                    Buffs.Add(ItemID.Sashimi);
                    continue;
                }
                else if (PotPot.Instance.MutualExclusives["Imbues"].Contains(i.type))
                {
                    if (!HasMeleeImbue)
                        HasMeleeImbue = true;
                    else
                        continue;
                }
                else if (PotPot.Instance.MutualExclusives.ContainsKey("ThrowImbues") && PotPot.Instance.MutualExclusives["ThrowImbues"].Contains(i.type))
                {
                    if (!HasThrowImbue)
                        HasThrowImbue = true;
                    else
                        continue;
                }
                Buffs.Add(i.type);
            }

            if (Buffs.Contains(CalamityID.Item("CadencePotion")))
            {
                if (Buffs.Contains(ItemID.LifeforcePotion))
                    Buffs.Remove(ItemID.LifeforcePotion);
                if (Buffs.Contains(ItemID.RegenerationPotion))
                    Buffs.Remove(ItemID.RegenerationPotion);
            }
            if (Buffs.Contains(CalamityID.Item("ShatteringPotion")))
            {
                if (Buffs.Contains(CalamityID.Item("CrumblingPotion")))
                    Buffs.Remove(CalamityID.Item("CrumblingPotion"));
            }
            if (Buffs.Contains(CalamityID.Item("HolyWrathPotion")))
            {
                if (Buffs.Contains(ItemID.WrathPotion))
                    Buffs.Remove(ItemID.WrathPotion);
            }
            if (Buffs.Contains(CalamityID.Item("ProfanedRagePotion")))
            {
                if (Buffs.Contains(ItemID.RagePotion))
                    Buffs.Remove(ItemID.RagePotion);
            }
        }
    }
}
