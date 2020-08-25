using PotPot.UI;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using PotPot.Buffs;
using Terraria.ID;
//using CalamityMod.CalPlayer;

namespace PotPot.Players
{
    class PotPotPlayer : ModPlayer
    {
        public IList<Item> PotPotContent;
        public VanillaPotionBuffs vpb = VanillaPotionBuffs.None;
        public VanillaFlaskBuffs vfb = VanillaFlaskBuffs.None;
        public VanillaMiscBuffs mb = VanillaMiscBuffs.None;
        public CalamityPotionBuffs cpb = CalamityPotionBuffs.None;
        public CalamityDrunkPrincessBuffs cdpb = CalamityDrunkPrincessBuffs.None;

        public PotPotPlayer()
        {
            PotPotContent = new List<Item>();
            vpb = VanillaPotionBuffs.Lifeforce | VanillaPotionBuffs.Ironskin | VanillaPotionBuffs.Rage;
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

            if ((vpb & VanillaPotionBuffs.AmmoReservation ) != 0)
            {
                player.ammoPotion = true;
                player.buffImmune[BuffID.AmmoReservation] = true;
            }
            if ((vpb & VanillaPotionBuffs.Archery) != 0)
            {
                player.archery = true;
                player.buffImmune[BuffID.Archery] = true;
            }
            if ((vpb & VanillaPotionBuffs.Battle) != 0)
            {
                player.enemySpawns = true;
                player.buffImmune[BuffID.Battle] = true;
            }
            if ((vpb & VanillaPotionBuffs.Builder) != 0)
            {
                player.tileSpeed += 0.25f;
                player.wallSpeed += 0.25f;
                player.blockRange++;
                player.buffImmune[BuffID.Builder] = true;
            }
            if ((vpb & VanillaPotionBuffs.Calming) != 0)
            {
                player.calmed = true;
                player.buffImmune[BuffID.Calm] = true;
            }
            if ((vpb & VanillaPotionBuffs.Crate) != 0)
            {
                player.cratePotion = true;
                player.buffImmune[BuffID.Crate] = true;
            }
            if ((vpb & VanillaPotionBuffs.Dangersense) != 0)
            {
                player.dangerSense = true;
                player.buffImmune[BuffID.Dangersense] = true;
            }
            if ((vpb & VanillaPotionBuffs.Endurance) != 0)
            {
                player.endurance += 0.1f;
                player.buffImmune[BuffID.Endurance] = true;
            }
            if ((vpb & VanillaPotionBuffs.Featherfall) != 0)
            {
                player.slowFall = true;
                player.buffImmune[BuffID.Featherfall] = true;
            }
            if ((vpb & VanillaPotionBuffs.Fishing) != 0)
            {
                player.fishingSkill += 15;
                player.buffImmune[BuffID.Fishing] = true;
            }
            if ((vpb & VanillaPotionBuffs.Flipper) != 0)
            {
                player.ignoreWater = true;
                player.accFlipper = true;
                player.buffImmune[BuffID.Flipper] = true;
            }
            if ((vpb & VanillaPotionBuffs.Gills) != 0)
            {
                player.gills = true;
                player.buffImmune[BuffID.Gills] = true;
            }
            if ((vpb & VanillaPotionBuffs.Gravitation) != 0)
            {
                player.gravControl = true;
                player.buffImmune[BuffID.Gravitation] = true;
            }
            if ((vpb & VanillaPotionBuffs.Heartreach) != 0)
            {
                player.lifeMagnet = true;
                player.buffImmune[BuffID.Heartreach] = true;
            }
            if ((vpb & VanillaPotionBuffs.Hunter) != 0)
            {
                player.detectCreature = true;
                player.buffImmune[BuffID.Hunter] = true;
            }
            if ((vpb & VanillaPotionBuffs.Inferno) != 0)
            {
                // use vanilla buff
                player.buffImmune[BuffID.Inferno] = true;
            }
            if ((vpb & VanillaPotionBuffs.Invisibility) != 0)
            {
                player.invis = true;
                player.buffImmune[BuffID.Invisibility] = true;
            }
            if ((vpb & VanillaPotionBuffs.Ironskin) != 0)
            {
                player.statDefense += 8;
                player.buffImmune[BuffID.Ironskin] = true;
            }
            if ((vpb & VanillaPotionBuffs.Lifeforce) != 0)
            {
                player.lifeForce = true;
                player.statLifeMax2 += base.player.statLifeMax / 5 / 20 * 20;
                player.buffImmune[BuffID.Lifeforce] = true;
            }
            if ((vpb & VanillaPotionBuffs.Love) != 0)
            {
                player.buffImmune[BuffID.Lovestruck] = true;
            }
            if ((vpb & VanillaPotionBuffs.MagicPower) != 0)
            {
                player.magicDamage += 0.2f;
                player.buffImmune[BuffID.MagicPower] = true;
            }
            if ((vpb & VanillaPotionBuffs.ManaRegen) != 0)
            {
                player.manaRegenBuff = true;
                player.buffImmune[BuffID.ManaRegeneration] = true;
            }
            if ((vpb & VanillaPotionBuffs.Mining) != 0)
            {
                player.pickSpeed -= 0.25f;
                player.buffImmune[BuffID.Mining] = true;
            }
            if ((vpb & VanillaPotionBuffs.NightOwl) != 0)
            {
                player.nightVision = true;
                player.buffImmune[BuffID.NightOwl] = true;
            }
            if ((vpb & VanillaPotionBuffs.ObsidianSkin) != 0)
            {
                player.lavaImmune = true;
                player.fireWalk = true;
                player.buffImmune[24] = true;
                player.buffImmune[1] = true;
                player.buffImmune[BuffID.ObsidianSkin] = true;
            }
            if ((vpb & VanillaPotionBuffs.Rage) != 0)
            {
                //calamity rogue crit
                player.meleeCrit += 10;
                player.thrownCrit += 10;
                player.magicCrit += 10;
                player.rangedCrit += 10;
                if (calamityMod != null)
                {
                    //calamityPlayer.xRage = true;
                }
                player.buffImmune[BuffID.Rage] = true;
            }
            if ((vpb & VanillaPotionBuffs.Regeneration) != 0)
            {
                player.lifeRegen += 4;
                player.buffImmune[BuffID.Regeneration] = true;
            }
            if ((vpb & VanillaPotionBuffs.Shine) != 0)
            {
                Lighting.AddLight((int)base.player.Center.X >> 4, (int)base.player.Center.Y >> 4, 0.8f, 0.95f, 1f);
                player.buffImmune[BuffID.Shine] = true;
            }
            if ((vpb & VanillaPotionBuffs.Sonar) != 0)
            {
                player.sonarPotion = true;
                player.buffImmune[BuffID.Sonar] = true;
            }
            if ((vpb & VanillaPotionBuffs.Spelunker) != 0)
            {
                player.findTreasure = true;
                player.buffImmune[BuffID.Spelunker] = true;
            }
            if ((vpb & VanillaPotionBuffs.Stink) != 0)
            {
                player.stinky = true;
                player.buffImmune[BuffID.Stinky] = true;
            }
            if ((vpb & VanillaPotionBuffs.Summoning) != 0)
            {
                player.maxMinions++;
                player.buffImmune[BuffID.Summoning] = true;
            }
            if ((vpb & VanillaPotionBuffs.Swiftness) != 0)
            {
                player.moveSpeed += 0.25f;
                player.buffImmune[BuffID.Swiftness] = true;
            }
            if ((vpb & VanillaPotionBuffs.Thorns) != 0)
            {
                player.thorns += 0.33f;
                player.buffImmune[BuffID.Thorns] = true;
            }
            if ((vpb & VanillaPotionBuffs.Titan) != 0)
            {
                player.kbBuff = true;
                player.buffImmune[BuffID.Titan] = true;
            }
            if ((vpb & VanillaPotionBuffs.Warmth) != 0)
            {
                player.resistCold = true;
                player.buffImmune[BuffID.Warmth] = true;
            }
            if ((vpb & VanillaPotionBuffs.WaterWalking) != 0)
            {
                player.waterWalk = true;
                player.buffImmune[BuffID.WaterWalking] = true;
            }
            if ((vpb & VanillaPotionBuffs.Wrath) != 0)
            {
                player.meleeDamage += 0.1f;
                player.thrownDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.minionDamage += 0.1f;

                if ( calamityMod != null )
                {
                    //calamityPlayer.xWrath = true;  
                }
                player.buffImmune[BuffID.Wrath] = true;
            }

            // if VanillaPotionBuffs & FLAG != 0
            // TODO apply buff changes here
        }
        public void ApplyBuffs(Player player)
        {
            foreach (Item i in this.PotPotContent)
            {
                switch(i.buffType)
                {
                    // set flags
                }
            }
        }
    }
}
