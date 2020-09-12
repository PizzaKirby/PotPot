using PotPot.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.useAnimation = 15;
            item.useTime = 15;
            item.rare = 13;
            item.UseSound = SoundID.Item4;
            item.maxStack = 1;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
        public override bool UseItem(Player player)
        {
            PotPot.Instance.ShowUI();
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
    }
}
