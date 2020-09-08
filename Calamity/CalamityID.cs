namespace PotPot.Calamity
{
    public static class CalamityID
    {
        public static int Item(string itemname)
        { 
            return PotPot.Instance.Calamity.ItemType(itemname);
        }

        public static int Buff(string buffname)
        {
            return PotPot.Instance.Calamity.BuffType(buffname);
        }
    }
}
