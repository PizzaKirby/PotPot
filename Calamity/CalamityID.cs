namespace PotPot.Calamity
{
    public static class CalamityID
    {
        public static int Item(string itemname)
        {
            if(PotPot.Instance.Calamity != null)
                return PotPot.Instance.Calamity.ItemType(itemname);
            return 0;
        }

        public static int Buff(string buffname)
        {
            if (PotPot.Instance.Calamity != null)
                return PotPot.Instance.Calamity.BuffType(buffname);
            return 0;
        }
    }
}
