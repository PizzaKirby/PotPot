using System;

namespace PotPot.Buffs
{
    [Flags]
    enum VanillaPotionBuffs : UInt64
    {
        None = 1 << 0,
        AmmoReservation = 1 << 1,
        Archery = 1 << 2,
        Battle = 1 << 3,
        Builder = 1 << 4,
        Calming = 1 << 5,
        Crate = 1 << 6,
        Dangersense = 1 << 7,
        Endurance = 1 << 8,
        Featherfall = 1 << 9,
        Fishing = 1 << 10,
        Flipper = 1 << 11,
        Gills = 1 << 12,
        Gravitation = 1 << 13,
        Heartreach = 1 << 14,
        Hunter = 1 << 15,
        Inferno = 1 << 16,
        Invisibility = 1 << 17,
        Ironskin = 1 << 18,
        Lifeforce = 1 << 19,
        Love = 1 << 20,
        MagicPower = 1 << 21,
        ManaRegen = 1 << 22,
        Mining = 1 << 23,
        NightOwl = 1 << 24,
        ObsidianSkin = 1 << 25,
        Rage = 1 << 26,
        Regeneration = 1 << 27,
        Shine = 1 << 28,
        Sonar = 1 << 29,
        Spelunker = 1 << 30,
        Stink = 1L << 31,
        Summoning = 1 << 32,
        Swiftness = 1 << 33,
        Thorns = 1 << 34,
        Titan = 1 << 35,
        Warmth = 1 << 36,
        WaterWalking = 1 << 37,
        Wrath = 1 << 38
    }

    [Flags]
    enum VanillaFlaskBuffs : UInt16
    {
        None = 1 << 0,
        CursedFlames = 1 << 1,
        Fire = 1 << 2,
        Gold = 1 << 3,
        Ichor = 1 << 4,
        Nanites = 1 << 5,
        Party = 1 << 6,
        Poison = 1 << 7,
        Venom = 1 << 8
    }

    [Flags]
    enum VanillaMiscBuffs : UInt16
    {
        None = 1 << 0,
        WellFed = 1 << 1,
        Tipsy = 1 << 2,
        AmmoBox = 1 << 3,
        Bewitched = 1 << 4,
        Claivoyance = 1 << 5,
        Sharpened = 1 << 6,
        CozyFire = 1 << 7,
        HeartLamp = 1 << 8,
        Honey = 1 << 9,
        PeaceCandle = 1 << 10,
        Star = 1 << 11,
    }

    [Flags]
    enum CalamityPotionBuffs : UInt32
    {
        None = 1 << 0,
        AnechoicCoating = 1 << 1,
        AstralInjection = 1 << 2,
        Bounding = 1 << 3,
        CalamitasBrew = 1 << 4,
        Calcium = 1 << 5,
        CeaselessHunger = 1 << 6,
        Crumbling = 1 << 7,
        DraconicElixir = 1 << 8,
        GravityNormalizer = 1 << 9,
        HolyWrath = 1 << 10,
        Penumbra = 1 << 11,
        Photosynthesis = 1 << 12,
        ProfranedRage = 1 << 13,
        Revivify = 1 << 14,
        Shattering = 1 << 15,
        Shadow = 1 << 16,
        Soaring = 1 << 17,
        Sulphurskin = 1 << 18,
        Tesla = 1 << 19,
        TitanScale = 1 << 20,
        Triumph = 1 << 21,
        Zen = 1 << 22,
        Zerg = 1 << 23,
        Cadence = 1 << 24,
        Omniscience = 1 << 25,
        YharimsStimulants = 1 << 26
    }

    [Flags]
    enum CalamityDrunkPrincessBuffs : UInt32
    {
        None = 1 << 0,
        BloodyMary = 1 << 1,
        CarribeanRum = 1 << 2,
        CinnamonRoll = 1 << 3,
        Everclear = 1 << 4,
        EvergreenGin = 1 << 5,
        FabsolsVodka = 1 << 6,
        Fireball = 1 << 7,
        Moonshine = 1 << 8,
        MoscowMule = 1 << 9,
        Rum = 1 << 10,
        Screwdriver = 1 << 11,
        StarBeamRye = 1 << 12,
        Tequila = 1 << 13,
        TequilaSunrise = 1 << 14,
        Vodka = 1 << 15,
        Whiskey = 1 << 16,
        WhiteWine = 1 << 17
    }
}


