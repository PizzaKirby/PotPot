using Terraria.GameContent.UI.Elements;

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
