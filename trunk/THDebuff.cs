using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Common;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region BuffBurst
        //////done
        //private static readonly HashSet<int> BuffBurstHS = new HashSet<int>
        //    {
        //        51271, // "Pillar of Frost",
        //        49016, //"Unholy Frenzy",
        //        102560, // "Incarnation: Chosen of Elune",
        //        124974, //"Nature's Vigil",
        //        106952, //"Berserk ",
        //        19574, //"Bestial Wrath",
        //        3045, //"Rapid Fire",
        //        12472, //"Icy Veins",
        //        //"Time Warp",
        //        //"Tigereye Brew",
        //        31884, //"Avenging Wrath",
        //        105809, //"Holy Avenger",
        //        86698, //"Guardian of Ancient Kings",
        //        86669, //"Guardian of Ancient Kings",
        //        51713, //"Shadow Dance",
        //        51690, // "Killing Spree",
        //        13750, //Adrenaline Rush
        //        114049, // "Ascendance",
        //        114051, // "Ascendance",
        //        //"Bloodlust",
        //        113858, //"Dark Soul: Instability",
        //        1719, //"Recklessness",
        //        114207, //"Skull Banner",
        //    };
        //////done
        private static readonly HashSet<int> BuffBurstHS = new HashSet<int> { 
            0x1a436, 0x3004, 0x1da7f, 0xc847, 0xbf78, 0x190a0, 0x1e82e, 0x1a1c8, 0x4c76, 0xbe5, 0x30b8, 0x7c8c, 0x19d51, 0x152aa, 0x1528d, 0xca01, 
            0xc9ea, 0x35b6, 0x1bd81, 0x1bd83, 0x1bcc2, 0x6b7, 0x1be1f, 0xb74c, 0x3004
         };
        private static bool BuffBurst(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(aura => BuffBurstHS.Contains(aura.SpellId));
        }



        #endregion

        #region BuffEnrage

        private static readonly HashSet<int> BuffEnrageHS = new HashSet<int>
            {
                13046, //"Enraged",
                18499, //"Berserker Rage",
                55694, //"Enraged Regeneratione",
            };

        private static bool BuffEnrage(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(delegate(WoWAura aura)
            {
                if (!BuffEnrageHS.Contains(aura.SpellId))
                {
                    return (aura.get_Spell().get_DispelType() == ((WoWDispelType)((int)WoWDispelType.Enrage)));
                }
                return true;
            });
        }



        #endregion

        #region BuffHeal

        private static readonly HashSet<int> BuffHealHS = new HashSet<int>
            {
                774, // "Rejuvenation",
                8936, //"Regrowth",
                33763, //"Lifebloom",
                48438, //"Wild Growth",
//"Recuperate",
                61295, //"Riptide",
                974, //"Earth Shield",
                51730, //"Earthliving",
                139, //"Renew",
                77485, //"Echo of Light",
                77489, //"Divine Aegis",
                48500, //"Living Seed",
//"Blood Shield",
                115151, //"Renewing Mist",
                124682, //"Enveloping Mist",
                115175, //"Soothing Mist",
                124081, //"Zen Sphere",
                17, //"Power Word: Shield",
                33206, //"Pain Suppression",
                76669, //"Illuminated Healing",
                20925, //"Sacred Shield"
                //"Enraged Regeneratione")
            };

        private static bool BuffHeal(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(aura => BuffHealHS.Contains(aura.SpellId));
        }



        #endregion

        #region DebuffRootCanCloak

        private static readonly HashSet<int> DebuffRootCanCloakHS = new HashSet<int>
            {
                96294, //Chains of Ice (Chilblains)
                64695, //Earthgrab (Earthgrab Totem)
                339, //Entangling Roots
                113770, //Entangling Roots (Force of Nature - Balance Treants)
                19975, //Entangling Roots (Nature's Grasp)
                113275, //Entangling Roots (Symbiosis)
                113275, //Entangling Roots (Symbiosis)
                19185, //Entrapment
                33395, //Freeze
                63685, //Freeze (Frozen Power)
                122, //Frost Nova
                110693, //Frost Nova (Mage)
                87194, //Glyph of Mind Blast
                111340, //Ice Ward
                102359, //Mass Entanglement
                123407, //Spinning Fire Blossom
                54706, //Venom Web Spray (Silithid)
                114404, //Void Tendril's Grasp
                4167, //Web (Spider)
            };

        private static bool DebuffRootCanCloak(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(aura => DebuffRootCanCloakHS.Contains(aura.SpellId));
        }



        #endregion

        #region DebuffCC

        private static readonly HashSet<int> DebuffCCHS = new HashSet<int>
            {
                30217, //Adamantite Grenade
                89766, //Axe Toss (Felguard/Wrathguard)
                90337, //Bad Manner (Monkey)
                710, //Banish
                113801, //Bash (Force of Nature - Feral Treants)
                102795, //Bear Hug
                76780, //Bind Elemental
                117526, //Binding Shot
                2094, //Blind
                105421, //Blinding Light
                115752, //Blinding Light (Glyph of Blinding Light)
                123393, //Breath of Fire (Glyph of Breath of Fire)
                126451, //Clash
                122242, //Clash (not sure which one is right)
                67769, //Cobalt Frag Bomb
                118271, //Combustion Impact
                33786, //Cyclone
                113506, //Cyclone (Symbiosis)
                7922, //Charge Stun
                119392, //Charging Ox Wave
                1833, //Cheap Shot
                44572, //Deep Freeze
                54786, //Demonic Leap (Metamorphosis)
                99, //Disorienting Roar
                605, //Dominate Mind
                118895, //Dragon Roar
                31661, //Dragon's Breath
                77505, //Earthquake
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                30216, //Fel Iron Bomb
                105593, //Fist of Justice
                117418, //Fists of Fury
                3355, //Freezing Trap
                91800, //Gnaw
                1776, //Gouge
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                51514, //Hex
                2637, //Hibernate
                88625, //Holy Word: Chastise
                119072, //Holy Wrath
                5484, //Howl of Terror
                22703, //Infernal Awakening
                113056, //Intimidating Roar [Cowering in fear] (Warrior)
                113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                5246, //Intimidating Shout (aoe)
                20511, //Intimidating Shout (targeted)
                24394, //Intimidation
                408, //Kidney Shot
                119381, //Leg Sweep
                126246, //Lullaby (Crane)
                22570, //Maim
                115268, //Mesmerize (Shivarra)
                5211, //Mighty Bash
                91797, //Monstrous Blow (Dark Transformation)
                6789, //Mortal Coil
                115078, //Paralysis
                113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                126423, //Petrifying Gaze (Basilisk)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                9005, //Pounce
                102546, //Pounce (Incarnation)
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                118345, //Pulverize
                107079, //Quaking Palm
                13327, //Reckless Charge
                115001, //Remorseless Winter
                20066, //Repentance
                82691, //Ring of Frost
                6770, //Sap
                1513, //Scare Beast
                19503, //Scatter Shot
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                9484, //Shackle Undead
                30283, //Shadowfury
                132168, //Shockwave
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
                50519, //Sonic Blast (Bat)
                118905, //Static Charge (Capacitor Totem)
                56626, //Sting (Wasp)
                107570, //Storm Bolt
                10326, //Turn Evil
                20549, //War Stomp
                105771, //Warbringer
                19386, //Wyvern Sting
                108194, //Asphyxiate
            };

        private static bool DebuffCC(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(aura => DebuffCCHS.Contains(aura.SpellId));
        }

 


        #endregion

        #region DebuffCCDuration

        private static readonly HashSet<int> DebuffCCDurationHS = new HashSet<int>
            {
                30217, //Adamantite Grenade
                89766, //Axe Toss (Felguard/Wrathguard)
                90337, //Bad Manner (Monkey)
                710, //Banish
                113801, //Bash (Force of Nature - Feral Treants)
                102795, //Bear Hug
                76780, //Bind Elemental
                117526, //Binding Shot
                2094, //Blind
                105421, //Blinding Light
                115752, //Blinding Light (Glyph of Blinding Light)
                123393, //Breath of Fire (Glyph of Breath of Fire)
                126451, //Clash
                122242, //Clash (not sure which one is right)
                67769, //Cobalt Frag Bomb
                118271, //Combustion Impact
                33786, //Cyclone
                113506, //Cyclone (Symbiosis)
                7922, //Charge Stun
                119392, //Charging Ox Wave
                1833, //Cheap Shot
                44572, //Deep Freeze
                54786, //Demonic Leap (Metamorphosis)
                99, //Disorienting Roar
                605, //Dominate Mind
                118895, //Dragon Roar
                31661, //Dragon's Breath
                77505, //Earthquake
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                30216, //Fel Iron Bomb
                105593, //Fist of Justice
                117418, //Fists of Fury
                3355, //Freezing Trap
                91800, //Gnaw
                1776, //Gouge
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                51514, //Hex
                2637, //Hibernate
                88625, //Holy Word: Chastise
                119072, //Holy Wrath
                5484, //Howl of Terror
                22703, //Infernal Awakening
                113056, //Intimidating Roar [Cowering in fear] (Warrior)
                113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                5246, //Intimidating Shout (aoe)
                20511, //Intimidating Shout (targeted)
                24394, //Intimidation
                408, //Kidney Shot
                119381, //Leg Sweep
                126246, //Lullaby (Crane)
                22570, //Maim
                115268, //Mesmerize (Shivarra)
                5211, //Mighty Bash
                91797, //Monstrous Blow (Dark Transformation)
                6789, //Mortal Coil
                115078, //Paralysis
                113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                126423, //Petrifying Gaze (Basilisk)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                9005, //Pounce
                102546, //Pounce (Incarnation)
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                118345, //Pulverize
                107079, //Quaking Palm
                13327, //Reckless Charge
                115001, //Remorseless Winter
                20066, //Repentance
                82691, //Ring of Frost
                6770, //Sap
                1513, //Scare Beast
                19503, //Scatter Shot
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                9484, //Shackle Undead
                30283, //Shadowfury
                132168, //Shockwave
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
                50519, //Sonic Blast (Bat)
                118905, //Static Charge (Capacitor Totem)
                56626, //Sting (Wasp)
                107570, //Storm Bolt
                10326, //Turn Evil
                20549, //War Stomp
                105771, //Warbringer
                19386, //Wyvern Sting
                108194, //Asphyxiate
            };

        private static bool DebuffCCDuration(WoWUnit target, double duration, bool LogSpell = false)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            var DebuffAura = AuraCacheList.OrderByDescending(aura => aura.AuraCacheAura.TimeLeft)
                                          .FirstOrDefault(
                                              aura => aura.AuraCacheUnit == target.Guid &&
                                                      aura.AuraCacheAura.TimeLeft.TotalMilliseconds > duration &&
                                                      //aura.AuraCacheAura.IsActive &&
                                                      DebuffCCDurationHS.Contains(aura.AuraCacheId));


            if (DebuffAura != null)
            {
                if (LogSpell)
                {
                    Logging.Write("DebuffCCDuration on {0} {1} SpellID {2} Duration: {3}",
                                  target.SafeName,
                                  DebuffAura.AuraCacheAura.Name,
                                  DebuffAura.AuraCacheId,
                                  DebuffAura.AuraCacheAura.TimeLeft.TotalMilliseconds);
                }
                return true;
            }
            return false;
        }

        #endregion

        #region DebuffMagicCCDuration

        private static readonly HashSet<int> DebuffMagicCCDurationHS = new HashSet<int>
            {
                710, //Banish
                76780, //Bind Elemental
                117526, //Binding Shot
                105421, //Blinding Light
                115752, //Blinding Light (Glyph of Blinding Light)
                123393, //Breath of Fire (Glyph of Breath of Fire)
                118271, //Combustion Impact
                44572, //Deep Freeze
                99, //Disorienting Roar
                605, //Dominate Mind
                31661, //Dragon's Breath
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                105593, //Fist of Justice
                117418, //Fists of Fury
                3355, //Freezing Trap
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                2637, //Hibernate
                51514, //Hex
                88625, //Holy Word: Chastise
                119072, //Holy Wrath
                5484, //Howl of Terror
                126246, //Lullaby (Crane)
                115268, //Mesmerize (Shivarra)
                6789, //Mortal Coil
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                107079, //Quaking Palm
                115001, //Remorseless Winter
                20066, //Repentance
                82691, //Ring of Frost
                1513, //Scare Beast
                19503, //Scatter Shot
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                9484, //Shackle Undead
                30283, //Shadowfury
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
                50519, //Sonic Blast (Bat)
                118905, //Static Charge (Capacitor Totem)
                10326, //Turn Evil
                108194, //Asphyxiate
            };

        private static bool DebuffMagicCCDuration(WoWUnit target, double duration)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        aura.AuraCacheAura.TimeLeft.TotalMilliseconds > duration &&
                        //aura.AuraCacheAura.IsActive &&
                        DebuffMagicCCDurationHS.Contains(aura.AuraCacheId));
        }

        #endregion

        #region DebuffCCleanseASAP

        private static readonly HashSet<int> DebuffCCleanseASAPHS = new HashSet<int>
            {
                105421, //Blinding Light
                123393, //Breath of Fire (Glyph of Breath of Fire)
                44572, //Deep Freeze
                605, //Dominate Mind
                31661, //Dragon's Breath
                5782, // target.Class != WoWClass.Warrior || //Fear
                118699, // target.Class != WoWClass.Warrior || //Fear
                130616, // target.Class != WoWClass.Warrior || //Fear (Glyph of Fear)
                3355, //Freezing Trap
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                51514, //Hex
                2637, //Hibernate
                88625, //Holy Word: Chastise
                119072, //Holy Wrath
                5484, // target.Class != WoWClass.Warrior || //Howl of Terror
                115268, //Mesmerize (Shivarra)
                6789, //Mortal Coil
                115078, //Paralysis
                113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                64044, // target.Class != WoWClass.Warrior || //Psychic Horror
                8122, // target.Class != WoWClass.Warrior || //Psychic Scream
                113792, // target.Class != WoWClass.Warrior || //Psychic Terror (Psyfiend)
                107079, //Quaking Palm
                115001, //Remorseless Winter
                20066, // target.Class != WoWClass.Warrior || //Repentance
                82691, //Ring of Frost
                1513, //Scare Beast
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                9484, //Shackle Undead
                30283, //Shadowfury
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
                118905, //Static Charge (Capacitor Totem)
                10326, //Turn Evil
                19386, //Wyvern Sting //Thank bp423
                118552, //Flesh to Stone" //Thank bp423
                119985, //Dread Spray" //Thank mnipper
                117436, //Lightning Prison
                124863, //Visions of Demise
                123011, //Terrorize (10%)
                123012, //Terrorize (5%)
//Farraki
                136708, //Stone Gaze - Thank Clubwar
                136719, //Blazing Sunlight - Thank Clubwar && bp423
//Gurubashi
                136587, //Venom Bolt Volley - Thank Clubwar
//Drakaki
                136710, //Deadly Plague - Thank Clubwar
//Amani
                136512, //Hex of Confusion - Thank Clubwar
                136857, //Entrapped - Thank amputations
                136185, //Fragile Bones - Thank Sk1vvy 
                136187, //Clouded Mind - Thank Sk1vvy 
                136183, //Dulled Synapses - Thank Sk1vvy 
                136181, //Impaired Eyesight - Thank Sk1vvy 
                138040, //Horrific Visage - Thank macVsog
                117949, //Closed Curcuit
                112948, //Frostbomb
            };

        private static readonly HashSet<int> DebuffCCleanseASAPHSWarrior = new HashSet<int>
            {
                5782,
                118699,
                130616,
                64044,
                8122,
                113792
            };

        private static bool DebuffCCleanseASAP(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            if (target.IsPlayer &&
                target.Class == WoWClass.Warrior &&
                AuraCacheList.Any(
                    aura => aura.AuraCacheUnit == target.Guid &&
                            DebuffCCleanseASAPHSWarrior.Contains(aura.AuraCacheId)))
            {
                return false;
            }


            return AuraCacheList.Any(
                a => a.AuraCacheUnit == target.Guid &&
                     //a.AuraCacheAura.IsActive &&
                     a.AuraCacheAura.TimeLeft.TotalMilliseconds > 3000 &&
                     DebuffCCleanseASAPHS.Contains(a.AuraCacheId));
        }

        #endregion

        #region DebuffCCBreakonDamage

        private static readonly HashSet<int> DebuffCCBreakonDamageHS = new HashSet<int>
            {
                2094, //Blind
                105421, //Blinding Light
                99, //Disorienting Roar
                31661, //Dragon's Breath
                3355, //Freezing Trap
                1776, //Gouge
                2637, //Hibernate
                115268, //Mesmerize (Shivarra)
                115078, //Paralysis
                113953, //Paralysis (Paralytic Poison)
                126355, //Paralyzing Quill (Porcupine)
                126423, //Petrifying Gaze (Basilisk)
                118, //Polymorph
                61305, //Polymorph: Black Cat
                28272, //Polymorph: Pig
                61721, //Polymorph: Rabbit
                61780, //Polymorph: Turkey
                28271, //Polymorph: Turtle
                20066, //Repentance
                6770, //Sap
                19503, //Scatter Shot
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                104045, //Sleep (Metamorphosis)
                19386, //Wyvern Sting
            };

        private static bool DebuffCCBreakonDamage(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura =>
                aura.AuraCacheUnit == target.Guid &&
                //aura.AuraCacheAura.IsActive &&
                DebuffCCBreakonDamageHS.Contains(aura.AuraCacheId));
        }

        #endregion

        #region DebuffDisarm

        private static readonly HashSet<int> DebuffDisarmHS = new HashSet<int>
            {
                50541, //Clench (Scorpid)
                676, //Disarm
                118093, //Disarm (Voidwalker/Voidlord)
                51722, //Dismantle
                117368, //Grapple Weapon
                126458, //Grapple Weapon (Monk)
                64058, //Psychic Horror
                91644, //Snatch (Bird of Prey)
            };

        private static bool DebuffDisarm(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura =>
                aura.AuraCacheUnit == target.Guid &&
                //aura.AuraCacheAura.IsActive &&
                DebuffDisarmHS.Contains(aura.AuraCacheId));
        }

        #endregion

        #region DebuffDoNotHeal

        private static readonly HashSet<int> DebuffDoNotHealHS = new HashSet<int>
            {
                137341, // Beast of Nightmares
                137332, // Beast of Nightmares
                605, //"Dominate Mind",
                33786, //"Cyclone",
                123255, //"Dissonance Field",
                123184, //"Dissonance Field",
                121949, //"Parasitic Growth",
                137360, //"Corrupted Healing",
                122370, //"Reshape Life"
            };

        private static bool DebuffDoNotHeal(WoWUnit target)
        {
            //if (!BasicCheck(target))
            //{
            //    return false;
            //}

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura =>
                aura.AuraCacheUnit == target.Guid &&
                //aura.AuraCacheAura.IsActive && 
                DebuffDoNotHealHS.Contains(aura.AuraCacheId));
        }

        #endregion

        #region DebuffDoNotCleanse

        private static readonly HashSet<int> DebuffDoNotCleanseHS = new HashSet<int>
            {
                8050, //"Flame Shock",
                136184, // "Thick Bones",
                136186, //"Clear Mind",
                136180, //"Keen Eyesight",
                136182, //"Improved Synapses",
                30108, //"Unstable Affliction", //Grapple Weapon (Monk)
                138609, //"Matter Swap",
                138732, //"Ionization",
                138733, //"Ionization",
                34914, //"Vampiric Touch"
            };

        private static bool DebuffDoNotCleanse(WoWUnit target)
        {
            //if (!BasicCheck(target))
            //{
            //    return false;
            //}

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura =>
                aura.AuraCacheUnit == target.Guid &&
                //aura.AuraCacheAura.IsActive &&
                DebuffDoNotCleanseHS.Contains(aura.AuraCacheId));
        }

        #endregion

        #region DebuffDot

        /// <summary>
        /// Credit worklifebalance http://www.thebuddyforum.com/honorbuddy-forum/developer-forum/113473-wowapplyauratype-periodicleech-temporal-displacement.html#post1107923
        /// </summary>
        private static DateTime DebuffDotLast;

        private static readonly HashSet<WoWApplyAuraType> HS_HasAuraTypeDOT = new HashSet<WoWApplyAuraType>()
            {
                WoWApplyAuraType.PeriodicDamage,
                WoWApplyAuraType.PeriodicDamagePercent,
                WoWApplyAuraType.PeriodicLeech
            };

        private static readonly HashSet<int> HS_NOTDOT = new HashSet<int>()
            {
                84747, //Deep Insight
                84746, //Moderate Insight 
                84745, //Shallow Insight
                95223, //Mass Resurrected    
                71328, //Dungeon Cooldown 
                45181, //Cheated Death  
                119611, //Renewing Mist 
                //118253, //Serpent Sting
                118283, //Ursol's Vortex  
                53651, //Light's Beacon 
                57934, //Tricks of the Trade 
                100340, //CSA Area Trigger Dummy Timer
                57723, //Exhaustion
                57724, //Sated
                80354, //Temporal Displacement
                25771, //Forbearance
                96223, //Run Speed Marker 
                95809, //Insanity 
                11196, //PeriodicLeech Recently Bandaged 
                127973, //CSA Area Trigger Dummy Timer: 
            };

        private static bool DebuffDot(WoWUnit target)
        {
            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        //aura.AuraCacheAura.IsActive &&
                        !HS_NOTDOT.Contains(aura.AuraCacheId) &&
                        HS_HasAuraTypeDOT.Contains(aura.AuraCacheAura.ApplyAuraType));
        }

        private static bool DebuffDotDuration(WoWUnit target, int duration)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        aura.AuraCacheAura.TimeLeft.TotalMilliseconds > duration &&
                        //aura.AuraCacheAura.IsActive &&
                        HS_HasAuraTypeDOT.Contains(aura.AuraCacheAura.ApplyAuraType) &&
                        !HS_NOTDOT.Contains(aura.AuraCacheId));
        }

        private static double DebuffDotCount(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return 0;
            }

            AuraCacheUpdate(target);

            return
                AuraCacheList.Count(
                    a => a.AuraCacheUnit == target.Guid &&
                         //a.AuraCacheAura.IsActive &&
                         !HS_NOTDOT.Contains(a.AuraCacheId) &&
                         HS_HasAuraTypeDOT.Contains(a.AuraCacheAura.ApplyAuraType));
        }

        #endregion

        #region DebuffFearDuration

        private static readonly HashSet<int> DebuffFearDurationHS = new HashSet<int>
            {
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                5484, //Howl of Terror
                113056, //Intimidating Roar [Cowering in fear] (Warrior)
                113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                5246, //Intimidating Shout (aoe)
                20511, //Intimidating Shout (targeted)
                115268, //Mesmerize (Shivarra)
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                87204, //Sin and Punishment
                104045 //Sleep (Metamorphosis)
            };

        private static bool DebuffFearDuration(WoWUnit target, double duration)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        aura.AuraCacheAura.TimeLeft.TotalMilliseconds > duration &&
                        DebuffFearDurationHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModFear ||
            // aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModCharm ||
            // DebuffFearDurationHS.Contains(aura.AuraCacheId)) );
        }

        #endregion

        #region DebuffRoot

        private static readonly HashSet<int> DebuffRootHS = new HashSet<int>
            {
                96294, //Chains of Ice (Chilblains)
                116706, //Disable
                64695, //Earthgrab (Earthgrab Totem)
                339, //Entangling Roots
                113770, //Entangling Roots (Force of Nature - Balance Treants)
                19975, //Entangling Roots (Nature's Grasp)
                113275, //Entangling Roots (Symbiosis)
                113275, //Entangling Roots (Symbiosis)
                19185, //Entrapment
                33395, //Freeze
                63685, //Freeze (Frozen Power)
                39965, //Frost Grenade
                122, //Frost Nova
                110693, //Frost Nova (Mage)
                55536, //Frostweave Net
                87194, //Glyph of Mind Blast
                111340, //Ice Ward
                45334, //Immobilized (Wild Charge - Bear)
                90327, //Lock Jaw (Dog)
                102359, //Mass Entanglement
                128405, //Narrow Escape
                13099, //Net-o-Matic
                115197, //Partial Paralysis
                50245, //Pin (Crab)
                91807, //Shambling Rush (Dark Transformation)
                123407, //Spinning Fire Blossom
                107566, //Staggering Shout
                54706, //Venom Web Spray (Silithid)
                114404, //Void Tendril's Grasp
                4167, //Web (Spider)
            };

        private static bool DebuffRoot(WoWUnit target)
        {
            if (InRaid)
            {
                return false;
            }

            AuraCacheUpdate(target,false);
            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        DebuffRootHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModRoot ||
            // DebuffRootHS.Contains(aura.AuraCacheId)));
        }

        #endregion

        #region DebuffRootCanCleanse

        private static readonly HashSet<int> DebuffRootCanCleanseHS = new HashSet<int>
            {
                96294, //Chains of Ice (Chilblains)
                64695, //Earthgrab (Earthgrab Totem)
                339, //Entangling Roots
                113770, //Entangling Roots (Force of Nature - Balance Treants)
                19975, //Entangling Roots (Nature's Grasp)
                113275, //Entangling Roots (Symbiosis)
                113275, //Entangling Roots (Symbiosis)
                19185, //Entrapment
                33395, //Freeze
                63685, //Freeze (Frozen Power)
                122, //Frost Nova
                110693, //Frost Nova (Mage)
                87194, //Glyph of Mind Blast
                111340, //Ice Ward
                102359, //Mass Entanglement
                115197, //Partial Paralysis
                91807, //Shambling Rush (Dark Transformation)
                123407, //Spinning Fire Blossom
                54706, //Venom Web Spray (Silithid)
                //114404, //Void Tendril's Grasp
                4167, //Web (Spider)
            };

        private static bool DebuffRootCanCleanse(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        DebuffRootCanCleanseHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModRoot &&
            // (aura.AuraCacheAura.Spell.DispelType == WoWDispelType.Magic ||
            //  aura.AuraCacheAura.Spell.DispelType == WoWDispelType.Poison) ||
            // DebuffRootCanCleanseHS.Contains(aura.AuraCacheId)));
        }

        #endregion

        #region DebuffRootorSnare

        private static readonly HashSet<int> DebuffRootorSnareHS = new HashSet<int>
            {
                96294, //Chains of Ice (Chilblains)
                116706, //Disable
                64695, //Earthgrab (Earthgrab Totem)
                339, //Entangling Roots
                113770, //Entangling Roots (Force of Nature - Balance Treants)
                19975, //Entangling Roots (Nature's Grasp)
                113275, //Entangling Roots (Symbiosis)
                113275, //Entangling Roots (Symbiosis)
                19185, //Entrapment
                33395, //Freeze
                63685, //Freeze (Frozen Power)
                39965, //Frost Grenade
                122, //Frost Nova
                110693, //Frost Nova (Mage)
                55536, //Frostweave Net
                87194, //Glyph of Mind Blast
                111340, //Ice Ward
                45334, //Immobilized (Wild Charge - Bear)
                90327, //Lock Jaw (Dog)
                102359, //Mass Entanglement
                128405, //Narrow Escape
                13099, //Net-o-Matic
                115197, //Partial Paralysis
                50245, //Pin (Crab)
                91807, //Shambling Rush (Dark Transformation)
                123407, //Spinning Fire Blossom
                107566, //Staggering Shout
                54706, //Venom Web Spray (Silithid)
                114404, //Void Tendril's Grasp
                4167, //Web (Spider)
                50433, //Ankle Crack (Crocolisk)
                110300, //Burden of Guilt
                35101, //Concussive Barrage
                5116, //Concussive Shot
                120, //Cone of Cold
                3409, //Crippling Poison
                18223, //Curse of Exhaustion
                45524, //Chains of Ice
                50435, //Chilblains
                121288, //Chilled (Frost Armor)
                1604, //Dazed
                63529, //Dazed - Avenger's Shield
                50259, //Dazed (Wild Charge - Cat)
                26679, //Deadly Throw
                119696, //Debilitation
                116095, //Disable
                123727, //Dizzying Haze
                3600, //Earthbind (Earthbind Totem)
                77478, //Earthquake (Glyph of Unstable Earth)
                123586, //Flying Serpent Kick
                113092, //Frost Bomb
                54644, //Frost Breath (Chimaera)
                8056, //Frost Shock
                116, //Frostbolt
                8034, //Frostbrand Attack
                44614, //Frostfire Bolt
                61394, //Frozen Wake (Glyph of Freezing Trap)
                1715, //Hamstring
                13810, //Ice Trap
                58180, //Infected Wounds
                118585, //Leer of the Ox
                15407, //Mind Flay
                12323, //Piercing Howl
                115000, //Remorseless Winter
                20170, //Seal of Justice
                47960, //Shadowflame
                31589, //Slow
                129923, //Sluggish (Glyph of Hindering Strikes)
                61391, //Typhoon
                51490, //Thunderstorm
                127797, //Ursol's Vortex
                137637, //Warbringer
            };

        private static bool DebuffRootorSnare(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            if (UnitHasAura("Hand of Freedom", target))
            {
                return false;
            }

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        DebuffRootorSnareHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModRoot ||
            // aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModDecreaseSpeed ||
            // DebuffRootorSnareHS.Contains(aura.AuraCacheId)));
        }

        private static int DebuffRootorSnareCount(WoWUnit target)
        {
            if (BasicCheck(target))
            {
                return 0;
            }

            return AuraCacheList.Count(
                aura => aura.AuraCacheUnit == target.Guid &&
                        DebuffRootorSnareHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModRoot ||
            // aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModDecreaseSpeed ||
            // ));
        }

        #endregion

        #region DebuffRootDuration

        private static bool DebuffRootDuration(WoWUnit target, int duration)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            if (UnitHasAura("Hand of Freedom", target))
            {
                return false;
            }

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        aura.AuraCacheAura.TimeLeft.TotalMilliseconds > duration &&
                        DebuffRootHS.Contains(aura.AuraCacheId));

            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModRoot ||
            // DebuffRootHS.Contains(aura.AuraCacheId)) &&
        }

        #endregion

        #region DebuffSilence

        private static readonly HashSet<int> DebuffSilenceHS = new HashSet<int>
            {
                129597, //Arcane Torrent (Chi)
                25046, //Arcane Torrent (Energy)
                80483, //Arcane Torrent (Focus)
                28730, //Arcane Torrent (Mana)
                69179, //Arcane Torrent (Rage)
                50613, //Arcane Torrent (Runic Power)
                31935, //Avenger's Shield
                114238, //Fae Silence (Glyph of Fae Silence)
                102051, //Frostjaw (also a root)
                1330, //Garrote - Silence
                115782, //Optical Blast (Observer)
                15487, //Silence
                18498, //Silenced - Gag Order
                55021, //Silenced - Improved Counterspell
                34490, //Silencing Shot
                81261, //Solar Beam
                113287, //Solar Beam (Symbiosis)
                116709, //Spear Hand Strike
                24259, //Spell Lock (Felhunter)
                132409, //Spell Lock (Grimoire of Sacrifice)
                47476, //Strangulate
                31117, //Unstable Affliction
            };

        private static bool DebuffSilence(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        DebuffSilenceHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModSilence ||
            // ));
        }

        #endregion

        #region DebuffSnare

        private static readonly HashSet<int> DebuffSnareHS = new HashSet<int>
            {
                50433, //Ankle Crack (Crocolisk)
                110300, //Burden of Guilt
                35101, //Concussive Barrage
                5116, //Concussive Shot
                120, //Cone of Cold
                3409, //Crippling Poison
                18223, //Curse of Exhaustion
                45524, //Chains of Ice
                50435, //Chilblains
                121288, //Chilled (Frost Armor)
                1604, //Dazed
                63529, //Dazed - Avenger's Shield
                50259, //Dazed (Wild Charge - Cat)
                26679, //Deadly Throw
                119696, //Debilitation
                116095, //Disable
                123727, //Dizzying Haze
                3600, //Earthbind (Earthbind Totem)
                77478, //Earthquake (Glyph of Unstable Earth)
                123586, //Flying Serpent Kick
                113092, //Frost Bomb
                54644, //Frost Breath (Chimaera)
                8056, //Frost Shock
                116, //Frostbolt
                8034, //Frostbrand Attack
                44614, //Frostfire Bolt
                61394, //Frozen Wake (Glyph of Freezing Trap)
                1715, //Hamstring
                13810, //Ice Trap
                58180, //Infected Wounds
                118585, //Leer of the Ox
                15407, //Mind Flay
                12323, //Piercing Howl
                115000, //Remorseless Winter
                20170, //Seal of Justice
                47960, //Shadowflame
                31589, //Slow
                129923, //Sluggish (Glyph of Hindering Strikes)
                61391, //Typhoon
                51490, //Thunderstorm
                127797, //Ursol's Vortex
                137637, //Warbringer
                73682, //Unleash Frost
            };

        private static bool DebuffSnare(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            if (UnitHasAura("Hand of Freedom", target))
            {
                return false;
            }

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        DebuffSnareHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModDecreaseSpeed ||
            // DebuffSnareHS.Contains(aura.AuraCacheId)));
        }

        #endregion

        #region DebuffStun

        private static double DebuffStunDurationTimeLeft(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return 0;
            }

            AuraCacheUpdate(target);

            var auraStun = AuraCacheList.OrderByDescending(aura => aura.AuraCacheAura.TimeLeft)
                                        .FirstOrDefault(
                                            aura => aura.AuraCacheUnit == target.Guid &&
                                                    DebuffStunDurationHS.Contains(aura.AuraCacheId));

            return auraStun != null ? auraStun.AuraCacheAura.TimeLeft.TotalMilliseconds : 0;
        }

        private static bool DebuffStun(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        DebuffStunDurationHS.Contains(aura.AuraCacheId));
            //aura.AuraCacheAura.IsActive &&
            //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModStun ||
            // DebuffStunDurationHS.Contains(aura.AuraCacheId)));
        }

        private static readonly HashSet<int> DebuffStunDurationHS = new HashSet<int>
            {
                89766, //Axe Toss (Felguard/Wrathguard)
                113801, //Bash (Force of Nature - Feral Treants)
                102795, //Bear Hug
                117526, //Binding Shot
                126451, //Clash
                122242, //Clash (not sure which one is right)
                119392, //Charging Ox Wave
                1833, //Cheap Shot
                44572, //Deep Freeze
                54786, //Demonic Leap (Metamorphosis)
                105593, //Fist of Justice
                117418, //Fists of Fury
                91800, //Gnaw
                853, //Hammer of Justice
                110698, //Hammer of Justice (Paladin)
                24394, //Intimidation
                408, //Kidney Shot
                119381, //Leg Sweep
                5211, //Mighty Bash
                91797, //Monstrous Blow (Dark Transformation)
                9005, //Pounce
                102546, //Pounce (Incarnation)
                115001, //Remorseless Winter
                82691, //Ring of Frost
                30283, //Shadowfury
                132168, //Shockwave
                118905, //Static Charge (Capacitor Totem)
                20549, //War Stomp
                108194 //Asphyxiate
            };

        private static bool DebuffStunDuration(WoWUnit target, double duration)
        {
            if (InRaid || !BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura => aura.AuraCacheUnit == target.Guid &&
                        aura.AuraCacheAura.TimeLeft.TotalMilliseconds > duration &&
                        DebuffStunDurationHS.Contains(aura.AuraCacheId));
        }

        //aura.AuraCacheAura.IsActive &&
        //(aura.AuraCacheAura.ApplyAuraType == WoWApplyAuraType.ModStun ||
        // DebuffSnareHS.Contains(aura.AuraCacheId)) &&

        #endregion

        #region DebuffStunFearDuration

        private static readonly HashSet<int> DebuffStunFearDurationHS = new HashSet<int>
            {
                119392, //Charging Ox Wave
                1833, //Cheap Shot
                44572, //Deep Freeze
                31661, //Dragon's Breath
                77505, //Earthquake
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                105593, //Fist of Justice
                117418, //Fists of Fury
                853, //Hammer of Justice
                110698,
                113056, //Intimidating Roar [Cowering in fear] (Warrior)
                113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                5246, //Intimidating Shout (aoe)
                20511, //Intimidating Shout (targeted)
                24394, //Intimidation
                408, //Kidney Shot
                119381, //Leg Sweep
                5211, //Mighty Bash
                91797, //Monstrous Blow (Dark Transformation)
                9005, //Pounce
                102546, //Pounce (Incarnation)
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                118345, //Pulverize
                115001, //Remorseless Winter
                82691, //Ring of Frost
                30283, //Shadowfury
                132168, //Shockwave
                87204, //Sin and Punishment
                105771, //Warbringer
                108194, //Asphyxiate
            };

        private static bool DebuffStunFearDuration(WoWUnit target, double duration)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(delegate(WoWAura aura)
            {
                if ((!DebuffStunFearDurationHS.Contains(aura.SpellId) && (aura.get_ApplyAuraType() != ((WoWApplyAuraType)((int)WoWApplyAuraType.ModStun)))) && (aura.get_ApplyAuraType() != ((WoWApplyAuraType)((int)WoWApplyAuraType.ModFear))))
                {
                    return false;
                }
                return (aura.TimeLeft.TotalMilliseconds >= duration);
            });

        }

        #endregion

        #region Invulnerable

        private static readonly HashSet<int> InvulnerableHS = new HashSet<int>
            {
                //"Bladestorm",
                33786, //"Cyclone",0x83fa
                //"Desecrated Ground",
                19263, //"Deterrence",0x4b3f
                47585, //"Dispersion",0xb9e1
                642, //"Divine Shield", //Grapple Weapon (Monk)0x282
                45438, //"Ice Block"0xb17e
                27827, //Spirit of Redemption,0x6cb3
                0x243f3, 0x1e8f6
            };

        private static bool Invulnerable(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(aura => InvulnerableHS.Contains(aura.SpellId));
        }



        #endregion

        #region InvulnerablePhysic

        //1022/hand-of-protection
        private static bool InvulnerablePhysic(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return UnitHasAura(1022, target);

        }

        #endregion

        #region InvulnerableSpell

        private static readonly HashSet<int> InvulnerableSpellHS = new HashSet<int>
            {
                48707, //"Anti-Magic Shell",
                31224, //"Cloak of Shadows",
                115723, //"Glyph of Ice Block",
                8178, //"Grounding Totem Effect",
                114028, //"Mass Spell Reflection",
                23920, //"Spell Reflection",
                131523, //"Zen Meditation"
            };

        private static bool InvulnerableSpell(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(aura => InvulnerableSpellHS.Contains(aura.SpellId));
        }

 


        #endregion

        #region InvulnerableRootandSnare

        private static readonly HashSet<int> InvulnerableRootandSnareHS = new HashSet<int>
            {
                53271, //"Master's Call",
                46924, //"Bladestorm",
                1044, //"Hand of Freedom"
            };

        private static bool InvulnerableRootandSnare(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return target.GetAllAuras().Any<WoWAura>(aura => InvulnerableRootandSnareHS.Contains(aura.SpellId));
        }



        #endregion

        #region InvulnerableStun

        private static readonly HashSet<int> InvulnerableStunHS = new HashSet<int>
            {
                48792, //"Icebound Fortitude",
                46924, //"Bladestorm"
            };

        private static bool InvulnerableStun(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return (((target.get_Class() == ((WoWClass)((uint)WoWClass.Monk))) && (TalentSort(target) > 3)) || target.GetAllAuras().Any<WoWAura>(aura => InvulnerableStunHS.Contains(aura.SpellId)));
        }

 


        #endregion

        #region SafeUsingCooldown

        private static readonly HashSet<int> SafeUsingCooldownHS = new HashSet<int>
            {
                5277, //"Evasion",
                46924, //"Bladestorm",
            };

        private static bool SafeUsingCooldown(WoWUnit target)
        {
            if (Me.get_CurrentMap().IsRaid)
            {
                return false;
            }
            return !target.GetAllAuras().Any<WoWAura>(aura => SafeUsingCooldownHS.Contains(aura.SpellId));
        }

 


        #endregion

        #region WriteDebuff

        private static void WriteDebuff(WoWUnit target)
        {
            if (InRaid || !BasicCheck(target))
            {
                return;
            }

            foreach (WoWAura aura in target.GetAllAuras())
            {
                if (!aura.IsHarmful) continue; //Name: Battle Fatigue (134735)
                if (aura.SpellId == 134735) continue; //Name: Battle Fatigue (134735)
                var spell = aura.Spell;
                Logging.Write("--------" + aura.Spell.Name + " (" + aura.SpellId + ")" + " (" +
                              target.Class + ")" +
                              "--------");
                //Logging.Write("Name: " + aura.Name + " (" + aura.AuraCacheId + ")");
                Logging.Write("ApplyAuraType: " + aura.ApplyAuraType);
                Logging.Write("SpellEffects: " + spell.SpellEffects);
                Logging.Write("SpellEffect1: " + spell.SpellEffect1);
                Logging.Write("SpellEffect2: " + spell.SpellEffect2);
                Logging.Write("SpellEffect3: " + spell.SpellEffect3);
            }
        }

        #endregion
    }
}