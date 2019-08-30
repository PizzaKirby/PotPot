using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace PotPot.Items
{
    public class PotPotMeleePot : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("PotPot pot melee edition");
            DisplayName.SetDefault("PotPot Melee Pot");
        }

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.useStyle = 2;
            item.useAnimation = 15;
            item.useTime = 15;
            item.rare = 13;
            item.UseSound = SoundID.Item3;
            item.buffType = BuffID.Ironskin;
            item.buffTime = int.MaxValue;
            item.expert = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WaterCandle, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(BuffID.Endurance, this.item.buffTime);
            player.AddBuff(BuffID.Swiftness, this.item.buffTime);
            player.AddBuff(BuffID.WellFed, this.item.buffTime);
            player.AddBuff(BuffID.WeaponImbueIchor, this.item.buffTime);

            Mod CMod = ModLoader.GetMod("CalamityMod");
            if (CMod != null)
            {
                player.AddBuff(CMod.BuffType("Cadence"), this.item.buffTime);
                player.AddBuff(CMod.BuffType("YharimPower"), this.item.buffTime);
                player.AddBuff(CMod.BuffType("TriumphBuff"), this.item.buffTime);
                player.AddBuff(CMod.BuffType("TitanScale"), this.item.buffTime);
                player.AddBuff(CMod.BuffType("Omniscience"), this.item.buffTime);
                player.AddBuff(CMod.BuffType("ArmorShattering"), this.item.buffTime);
                player.AddBuff(CMod.BuffType("AbyssalWeapon"), this.item.buffTime);
            }
            return true;
        }
    }
}