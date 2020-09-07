using Microsoft.Xna.Framework;
using PotPot.Calamity;
using PotPot.Players;
using PotPot.UI;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace PotPot
{
    public class PotPot : Mod
    {
        //for future version
        //public override uint ExtraBuffSlots { get { return 22; } }

        internal static PotPot Instance;
        internal UserInterface PotPotInterface;
        internal PotPotUI MainUI;
        private GameTime _lastUpdateUiGameTime;
        internal Mod Calamity => ModLoader.GetMod("CalamityMod");
        internal Dictionary<int, Action<PotPotPlayer>> BuffCallbacks;
        public PotPot()
        {
            Instance = this;
            BuffCallbacks = new Dictionary<int, Action<PotPotPlayer>>();
        }

        public override void Load()
        {
            Logger.InfoFormat("{0} loading", Name);

            if (!Main.dedServ)
            {
                PotPotInterface = new UserInterface();
            }

            RegisterVanillaBuffCallbacks();

            if (this.Calamity != null)
                SetupCalamityInterop();   
        }

        internal bool IsRegisteredItem(int itemtype)
        {
            return BuffCallbacks.ContainsKey(itemtype);
        }
 
        private void SetupCalamityInterop()
        {
            RegisterCalamityBuffCallbacks();
        }

        private void RegisterCallback(int index, Action<PotPotPlayer> action)
        {
            if (index == 0)
                throw new ArgumentException("Callback index is 0 ; Item not found");
            BuffCallbacks.Add(index, action);
        }

        private void RegisterVanillaBuffCallbacks()
        {
            #region Potions

            RegisterCallback(ItemID.AmmoReservationPotion, (PPP) =>
            {
                Main.LocalPlayer.ammoPotion = true;
                Main.LocalPlayer.buffImmune[BuffID.AmmoReservation] = true;
            });
            RegisterCallback(ItemID.ArcheryPotion, (PPP) =>
            {
                Main.LocalPlayer.archery = true;
                Main.LocalPlayer.buffImmune[BuffID.Archery] = true;
            });
            RegisterCallback(ItemID.BattlePotion, (PPP) =>
            {
                Main.LocalPlayer.enemySpawns = true;
                Main.LocalPlayer.buffImmune[BuffID.Battle] = true;
            });
            RegisterCallback(ItemID.BuilderPotion, (PPP) =>
            {
                Main.LocalPlayer.tileSpeed += 0.25f;
                Main.LocalPlayer.wallSpeed += 0.25f;
                Main.LocalPlayer.blockRange++;
                Main.LocalPlayer.buffImmune[BuffID.Builder] = true;
            });
            RegisterCallback(ItemID.CalmingPotion, (PPP) =>
            {
                Main.LocalPlayer.calmed = true;
                Main.LocalPlayer.buffImmune[BuffID.Calm] = true;
            });
            RegisterCallback(ItemID.CratePotion, (PPP) =>
            {
                Main.LocalPlayer.cratePotion = true;
                Main.LocalPlayer.buffImmune[BuffID.Crate] = true;
            });
            RegisterCallback(ItemID.TrapsightPotion, (PPP) =>
            {
                Main.LocalPlayer.dangerSense = true;
                Main.LocalPlayer.buffImmune[BuffID.Dangersense] = true;
            });
            RegisterCallback(ItemID.EndurancePotion, (PPP) =>
            {
                Main.LocalPlayer.endurance += 0.1f;
                Main.LocalPlayer.buffImmune[BuffID.Endurance] = true;
            });
            RegisterCallback(ItemID.FeatherfallPotion, (PPP) =>
            {
                Main.LocalPlayer.slowFall = true;
                Main.LocalPlayer.buffImmune[BuffID.Featherfall] = true;
            });
            RegisterCallback(ItemID.FishingPotion, (PPP) =>
            {
                Main.LocalPlayer.fishingSkill += 15;
                Main.LocalPlayer.buffImmune[BuffID.Fishing] = true;
            });
            RegisterCallback(ItemID.FlipperPotion, (PPP) =>
            {
                Main.LocalPlayer.ignoreWater = true;
                Main.LocalPlayer.accFlipper = true;
                Main.LocalPlayer.buffImmune[BuffID.Flipper] = true;
            });
            RegisterCallback(ItemID.GillsPotion, (PPP) =>
            {
                Main.LocalPlayer.gills = true;
                Main.LocalPlayer.buffImmune[BuffID.Gills] = true;
            });
            RegisterCallback(ItemID.GravitationPotion, (PPP) =>
            {
                Main.LocalPlayer.gravControl = true;
                Main.LocalPlayer.buffImmune[BuffID.Gravitation] = true;
            });
            RegisterCallback(ItemID.HeartreachPotion, (PPP) =>
            {
                Main.LocalPlayer.lifeMagnet = true;
                Main.LocalPlayer.buffImmune[BuffID.Heartreach] = true;
            });
            RegisterCallback(ItemID.HunterPotion, (PPP) =>
            {
                Main.LocalPlayer.detectCreature = true;
                Main.LocalPlayer.buffImmune[BuffID.Hunter] = true;
            });
            RegisterCallback(ItemID.InfernoPotion, (PPP) =>
            {
                Main.LocalPlayer.inferno = true;
                Lighting.AddLight((int)Main.LocalPlayer.Center.X >> 4, (int)Main.LocalPlayer.Center.Y >> 4, 0.65f, 0.4f, 0.1f);
                float num = 40000f;
                bool flag = Main.LocalPlayer.infernoCounter % 60 == 0;
                int damage = 10;
                if (Main.LocalPlayer.whoAmI == Main.myPlayer)
                {
                    for (int i = 0; i < 200; i++)
                    {
                        NPC nPC = Main.npc[i];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[24] && Vector2.DistanceSquared(Main.LocalPlayer.Center, nPC.Center) <= num)
                        {
                            if (nPC.FindBuffIndex(BuffID.OnFire) == -1)
                            {
                                nPC.AddBuff(BuffID.OnFire, 120);
                            }
                            if (flag)
                            {
                                Main.LocalPlayer.ApplyDamageToNPC(nPC, damage, 0f, 0, crit: false);
                            }
                        }
                    }
                }
                //pvp
                if (Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.hostile)
                {
                    for (int j = 0; j < 255; j++)
                    {
                        Player target = Main.player[j];
                        if (target != Main.LocalPlayer && target.active && !target.dead && target.hostile && !target.buffImmune[BuffID.OnFire] && (target.team != Main.LocalPlayer.team || target.team == 0) && Vector2.DistanceSquared(Main.LocalPlayer.Center, target.Center) <= num)
                        {
                            if (target.FindBuffIndex(BuffID.OnFire) == -1)
                            {
                                target.AddBuff(BuffID.OnFire, 120);
                            }
                            if (flag)
                            {
                                target.Hurt(PlayerDeathReason.LegacyEmpty(), damage, 0, pvp: true);
                                PlayerDeathReason reason = PlayerDeathReason.ByPlayer(Main.LocalPlayer.whoAmI);
                                NetMessage.SendPlayerHurt(j, reason, damage, 0, critical: false, pvp: true, 0);
                            }
                        }
                    }
                }
                Main.LocalPlayer.buffImmune[BuffID.Inferno] = true;
            });
            RegisterCallback(ItemID.InvisibilityPotion, (PPP) =>
            {
                Main.LocalPlayer.invis = true;
                Main.LocalPlayer.buffImmune[BuffID.Invisibility] = true;
            });
            RegisterCallback(ItemID.IronskinPotion, (PPP) =>
            {
                Main.LocalPlayer.statDefense += 8;
                Main.LocalPlayer.buffImmune[BuffID.Ironskin] = true;
            });
            RegisterCallback(ItemID.LifeforcePotion, (PPP) =>
            {
                Main.LocalPlayer.lifeForce = true;
                Main.LocalPlayer.statLifeMax2 += Main.LocalPlayer.statLifeMax / 5 / 20 * 20;
                Main.LocalPlayer.buffImmune[BuffID.Lifeforce] = true;
            });
            RegisterCallback(ItemID.LovePotion, (PPP) =>
            {
                Main.LocalPlayer.loveStruck = true;
                Main.LocalPlayer.buffImmune[BuffID.Lovestruck] = true;
            });
            RegisterCallback(ItemID.MagicPowerPotion, (PPP) =>
            {
                Main.LocalPlayer.magicDamage += 0.2f;
                Main.LocalPlayer.buffImmune[BuffID.MagicPower] = true;
            });
            RegisterCallback(ItemID.ManaRegenerationPotion, (PPP) =>
            {
                Main.LocalPlayer.manaRegenBuff = true;
                Main.LocalPlayer.buffImmune[BuffID.ManaRegeneration] = true;
            });
            RegisterCallback(ItemID.MiningPotion, (PPP) =>
            {
                Main.LocalPlayer.pickSpeed -= 0.25f;
                Main.LocalPlayer.buffImmune[BuffID.Mining] = true;
            });
            RegisterCallback(ItemID.NightOwlPotion, (PPP) =>
            {
                Main.LocalPlayer.nightVision = true;
                Main.LocalPlayer.buffImmune[BuffID.NightOwl] = true;
            });
            RegisterCallback(ItemID.ObsidianSkinPotion, (PPP) =>
            {
                Main.LocalPlayer.lavaImmune = true;
                Main.LocalPlayer.fireWalk = true;
                Main.LocalPlayer.buffImmune[BuffID.OnFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.ObsidianSkin] = true;
            });
            RegisterCallback(ItemID.RagePotion, (PPP) =>
            {
                Main.LocalPlayer.meleeCrit += 10;
                Main.LocalPlayer.thrownCrit += 10;
                Main.LocalPlayer.magicCrit += 10;
                Main.LocalPlayer.rangedCrit += 10;
                Main.LocalPlayer.buffImmune[BuffID.Rage] = true;
            });
            RegisterCallback(ItemID.RegenerationPotion, (PPP) =>
            {
                Main.LocalPlayer.lifeRegen += 4;
                Main.LocalPlayer.buffImmune[BuffID.Regeneration] = true;
            });
            RegisterCallback(ItemID.ShinePotion, (PPP) =>
            {
                Lighting.AddLight((int)Main.LocalPlayer.Center.X >> 4, (int)Main.LocalPlayer.Center.Y >> 4, 0.8f, 0.95f, 1f);
                Main.LocalPlayer.buffImmune[BuffID.Shine] = true;
            });
            RegisterCallback(ItemID.SonarPotion, (PPP) =>
            {
                Main.LocalPlayer.sonarPotion = true;
                Main.LocalPlayer.buffImmune[BuffID.Sonar] = true;
            });
            RegisterCallback(ItemID.SpelunkerPotion, (PPP) =>
            {
                Main.LocalPlayer.findTreasure = true;
                Main.LocalPlayer.buffImmune[BuffID.Spelunker] = true;
            });
            RegisterCallback(ItemID.StinkPotion, (PPP) =>
            {
                Main.LocalPlayer.stinky = true;
                Main.LocalPlayer.buffImmune[BuffID.Stinky] = true;
            });
            RegisterCallback(ItemID.SummoningPotion, (PPP) =>
            {
                Main.LocalPlayer.maxMinions++;
                Main.LocalPlayer.buffImmune[BuffID.Summoning] = true;
            });
            RegisterCallback(ItemID.SwiftnessPotion, (PPP) =>
            {
                Main.LocalPlayer.moveSpeed += 0.25f;
                Main.LocalPlayer.buffImmune[BuffID.Swiftness] = true;
            });
            RegisterCallback(ItemID.ThornsPotion, (PPP) =>
            {
                Main.LocalPlayer.thorns += 0.33f;
                Main.LocalPlayer.buffImmune[BuffID.Thorns] = true;
            });
            RegisterCallback(ItemID.TitanPotion, (PPP) =>
            {
                Main.LocalPlayer.kbBuff = true;
                Main.LocalPlayer.buffImmune[BuffID.Titan] = true;
            });
            RegisterCallback(ItemID.WarmthPotion, (PPP) =>
            {
                Main.LocalPlayer.resistCold = true;
                Main.LocalPlayer.buffImmune[BuffID.Warmth] = true;
            });
            RegisterCallback(ItemID.WaterWalkingBoots, (PPP) =>
            {
                Main.LocalPlayer.waterWalk = true;
                Main.LocalPlayer.buffImmune[BuffID.WaterWalking] = true;
            });
            RegisterCallback(ItemID.WrathPotion, (PPP) =>
            {
                Main.LocalPlayer.meleeDamage += 0.1f;
                Main.LocalPlayer.thrownDamage += 0.1f;
                Main.LocalPlayer.magicDamage += 0.1f;
                Main.LocalPlayer.rangedDamage += 0.1f;
                Main.LocalPlayer.minionDamage += 0.1f;
                Main.LocalPlayer.buffImmune[BuffID.Wrath] = true;
            });

            #endregion Potions
            #region Flasks

            RegisterCallback(ItemID.FlaskofIchor, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 5;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
            });

            #endregion Flasks
            #region Food

            RegisterCallback(ItemID.Sashimi, (PPP) =>
            {
                Main.LocalPlayer.wellFed = true;
                Main.LocalPlayer.statDefense += 2;
                Main.LocalPlayer.meleeCrit += 2;
                Main.LocalPlayer.meleeDamage += 0.05f;
                Main.LocalPlayer.meleeSpeed += 0.05f;
                Main.LocalPlayer.magicCrit += 2;
                Main.LocalPlayer.magicDamage += 0.05f;
                Main.LocalPlayer.rangedCrit += 2;
                Main.LocalPlayer.rangedDamage += 0.05f;
                Main.LocalPlayer.thrownCrit += 2;
                Main.LocalPlayer.thrownDamage += 0.05f;
                Main.LocalPlayer.minionDamage += 0.05f;
                Main.LocalPlayer.minionKB += 0.5f;
                Main.LocalPlayer.moveSpeed += 0.2f;
                Main.LocalPlayer.buffImmune[BuffID.WellFed] = true;
            });
            RegisterCallback(ItemID.Sake, (PPP) =>
            {
                Main.LocalPlayer.statDefense -= 4;
                Main.LocalPlayer.meleeDamage += 0.1f;
                Main.LocalPlayer.meleeCrit += 2;
                Main.LocalPlayer.meleeSpeed += 0.1f;
                Main.LocalPlayer.buffImmune[BuffID.Tipsy] = true;
            });

            #endregion Food
            #region Station

            RegisterCallback(ItemID.AmmoBox, (PPP) =>
            {
                Main.LocalPlayer.ammoBox = true;
                Main.LocalPlayer.buffImmune[BuffID.AmmoBox] = true;
            });
            RegisterCallback(ItemID.BewitchingTable, (PPP) =>
            {
                Main.LocalPlayer.maxMinions++;
                Main.LocalPlayer.buffImmune[BuffID.Bewitched] = true;
            });
            RegisterCallback(ItemID.CrystalBall, (PPP) =>
            {
                Main.LocalPlayer.magicDamage += 0.05f;
                Main.LocalPlayer.magicCrit += 2;
                Main.LocalPlayer.statManaMax2 += 20;
                Main.LocalPlayer.manaCost -= 0.02f;
                Main.LocalPlayer.buffImmune[BuffID.Clairvoyance] = true;
            });
            RegisterCallback(ItemID.SharpeningStation, (PPP) =>
            {
                if (Main.LocalPlayer.inventory[Main.LocalPlayer.selectedItem].melee)
                {
                    Main.LocalPlayer.armorPenetration += 4;
                }
                Main.LocalPlayer.buffImmune[BuffID.Sharpened] = true;
            });
            RegisterCallback(ItemID.Campfire, (PPP) =>
            {
                if (Main.myPlayer == Main.LocalPlayer.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.campfire = true;
                }
                Main.LocalPlayer.buffImmune[BuffID.Campfire] = true;
            });
            RegisterCallback(ItemID.HeartLantern, (PPP) =>
            {
                if (Main.myPlayer == Main.LocalPlayer.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.heartLantern = true;
                }
                Main.LocalPlayer.buffImmune[BuffID.HeartLamp] = true;
            });
            RegisterCallback(ItemID.BottledHoney, (PPP) =>
            {
                Main.LocalPlayer.honey = true;
                Main.LocalPlayer.buffImmune[BuffID.Honey] = true;
            });
            RegisterCallback(ItemID.PeaceCandle, (PPP) =>
            {
                Main.LocalPlayer.ZonePeaceCandle = true;
                if (Main.myPlayer == Main.LocalPlayer.whoAmI)
                {
                    Main.peaceCandles = 0;
                }
                Main.LocalPlayer.buffImmune[BuffID.PeaceCandle] = true;
            });
            RegisterCallback(ItemID.StarinaBottle, (PPP) =>
            {
                if (Main.myPlayer == Main.LocalPlayer.whoAmI || Main.netMode == NetmodeID.Server)
                {
                    Main.starInBottle = false;
                }
                Main.LocalPlayer.manaRegenBonus += 2;
                Main.LocalPlayer.buffImmune[BuffID.StarInBottle] = true;
            });

            #endregion Station
        }

        private void RegisterCalamityBuffCallbacks()
        {
            #region Upgraded_Vanilla_Buffs

            BuffCallbacks.Remove(ItemID.RagePotion);
            RegisterCallback(ItemID.RagePotion, (PPP) =>
            {
                Main.LocalPlayer.meleeCrit += 10;
                Main.LocalPlayer.thrownCrit += 10;
                Main.LocalPlayer.magicCrit += 10;
                Main.LocalPlayer.rangedCrit += 10;
                PPP.CP.throwingCrit += 10;
                Main.LocalPlayer.buffImmune[BuffID.Rage] = true;
            });
            BuffCallbacks.Remove(ItemID.WrathPotion);
            RegisterCallback(ItemID.WrathPotion, (PPP) =>
            {
                Main.LocalPlayer.meleeDamage += 0.1f;
                Main.LocalPlayer.thrownDamage += 0.1f;
                Main.LocalPlayer.magicDamage += 0.1f;
                Main.LocalPlayer.rangedDamage += 0.1f;
                Main.LocalPlayer.minionDamage += 0.1f;
                PPP.CP.throwingDamage += 0.1f;
                Main.LocalPlayer.buffImmune[BuffID.Wrath] = true;
            });

            #endregion
            #region Calamity_Potions

            RegisterCallback(CalamityID.Item("AnechoicCoating"), (PPP) => 
            {
                PPP.CP.anechoicCoating = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("AnechoicCoatingBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("AstralInjection"), (PPP) => 
            {
                PPP.CP.astralInjection = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("AstralInjectionBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("BoundingPotion"), (PPP) => 
            {
                PPP.CP.bounding = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("BoundingBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("CadencePotion"), (PPP) => 
            {
                PPP.CP.cadence = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("Cadence")] = true;
            });
            RegisterCallback(CalamityID.Item("CalamitasBrew"), (PPP) => 
            {
                PPP.CP.aWeapon = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("AbyssalWeapon")] = true;
            });
            RegisterCallback(CalamityID.Item("CalciumPotion"), (PPP) => 
            {
                PPP.CP.calcium = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("CalciumBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("CeaselessHungerPotion"), (PPP) => 
            {
                PPP.CP.ceaselessHunger = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("CeaselessHunger")] = true;
            });
            RegisterCallback(CalamityID.Item("CrumblingPotion"), (PPP) => 
            {
                PPP.CP.armorCrumbling = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("ArmorCrumbling")] = true;
            });
            RegisterCallback(CalamityID.Item("DraconicElixir"), (PPP) => 
            {
                if(!PPP.CP.draconicSurgeCooldown)
                    PPP.CP.draconicSurge = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("DraconicSurgeBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("GravityNormalizerPotion"), (PPP) => 
            {
                PPP.CP.gravityNormalizer = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("GravityNormalizerBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("HolyWrathPotion"), (PPP) => 
            {
                PPP.CP.holyWrath = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("HolyWrathBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("PenumbraPotion"), (PPP) => 
            {
                PPP.CP.penumbra = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("PenumbraBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("PhotosynthesisPotion"), (PPP) => 
            {
                PPP.CP.photosynthesis = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("PhotosynthesisBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("PotionofOmniscience"), (PPP) => 
            {
                PPP.CP.omniscience = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("Omniscience")] = true;
            });
            RegisterCallback(CalamityID.Item("ProfanedRagePotion"), (PPP) => 
            {
                PPP.CP.profanedRage = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("ProfanedRageBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("RevivifyPotion"), (PPP) =>
            {
                PPP.CP.revivify = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("Revivify")] = true;
            });
            RegisterCallback(CalamityID.Item("ShadowPotion"), (PPP) => 
            {
                PPP.CP.shadow = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("ShadowBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("ShatteringPotion"), (PPP) => 
            {
                PPP.CP.armorShattering = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("ArmorShattering")] = true;
            });
            RegisterCallback(CalamityID.Item("SoaringPotion"), (PPP) => 
            {
                PPP.CP.soaring = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("Soaring")] = true;
            });
            RegisterCallback(CalamityID.Item("SulphurskinPotion"), (PPP) => 
            {
                PPP.CP.sulphurskin = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("SulphurskinBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("TeslaPotion"), (PPP) => 
            {
                PPP.CP.tesla = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("TeslaBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("TitanScalePotion"), (PPP) => 
            {
                PPP.CP.tScale = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("TitanScale")] = true;
            });
            RegisterCallback(CalamityID.Item("TriumphPotion"), (PPP) =>
            {
                PPP.CP.triumph = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("TriumphBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("YharimsStimulants"), (PPP) => 
            {
                PPP.CP.yPower = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("YharimPower")] = true;
            });
            RegisterCallback(CalamityID.Item("ZenPotion"), (PPP) => 
            {
                PPP.CP.zen = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("Zen")] = true;
            });
            RegisterCallback(CalamityID.Item("ZergPotion"), (PPP) => 
            {
                PPP.CP.zerg = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("Zerg")] = true;
            });

            #endregion Calamity_Potions
            #region Alcohol

            RegisterCallback(CalamityID.Item("BloodyMary"), (PPP) => 
            {
                PPP.CP.bloodyMary = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("BloodyMaryBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("CaribbeanRum"), (PPP) => 
            {
                PPP.CP.caribbeanRum = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("CaribbeanRumBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("CinnamonRoll"), (PPP) => 
            {
                PPP.CP.cinnamonRoll = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("CinnamonRollBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Everclear"), (PPP) => 
            {
                PPP.CP.everclear = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("EverclearBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("EvergreenGin"), (PPP) => 
            {
                PPP.CP.evergreenGin = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("EvergreenGinBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("FabsolsVodka"), (PPP) => 
            {
                PPP.CP.fabsolVodka = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("FabsolVodkaBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Fireball"), (PPP) => 
            {
                PPP.CP.fireball = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("FireballBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Moonshine"), (PPP) => 
            {
                PPP.CP.moonshine = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("MoonshineBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("MoscowMule"), (PPP) => 
            {
                PPP.CP.moscowMule = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("MoscowMuleBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Rum"), (PPP) => 
            {
                PPP.CP.rum = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("RumBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Screwdriver"), (PPP) => 
            {
                PPP.CP.screwdriver = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("ScrewdriverBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("StarBeamRye"), (PPP) => 
            {
                PPP.CP.starBeamRye = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("StarBeamRyeBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Tequila"), (PPP) => 
            {
                PPP.CP.tequila = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("TequilaBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("TequilaSunrise"), (PPP) => 
            {
                PPP.CP.tequilaSunrise = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("TequilaSunriseBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Vodka"), (PPP) => 
            {
                PPP.CP.vodka = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("VodkaBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("Whiskey"), (PPP) => 
            {
                PPP.CP.whiskey = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("WhiskeyBuff")] = true;
            });
            RegisterCallback(CalamityID.Item("WhiteWine"), (PPP) => 
            {
                PPP.CP.whiteWine = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("WhiteWineBuff")] = true;
            });

            #endregion Alcohol
            #region Lore

            RegisterCallback(CalamityID.Item("KnowledgeBrimstoneElemental"), (PPP) =>
            {
                PPP.CP.brimstoneElementalLore = true;
            });
            #endregion Lore
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (PotPotInterface?.CurrentState != null)
            {
                PotPotInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "PotPot: PotPotInterface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && PotPotInterface?.CurrentState != null)
                        {
                            PotPotInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        public override void Unload()
        {
            MainUI = null;
            Instance = null;
        }

        internal void ShowUI()
        {
            PotPotInterface?.SetState(MainUI);
        }

        internal void HideUI()
        {
            PotPotInterface?.SetState(null);
        }
    }
}