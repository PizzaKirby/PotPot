using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using PotPot.UI;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.CalPlayer;

namespace PotPot
{
    public class PotPot : Mod
    {
        //for future version
        //public override uint ExtraBuffSlots { get { return 22; } }

        internal static PotPot Instance;
        private Mod Calamity => ModLoader.GetMod("CalamityMod");
        internal CalamityPlayer RefCalamityPlayer;
        internal UserInterface PotPotInterface;
        internal PotPotUI MainUI;
        private GameTime _lastUpdateUiGameTime;

        public PotPot()
        {
            Instance = this;
        }

        public override void Load()
        {
            Logger.InfoFormat("{0} loading", Name);

            if (!Main.dedServ)
            {
                PotPotInterface = new UserInterface();
            }

            if ( Calamity != null)
            {
                On.CalamityMod.CalPlayer.CalamityPlayer.OnEnterWorld += CalamityPlayer_OnEnterWorld;
            }
        }

        private void CalamityPlayer_OnEnterWorld(On.CalamityMod.CalPlayer.CalamityPlayer.orig_OnEnterWorld orig, CalamityPlayer self, object player)
        {
            RefCalamityPlayer = self;
            orig(self,player);
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