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

namespace PotPot.UI
{
    class PotPotButton : UIPanel
    {
        UIText btnText;
        public PotPotButton(string buttonText)
        {
            this.Width.Set(64, 0);
            this.Height.Set(64, 0);
            btnText = new UIText(buttonText,0.8f);
            btnText.HAlign = btnText.VAlign = 0.5f;
            this.Append(btnText);
        }
    }
}
