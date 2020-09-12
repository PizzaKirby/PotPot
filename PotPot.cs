using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using PotPot.Calamity;
using PotPot.Players;
using PotPot.Thorium;
using PotPot.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using ThoriumMod;
using ThoriumMod.NPCs;
using ThoriumMod.NPCs.BloodMoon;
using ThoriumMod.NPCs.Depths;


namespace PotPot
{
    public class PotPot : Mod
    {
        //for future version
        //public override uint ExtraBuffSlots { get { return 22; } }

        internal static PotPot Instance;
        internal UserInterface PotPotInterface;
        internal PotPotUI MainUI;
        internal Mod Calamity => ModLoader.GetMod("CalamityMod");
        internal Mod Thorium => ModLoader.GetMod("ThoriumMod");
        internal Dictionary<int, Action<PotPotPlayer>> BuffCallbacks; 

        private GameTime _lastUpdateUiGameTime;
        internal Dictionary<string, List<int>> MutualExclusives;
        
        public PotPot()
        {
            Instance = this;
            BuffCallbacks = new Dictionary<int, Action<PotPotPlayer>>();
            MutualExclusives = new Dictionary<string, List<int>>();
        }

        public override void Load()
        {
            Logger.InfoFormat("{0} loading", Name);

            if (!Main.dedServ)
            {
                PotPotInterface = new UserInterface();
            }

            RegisterVanillaBuffCallbacks();

            MutualExclusives.Add("Imbues",new List<int>
            {
                ItemID.FlaskofCursedFlames,
                ItemID.FlaskofFire,
                ItemID.FlaskofGold,
                ItemID.FlaskofIchor,
                ItemID.FlaskofNanites,
                ItemID.FlaskofParty,
                ItemID.FlaskofPoison,
                ItemID.FlaskofVenom,
            });

            if (this.Calamity != null)
            {
                Logger.Info("CalamityMod found, enabling interop.");
                SetupCalamityInterop();
            }

            if (this.Thorium != null)
            {
                Logger.Info("ThoriumMod found, enabling interop.");
                SetupThoriumInterop();
            }
        }

        internal bool IsRegisteredItem(int itemtype)
        {
            return BuffCallbacks.ContainsKey(itemtype);
        }
 
        private void SetupCalamityInterop()
        {
            RegisterCalamityBuffCallbacks();
        }

        private void SetupThoriumInterop()
        {
            RegisterThoriumBuffCallbacks();
            MutualExclusives.Add("ThrowImbues", new List<int>
            {
                ThoriumID.Item("ExplosiveCoatingItem"),
                ThoriumID.Item("FrostCoatingItem"),
                ThoriumID.Item("GorganCoatingItem"),
                ThoriumID.Item("SporeCoatingItem"),
                ThoriumID.Item("ToxicCoatingItem")
            });
        }

        private void RegisterCallback(int index, Action<PotPotPlayer> action)
        {
            if (index == 0)
                throw new ArgumentException("Callback index is 0 ; Item not found");
            if (action == null)
                throw new ArgumentException("Action is null, please provide an action to execute");
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
            RegisterCallback(ItemID.FlaskofVenom, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 1;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
            });
            RegisterCallback(ItemID.CursedFlame, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 2;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
            });
            RegisterCallback(ItemID.FlaskofFire, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 3;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
            });
            RegisterCallback(ItemID.FlaskofGold, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 4;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
            });
            RegisterCallback(ItemID.FlaskofIchor, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 5;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
            });
            RegisterCallback(ItemID.FlaskofNanites, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 6;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
            });
            RegisterCallback(ItemID.FlaskofParty, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 7;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
            });
            RegisterCallback(ItemID.FlaskofPoison, (PPP) =>
            {
                Main.LocalPlayer.meleeEnchant = 8;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueVenom] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueCursedFlames] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueFire] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueGold] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueIchor] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueNanites] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbueConfetti] = true;
                Main.LocalPlayer.buffImmune[BuffID.WeaponImbuePoison] = true;
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
            #region Potions

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

            #endregion Potions
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

            RegisterCallback(CalamityID.Item("KnowledgeAquaticScourge"), (PPP) =>
            {
                PPP.CP.aquaticScourgeLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeAstrumAureus"), (PPP) =>
            {
                PPP.CP.astrumAureusLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeAstrumDeus"), (PPP) =>
            {
                PPP.CP.astrumDeusLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeBrimstoneElemental"), (PPP) =>
            {
                PPP.CP.brimstoneElementalLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeCalamitas"), (PPP) =>
            {
                PPP.CP.SCalLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeCalamitasClone"), (PPP) =>
            {
                PPP.CP.calamitasLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeCorruption"), (PPP) =>
            {
                PPP.CP.corruptionLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeCrabulon"), (PPP) =>
            {
                PPP.CP.crabulonLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeCrimson"), (PPP) =>
            {
                PPP.CP.crimsonLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeCryogen"), (PPP) =>
            {
                PPP.CP.dashMod = 6;
            });
            RegisterCallback(CalamityID.Item("KnowledgeDesertScourge"), (PPP) =>
            {
                PPP.CP.desertScourgeLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeDestroyer"), (PPP) =>
            {
                PPP.CP.destroyerLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeDevourerofGods"), (PPP) =>
            {
                PPP.CP.DoGLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeDukeFishron"), (PPP) =>
            {
                PPP.CP.dukeFishronLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeEaterofWorlds"), (PPP) =>
            {
                PPP.CP.eaterOfWorldsLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeEyeofCthulhu"), (PPP) =>
            {
                if (!Main.dayTime)
                {
                    Main.LocalPlayer.nightVision = true;
                    return;
                }
                Main.LocalPlayer.blind = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeGolem"), (PPP) =>
            {
                PPP.CP.golemLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeHiveMind"), (PPP) =>
            {
                if (Main.LocalPlayer.ZoneCorrupt)
                    PPP.CP.hiveMindLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeKingSlime"), (PPP) =>
            {
                PPP.CP.kingSlimeLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeLeviathanandSiren"), (PPP) =>
            {
                PPP.CP.leviathanAndSirenLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeLunaticCultist"), (PPP) =>
            {
                if(NPC.LunarApocalypseIsUp)
                    PPP.CP.lunaticCultistLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeMoonLord"), (PPP) =>
            {
                PPP.CP.moonLordLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeOcean"), (PPP) =>
            {
                PPP.CP.oceanLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeOldDuke"), (PPP) =>
            {
                PPP.CP.boomerDukeLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgePerforators"), (PPP) =>
            {
                if(Main.LocalPlayer.ZoneCrimson)
                    PPP.CP.perforatorLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgePlaguebringerGoliath"), (PPP) =>
            {
                PPP.CP.plaguebringerGoliathLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgePlantera"), (PPP) =>
            {
                PPP.CP.planteraLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgePolterghast"), (PPP) =>
            {
                PPP.CP.polterghastLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeProvidence"), (PPP) =>
            {
                PPP.CP.providenceLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeQueenBee"), (PPP) =>
            {
                PPP.CP.queenBeeLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeRavager"), (PPP) =>
            {
                PPP.CP.ravagerLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeSkeletron"), (PPP) =>
            {
                if(Main.LocalPlayer.ZoneDungeon)
                    PPP.CP.skeletronLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeSkeletronPrime"), (PPP) =>
            {
                PPP.CP.skeletronLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeSlimeGod"), (PPP) =>
            {
                if (Main.LocalPlayer.mount.Active || PPP.CP.slimeGodLoreProcessed)
                {
                    return;
                }
                PPP.CP.slimeGodLoreProcessed = true;
                if (Main.LocalPlayer.dashDelay < 0 || (Main.LocalPlayer.velocity.Length() >= 11f && CalamityPlayer.areThereAnyDamnBosses))
                {
                    Main.LocalPlayer.velocity.X = Main.LocalPlayer.velocity.X * 0.9f;
                }
                Main.LocalPlayer.slippy2 = true;
                if (Main.myPlayer == Main.LocalPlayer.whoAmI)
                {
                    Main.LocalPlayer.AddBuff(137, 2, true);
                }
                Main.LocalPlayer.statDefense -= 10;
            });
            RegisterCallback(CalamityID.Item("KnowledgeTwins"), (PPP) =>
            {
                PPP.CP.twinsLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeUnderworld"), (PPP) =>
            {
                PPP.CP.underworldLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeWallofFlesh"), (PPP) =>
            {
                PPP.CP.wallOfFleshLore = true;
            });
            RegisterCallback(CalamityID.Item("KnowledgeYharon"), (PPP) =>
            {
                PPP.CP.yharonLore = true;
            });

            #endregion Lore
            #region Furniture

            RegisterCallback(CalamityID.Item("BlueCandle"), (PPP) =>
            {
                PPP.CP.blueCandle = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("BlueSpeedCandle")] = true;
            });
            RegisterCallback(CalamityID.Item("PinkCandle"), (PPP) =>
            {
                PPP.CP.pinkCandle = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("PinkHealthCandle")] = true;
            });
            RegisterCallback(CalamityID.Item("PurpleCandle"), (PPP) =>
            {
                PPP.CP.purpleCandle = true;
                Main.LocalPlayer.buffImmune[CalamityID.Buff("PurpleDefenseCandle")] = true;
            });

            #endregion Furniture
        }

        private void RegisterThoriumBuffCallbacks()
        {
            #region Potions

            RegisterCallback(ThoriumID.Item("ArtilleryPotion"), (PPP) => 
            {
                Main.LocalPlayer.maxTurrets++;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ArtilleryBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("BloodPotion"), (PPP) =>
            {
                Main.LocalPlayer.pickSpeed -= 0.15f;
                Main.LocalPlayer.pickSpeed += 0.15f;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("BloodRush")] = true;
            });
            RegisterCallback(ThoriumID.Item("BouncingFlamePotion"), (PPP) =>
            {
                ThoriumPlayer TP = PPP.TP;
                for (int i = 0; i < 200; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && Vector2.Distance(Main.LocalPlayer.Center, npc.Center) < 375f)
                    {
                        if (npc.shadowFlame)
                        {
                            TP.bouncingFlameBool = true;
                            TP.bouncingFlameBoolShadowFire = true;
                        }
                        else if (npc.GetGlobalNPC<ThoriumGlobalNPC>().melting)
                        {
                            TP.bouncingFlameBool = true;
                            TP.bouncingFlameBoolMeltFire = true;
                        }
                        else if (npc.onFrostBurn)
                        {
                            TP.bouncingFlameBool = true;
                            TP.bouncingFlameBoolFrostFire = true;
                        }
                        else if (npc.onFire2)
                        {
                            TP.bouncingFlameBool = true;
                            TP.bouncingFlameBoolCursedFire = true;
                        }
                        else if (npc.onFire)
                        {
                            TP.bouncingFlameBool = true;
                            TP.bouncingFlameBoolFire = true;
                        }
                        else if (npc.GetGlobalNPC<ThoriumGlobalNPC>().singed)
                        {
                            TP.bouncingFlameBool = true;
                            TP.bouncingFlameBoolSingedFire = true;
                        }
                    }
                }
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("BouncingFlameBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("BugRepellent"), (PPP) =>
            {
                Main.LocalPlayer.npcTypeNoAggro[42] = true;
                Main.LocalPlayer.npcTypeNoAggro[231] = true;
                Main.LocalPlayer.npcTypeNoAggro[232] = true;
                Main.LocalPlayer.npcTypeNoAggro[233] = true;
                Main.LocalPlayer.npcTypeNoAggro[234] = true;
                Main.LocalPlayer.npcTypeNoAggro[235] = true;
                Main.LocalPlayer.npcTypeNoAggro[176] = true;
                Main.LocalPlayer.npcTypeNoAggro[69] = true;
                Main.LocalPlayer.npcTypeNoAggro[508] = true;
                Main.LocalPlayer.npcTypeNoAggro[509] = true;
                Main.LocalPlayer.npcTypeNoAggro[205] = true;
                Main.LocalPlayer.npcTypeNoAggro[210] = true;
                Main.LocalPlayer.npcTypeNoAggro[211] = true;
                Main.LocalPlayer.npcTypeNoAggro[217] = true;
                Main.LocalPlayer.npcTypeNoAggro[218] = true;
                Main.LocalPlayer.npcTypeNoAggro[219] = true;
                Main.LocalPlayer.npcTypeNoAggro[258] = true;
                Main.LocalPlayer.npcTypeNoAggro[530] = true;
                Main.LocalPlayer.npcTypeNoAggro[531] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<MossWasp>()] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("InsectRepellentBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("ConflagrationPotion"), (PPP) =>
            {
                Main.LocalPlayer.allDamage += 0.15f;
                PPP.TP.conflagrate = true;
                Lighting.AddLight(Main.LocalPlayer.position, 0.3f, 0.1f, 0.05f);
                Vector2 vector;
                vector = new Vector2((float)Main.rand.Next(-40, 41), (float)Main.rand.Next(-40, 41));
                Dust dust = Dust.NewDustDirect(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, 127, 0f, 0f, 100, default(Color), 1.75f);
                dust.noGravity = true;
                dust.noLight = true;
                dust.position += vector;
                dust.velocity = -vector * 0.055f;
                vector = new Vector2((float)Main.rand.Next(-40, 41), (float)Main.rand.Next(-40, 41));
                Dust dust2 = Dust.NewDustDirect(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, 6, 0f, 0f, 100, default(Color), 1.25f);
                dust2.noGravity = true;
                dust2.noLight = true;
                dust2.position += vector;
                dust2.velocity = -vector * 0.055f;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("Conflagrate")] = true;
            });
            RegisterCallback(ThoriumID.Item("CreativityPotion"), (PPP) =>
            {
                PPP.TP.bardDropPotion++;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("CreativityDrop")] = true;
            });
            RegisterCallback(ThoriumID.Item("FishRepellent"), (PPP) =>
            {
                Main.LocalPlayer.npcTypeNoAggro[58] = true;
                Main.LocalPlayer.npcTypeNoAggro[224] = true;
                Main.LocalPlayer.npcTypeNoAggro[102] = true;
                Main.LocalPlayer.npcTypeNoAggro[241] = true;
                Main.LocalPlayer.npcTypeNoAggro[57] = true;
                Main.LocalPlayer.npcTypeNoAggro[465] = true;
                Main.LocalPlayer.npcTypeNoAggro[157] = true;
                Main.LocalPlayer.npcTypeNoAggro[65] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<Blowfish>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<AbyssalAngler>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<AbyssalAngler2>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<Hammerhead>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<Barracuda>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<Sharptooth>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<FeedingFrenzy>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<FeedingFrenzy2>()] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("FishRepellentBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("HolyPotion"), (PPP) =>
            {
                PPP.TP.healBonus++;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("HolyBonus")] = true;
            });
            RegisterCallback(ThoriumID.Item("SilverTonguePotion"), (PPP) =>
            {
                Main.LocalPlayer.discount = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("SilverTongue")] = true;
            });
            RegisterCallback(ThoriumID.Item("SkeletonRepellent"), (PPP) =>
            {
                Main.LocalPlayer.npcTypeNoAggro[21] = true;
                Main.LocalPlayer.npcTypeNoAggro[77] = true;
                Main.LocalPlayer.npcTypeNoAggro[110] = true;
                Main.LocalPlayer.npcTypeNoAggro[201] = true;
                Main.LocalPlayer.npcTypeNoAggro[202] = true;
                Main.LocalPlayer.npcTypeNoAggro[203] = true;
                Main.LocalPlayer.npcTypeNoAggro[291] = true;
                Main.LocalPlayer.npcTypeNoAggro[292] = true;
                Main.LocalPlayer.npcTypeNoAggro[293] = true;
                Main.LocalPlayer.npcTypeNoAggro[322] = true;
                Main.LocalPlayer.npcTypeNoAggro[323] = true;
                Main.LocalPlayer.npcTypeNoAggro[324] = true;
                Main.LocalPlayer.npcTypeNoAggro[449] = true;
                Main.LocalPlayer.npcTypeNoAggro[450] = true;
                Main.LocalPlayer.npcTypeNoAggro[451] = true;
                Main.LocalPlayer.npcTypeNoAggro[452] = true;
                Main.LocalPlayer.npcTypeNoAggro[481] = true;
                Main.LocalPlayer.npcTypeNoAggro[31] = true;
                Main.LocalPlayer.npcTypeNoAggro[32] = true;
                Main.LocalPlayer.npcTypeNoAggro[269] = true;
                Main.LocalPlayer.npcTypeNoAggro[270] = true;
                Main.LocalPlayer.npcTypeNoAggro[271] = true;
                Main.LocalPlayer.npcTypeNoAggro[272] = true;
                Main.LocalPlayer.npcTypeNoAggro[273] = true;
                Main.LocalPlayer.npcTypeNoAggro[274] = true;
                Main.LocalPlayer.npcTypeNoAggro[275] = true;
                Main.LocalPlayer.npcTypeNoAggro[276] = true;
                Main.LocalPlayer.npcTypeNoAggro[277] = true;
                Main.LocalPlayer.npcTypeNoAggro[278] = true;
                Main.LocalPlayer.npcTypeNoAggro[279] = true;
                Main.LocalPlayer.npcTypeNoAggro[280] = true;
                Main.LocalPlayer.npcTypeNoAggro[281] = true;
                Main.LocalPlayer.npcTypeNoAggro[282] = true;
                Main.LocalPlayer.npcTypeNoAggro[283] = true;
                Main.LocalPlayer.npcTypeNoAggro[284] = true;
                Main.LocalPlayer.npcTypeNoAggro[285] = true;
                Main.LocalPlayer.npcTypeNoAggro[286] = true;
                Main.LocalPlayer.npcTypeNoAggro[287] = true;
                Main.LocalPlayer.npcTypeNoAggro[294] = true;
                Main.LocalPlayer.npcTypeNoAggro[295] = true;
                Main.LocalPlayer.npcTypeNoAggro[296] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<AncientCharger>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<DarksteelKnight>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<Shambler>()] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("SkeletonRepellentBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("ZombieRepellent"), (PPP) =>
            {
                Main.LocalPlayer.npcTypeNoAggro[3] = true;
                Main.LocalPlayer.npcTypeNoAggro[132] = true;
                Main.LocalPlayer.npcTypeNoAggro[161] = true;
                Main.LocalPlayer.npcTypeNoAggro[186] = true;
                Main.LocalPlayer.npcTypeNoAggro[187] = true;
                Main.LocalPlayer.npcTypeNoAggro[188] = true;
                Main.LocalPlayer.npcTypeNoAggro[189] = true;
                Main.LocalPlayer.npcTypeNoAggro[200] = true;
                Main.LocalPlayer.npcTypeNoAggro[223] = true;
                Main.LocalPlayer.npcTypeNoAggro[254] = true;
                Main.LocalPlayer.npcTypeNoAggro[255] = true;
                Main.LocalPlayer.npcTypeNoAggro[319] = true;
                Main.LocalPlayer.npcTypeNoAggro[320] = true;
                Main.LocalPlayer.npcTypeNoAggro[321] = true;
                Main.LocalPlayer.npcTypeNoAggro[331] = true;
                Main.LocalPlayer.npcTypeNoAggro[332] = true;
                Main.LocalPlayer.npcTypeNoAggro[430] = true;
                Main.LocalPlayer.npcTypeNoAggro[431] = true;
                Main.LocalPlayer.npcTypeNoAggro[432] = true;
                Main.LocalPlayer.npcTypeNoAggro[433] = true;
                Main.LocalPlayer.npcTypeNoAggro[434] = true;
                Main.LocalPlayer.npcTypeNoAggro[435] = true;
                Main.LocalPlayer.npcTypeNoAggro[436] = true;
                Main.LocalPlayer.npcTypeNoAggro[489] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<ImpaledZombie>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<Abomination>()] = true;
                Main.LocalPlayer.npcTypeNoAggro[ModContent.NPCType<Biter>()] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ZombieRepellentBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("AquaPotion"), (PPP) =>
            {
                Main.LocalPlayer.ignoreWater = true;
                if (Main.LocalPlayer.wet)
                {
                    Main.LocalPlayer.moveSpeed += 0.1f;
                    Main.LocalPlayer.runAcceleration += 0.08f;
                }
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("AquaAffinity")] = true;
            });
            RegisterCallback(ThoriumID.Item("FrenzyPotion"), (PPP) =>
            {
                PPP.TP.attackSpeed += 0.08f;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("Frenzy")] = true;
            });
            RegisterCallback(ThoriumID.Item("GlowingPotion"), (PPP) =>
            {
                PPP.TP.radiantBoost += 0.1f;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("RadiantBoost")] = true;
            });
            RegisterCallback(ThoriumID.Item("DashPotion"), (PPP) =>
            {
                Main.LocalPlayer.dash = 1;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("DashBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("AssassinPotion"), (PPP) =>
            {
                PPP.TP.assassinThrower = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("AssassinBuff")] = true;
            });
            RegisterCallback(ThoriumID.Item("HydrationPotion"), (PPP) =>
            {
                PPP.TP.techRechargeBonus = 30;
                PPP.TP.throwerExhaustionRegenBonus += 0.25f;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("HydrationBuff")] = true;
            });

            #endregion Potions
            #region Coatings

            RegisterCallback(ThoriumID.Item("ExplosiveCoatingItem"), (PPP) =>
            {
                PPP.TP.explodeCoat = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ExplosionCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("FrostCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("GorganCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("SporeCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ToxicCoating")] = true;
            });
            RegisterCallback(ThoriumID.Item("FrostCoatingItem"), (PPP) =>
            {
                PPP.TP.freezeCoat = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ExplosionCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("FrostCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("GorganCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("SporeCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ToxicCoating")] = true;
            });
            RegisterCallback(ThoriumID.Item("GorganCoatingItem"), (PPP) =>
            {
                PPP.TP.gorganCoat = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ExplosionCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("FrostCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("GorganCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("SporeCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ToxicCoating")] = true;
            });
            RegisterCallback(ThoriumID.Item("SporeCoatingItem"), (PPP) =>
            {
                PPP.TP.sporeCoat = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ExplosionCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("FrostCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("GorganCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("SporeCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ToxicCoating")] = true;
            });
            RegisterCallback(ThoriumID.Item("ToxicCoatingItem"), (PPP) =>
            {
                PPP.TP.sporeCoat = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ExplosionCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("FrostCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("GorganCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("SporeCoating")] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("ToxicCoating")] = true;
            });

            #endregion Coatings
            #region Misc

            RegisterCallback(ThoriumID.Item("CactusFruit"), (PPP) =>
            {
                Main.LocalPlayer.buffImmune[46] = true;
                Main.LocalPlayer.buffImmune[47] = true;
                Main.LocalPlayer.buffImmune[22] = true;
                Main.LocalPlayer.buffImmune[23] = true;
                Main.LocalPlayer.buffImmune[30] = true;
                Main.LocalPlayer.buffImmune[31] = true;
                Main.LocalPlayer.buffImmune[32] = true;
                Main.LocalPlayer.buffImmune[33] = true;
                Main.LocalPlayer.buffImmune[35] = true;
                Main.LocalPlayer.buffImmune[69] = true;
                Main.LocalPlayer.buffImmune[80] = true;
                Main.LocalPlayer.buffImmune[156] = true;
                Main.LocalPlayer.buffImmune[149] = true;
                Main.LocalPlayer.buffImmune[ThoriumID.Buff("CactusJuice")] = true;
            });

            #endregion Misc
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
            PotPotInterface = null;
            BuffCallbacks = null;
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