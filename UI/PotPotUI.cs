using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using PotPot.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI.Gamepad;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI.ModBrowser;
using Terraria.ModLoader.Core;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader.IO;
using PotPot.Players;
using log4net.Repository.Hierarchy;
using Terraria.ModLoader.IO;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent.Dyes;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using PotPot.UI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.UI.Gamepad;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI.ModBrowser;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;

namespace PotPot.UI
{
    class PotPotUI : UIState
    {
        private List<PotPotItemSlot> potInv;
        private int SLOTCOUNT = 25;
        public PotPotUI()
        {
            potInv = new List<PotPotItemSlot>();
        }

        public override void OnInitialize()
        {
            PotPotDraggablePanel panel = new PotPotDraggablePanel();
            panel.Width.Set(Main.inventoryBackTexture.Width * 6f, 0);
            panel.Height.Set(Main.inventoryBackTexture.Height * 6f, 0);
            panel.Top.Set(340, 0);
            panel.Left.Set(340, 0);
            Append(panel);

            Player player = Main.LocalPlayer;
            PotPotPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PotPotPlayer>();

            for ( int i = 0; i < SLOTCOUNT; i++)
            {
                PotPotItemSlot potionSlot = new PotPotItemSlot();
                potionSlot.ValidItemFunc = this.IsValidItem;
                potionSlot.Left.Set(Main.inventoryBackTexture.Width * (i % 5) + (Main.inventoryBackTexture.Width / 4), 0);
                potionSlot.Top.Set(Main.inventoryBackTexture.Height * (i / 5) + (Main.inventoryBackTexture.Width / 4), 0);
                potionSlot.onItemChanged += OnItemChanged;
                potInv.Add(potionSlot);
                panel.Append(potionSlot);
            }

            if (modPlayer.PotPotContent != null)
            {
                int index = 0;
                foreach (Item i in modPlayer.PotPotContent)
                {
                    modPlayer.mod.Logger.Info("[STORAGE@" + index + "] " + i);
                    potInv[index].SetItem(i);
                    modPlayer.mod.Logger.Debug("&&" + potInv[index].Item);
                    index++;
                    if (index >= SLOTCOUNT)
                        break;
                }
            }

            UIText btnClose = new UIText("X", 1f);
            btnClose.HAlign = 0.99f;
            btnClose.VAlign = 0.01f;
            btnClose.TextColor = Color.Black;
            btnClose.OnClick += (UIMouseEvent evt, UIElement listeningElement) => { PotPot.Instance.HideUI(); Terraria.Main.NewText("Hewwo"); };
            btnClose.OnMouseOver += (UIMouseEvent evt, UIElement listeningElement) => btnClose.TextColor = Color.White;
            btnClose.OnMouseOut += (UIMouseEvent evt, UIElement listeningElement) => btnClose.TextColor = Color.Black;
            panel.Append(btnClose);
        }

        static void OnItemChanged(Object sender, ItemChangedEventArgs e)
        {
            PotPotPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PotPotPlayer>();
            modPlayer.mod.Logger.Info("EventArgs : OLD[" + e.Old +"] | NEW[" + e.New + "]");
            modPlayer.PotPotContent.Remove(e.Old);
            modPlayer.PotPotContent.Add(e.New);
        }

        private bool IsValidItem(Item newItem, Item currentItem)
        {
            if (currentItem.buffType != 0)
            {
                if (newItem.buffType != 0)
                    return true;

                if (newItem.Name.Equals(""))
                    return true;
            }
            else
            {
                if (newItem.buffType != 0)
                    return true;
            }
            return false;
        }
        public override void OnDeactivate()
        {
            //update contents
        }

    }
}
