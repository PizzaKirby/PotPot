using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;
using Terraria.ModLoader;
using Steamworks;
using PotPot.Players;

namespace PotPot
{
    internal class PotPotItemSlot : UIElement
    {
        public Item Item;
        private readonly int _context;
        private readonly float _scale;
        internal Func<Item,Item, bool> ValidItemFunc;
        public event EventHandler<ItemChangedEventArgs> onItemChanged;
        public PotPotItemSlot(int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            Item = new Item();
            Item.SetDefaults(0);

            Width.Set(Main.inventoryBack9Texture.Width * scale, 0f);
            Height.Set(Main.inventoryBack9Texture.Height * scale, 0f);
        }

        protected virtual void OnItemChangedEvent(ItemChangedEventArgs e)
        {
            EventHandler<ItemChangedEventArgs> handler = onItemChanged;
            handler?.Invoke(this, e);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (ValidItemFunc == null || ValidItemFunc(Main.mouseItem, this.Item))
                {
                    ItemChangedEventArgs args = new ItemChangedEventArgs();
                    args.Old = this.Item;

                    ItemSlot.Handle(ref this.Item, _context);

                    args.New = this.Item;
                    if(args.New != args.Old)
                        OnItemChangedEvent(args);
                }
            }
            ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft());
            Main.inventoryScale = oldScale;
        }

        public void SetItem(Item item)
        {

            if (ValidItemFunc == null || ValidItemFunc(item, this.Item))
            {
                this.Item = item;
                ItemSlot.Handle(ref this.Item, _context);
            }
        }

        public void SetItem(Item item, bool fireEvent)
        {

        }

    }

    public class ItemChangedEventArgs : EventArgs
    {
        public Item Old { get; set; }
        public Item New { get; set; }
    }
}