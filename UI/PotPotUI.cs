﻿using Microsoft.Xna.Framework;
using PotPot.Players;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace PotPot.UI
{
    class PotPotUI : UIState
    {
        private readonly List<PotPotItemSlot> potInv;
        private readonly int SLOTCOUNT = 49;
        public PotPotUI()
        {
            potInv = new List<PotPotItemSlot>();
        }

        public override void OnInitialize()
        {
            PotPotDraggablePanel panel = new PotPotDraggablePanel();
            panel.Width.Set(Main.inventoryBackTexture.Width * 8f, 0);
            panel.Height.Set(Main.inventoryBackTexture.Height * 8f, 0);
            panel.Top.Set(560, 0);
            panel.Left.Set(500, 0);
            Append(panel);

            PotPotPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PotPotPlayer>();

            for ( int i = 0; i < SLOTCOUNT; i++)
            {
                PotPotItemSlot potionSlot = new PotPotItemSlot();
                potionSlot.ValidItemFunc = this.IsValidItem;
                potionSlot.Left.Set(Main.inventoryBackTexture.Width * (i % 7) + (Main.inventoryBackTexture.Width / 4) , 0);
                potionSlot.Top.Set(Main.inventoryBackTexture.Height * (i / 7) + (Main.inventoryBackTexture.Width / 4) , 0);
                potionSlot.OnItemChanged += OnItemChanged;
                potInv.Add(potionSlot);
                panel.Append(potionSlot);
            }

            if (modPlayer.PotPotContent != null)
            {
                int index = 0;
                foreach (Item i in modPlayer.PotPotContent)
                {
                    potInv[index].SetItem(i);
                    index++;
                    if (index >= SLOTCOUNT)
                        break;
                }
            }

            UIText btnClose = new UIText("X", 1f);
            btnClose.HAlign = 0.99f;
            btnClose.VAlign = 0.01f;
            btnClose.TextColor = Color.Black;
            btnClose.OnClick += (UIMouseEvent evt, UIElement listeningElement) => { PotPot.Instance.HideUI(); modPlayer.ApplyBuffs(); };
            btnClose.OnMouseOver += (UIMouseEvent evt, UIElement listeningElement) => btnClose.TextColor = Color.White;
            btnClose.OnMouseOut += (UIMouseEvent evt, UIElement listeningElement) => btnClose.TextColor = Color.Black;
            panel.Append(btnClose);
        }

        static void OnItemChanged(Object sender, ItemChangedEventArgs e)
        {
            PotPotPlayer modPlayer = Main.LocalPlayer.GetModPlayer<PotPotPlayer>();
            modPlayer.PotPotContent.Remove(e.Old);
            modPlayer.PotPotContent.Add(e.New);
        }

        private bool IsValidItem(Item newItem, Item currentItem)
        {

            if (newItem.Name == "")
            {
                return true;
            }
            else if (PotPot.Instance.IsRegisteredItem(newItem.type) || newItem.Name.Contains("Campfire")) // TODO check new whitelist here
            {
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
