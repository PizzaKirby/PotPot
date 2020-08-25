using PotPot.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using PotPot.Buffs;
using Terraria.ID;
using PotPot;
using CalamityMod.CalPlayer;
using System.Numerics;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace PotPot.Players
{
    class PotPotPlayer : ModPlayer
    {
        public IList<Item> PotPotContent;
        public VanillaBuffs vb = VanillaBuffs.None;
        public CalamityBuffs cpb = CalamityBuffs.None;
        internal CalamityPlayer CP;
        public PotPotPlayer()
        {
            PotPotContent = new List<Item>();
            vb = VanillaBuffs.WellFed | VanillaBuffs.Ironskin | VanillaBuffs.CozyFire | VanillaBuffs.Bewitched | VanillaBuffs.Inferno;
            CP = PotPot.Instance.RefCalamityPlayer;
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

        public override void OnRespawn(Player player)
        {
            ApplyBuffs(player); 
        }

        public override void PostUpdateEquips()
        {
            Player player = base.player;
            Mod calamityMod = ModLoader.GetMod("CalamityMod");
            //CalamityPlayer calamityPlayer = Main.LocalPlayer.GetModPlayer<CalamityPlayer>();

            if ((vb & VanillaBuffs.AmmoReservation ) != 0)
            {
                player.ammoPotion = true;
                player.buffImmune[BuffID.AmmoReservation] = true;
            }
            if ((vb & VanillaBuffs.Archery) != 0)
            {
                player.archery = true;
                player.buffImmune[BuffID.Archery] = true;
            }
            if ((vb & VanillaBuffs.Battle) != 0)
            {
                player.enemySpawns = true;
                player.buffImmune[BuffID.Battle] = true;
            }
            if ((vb & VanillaBuffs.Builder) != 0)
            {
                player.tileSpeed += 0.25f;
                player.wallSpeed += 0.25f;
                player.blockRange++;
                player.buffImmune[BuffID.Builder] = true;
            }
            if ((vb & VanillaBuffs.Calming) != 0)
            {
                player.calmed = true;
                player.buffImmune[BuffID.Calm] = true;
            }
            if ((vb & VanillaBuffs.Crate) != 0)
            {
                player.cratePotion = true;
                player.buffImmune[BuffID.Crate] = true;
            }
            if ((vb & VanillaBuffs.Dangersense) != 0)
            {
                player.dangerSense = true;
                player.buffImmune[BuffID.Dangersense] = true;
            }
            if ((vb & VanillaBuffs.Endurance) != 0)
            {
                player.endurance += 0.1f;
                player.buffImmune[BuffID.Endurance] = true;
            }
            if ((vb & VanillaBuffs.Featherfall) != 0)
            {
                player.slowFall = true;
                player.buffImmune[BuffID.Featherfall] = true;
            }
            if ((vb & VanillaBuffs.Fishing) != 0)
            {
                player.fishingSkill += 15;
                player.buffImmune[BuffID.Fishing] = true;
            }
            if ((vb & VanillaBuffs.Flipper) != 0)
            {
                player.ignoreWater = true;
                player.accFlipper = true;
                player.buffImmune[BuffID.Flipper] = true;
            }
            if ((vb & VanillaBuffs.Gills) != 0)
            {
                player.gills = true;
                player.buffImmune[BuffID.Gills] = true;
            }
            if ((vb & VanillaBuffs.Gravitation) != 0)
            {
                player.gravControl = true;
                player.buffImmune[BuffID.Gravitation] = true;
            }
            if ((vb & VanillaBuffs.Heartreach) != 0)
            {
                player.lifeMagnet = true;
                player.buffImmune[BuffID.Heartreach] = true;
            }
            if ((vb & VanillaBuffs.Hunter) != 0)
            {
                player.detectCreature = true;
                player.buffImmune[BuffID.Hunter] = true;
            }
            if ((vb & VanillaBuffs.Inferno) != 0)
            {
                player.inferno = true;
                Lighting.AddLight((int)player.Center.X >> 4, (int)player.Center.Y >> 4, 0.65f, 0.4f, 0.1f);
                float num = 40000f;
                bool flag = player.infernoCounter % 60 == 0;
                int damage = 10;
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        NPC nPC = Main.npc[i];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[24] && Vector2.DistanceSquared(player.Center, nPC.Center) <= num)
                        {
                            if (nPC.FindBuffIndex(BuffID.OnFire) == -1)
                            {
                                nPC.AddBuff(BuffID.OnFire, 120);
                            }
                            if (flag)
                            {
                               player.ApplyDamageToNPC(nPC, damage, 0f, 0, crit: false);
                            }
                        }
                    }
                }
                //pvp
                if (Main.netMode != 0 && base.player.hostile)
                {
                    for (int j = 0; j < 255; j++)
                    {
                        Player target = Main.player[j];
                        if (target != base.player && target.active && !target.dead && target.hostile && !target.buffImmune[BuffID.OnFire] && (target.team != player.team || target.team == 0) && Vector2.DistanceSquared(player.Center, target.Center) <= num)
                        {
                            if (target.FindBuffIndex(BuffID.OnFire) == -1)
                            {
                                target.AddBuff(BuffID.OnFire, 120);
                            }
                            if (flag)
                            {
                                target.Hurt(PlayerDeathReason.LegacyEmpty(), damage, 0, pvp: true);
                                PlayerDeathReason reason = PlayerDeathReason.ByPlayer(player.whoAmI);
                                NetMessage.SendPlayerHurt(j, reason, damage, 0, critical: false, pvp: true, 0);
                            }
                        }
                    }
                }
                base.player.buffImmune[BuffID.Inferno] = true;
            }
            if ((vb & VanillaBuffs.Invisibility) != 0)
            {
                player.invis = true;
                player.buffImmune[BuffID.Invisibility] = true;
            }
            if ((vb & VanillaBuffs.Ironskin) != 0)
            {
                player.statDefense += 8;
                player.buffImmune[BuffID.Ironskin] = true;
            }
            if ((vb & VanillaBuffs.Lifeforce) != 0)
            {
                player.lifeForce = true;
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 20;
                player.buffImmune[BuffID.Lifeforce] = true;
            }
            if ((vb & VanillaBuffs.Love) != 0)
            {
                player.buffImmune[BuffID.Lovestruck] = true;
            }
            if ((vb & VanillaBuffs.MagicPower) != 0)
            {
                player.magicDamage += 0.2f;
                player.buffImmune[BuffID.MagicPower] = true;
            }
            if ((vb & VanillaBuffs.ManaRegen) != 0)
            {
                player.manaRegenBuff = true;
                player.buffImmune[BuffID.ManaRegeneration] = true;
            }
            if ((vb & VanillaBuffs.Mining) != 0)
            {
                player.pickSpeed -= 0.25f;
                player.buffImmune[BuffID.Mining] = true;
            }
            if ((vb & VanillaBuffs.NightOwl) != 0)
            {
                player.nightVision = true;
                player.buffImmune[BuffID.NightOwl] = true;
            }
            if ((vb & VanillaBuffs.ObsidianSkin) != 0)
            {
                player.lavaImmune = true;
                player.fireWalk = true;
                player.buffImmune[24] = true;
                player.buffImmune[1] = true;
                player.buffImmune[BuffID.ObsidianSkin] = true;
            }
            if ((vb & VanillaBuffs.Rage) != 0)
            {
                //calamity rogue crit
                player.meleeCrit += 10;
                player.thrownCrit += 10;
                player.magicCrit += 10;
                player.rangedCrit += 10;
                if (calamityMod != null)
                {
                    CP.xRage = true;
                }
                player.buffImmune[BuffID.Rage] = true;
            }
            if ((vb & VanillaBuffs.Regeneration) != 0)
            {
                player.lifeRegen += 4;
                player.buffImmune[BuffID.Regeneration] = true;
            }
            if ((vb & VanillaBuffs.Shine) != 0)
            {
                Lighting.AddLight((int)base.player.Center.X >> 4, (int)base.player.Center.Y >> 4, 0.8f, 0.95f, 1f);
                player.buffImmune[BuffID.Shine] = true;
            }
            if ((vb & VanillaBuffs.Sonar) != 0)
            {
                player.sonarPotion = true;
                player.buffImmune[BuffID.Sonar] = true;
            }
            if ((vb & VanillaBuffs.Spelunker) != 0)
            {
                player.findTreasure = true;
                player.buffImmune[BuffID.Spelunker] = true;
            }
            if ((vb & VanillaBuffs.Stink) != 0)
            {
                player.stinky = true;
                player.buffImmune[BuffID.Stinky] = true;
            }
            if ((vb & VanillaBuffs.Summoning) != 0)
            {
                player.maxMinions++;
                player.buffImmune[BuffID.Summoning] = true;
            }
            if ((vb & VanillaBuffs.Swiftness) != 0)
            {
                player.moveSpeed += 0.25f;
                player.buffImmune[BuffID.Swiftness] = true;
            }
            if ((vb & VanillaBuffs.Thorns) != 0)
            {
                player.thorns += 0.33f;
                player.buffImmune[BuffID.Thorns] = true;
            }
            if ((vb & VanillaBuffs.Titan) != 0)
            {
                player.kbBuff = true;
                player.buffImmune[BuffID.Titan] = true;
            }
            if ((vb & VanillaBuffs.Warmth) != 0)
            {
                player.resistCold = true;
                player.buffImmune[BuffID.Warmth] = true;
            }
            if ((vb & VanillaBuffs.WaterWalking) != 0)
            {
                player.waterWalk = true;
                player.buffImmune[BuffID.WaterWalking] = true;
            }
            if ((vb & VanillaBuffs.Wrath) != 0)
            {
                player.meleeDamage += 0.1f;
                player.thrownDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.minionDamage += 0.1f;

                if ( calamityMod != null )
                {
                   CP.xWrath = true;  
                }
                player.buffImmune[BuffID.Wrath] = true;
            }
            if ((vb & VanillaBuffs.WellFed) !=0)
            {
                player.wellFed = true;
                player.statDefense += 2;
                player.meleeCrit += 2;
                player.meleeDamage += 0.05f;
                player.meleeSpeed += 0.05f;
                player.magicCrit += 2;
                player.magicDamage += 0.05f;
                player.rangedCrit += 2;
                player.rangedDamage += 0.05f;
                player.thrownCrit += 2;
                player.thrownDamage += 0.05f;
                player.minionDamage += 0.05f;
                player.minionKB += 0.5f;
                player.moveSpeed += 0.2f;
                player.buffImmune[26] = true;
            }
            if ((vb & VanillaBuffs.Tipsy) != 0)
            {
                Main.NewText("tipsy");
                player.statDefense -= 4;
                player.meleeDamage += 0.1f;
                player.meleeCrit += 2;
                player.meleeSpeed += 0.1f;
                player.buffImmune[BuffID.Tipsy] = true;
            }
            if ((vb & VanillaBuffs.AmmoBox) != 0)
            {
                player.ammoBox = true;
                player.buffImmune[BuffID.AmmoBox] = true;
            }
            if ((vb & VanillaBuffs.Bewitched) != 0)
            {
                player.maxMinions++;
                player.buffImmune[BuffID.Bewitched] = true;
            }
            if ((vb & VanillaBuffs.Claivoyance) != 0)
            {
                player.magicDamage += 0.05f;
                player.magicCrit += 2;
                player.statManaMax2 += 20;
                player.manaCost -= 0.02f;
                player.buffImmune[BuffID.Clairvoyance] = true;
            }
            if ((vb & VanillaBuffs.Sharpened) != 0)
            {
                if (player.inventory[player.selectedItem].melee)
                {
                    player.armorPenetration += 4;
                }
                player.buffImmune[159] = true;
            }
            if ((vb & VanillaBuffs.CozyFire) != 0)
            {
                if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.campfire = true;
                }
                player.buffImmune[BuffID.Campfire] = true;
            }
            if ((vb & VanillaBuffs.HeartLamp) !=0)
            {
                if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.heartLantern = true;
                }
                player.buffImmune[BuffID.HeartLamp] = true;
            }
            if ((vb & VanillaBuffs.Honey) !=0 )
            {
                player.honey = true;
                player.buffImmune[BuffID.Honey] = true;
            }
            if ((vb & VanillaBuffs.PeaceCandle) != 0)
            {
                player.ZonePeaceCandle = true;
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.peaceCandles = 0;
                }
                player.buffImmune[BuffID.PeaceCandle] = true;
            }
            if ((vb & VanillaBuffs.Star) != 0)
            {
                if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.starInBottle = false;
                }
                player.manaRegenBonus += 2;
                player.buffImmune[BuffID.StarInBottle] = true;
            }
        }
        public void ApplyBuffs(Player player)
        {
            for ( int i = 0; i < 40; i++)
            {
                mod.Logger.Debug("[" + i + "]" + (1L << i));
            }
            foreach (Item i in this.PotPotContent)
            {
                switch(i.buffType)
                {
                    case BuffID.Inferno:
                        player.AddBuff(i.buffType, int.MaxValue);
                        break;
                    // set flags
                }
            }
        }
    }
}
