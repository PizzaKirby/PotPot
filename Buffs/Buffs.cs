﻿using System;

namespace PotPot.Buffs
{
    [Flags]
    enum VanillaBuffs : UInt64
    {
        None = 0,
        AmmoReservation = 1L << 1,
        Archery = 1L << 2,
        Battle = 1L << 3,
        Builder = 1L << 4,
        Calming = 1L << 5,
        Crate = 1L << 6,
        Dangersense = 1L << 7,
        Endurance = 1L << 8,
        Featherfall = 1L << 9,
        Fishing = 1L << 10,
        Flipper = 1L << 11,
        Gills = 1L << 12,
        Gravitation = 1L << 13,
        Heartreach = 1L << 14,
        Hunter = 1L << 15,
        Inferno = 1L << 16,
        Invisibility = 1L << 17,
        Ironskin = 1L << 18,
        Lifeforce = 1L << 19,
        Lovestruck = 1L << 20,
        MagicPower = 1L << 21,
        ManaRegen = 1L << 22,
        Mining = 1L << 23,
        NightOwl = 1L << 24,
        ObsidianSkin = 1L << 25,
        Rage = 1L << 26,
        Regeneration = 1L << 27,
        Shine = 1L << 28,
        Sonar = 1L << 29,
        Spelunker = 1L << 30,
        Stinky = 1L << 31,
        Summoning = 1L << 32,
        Swiftness = 1L << 33,
        Thorns = 1L << 34,
        Titan = 1L << 35,
        Warmth = 1L << 36,
        WaterWalking = 1L << 37,
        Wrath = 1L << 38,
        FlaskCursedFlames = 1L << 39,
        FlaskFire = 1L << 40,
        FlaskGold = 1L << 41,
        FlaskIchor = 1L << 42,
        FlaskNanites = 1L << 43,
        FlaskParty = 1L << 44,
        FlaskPoison = 1L << 45,
        FlaskVenom = 1L << 46,
        WellFed = 1L << 47,
        Tipsy = 1L << 48,
        AmmoBox = 1L << 49,
        Bewitched = 1L << 50,
        Clairvoyance = 1L << 51,
        Sharpened = 1L << 52,
        Campfire = 1L << 53,
        HeartLamp = 1L << 54,
        Honey = 1L << 55,
        PeaceCandle = 1L << 56,
        StarInBottle = 1L << 57,
    }

    [Flags]
    enum CalamityBuffs : UInt64
    {
        None = 0,
        AnechoicCoating = 1 << 1,
        AstralInjection = 1L << 2,
        Bounding = 1L << 3,
        CalamitasBrew = 1L << 4,
        Calcium = 1L << 5,
        CeaselessHunger = 1L << 6,
        Crumbling = 1L << 7,
        DraconicElixir = 1L << 8,
        GravityNormalizer = 1L << 9,
        HolyWrath = 1L << 10,
        Penumbra = 1L << 11,
        Photosynthesis = 1L << 12,
        ProfanedRage = 1L << 13,
        Revivify = 1L << 14,
        Shattering = 1L << 15,
        Shadow = 1L << 16,
        Soaring = 1L << 17,
        Sulphurskin = 1 << 18,
        Tesla = 1L << 19,
        TitanScale = 1L << 20,
        Triumph = 1L << 21,
        Zen = 1L << 22,
        Zerg = 1L << 23,
        Cadance = 1L << 24,
        Omniscience = 1L << 25,
        YharimsStimulants = 1L << 26,
        BloodyMary = 1L << 27,
        CarribeanRum = 1L << 28,
        CinnamonRoll = 1L << 29,
        Everclear = 1L << 30,
        EvergreenGin = 1L << 31,
        FabsolsVodka = 1L << 32,
        Fireball = 1L << 33,
        Moonshine = 1L << 34,
        MoscowMule = 1L << 35,
        Rum = 1L << 36,
        Screwdriver = 1L << 37,
        StarBeamRye = 1L << 38,
        Tequila = 1L << 39,
        TequilaSunrise = 1L << 40,
        Vodka = 1L << 41,
        Whiskey = 1L << 42,
        WhiteWine = 1L << 43,
        AureusCell = 1L << 44,
        DraconicSurgeCD = 1L << 45,
        BlueCandle = 1L << 46, // blueCandle | BlueSpeedCandle
        PinkCandle = 1L << 47, // pinkCandle | PinkHealthCandle
        PurpleCandle = 1L << 48, // purpleCandle | PurpleDefenseCandle
        YellowCandle = 1L << 49, // yellowCandle | YellowDamageCandle
        OddMushroom = 1L << 50, // Trippy 
        BrimstoneLore = 1L << 51
    }

    enum CalamityBuffID : int
    {
        BloodyMary = 207,
        CaribbeanRum = 208,
        CinnamonRoll = 209,
        Everclear = 210,
        EvergreenGin = 211,
        FabsolsVodka = 212,
        Fireball = 213,
        Moonshine = 216,
        MoscowMule = 217,
        Rum = 219,
        Screwdriver = 220,
        StarBeamRye = 221,
        Tequila = 222,
        TequilaSunrise = 223,
        Vodka = 225,
        Whiskey = 226,
        WhiteWine = 227,
        AnechoicCoating = 313,
        AstralInjection = 316,
        AureusCell = 7,
        Bounding = 319,
        Cadance = 320,
        CalamitasBrew = 312,
        CeaselessHunger = 322,
        Calcium = 321,
        Crumbling = 314,
        DraconicElixir = 323,
        GravityNormalizer = 324,
        HolyWrath = 325,
        Penumbra = 328,
        Photosynthesis = 329,
        Omniscience = 327,
        ProfanedRage = 330,
        Revivify = 331,
        Shadow = 332,
        Shattering = 315,
        Soaring = 333,
        Sulphurskin = 334,
        HadalStew = 26,
        Tesla = 335,
        TitanScale = 336,
        Triumph = 337,
        YharimsStimulants = 338,
        Zen = 339,
        Zerg = 340
    }
}