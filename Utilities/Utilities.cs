using PotPot.Buffs;
using System;
using System.Collections.Generic;
using Terraria;

namespace PotPot.Utilities
{
    struct Utilities
    {
        static IEnumerable<VanillaBuffs> GetFlags(VanillaBuffs input)
        {
            foreach (VanillaBuffs value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                {
                    Main.NewText(value);
                    yield return value;
                }
        }
    }
}