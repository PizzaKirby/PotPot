using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace PotPot
{
    public class PotPot : Mod
    {
        internal static PotPot Instance;
        public PotPot()
        {
            Instance = this;
        }

        public override void Load()
        {
            Logger.InfoFormat("{0} loading", Name);
        }

        public override void Unload()
        {
            Instance = null;
        }
    }
}