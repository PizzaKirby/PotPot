using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using PotPot.Buffs;
using PotPot.UI;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using CBID = PotPot.Buffs.CalamityBuffID;
using CIID = PotPot.Items.CalamityItemID;
using TIID = Terraria.ID.ItemID;
using CalamityMod;
using CalamityMod.Buffs.Cooldowns;
using System;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.Potions;

namespace PotPot.Players
{
    class PotPotPlayer : ModPlayer
    {
        public List<Item> PotPotContent;
        public VanillaBuffs vb = VanillaBuffs.None;
        public CalamityBuffs cb = CalamityBuffs.None;
        internal CalamityPlayer CP;
 
        public PotPotPlayer()
        {
            PotPotContent = new List<Item>();
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
            PotPot.Instance.MainUI.Deactivate();
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
            Player player = base.player;

            UpdateVanillaPlayer(player);
            
            if ( PotPot.Instance.Calamity != null)
            {
                UpdateCalamityPlayer(player);
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

        public void UpdateVanillaPlayer(Player player)
        {
            if ((vb & VanillaBuffs.AmmoReservation) != VanillaBuffs.None)
            {
                player.ammoPotion = true;
                player.buffImmune[BuffID.AmmoReservation] = true;
            }
            if ((vb & VanillaBuffs.Archery) != VanillaBuffs.None)
            {
                player.archery = true;
                player.buffImmune[BuffID.Archery] = true;
            }
            if ((vb & VanillaBuffs.Battle) != VanillaBuffs.None)
            {
                player.enemySpawns = true;
                player.buffImmune[BuffID.Battle] = true;
            }
            if ((vb & VanillaBuffs.Builder) != VanillaBuffs.None)
            {
                player.tileSpeed += 0.25f;
                player.wallSpeed += 0.25f;
                player.blockRange++;
                player.buffImmune[BuffID.Builder] = true;
            }
            if ((vb & VanillaBuffs.Calming) != VanillaBuffs.None)
            {
                player.calmed = true;
                player.buffImmune[BuffID.Calm] = true;
            }
            if ((vb & VanillaBuffs.Crate) != VanillaBuffs.None)
            {
                player.cratePotion = true;
                player.buffImmune[BuffID.Crate] = true;
            }
            if ((vb & VanillaBuffs.Dangersense) != VanillaBuffs.None)
            {
                player.dangerSense = true;
                player.buffImmune[BuffID.Dangersense] = true;
            }
            if ((vb & VanillaBuffs.Endurance) != VanillaBuffs.None)
            {
                player.endurance += 0.1f;
                player.buffImmune[BuffID.Endurance] = true;
            }
            if ((vb & VanillaBuffs.Featherfall) != VanillaBuffs.None)
            {
                player.slowFall = true;
                player.buffImmune[BuffID.Featherfall] = true;
            }
            if ((vb & VanillaBuffs.Fishing) != VanillaBuffs.None)
            {
                player.fishingSkill += 15;
                player.buffImmune[BuffID.Fishing] = true;
            }
            if ((vb & VanillaBuffs.Flipper) != VanillaBuffs.None)
            {
                player.ignoreWater = true;
                player.accFlipper = true;
                player.buffImmune[BuffID.Flipper] = true;
            }
            if ((vb & VanillaBuffs.Gills) != VanillaBuffs.None)
            {
                player.gills = true;
                player.buffImmune[BuffID.Gills] = true;
            }
            if ((vb & VanillaBuffs.Gravitation) != VanillaBuffs.None)
            {
                player.gravControl = true;
                player.buffImmune[BuffID.Gravitation] = true;
            }
            if ((vb & VanillaBuffs.Heartreach) != VanillaBuffs.None)
            {
                player.lifeMagnet = true;
                player.buffImmune[BuffID.Heartreach] = true;
            }
            if ((vb & VanillaBuffs.Hunter) != VanillaBuffs.None)
            {
                player.detectCreature = true;
                player.buffImmune[BuffID.Hunter] = true;
            }
            if ((vb & VanillaBuffs.Inferno) != VanillaBuffs.None)
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
                if (Main.netMode != NetmodeID.SinglePlayer && base.player.hostile)
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
            if ((vb & VanillaBuffs.Invisibility) != VanillaBuffs.None)
            {
                player.invis = true;
                player.buffImmune[BuffID.Invisibility] = true;
            }
            if ((vb & VanillaBuffs.Ironskin) != VanillaBuffs.None)
            {
                player.statDefense += 8;
                player.buffImmune[BuffID.Ironskin] = true;
            }
            if ((vb & VanillaBuffs.Lifeforce) != VanillaBuffs.None)
            {
                if ((cb & CalamityBuffs.Cadance) == CalamityBuffs.None)
                {
                    player.lifeForce = true;
                    player.statLifeMax2 += player.statLifeMax / 5 / 20 * 20;
                    player.buffImmune[BuffID.Lifeforce] = true;
                }
                else
                    vb &= ~VanillaBuffs.Lifeforce;
            }
            if ((vb & VanillaBuffs.Lovestruck) != VanillaBuffs.None)
            {
                player.buffImmune[BuffID.Lovestruck] = true;
            }
            if ((vb & VanillaBuffs.MagicPower) != VanillaBuffs.None)
            {
                player.magicDamage += 0.2f;
                player.buffImmune[BuffID.MagicPower] = true;
            }
            if ((vb & VanillaBuffs.ManaRegen) != VanillaBuffs.None)
            {
                player.manaRegenBuff = true;
                player.buffImmune[BuffID.ManaRegeneration] = true;
            }
            if ((vb & VanillaBuffs.Mining) != VanillaBuffs.None)
            {
                player.pickSpeed -= 0.25f;
                player.buffImmune[BuffID.Mining] = true;
            }
            if ((vb & VanillaBuffs.NightOwl) != VanillaBuffs.None)
            {
                player.nightVision = true;
                player.buffImmune[BuffID.NightOwl] = true;
            }
            if ((vb & VanillaBuffs.ObsidianSkin) != VanillaBuffs.None)
            {
                player.lavaImmune = true;
                player.fireWalk = true;
                player.buffImmune[24] = true;
                player.buffImmune[1] = true;
                player.buffImmune[BuffID.ObsidianSkin] = true;
            }
            if ((vb & VanillaBuffs.Rage) != VanillaBuffs.None)
            {
                if ((cb & CalamityBuffs.ProfanedRage) == CalamityBuffs.None)
                {
                    player.meleeCrit += 10;
                    player.thrownCrit += 10;
                    player.magicCrit += 10;
                    player.rangedCrit += 10;
                    player.buffImmune[BuffID.Rage] = true;
                }
                else
                    vb &= ~VanillaBuffs.Rage;
            }
            if ((vb & VanillaBuffs.Regeneration) != VanillaBuffs.None)
            {
                if ((cb & CalamityBuffs.Cadance) == CalamityBuffs.None)
                {
                    player.lifeRegen += 4;
                    player.buffImmune[BuffID.Regeneration] = true;
                }
                else
                    vb &= ~VanillaBuffs.Regeneration;
            }
            if ((vb & VanillaBuffs.Shine) != VanillaBuffs.None)
            {
                Lighting.AddLight((int)base.player.Center.X >> 4, (int)base.player.Center.Y >> 4, 0.8f, 0.95f, 1f);
                player.buffImmune[BuffID.Shine] = true;
            }
            if ((vb & VanillaBuffs.Sonar) != VanillaBuffs.None)
            {
                player.sonarPotion = true;
                player.buffImmune[BuffID.Sonar] = true;
            }
            if ((vb & VanillaBuffs.Spelunker) != VanillaBuffs.None)
            {
                player.findTreasure = true;
                player.buffImmune[BuffID.Spelunker] = true;
            }
            if ((vb & VanillaBuffs.Stinky) != VanillaBuffs.None)
            {
                player.stinky = true;
                player.buffImmune[BuffID.Stinky] = true;
            }
            if ((vb & VanillaBuffs.Summoning) != VanillaBuffs.None)
            {
                player.maxMinions++;
                player.buffImmune[BuffID.Summoning] = true;
            }
            if ((vb & VanillaBuffs.Swiftness) != VanillaBuffs.None)
            {
                player.moveSpeed += 0.25f;
                player.buffImmune[BuffID.Swiftness] = true;
            }
            if ((vb & VanillaBuffs.Thorns) != VanillaBuffs.None)
            {
                player.thorns += 0.33f;
                player.buffImmune[BuffID.Thorns] = true;
            }
            if ((vb & VanillaBuffs.Titan) != VanillaBuffs.None)
            {
                player.kbBuff = true;
                player.buffImmune[BuffID.Titan] = true;
            }
            if ((vb & VanillaBuffs.Warmth) != VanillaBuffs.None)
            {
                player.resistCold = true;
                player.buffImmune[BuffID.Warmth] = true;
            }
            if ((vb & VanillaBuffs.WaterWalking) != VanillaBuffs.None)
            {
                player.waterWalk = true;
                player.buffImmune[BuffID.WaterWalking] = true;
            }
            if ((vb & VanillaBuffs.Wrath) != VanillaBuffs.None)
            {
                if ((cb & CalamityBuffs.HolyWrath) == CalamityBuffs.None)
                {
                    player.meleeDamage += 0.1f;
                    player.thrownDamage += 0.1f;
                    player.magicDamage += 0.1f;
                    player.rangedDamage += 0.1f;
                    player.minionDamage += 0.1f;
                    player.buffImmune[BuffID.Wrath] = true;
                }
                else
                    vb &= ~VanillaBuffs.Wrath;
            }
            if ((vb & VanillaBuffs.WellFed) != 0)
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
                player.buffImmune[BuffID.WellFed] = true;
            }
            if ((vb & VanillaBuffs.FlaskIchor) != VanillaBuffs.None)
            {
                player.meleeEnchant = 5;
                player.buffImmune[BuffID.WeaponImbueIchor] = true;
            }
            if ((vb & VanillaBuffs.Tipsy) != VanillaBuffs.None)
            {
                player.statDefense -= 4;
                player.meleeDamage += 0.1f;
                player.meleeCrit += 2;
                player.meleeSpeed += 0.1f;
                player.buffImmune[BuffID.Tipsy] = true;
            }
            if ((vb & VanillaBuffs.AmmoBox) != VanillaBuffs.None)
            {
                player.ammoBox = true;
                player.buffImmune[BuffID.AmmoBox] = true;
            }
            if ((vb & VanillaBuffs.Bewitched) != VanillaBuffs.None)
            {
                player.maxMinions++;
                player.buffImmune[BuffID.Bewitched] = true;
            }
            if ((vb & VanillaBuffs.Clairvoyance) != VanillaBuffs.None)
            {
                player.magicDamage += 0.05f;
                player.magicCrit += 2;
                player.statManaMax2 += 20;
                player.manaCost -= 0.02f;
                player.buffImmune[BuffID.Clairvoyance] = true;
            }
            if ((vb & VanillaBuffs.Sharpened) != VanillaBuffs.None)
            {
                if (player.inventory[player.selectedItem].melee)
                {
                    player.armorPenetration += 4;
                }
                player.buffImmune[159] = true;
            }
            if ((vb & VanillaBuffs.Campfire) != VanillaBuffs.None)
            {
                if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.campfire = true;
                }
                player.buffImmune[BuffID.Campfire] = true;
            }
            if ((vb & VanillaBuffs.HeartLamp) != VanillaBuffs.None)
            {
                if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.heartLantern = true;
                }
                player.buffImmune[BuffID.HeartLamp] = true;
            }
            if ((vb & VanillaBuffs.Honey) != VanillaBuffs.None)
            {
                player.honey = true;
                player.buffImmune[BuffID.Honey] = true;
            }
            if ((vb & VanillaBuffs.PeaceCandle) != VanillaBuffs.None)
            {
                player.ZonePeaceCandle = true;
                if (Main.myPlayer == player.whoAmI)
                {
                    Main.peaceCandles = 0;
                }
                player.buffImmune[BuffID.PeaceCandle] = true;
            }
            if ((vb & VanillaBuffs.StarInBottle) != VanillaBuffs.None)
            {
                if (Main.myPlayer == player.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.starInBottle = false;
                }
                player.manaRegenBonus += 2;
                player.buffImmune[BuffID.StarInBottle] = true;
            }
        }

        public void UpdateCalamityPlayer(Player player)
        {
            if (CP != null)
            {
                #region Potions
                if ((vb & VanillaBuffs.Rage) != VanillaBuffs.None)
                {
                    CP.xRage = true;
                }
                if ((vb & VanillaBuffs.Wrath) != VanillaBuffs.None)
                {
                    CP.xWrath = true;
                }
                if ((cb & CalamityBuffs.AnechoicCoating) != CalamityBuffs.None)
                {
                    CP.anechoicCoating = true;
                    player.buffImmune[(int)CBID.AnechoicCoating] = true;
                }
                if ((cb & CalamityBuffs.AstralInjection) != CalamityBuffs.None)
                {
                    CP.astralInjection = true;
                    player.buffImmune[(int)CBID.AstralInjection] = true;
                }
                if ((cb & CalamityBuffs.Bounding) != CalamityBuffs.None)
                {
                    CP.bounding = true;
                    player.buffImmune[(int)CBID.Bounding] = true;
                }
                if ((cb & CalamityBuffs.CalamitasBrew) != CalamityBuffs.None)
                {
                    CP.aWeapon = true;
                    player.buffImmune[(int)CBID.CalamitasBrew] = true;
                }
                if ((cb & CalamityBuffs.Calcium) != CalamityBuffs.None)
                {
                    CP.calcium = true;
                    player.buffImmune[(int)CBID.Calcium] = true;
                }
                if ((cb & CalamityBuffs.CeaselessHunger) != CalamityBuffs.None)
                {
                    CP.ceaselessHunger = true;
                    player.buffImmune[(int)CBID.CeaselessHunger] = true;
                }
                if ((cb & CalamityBuffs.Crumbling) != CalamityBuffs.None)
                {
                    if ((cb & CalamityBuffs.Shattering) == CalamityBuffs.None)
                    {
                        CP.armorCrumbling = true;
                        player.buffImmune[(int)CBID.Crumbling] = true;
                    }
                    else
                        cb &= ~CalamityBuffs.Crumbling;
                }
                if ((cb & CalamityBuffs.DraconicElixir) != CalamityBuffs.None)
                {
                    if (!CP.draconicSurgeCooldown)
                    {
                        CP.draconicSurge = true;
                        player.buffImmune[(int)CBID.DraconicElixir] = true;
                    }
                }
                if ((cb & CalamityBuffs.GravityNormalizer) != CalamityBuffs.None)
                {
                    CP.gravityNormalizer = true;
                    player.buffImmune[(int)CBID.GravityNormalizer] = true;
                }
                if ((cb & CalamityBuffs.HolyWrath) != CalamityBuffs.None)
                {
                    CP.holyWrath = true;
                    player.buffImmune[(int)CBID.HolyWrath] = true;
                }
                if ((cb & CalamityBuffs.Penumbra) != CalamityBuffs.None)
                {
                    CP.penumbra = true;
                    player.buffImmune[(int)CBID.Penumbra] = true;
                }
                if ((cb & CalamityBuffs.Photosynthesis) != CalamityBuffs.None)
                {
                    CP.photosynthesis = true;
                    player.buffImmune[(int)CBID.Photosynthesis] = true;
                }
                if ((cb & CalamityBuffs.ProfanedRage) != CalamityBuffs.None)
                {
                    CP.profanedRage = true;
                    player.buffImmune[(int)CBID.ProfanedRage] = true;
                }
                if ((cb & CalamityBuffs.Revivify) != CalamityBuffs.None)
                {
                    CP.revivify = true;
                    player.buffImmune[(int)CBID.Revivify] = true;
                }
                if ((cb & CalamityBuffs.Shattering) != CalamityBuffs.None)
                {
                    CP.armorShattering = true;
                    player.buffImmune[(int)CBID.Shattering] = true;
                }
                if ((cb & CalamityBuffs.Shadow) != CalamityBuffs.None)
                {
                    player.invis = true;
                    CP.shadow = true;
                    player.buffImmune[(int)CBID.Shadow] = true;
                }
                if ((cb & CalamityBuffs.Soaring) != CalamityBuffs.None)
                {
                    CP.soaring = true;
                    player.buffImmune[(int)CBID.Soaring] = true;
                }
                if ((cb & CalamityBuffs.Sulphurskin) != CalamityBuffs.None)
                {
                    CP.sulphurskin = true;
                    player.buffImmune[(int)CBID.Sulphurskin] = true;
                }
                if ((cb & CalamityBuffs.Tesla) != CalamityBuffs.None)
                {
                    CP.tesla = true;
                    player.buffImmune[(int)CBID.Tesla] = true;
                }
                if ((cb & CalamityBuffs.TitanScale) != CalamityBuffs.None)
                {
                    CP.tScale = true;
                    player.buffImmune[(int)CBID.TitanScale] = true;
                }
                if ((cb & CalamityBuffs.Triumph) != CalamityBuffs.None)
                {
                    CP.triumph = true;
                    player.buffImmune[(int)CBID.Triumph] = true;
                }
                if ((cb & CalamityBuffs.Zen) != CalamityBuffs.None)
                {
                    CP.zen = true;
                    player.buffImmune[(int)CBID.Zen] = true;
                }
                if ((cb & CalamityBuffs.Zerg) != CalamityBuffs.None)
                {
                    CP.anechoicCoating = true;
                    player.buffImmune[(int)CBID.Zerg] = true;
                }
                if ((cb & CalamityBuffs.Cadance) != CalamityBuffs.None)
                {
                    CP.cadence = true;
                    player.buffImmune[(int)CBID.Cadance] = true;
                }
                if ((cb & CalamityBuffs.Omniscience) != CalamityBuffs.None)
                {
                    CP.omniscience = true;
                    player.buffImmune[(int)CBID.Omniscience] = true;
                }
                if ((cb & CalamityBuffs.YharimsStimulants) != CalamityBuffs.None)
                {
                    CP.yPower = true;
                    player.buffImmune[(int)CBID.YharimsStimulants] = true;
                }
                if ((cb & CalamityBuffs.BloodyMary) != CalamityBuffs.None)
                {
                    CP.bloodyMary = true;
                    player.buffImmune[(int)CBID.BloodyMary] = true;
                }
                if ((cb & CalamityBuffs.CarribeanRum) != CalamityBuffs.None)
                {
                    CP.caribbeanRum = true;
                    player.buffImmune[(int)CBID.CaribbeanRum] = true;
                }
                if ((cb & CalamityBuffs.CinnamonRoll) != CalamityBuffs.None)
                {
                    CP.cinnamonRoll = true;
                    player.buffImmune[(int)CBID.CinnamonRoll] = true;
                }
                if ((cb & CalamityBuffs.Everclear) != CalamityBuffs.None)
                {
                    CP.everclear = true;
                    player.buffImmune[(int)CBID.Everclear] = true;
                }
                if ((cb & CalamityBuffs.EvergreenGin) != CalamityBuffs.None)
                {
                    CP.evergreenGin = true;
                    player.buffImmune[(int)CBID.EvergreenGin] = true;
                }
                if ((cb & CalamityBuffs.FabsolsVodka) != CalamityBuffs.None)
                {
                    CP.fabsolVodka = true;
                    player.buffImmune[(int)CBID.FabsolsVodka] = true;
                }
                if ((cb & CalamityBuffs.Fireball) != CalamityBuffs.None)
                {
                    CP.fireball = true;
                    player.buffImmune[(int)CBID.Fireball] = true;
                }
                if ((cb & CalamityBuffs.Moonshine) != CalamityBuffs.None)
                {
                    CP.moonshine = true;
                    player.buffImmune[(int)CBID.Moonshine] = true;
                }
                if ((cb & CalamityBuffs.MoscowMule) != CalamityBuffs.None)
                {
                    CP.moscowMule = true;
                    player.buffImmune[(int)CBID.MoscowMule] = true;
                }
                if ((cb & CalamityBuffs.Rum) != CalamityBuffs.None)
                {
                    CP.rum = true;
                    player.buffImmune[(int)CBID.Rum] = true;
                }
                if ((cb & CalamityBuffs.Screwdriver) != CalamityBuffs.None)
                {
                    CP.rum = true;
                    player.buffImmune[(int)CBID.Screwdriver] = true;
                }
                if ((cb & CalamityBuffs.StarBeamRye) != CalamityBuffs.None)
                {
                    CP.starBeamRye = true;
                    player.buffImmune[(int)CBID.StarBeamRye] = true;
                }
                if ((cb & CalamityBuffs.Tequila) != CalamityBuffs.None)
                {
                    CP.tequila = true;
                    player.buffImmune[(int)CBID.Tequila] = true;
                }
                if ((cb & CalamityBuffs.TequilaSunrise) != CalamityBuffs.None)
                {
                    CP.tequilaSunrise = true;
                    player.buffImmune[(int)CBID.TequilaSunrise] = true;
                }
                if ((cb & CalamityBuffs.Vodka) != CalamityBuffs.None)
                {
                    CP.vodka = true;
                    player.buffImmune[(int)CBID.Vodka] = true;
                }
                if ((cb & CalamityBuffs.Whiskey) != CalamityBuffs.None)
                {
                    CP.whiskey = true;
                    player.buffImmune[(int)CBID.Whiskey] = true;
                }
                if ((cb & CalamityBuffs.WhiteWine) != CalamityBuffs.None)
                {
                    CP.whiteWine = true;
                    player.buffImmune[(int)CBID.WhiteWine] = true;
                }
                #endregion Potions
                #region LoreItems
                if ((cb & CalamityBuffs.BrimstoneLore) !=  CalamityBuffs.None)
                {
                    if ((CP.brimstoneElementalLore || CP.ataxiaBlaze) && ((vb & VanillaBuffs.Inferno) != VanillaBuffs.None))
                    {
                        //DEBUG
                        Main.NewText("Brimferno");
                        CP.brimLoreInfernoTimer = (CP.brimLoreInfernoTimer + 1) % 30;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            int damage = (int)(50f * CalamityUtils.AverageDamage(player));
                            float range = 300f;
                            for (int l = 0; l < 200; l++)
                            {
                                NPC npc = Main.npc[l];
                                if (npc.active && !npc.friendly && npc.damage > 0 && !npc.dontTakeDamage && Vector2.Distance(player.Center, npc.Center) <= range)
                                {
                                    npc.AddBuff(PotPot.Instance.Calamity.BuffType("BrimstoneFlames"), 120, false);
                                    if (CP.brimLoreInfernoTimer == 0)
                                    {
                                        Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.DirectStrike>(), damage, 0f, player.whoAmI, (float)l, 0f);
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion LoreItems
            }
        }

        public void ApplyBuffs(Player player)
        {
            vb = VanillaBuffs.None;
            cb = CalamityBuffs.None;
            foreach (Item i in this.PotPotContent)
            {
                mod.Logger.Debug("[" + i + "]" + i.buffType);
                switch (i.type)
                {
                    case TIID.AmmoReservationPotion:
                        vb |= VanillaBuffs.AmmoReservation;
                        break;
                    case TIID.ArcheryPotion:
                        vb |= VanillaBuffs.Archery;
                        break;
                    case TIID.BattlePotion:
                        vb |= VanillaBuffs.Battle;
                        break;
                    case TIID.BuilderPotion:
                        vb |= VanillaBuffs.Builder;
                        break;
                    case TIID.CalmingPotion:
                        vb |= VanillaBuffs.Calming;
                        break;
                    case TIID.CratePotion:
                        vb |= VanillaBuffs.Crate;
                        break;
                    case TIID.TrapsightPotion:
                        vb |= VanillaBuffs.Crate;
                        break;
                    case TIID.EndurancePotion:
                        vb |= VanillaBuffs.Endurance;
                        break;
                    case TIID.FeatherfallPotion:
                        vb |= VanillaBuffs.Featherfall;
                        break;
                    case TIID.FishingPotion:
                        vb |= VanillaBuffs.Fishing;
                        break;
                    case TIID.FlipperPotion:
                        vb |= VanillaBuffs.Flipper;
                        break;
                    case TIID.GillsPotion:
                        vb |= VanillaBuffs.Gills;
                        break;
                    case TIID.GravitationPotion:
                        vb |= VanillaBuffs.Gravitation;
                        break;
                    case TIID.HeartreachPotion:
                        vb |= VanillaBuffs.Heartreach;
                        break;
                    case TIID.HunterPotion:
                        vb |= VanillaBuffs.Hunter;
                        break;
                    case TIID.InfernoPotion:
                        vb |= VanillaBuffs.Inferno;
                        break;
                    case TIID.InvisibilityPotion:
                        vb |= VanillaBuffs.Invisibility;
                        break;
                    case TIID.IronskinPotion:
                        vb |= VanillaBuffs.Ironskin;
                        break;
                    case TIID.LifeforcePotion:
                        vb |= VanillaBuffs.Lifeforce;
                        break;
                    case TIID.LovePotion:
                        vb |= VanillaBuffs.Lovestruck;
                        break;
                    case TIID.MagicPowerPotion:
                        vb |= VanillaBuffs.MagicPower;
                        break;
                    case TIID.ManaRegenerationPotion:
                        vb |= VanillaBuffs.ManaRegen;
                        break;
                    case TIID.MiningPotion:
                        vb |= VanillaBuffs.Mining;
                        break;
                    case TIID.NightOwlPotion:
                        vb |= VanillaBuffs.NightOwl;
                        break;
                    case TIID.ObsidianSkinPotion:
                        vb |= VanillaBuffs.ObsidianSkin;
                        break;
                    case TIID.RagePotion:
                        vb |= VanillaBuffs.Rage;
                        break;
                    case TIID.RegenerationPotion:
                        vb |= VanillaBuffs.Regeneration;
                        break;
                    case TIID.ShinePotion:
                        vb |= VanillaBuffs.Shine;
                        break;
                    case TIID.SonarPotion:
                        vb |= VanillaBuffs.Sonar;
                        break;
                    case TIID.SpelunkerPotion:
                        vb |= VanillaBuffs.Spelunker;
                        break;
                    case TIID.StinkPotion:
                        vb |= VanillaBuffs.Stinky;
                        break;
                    case TIID.SummoningPotion:
                        vb |= VanillaBuffs.Summoning;
                        break;
                    case TIID.SwiftnessPotion:
                        vb |= VanillaBuffs.Swiftness;
                        break;
                    case TIID.ThornsPotion:
                        vb |= VanillaBuffs.Thorns;
                        break;
                    case TIID.TitanPotion:
                        vb |= VanillaBuffs.Titan;
                        break;
                    case TIID.WarmthPotion:
                        vb |= VanillaBuffs.Warmth;
                        break;
                    case TIID.WaterWalkingPotion:
                        vb |= VanillaBuffs.WaterWalking;
                        break;
                    case TIID.WrathPotion:
                        vb |= VanillaBuffs.Wrath;
                        break;
                    case TIID.FlaskofCursedFlames:
                        vb |= VanillaBuffs.FlaskCursedFlames;
                        break;
                    case TIID.FlaskofFire:
                        vb |= VanillaBuffs.FlaskFire;
                        break;
                    case TIID.FlaskofGold:
                        vb |= VanillaBuffs.FlaskGold;
                        break;
                    case TIID.FlaskofIchor:
                        vb |= VanillaBuffs.FlaskIchor;
                        break;
                    case TIID.FlaskofNanites:
                        vb |= VanillaBuffs.FlaskNanites;
                        break;
                    case TIID.FlaskofParty:
                        vb |= VanillaBuffs.FlaskParty;
                        break;
                    case TIID.FlaskofPoison:
                        vb |= VanillaBuffs.FlaskPoison;
                        break;
                    case TIID.FlaskofVenom:
                        vb |= VanillaBuffs.FlaskVenom;
                        break;
                    case TIID.AmmoBox:
                        vb |= VanillaBuffs.AmmoBox;
                        break;
                    case TIID.BewitchingTable:
                        vb |= VanillaBuffs.Bewitched;
                        break;
                    case TIID.CrystalBall:
                        vb |= VanillaBuffs.Clairvoyance;
                        break;
                    case TIID.SharpeningStation:
                        vb |= VanillaBuffs.Sharpened;
                        break;
                    case TIID.Campfire:
                        vb |= VanillaBuffs.Campfire;
                        break;
                    case TIID.HeartLantern:
                        vb |= VanillaBuffs.HeartLamp;
                        break;
                    case TIID.BottledHoney:
                        vb |= VanillaBuffs.Honey;
                        break;
                    case TIID.PeaceCandle:
                        vb |= VanillaBuffs.PeaceCandle;
                        break;
                    case TIID.StarinaBottle:
                        vb |= VanillaBuffs.StarInBottle;
                        break;
                    case (int)CIID.BloodyMary:
                        cb |= CalamityBuffs.BloodyMary;
                        break;
                    case (int)CIID.CaribbeanRum:
                        cb |= CalamityBuffs.CarribeanRum;
                        break;
                    case (int)CIID.CinnamonRoll:
                        cb |= CalamityBuffs.CinnamonRoll;
                        break;
                    case (int)CIID.Everclear:
                        cb |= CalamityBuffs.Everclear;
                        break;
                    case (int)CIID.EvergreenGin:
                        cb |= CalamityBuffs.EvergreenGin;
                        break;
                    case (int)CIID.FabsolsVodka:
                        cb |= CalamityBuffs.FabsolsVodka;
                        break;
                    case (int)CIID.Fireball:
                        cb |= CalamityBuffs.Fireball;
                        break;
                    case (int)CIID.Moonshine:
                        cb |= CalamityBuffs.Moonshine;
                        break;
                    case (int)CIID.MoscowMule:
                        cb |= CalamityBuffs.MoscowMule;
                        break;
                    case (int)CIID.Rum:
                        cb |= CalamityBuffs.Rum;
                        break;
                    case (int)CIID.Screwdriver:
                        cb |= CalamityBuffs.Screwdriver;
                        break;
                    case (int)CIID.StarBeamRye:
                        cb |= CalamityBuffs.StarBeamRye;
                        break;
                    case (int)CIID.Tequila:
                        cb |= CalamityBuffs.Tequila;
                        break;
                    case (int)CIID.TequilaSunrise:
                        cb |= CalamityBuffs.TequilaSunrise;
                        break;
                    case (int)CIID.Vodka:
                        cb |= CalamityBuffs.Vodka;
                        break;
                    case (int)CIID.Whiskey:
                        cb |= CalamityBuffs.Whiskey;
                        break;
                    case (int)CIID.WhiteWine:
                        cb |= CalamityBuffs.WhiteWine;
                        break;
                    case (int)CIID.AnechoicCoating:
                        cb |= CalamityBuffs.AnechoicCoating;
                        break;
                    case (int)CIID.AstralInjection:
                        cb |= CalamityBuffs.AstralInjection;
                        break;
                    case (int)CIID.AureusCell:
                        cb |= CalamityBuffs.AureusCell;
                        break;
                    case (int)CIID.BoundingPotion:
                        cb |= CalamityBuffs.Bounding;
                        break;
                    case (int)CIID.CadancePotion:
                        cb |= CalamityBuffs.Cadance;
                        break;
                    case (int)CIID.CalamitasBrew:
                        cb |= CalamityBuffs.CalamitasBrew;
                        break;
                    case (int)CIID.CeaselessHungerPotion:
                        cb |= CalamityBuffs.CeaselessHunger;
                        break;
                    case (int)CIID.CalciumPotion:
                        cb |= CalamityBuffs.Calcium;
                        break;
                    case (int)CIID.CrumblingPotion:
                        cb |= CalamityBuffs.Crumbling;
                        break;
                    case (int)CIID.DraconicElixir:
                        cb |= CalamityBuffs.DraconicElixir;
                        break;
                    case (int)CIID.GravityNormalizerPotion:
                        cb |= CalamityBuffs.GravityNormalizer;
                        break;
                    case (int)CIID.HolyWrathPotion:
                        // disabled wrath potion
                        cb |= CalamityBuffs.HolyWrath;
                        break;
                    case (int)CIID.PenumbraPotion:
                        cb |= CalamityBuffs.Penumbra;
                        break;
                    case (int)CIID.PhotosynthesisPotion:
                        cb |= CalamityBuffs.Photosynthesis;
                        break;
                    case (int)CIID.PotionofOmniscience:
                        cb |= CalamityBuffs.Omniscience;
                        break;
                    case (int)CIID.ProfanedRagePotion:
                        cb |= CalamityBuffs.ProfanedRage;
                        break;
                    case (int)CIID.RevivifyPotion:
                        cb |= CalamityBuffs.Revivify;
                        break;
                    case (int)CIID.ShadowPotion:
                        cb |= CalamityBuffs.Shadow;
                        break;
                    case (int)CIID.ShatteringPotion:
                        cb |= CalamityBuffs.Shattering;
                        break;
                    case (int)CIID.SoaringPotion:
                        cb |= CalamityBuffs.Soaring;
                        break;
                    case (int)CIID.SulphurskinPotion:
                        cb |= CalamityBuffs.Sulphurskin;
                        break;
                    case (int)CIID.TeslaPotion:
                        cb |= CalamityBuffs.Tesla;
                        break;
                    case (int)CIID.TitanScalePotion:
                        cb |= CalamityBuffs.TitanScale;
                        break;
                    case (int)CIID.TriumphPotion:
                        cb |= CalamityBuffs.Triumph;
                        break;
                    case (int)CIID.YharimsStimulants:
                        cb |= CalamityBuffs.YharimsStimulants;
                        break;
                    case (int)CIID.ZenPotion:
                        cb |= CalamityBuffs.Zen;
                        break;
                    case (int)CIID.ZergPotion:
                        cb |= CalamityBuffs.Zerg;
                        break;
                    default:
                        switch(i.buffType)
                        {
                            case BuffID.Tipsy:
                                vb |= VanillaBuffs.Tipsy;
                                break;
                            case BuffID.WellFed:
                                vb |= VanillaBuffs.WellFed;
                                break;
                            default:
                                if ( i.Name.Contains("Campfire"))
                                {
                                    vb |= VanillaBuffs.Campfire;
                                }
                                else if (i.Name != "")
                                {
                                    Main.NewText(":< [DEFAULT] " + i);
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
