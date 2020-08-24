using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using PotPot.Players;

namespace PotPot.Items
{
    class PotPotPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Multifunctional Potion");
            DisplayName.SetDefault("PotPot Potion");
        }
        public override void SetDefaults()
        {
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.rare = 13;
            item.UseSound = SoundID.Item3;
            item.buffType = mod.BuffType("PotPotDummyBuff");
            item.buffTime = 0;
            item.expert = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 1);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool UseItem(Player player)
        {
            player.ClearBuff(mod.BuffType("PotPotDummyBuff"));
            if (player.altFunctionUse == 2)
            {
                PotPot.Instance.ShowUI();
                return true;
            }
            PotPotPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PotPotPlayer>();
            modPlayer.ApplyBuffs(player);
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
