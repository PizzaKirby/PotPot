namespace PotPot.Thorium
{
    public static class ThoriumID
    {
        public static int Item(string itemname)
        {
            if (PotPot.Instance.Thorium != null)
                return PotPot.Instance.Thorium.ItemType(itemname);
            return 0;
        }

        public static int Buff(string buffname)
        {
            if (PotPot.Instance.Thorium != null)
                return PotPot.Instance.Thorium.BuffType(buffname);
            return 0;
        }
    }
}
