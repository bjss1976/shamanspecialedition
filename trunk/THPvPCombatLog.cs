using System;
using System.Collections.Generic;
using System.Linq;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region EnemyListCache

        private static readonly Dictionary<ulong, DateTime> InterruptonCooldownUnitList =
            new Dictionary<ulong, DateTime>();

        private static void InterruptonCooldownUnitListClear()
        {
            var indexToRemove =
                InterruptonCooldownUnitList.
                    Where(unit => unit.Value < DateTime.Now).
                    Select(unit => unit.Key).
                    ToList();

            foreach (var index in indexToRemove)
            {
                InterruptonCooldownUnitList.Remove(index);
            }
        }

        private static void InterruptonCooldownUnitAdd(WoWUnit unit, int expireSeconds)
        {
            var unitGUID = unit.Guid;
            var indexToRemove =
                InterruptonCooldownUnitList.
                    Where(u => u.Key == unitGUID).
                    Select(u => u.Key).
                    ToList();

            foreach (var index in indexToRemove)
            {
                InterruptonCooldownUnitList.Remove(index);
            }

            InterruptonCooldownUnitList.Add(unitGUID, DateTime.Now + TimeSpan.FromSeconds(expireSeconds));
        }

        #endregion

        #region InterruptSpell

        private static bool IsSafetoCast()
        {
            if (MeHasAura("Spiritwalker's Aegis") || MeHasAura("Devotion Aura")) //31821 Devotion Aura
            {
                return true;
            }

            InterruptonCooldownUnitListClear();

            return NearbyUnFriendlyPlayers.Any(unit => InterruptonCooldownUnitList.ContainsKey(unit.Guid));

            //if (NearbyFriendlyPlayers.Count(
            //    unit =>
            //    !InterruptonCooldownUnitList.ContainsKey(unit.Guid) &&
            //    ((unit.IsPlayer &&
            //      unit.Class != WoWClass.Warlock) ||
            //     unit.Entry == 417) &&
            //    (TalentSort(unit) == 1 &&
            //     unit.Distance < 15 ||
            //     TalentSort(unit) != 1) &&
            //    InLineOfSpellSightCheck(unit)) < 1)
            //{
            //    return true;
            //}

            //return false;
        }

        private static readonly HashSet<int> InterruptSpellHS = new HashSet<int>
            {
                6552, //Pummel
                1766, //Kick 
                47528, //Mind Freeze
                47476, //Strangulate
                96231, //Rebuke
                31935, //Avengers Shield
                57994, //Wind Shear
                2139, //Counterspell
                //19647, //Spell Lock
                103967, //Carrion Swarm
                115781, //Optical Blast
                34490, //Silencing Shot
                80965, //Skull Bash
                116705, //Spear Hand Strike
                15487, //Silence
            };

        private static int InterruptSpellCooldown(int spellID)
        {
            switch (spellID)
            {
                case 47476:
                    return 60000;
                case 2139:
                    return 22000;
                case 19647:
                    return 24000;
                case 103967:
                    return 12000;
                case 115781:
                    return 24000;
                case 34490:
                    return 24000;
                case 15487:
                    return 45000;
                default:
                    return 15000;
            }
        }

        #endregion

        #region HandleCombatLogPvP

        private static void HandleCombatLogPvP(object sender, LuaEventArgs args)
        {
            var e = new CombatLogEventArgs(args.EventName, args.FireTimeStamp, args.Args);

            switch (e.Event)
            {
                case "SPELL_CAST_SUCCESS":

                    //Logging.Write(LogLevel.Diagnostic, "Debug: {0} SPELL_CAST_SUCCESS {1} - {2} - {3} - {4} - {5}",
                    //              e.SourceName, e.SpellName,
                    //              e.SpellId, e.Timestamp, e.FireTimeStamp, e.DestName);

                    if (!IsEnemy(e.SourceUnit))
                    {
                        return;
                    }

                    if ((e.SpellId == 60192 || e.SpellId == 1499))
                    {
                        Logging.Write(LogLevel.Diagnostic, "Found Hunter Drop {0} - {1}", e.SpellName,
                                      e.SpellId);
                        LastTrapEvent = DateTime.Now;
                    }

                    if (InterruptSpellHS.Contains(e.SpellId))
                    {
                        Logging.Write(LogLevel.Diagnostic, "InterruptonCooldownUnit Add {0} - {1} - {2} - {3} ",
                                      e.SourceUnit.SafeName,
                                      e.SpellName,
                                      e.SpellId,
                                      InterruptSpellCooldown(e.SpellId) - 1000);

                        if (e.DestGuid == MeGuid)
                        {
                            Logging.Write(LogLevel.Diagnostic, "Incoming SPELL_CAST_SUCCESS {0} - {1} Stop Casting",
                                          e.SpellName,
                                          e.SpellId);
                            if (!MeHasAura(131558))
                            {
                                SpellManager.StopCasting();
                            }
                        }

                        InterruptonCooldownUnitAdd(e.SourceUnit, InterruptSpellCooldown(e.SpellId) - 1000);
                    }
                    break;

                    //case "SPELL_CAST_START":

                    //    Logging.Write(LogLevel.Diagnostic, "Debug: {0} SPELL_CAST_START {1} - {2} - {3} - {4} - {5}",
                    //                  e.SourceName,
                    //                  e.SpellName,
                    //                  e.SpellId,
                    //                  e.DestName,
                    //                  e.DestGuid,
                    //                  e.DestGuidStr);

                    //    //if (InterruptSpellHS.Contains(e.SpellId) &&
                    //    //    IsEnemy(e.SourceUnit))
                    //    //{
                    //    //    InterruptonCooldownUnitAdd(e.SourceUnit, InterruptSpellCooldown(e.SpellId - 1000));

                    //    //    if (e.SourceUnit.CurrentTarget != null &&
                    //    //        e.SourceUnit.CurrentTarget == Me &&
                    //    //        !MeHasAura(131558))
                    //    //    {
                    //    //        Logging.Write(LogLevel.Diagnostic, "Incoming SPELL_CAST_START {0} - {1} Stop Casting",
                    //    //                      e.SpellName, e.SpellId);

                    //    //        SpellManager.StopCasting();
                    //    //    }
                    //    //}
                    //    break;
            }
        }

        #endregion

        #region AttachCombatLogEventPvP

        private static bool CombatLogAttachedPvP;

        private static void AttachCombatLogEventPvP()
        {
            if (CombatLogAttachedPvP)
                return;

            Lua.Events.AttachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleCombatLogPvP);

            //const string filterCriteria = "return (args[2] == 'SPELL_CAST_START' or args[2] == 'SPELL_CAST_SUCCESS')";
            const string filterCriteria = "return (args[2] == 'SPELL_CAST_SUCCESS')";

            //string filterCriteria =
            //    "return args[4] == UnitGUID('player')"
            //    + " and (args[2] == 'SPELL_MISSED'"
            //    + " or args[2] == 'RANGE_MISSED'"
            //    + " or args[2] == 'SPELL_AURA_APPLIED'"
            //    + " or args[2] == 'SPELL_AURA_REFRESH'"
            //    + " or args[2] == 'SPELL_AURA_REMOVED'"
            //    + " or args[2] == 'SWING_MISSED'"
            //    + " or args[2] == 'SPELL_CAST_START'"
            //    + " or args[2] == 'SPELL_CAST_SUCCESS'"
            //    + " or args[2] == 'SPELL_CAST_FAILED')";

            if (!Lua.Events.AddFilter("COMBAT_LOG_EVENT_UNFILTERED", filterCriteria))
            {
                Logging.Write(
                    "ERROR: Could not add combat log event filter! - Performance may be horrible, and things may not work properly!");
            }

            Logging.Write("Attached CombatLogAttachedPvP");

            CombatLogAttachedPvP = true;
        }

        #endregion

        #region DetachCombatLogEventPvP

        private static void DetachCombatLogEventPvP()
        {
            if (!CombatLogAttachedPvP)
                return;

            Logging.Write("DetachCombatLogEventPvP");
            Logging.Write("Removed combat log filter");
            Lua.Events.RemoveFilter("COMBAT_LOG_EVENT_UNFILTERED");
            Logging.Write("Detached combat log");
            Lua.Events.DetachEvent("COMBAT_LOG_EVENT_UNFILTERED", HandleCombatLogPvP);

            CombatLogAttachedPvP = false;
        }

        #endregion

        #region PvPCombatLogEvent

        #endregion
    }
}