using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media;
using System.Reflection;
using CommonBehaviors.Actions;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Inventory;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;
using System.ComponentModel;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region Attackable

        private static bool Attackable(WoWUnit target, int range)
        {
            if (target == null ||
                !target.IsValid ||
                !target.Attackable ||
                !target.CanSelect ||
                !target.IsAlive ||
                //target.IsCritter ||
                //Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                GetDistance(target) > range ||
                Invulnerable(target) ||
                DebuffCCBreakonDamage(target) ||
                !IsEnemy(target) ||
                range > 5 &&
                GetDistance(target) > 5 &&
                !InLineOfSpellSightCheck(target))
            {
                return false;
            }
            return true;
        }

        private static bool AttackableNoCC(WoWUnit target, int range)
        {
            if (target == null ||
                !target.IsValid ||
                !target.Attackable ||
                !target.CanSelect ||
                !target.IsAlive ||
                //target.IsCritter ||
                //Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                target.Distance - target.CombatReach - 1 > range ||
                Invulnerable(target) ||
                !IsEnemy(target) ||
                range > 5 &&
                GetDistance(target) > 5 &&
                !InLineOfSpellSightCheck(target))
            {
                return false;
            }
            return true;
        }

        private static bool AttackableNoLoS(WoWUnit target, int range)
        {
            if (target == null ||
                !target.IsValid ||
                !target.Attackable ||
                !target.CanSelect ||
                !target.IsAlive ||
                //target.IsCritter ||
                //Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                GetDistance(target) > range ||
                Invulnerable(target) ||
                DebuffCCBreakonDamage(target) ||
                !IsEnemy(target))
            {
                return false;
            }
            return true;
        }

        private static bool AttackableNoCCLoS(WoWUnit target, int range)
        {
            if (target == null ||
                !target.IsValid ||
                !target.Attackable ||
                !target.CanSelect ||
                !target.IsAlive ||
                //target.IsCritter ||
                //Blacklist.Contains(target.Guid, BlacklistFlags.All) ||
                GetDistance(target) > range ||
                Invulnerable(target) ||
                !IsEnemy(target))
            {
                return false;
            }
            return true;
        }

        #endregion

        #region AuraCheck

        private static bool UnitHasAura(string auraName, WoWUnit target)
        {
            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", Me.SafeName);
            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == Me))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}

            return AuraCacheList != null &&
                   AuraCacheList.Any(aura => aura.AuraCacheUnit == target.Guid &&
                                             aura.AuraCacheAura.Name == auraName);
        }

        private static bool UnitHasAura(int auraID, WoWUnit target)
        {
            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", Me.SafeName);
            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == Me))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}


            return AuraCacheList != null &&
                   AuraCacheList.Any(aura => aura.AuraCacheUnit == target.Guid &&
                                             aura.AuraCacheId == auraID);
        }

        private static bool MeHasAura(string auraName)
        {
            AuraCacheUpdate(Me);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", Me.SafeName);
            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == Me))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}

            return AuraCacheList != null &&
                   AuraCacheList.Any(aura => aura.AuraCacheUnit == Me.Guid &&
                                             aura.AuraCacheAura.Name == auraName);
        }

        private static bool MeHasAura(int auraID)
        {
            AuraCacheUpdate(Me);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", Me.SafeName);
            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == Me))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}


            return AuraCacheList != null &&
                   AuraCacheList.Any(aura => aura.AuraCacheUnit == Me.Guid &&
                                             aura.AuraCacheId == auraID);
        }

        private static bool MyAura(string auraName, WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", target.SafeName);

            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == target .Guid))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}


            var AuraFound = AuraCacheList != null &&
                            AuraCacheList.Any(
                                aura => aura.AuraCacheUnit == target.Guid &&
                                        aura.AuraCacheAura.Name == auraName &&
                                        aura.AuraCacheAura.CreatorGuid == Me.Guid);

            return AuraFound;
        }


        private static bool MyAura(int auraID, WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", target.SafeName);

            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == target .Guid))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}

            var AuraFound = AuraCacheList != null &&
                            AuraCacheList.Any(
                                aura => aura.AuraCacheUnit == target.Guid &&
                                        aura.AuraCacheAura.CreatorGuid == Me.Guid &&
                                        aura.AuraCacheId == auraID);

            return AuraFound;
        }

        private static double MyAuraTimeLeft(string auraName, WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 0;
            }

            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", target.SafeName);

            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == target .Guid))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}

            return AuraCacheList == null
                       ? 0
                       : AuraCacheList.Where(
                           aura =>
                           aura.AuraCacheUnit == target.Guid && aura.AuraCacheAura.CreatorGuid == Me.Guid &&
                           aura.AuraCacheName == auraName)
                                      .Select(Aura => Aura.AuraCacheAura.TimeLeft.TotalMilliseconds)
                                      .FirstOrDefault();

            //if (AuraCacheList != null)
            //{
            //    foreach (var Aura in AuraCacheList)
            //    {
            //        if (Aura.AuraCacheUnit==target&&
            //            Aura.AuraCacheAura.CreatorGuid==Me.Guid&&
            //            Aura.AuraCacheName == auraName)
            //        {
            //            return Aura.AuraCacheAura.TimeLeft.TotalMilliseconds;
            //        }
            //    }
            //}
            //return 0;
        }

        private static double MyAuraTimeLeft(int auraID, WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 0;
            }

            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", target.SafeName);

            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == target .Guid))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}

            return AuraCacheList == null
                       ? 0
                       : AuraCacheList.Where(
                           aura =>
                           aura.AuraCacheUnit == target.Guid && aura.AuraCacheAura.CreatorGuid == Me.Guid &&
                           aura.AuraCacheId == auraID)
                                      .Select(Aura => Aura.AuraCacheAura.TimeLeft.TotalMilliseconds)
                                      .FirstOrDefault();
        }

        private static double MyAuraStackCount(string auraName, WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 0;
            }

            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", target.SafeName);

            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == target .Guid))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}

            return AuraCacheList == null
                       ? 0
                       : AuraCacheList.Where(
                           aura =>
                           aura.AuraCacheUnit == target.Guid && aura.AuraCacheAura.CreatorGuid == Me.Guid &&
                           aura.AuraCacheName == auraName)
                                      .Select(aura => aura.AuraCacheAura.StackCount)
                                      .FirstOrDefault();
        }

        private static double MyAuraStackCount(int auraID, WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 0;
            }

            AuraCacheUpdate(target);

            //Logging.Write("========================");
            //Logging.Write("Total AuraCacheAura of {0}", target.SafeName);

            //foreach (var Aura in AuraCacheList.Where(aura => aura.AuraCacheUnit == target .Guid))
            //{
            //    Logging.Write("Aura.AuraCacheUnit {0} - Aura.AuraCacheAura.Name {1} - Aura.AuraCacheId {2}",
            //                  Aura.AuraCacheUnit, Aura.AuraCacheAura.Name, Aura.AuraCacheId);
            //}

            return AuraCacheList == null
                       ? 0
                       : AuraCacheList.Where(
                           aura =>
                           aura.AuraCacheUnit == target.Guid && aura.AuraCacheAura.CreatorGuid == Me.Guid &&
                           aura.AuraCacheId == auraID)
                                      .Select(aura => aura.AuraCacheAura.StackCount)
                                      .FirstOrDefault();
        }

        #endregion

        #region BasicCheck

        private static bool BasicCheck(WoWUnit target)
        {
            return target != null && target.IsValid && target.IsAlive;
        }

        #endregion

        #region  BattleStandard@

        private static Composite UseBattleStandard()
        {
            return new Decorator(ret => (((THSettings.Instance.BattleStandard && Me.Combat) && ((HealWeight(Me) <= THSettings.Instance.BattleStandardHP) && (Me.CurrentTarget != null))) && (Me.CurrentTarget.HealthPercent > HealWeight(Me))) && InBattleground, new Styx.TreeSharp.Action(delegate(object param0)
            {
                if (Me.IsAlliance)
                {
                    WoWItem item = Me.BagItems.FirstOrDefault<WoWItem>(o => o.Entry == 0x48ae);
                    if ((!MeHasAura("Alliance Battle Standard") && (item != null)) && (item.CooldownTimeLeft.TotalMilliseconds <= MyLatency))
                    {
                        item.Use();
                        Styx.Common.Logging.Write("Use Battle Standard at " + HealWeight(Me) + "%");
                    }
                }
                if (Me.IsHorde)
                {
                    WoWItem item2 = Me.BagItems.FirstOrDefault<WoWItem>(o => o.Entry == 0x48af);
                    if ((!MeHasAura("Horde Battle Standard") && (item2 != null)) && (item2.CooldownTimeLeft.TotalMilliseconds <= MyLatency))
                    {
                        item2.Use();
                        Styx.Common.Logging.Write("Use Battle Standard at " + HealWeight(Me) + "%");
                    }
                }
                return RunStatus.Failure;
            }));
        }

        #endregion

        #region CanUseEquippedItem

        //Thank Apoc
        private static bool CanUseEquippedItem(WoWItem item)
        {
            // Check for engineering tinkers!
            var itemSpell = Lua.GetReturnVal<string>("return GetItemSpell(" + item.Entry + ")", 0);
            if (string.IsNullOrEmpty(itemSpell))
                return false;

            return item.Usable && item.Cooldown <= 0;
        }

        #endregion

        #region CancelAura

        private static void CancelAura(string aura, WoWUnit target)
        {
            WoWAura a = target.GetAuraByName(aura);
            if (a != null && a.Cancellable)
                a.TryCancelAura();
        }

        private static void CancelAura(int aura, WoWUnit target)
        {
            WoWAura a = target.GetAuraById(aura);
            if (a != null && a.Cancellable)
                a.TryCancelAura();
        }

        #endregion

        #region CastSpell

        private static WoWUnit LastCastUnit;
        private static string LastCastSpell;
        private static DateTime LastCastTime;
        //private static bool CastFailed;
        private static DateTime LastWriteLog;
        private static Color colorlog;

        private static void CastSpell(string spellName, WoWUnit target, string reason = "")
        {
            if (target == null || !target.IsValid)
            {
                return;
            }

            SpellManager.Cast(spellName, target);

            LastCastSpell = spellName;
            LastCastUnit = target;
            LastCastTime = DateTime.Now;

            if (!THSettings.Instance.AutoWriteLog || LastWriteLog > DateTime.Now) return;

            LastWriteLog = DateTime.Now + TimeSpan.FromMilliseconds(MyLatency);

            if (spellName == "Wind Shear" ||
                spellName == "Grounding Totem" ||
                spellName == "Capacitor Totem" ||
                spellName == "Hex")
            {
                colorlog = Colors.GhostWhite;
            }
            else if (target == Me)
            {
                colorlog = Colors.GreenYellow;
            }
            else if (target == Me.CurrentTarget)
            {
                colorlog = Colors.Yellow;
            }
            else
            {
                colorlog = Colors.YellowGreen;
            }

            Logging.Write(colorlog, DateTime.Now.ToString("ss:fff") + " HP: " +
                                    Math.Round(Me.HealthPercent) + "% Mana: " + Math.Round(Me.ManaPercent) + " " +
                                    SafeName(target) + " " +
                                    Math.Round(target.Distance, 2) + "y " +
                                    Math.Round(target.HealthPercent) + "% hp " + spellName + reason);
        }


        //private static Action CastSpell(string spellName, UnitSelectionDelegate target, string reason = "")
        //{
        //    return new Action(ret =>
        //        {
        //            if (target(ret) == null ||
        //                !target(ret).IsValid)
        //            {
        //                return RunStatus.Success;
        //            }

        //            SpellManager.Cast(spellName, target(ret));

        //            LastCastSpell = spellName;
        //            LastCastUnit = target(ret);
        //            LastCastTime = DateTime.Now;

        //            if (!THSettings.Instance.AutoWriteLog || LastWriteLog > DateTime.Now)
        //            {
        //                return RunStatus.Success;
        //            }

        //            LastWriteLog = DateTime.Now + TimeSpan.FromMilliseconds(MyLatency);

        //            WriteReason = "";

        //            if (reason != "")
        //            {
        //                WriteReason = " (" + reason + ")";
        //            }

        //            if (spellName == "Wind Shear" ||
        //                spellName == "Grounding Totem" ||
        //                spellName == "Capacitor Totem" ||
        //                spellName == "Hex")
        //            {
        //                colorlog = Colors.GhostWhite;
        //            }
        //            else if (target(ret) == Me)
        //            {
        //                colorlog = Colors.GreenYellow;
        //            }
        //            else if (target(ret) == Me.CurrentTarget)
        //            {
        //                colorlog = Colors.Yellow;
        //            }
        //            else
        //            {
        //                colorlog = Colors.YellowGreen;
        //            }

        //            Logging.Write(colorlog, DateTime.Now.ToString("ss:fff") + " HP: " +
        //                                    Math.Round(Me.HealthPercent) + "% Mana: " +
        //                                    Math.Round(Me.ManaPercent) + " " +
        //                                    SafeName(target(ret)) + " " +
        //                                    Math.Round(target(ret).Distance, 2) + "y " +
        //                                    Math.Round(target(ret).HealthPercent) + "% hp " + spellName +
        //                                    WriteReason);

        //            return RunStatus.Success;
        //        });
        //}

        #endregion

        #region CanCleanse

        //private bool CanCleanse(WoWAura aura)
        //{
        //    //if (aura == null)
        //    //    return false;

        //    //if (aura.Spell.DispelType == WoWDispelType.Disease || aura.Spell.DispelType == WoWDispelType.Magic ||
        //    //    aura.Spell.DispelType == WoWDispelType.Poison)
        //    if (aura.Spell.DispelType != WoWDispelType.None)
        //    {
        //        //Logging.Write("CanCleanse: " + aura.Name + " - " + aura.SpellId);
        //        return true;
        //    }
        //    return false;
        //}

        #endregion

        #region ConstantFace

        //private static void ConstantFace(WoWUnit unit)
        //{
        //    if (!IsOverrideModeOn && !Me.IsSafelyFacing(unit))
        //    {
        //        WoWMovement.ConstantFace(unit.Guid);
        //    }
        //}

        #endregion

        #region CountDebuff

        private static double CountDebuff(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 0;
            }

            int numberofDebuff =
                AuraCacheList.Count(
                    aura =>
                    aura.AuraCacheUnit == target.Guid &&
                    aura.AuraCacheAura.IsActive &&
                    aura.AuraCacheAura.IsHarmful &&
                    (aura.AuraCacheAura.Spell.DispelType == WoWDispelType.Magic ||
                     aura.AuraCacheAura.Spell.DispelType == WoWDispelType.Curse));

            return numberofDebuff;
        }

        #endregion

        #region CountDPSTarget

        //////done
        private static bool HaveDPSTarget(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return NearbyUnFriendlyPlayers.Where(BasicCheck).Any(
                        unit =>
                        unit.IsValid &&
                        //unit.GotTarget &&
                        unit.CurrentTarget == target &&
                        (TalentSort(unit) >= 2 &&
                         TalentSort(unit) <= 3 &&
                         unit.Location.Distance(target.Location) < 40 ||
                         TalentSort(unit) == 1 &&
                         unit.Location.Distance(target.Location) < 20) &&
                        !DebuffCC(unit) &&
                        (target != Me ||
                         target == Me &&
                         InLineOfSpellSightCheck(unit)));
                }
                return FarUnFriendlyUnits.Where(BasicCheck).Any(
                    unit =>
                    unit.IsValid &&
                    //unit.GotTarget &&
                    unit.CurrentTarget == target &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountFriendDPSTarget

        private static int CountFriendDPSTarget(WoWUnit target)
        {
            if (InArena || InBattleground)
            {
                return NearbyFriendlyPlayers.Count(
                    unit =>
                    BasicCheck(unit) &&
                    //unit.GotTarget &&
                    unit.CurrentTarget == target &&
                    (TalentSort(unit) >= 2 &&
                     TalentSort(unit) <= 3 &&
                     unit.Location.Distance(target.Location) < 40 ||
                     TalentSort(unit) == 1 &&
                     unit.Location.Distance(target.Location) < 15));
            }
            return NearbyFriendlyPlayers.Count(
                unit =>
                BasicCheck(unit) &&
                //unit.GotTarget &&
                unit.CurrentTarget == target);
        }

        private static bool HasFriendDPSTarget(WoWUnit target)
        {
            if (InArena || InBattleground)
            {
                return NearbyFriendlyPlayers.Where(BasicCheck).Any(
                    unit =>
                    //BasicCheck(unit) &&
                    //unit.GotTarget &&
                    unit.CurrentTarget == target &&
                    (TalentSort(unit) >= 2 &&
                     TalentSort(unit) <= 3 &&
                     unit.Location.Distance(target.Location) < 40 ||
                     TalentSort(unit) == 1 &&
                     unit.Location.Distance(target.Location) < 15));
            }
            return NearbyFriendlyPlayers.Where(BasicCheck).Any(
                unit =>
                //BasicCheck(unit) &&
                //unit.GotTarget &&
                unit.CurrentTarget == target);
        }

        #endregion

        #region CountMagicDPSTarget

        //private static int CountMagicDPSTarget(WoWUnit target)
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        if (InArena || InBattleground)
        //        {
        //            return NearbyUnFriendlyPlayers.Count(
        //                unit =>
        //                //unit.GotTarget &&
        //                unit.CurrentTarget == target &&
        //                TalentSort(unit) == 3 &&
        //                unit.Location.Distance(target.Location) < 40 &&
        //                !DebuffCC(unit));
        //        }
        //        return NearbyUnFriendlyUnits.Count(
        //            unit =>
        //            //unit.GotTarget &&
        //            unit.CurrentTarget == target &&
        //            unit.Location.Distance(target.Location) < 40 &&
        //            !DebuffCC(unit));
        //    }
        //}

        #endregion

        #region CountMeleeDPSTarget

        private static int CountMeleeDPSTarget(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return NearbyUnFriendlyPlayers.Count(
                        unit =>
                        unit.IsValid &&
                        //unit.GotTarget &&
                        unit.CurrentTarget == target &&
                        TalentSort(unit) == 1 &&
                        unit.Location.Distance(target.Location) < 15 &&
                        !DebuffCC(unit) &&
                        (target != Me ||
                         target == Me &&
                         InLineOfSpellSightCheck(unit)));
                }
                return NearbyUnFriendlyUnits.Count(
                    unit =>
                    unit.IsValid &&
                    //unit.GotTarget &&
                    unit.CurrentTarget == target &&
                    unit.Location.Distance(target.Location) < 15 &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountPhysicDPSTarget

        private static int CountPhysicDPSTarget(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (InArena || InBattleground)
                {
                    return NearbyUnFriendlyPlayers.Count(
                        unit =>
                        unit.IsValid &&
                        //unit.GotTarget &&
                        unit.CurrentTarget == target &&
                        (TalentSort(unit) == 2 &&
                         unit.Location.Distance(target.Location) < 40 ||
                         TalentSort(unit) == 1 &&
                         unit.Location.Distance(target.Location) < 25) &&
                        !DebuffCC(unit) &&
                        (target != Me ||
                         target == Me &&
                         InLineOfSpellSightCheck(unit)));
                }
                return NearbyUnFriendlyUnits.Count(
                    unit =>
                    unit.IsValid &&
                    //unit.GotTarget &&
                    unit.CurrentTarget == target &&
                    unit.Location.Distance(target.Location) < 15 &&
                    !DebuffCC(unit));
            }
        }

        #endregion

        #region CountEneyNeary

        private static int CountEnemyNear(WoWUnit unitCenter, float distance)
        {
            return FarUnFriendlyUnits.Count(
                unit =>
                BasicCheck(unit) &&
                unit.MaxHealth > MeMaxHealth * 0.3 &&
                //!unit.IsPet &&
                (InProvingGrounds || 
                 IsDummy(unit) ||
                 unit.Combat &&
                 FarFriendlyPlayers.Contains(unit.CurrentTarget)) &&
                GetDistance(unitCenter, unit) <= distance);
        }

        private static bool HasEnemyNear(WoWUnit unitCenter, float distance)
        {
            return FarUnFriendlyUnits.Where(BasicCheck).Any(
                unit =>
                //BasicCheck(unit) &&
                unit.MaxHealth > MeMaxHealth * 0.3 &&
                !unit.IsPet &&
                (InProvingGrounds ||
                 IsDummy(unit) ||
                 unit.Combat &&
                 //BasicCheck(unit.CurrentTarget) &&
                 FarFriendlyPlayers.Contains(unit.CurrentTarget)) &&
                GetDistance(unitCenter, unit) <= distance);
        }

        #endregion

        #region CountEneyTargettingUnit

        private static double CountEnemyTargettingUnit(WoWUnit target, float distance)
        {
            return
                NearbyUnFriendlyUnits.Count(
                    unit =>
                    unit.IsValid &&
                    GetDistance(unit) < distance &&
                    //unit.GotTarget &&
                    unit.Level <= target.Level + 3 &&
                    //unit.Combat && 
                    !unit.IsPet &&
                    unit.CurrentTarget == target &&
                    !DebuffCC(unit) &&
                    (target != Me ||
                     target == Me &&
                     InLineOfSpellSightCheck(unit)));
        }

        private static bool HasEnemyTargettingUnit(WoWUnit target, float distance)
        {
            return
                FarUnFriendlyUnits.Where(BasicCheck).Any(
                    unit =>
                    unit.IsValid &&
                    GetDistance(unit) < distance &&
                    //unit.GotTarget &&
                    unit.Level <= target.Level + 3 &&
                    //unit.Combat && 
                    unit.MaxHealth > MeMaxHealth * 0.3 &&
                    //!unit.IsPet &&
                    unit.CurrentTarget == target &&
                    !DebuffCC(unit) &&
                    (target != Me ||
                     target == Me &&
                     InLineOfSpellSightCheck(unit)));
        }

        #endregion

        #region CurrentTargetCheck

        private static WoWUnit CurrentTargetCheckLast;
        private static double CurrentTargetCheckDist;
        private static bool CurrentTargetCheckDebuffCCBreakonDamage;
        private static bool CurrentTargetCheckInvulnerable;
        private static bool CurrentTargetCheckInvulnerablePhysic;
        private static bool CurrentTargetCheckInvulnerableMagic;
        private static bool CurrentTargetCheckIsEnemy;
        private static bool CurrentTargetCheckInLineOfSpellSight;
        private static bool CurrentTargetCheckFacing;

        private static void CurrentTargetCheck()
        {
            CurrentTargetCheckLast = null;
            CurrentTargetCheckDist = 1000;
            CurrentTargetCheckDebuffCCBreakonDamage = false;
            CurrentTargetCheckInvulnerable = false;
            CurrentTargetCheckIsEnemy = false;
            CurrentTargetCheckInLineOfSpellSight = false;
            CurrentTargetCheckFacing = false;

            if (!BasicCheck(Me.CurrentTarget))
            {
                return;
            }

            CurrentTargetCheckLast = Me.CurrentTarget;
            CurrentTargetCheckFacing = Me.IsSafelyFacing(Me.CurrentTarget,180);
            CurrentTargetCheckDist = GetDistance(Me.CurrentTarget);

            CurrentTargetCheckIsEnemy = IsEnemy(Me.CurrentTarget);
            if (!CurrentTargetCheckIsEnemy)
            {
                return;
            }

            CurrentTargetCheckInvulnerable = Invulnerable(Me.CurrentTarget);
            if (CurrentTargetCheckInvulnerable)
            {
                return;
            }

            CurrentTargetCheckDebuffCCBreakonDamage = DebuffCCBreakonDamage(Me.CurrentTarget);
            if (CurrentTargetCheckDebuffCCBreakonDamage)
            {
                return;
            }

            CurrentTargetCheckInvulnerableMagic = InvulnerableSpell(Me.CurrentTarget);
            CurrentTargetCheckInvulnerablePhysic = InvulnerablePhysic(Me.CurrentTarget);

            if (CurrentTargetCheckDist <= 5)
            {
                CurrentTargetCheckInLineOfSpellSight = true;
            }
            else if (InLineOfSpellSightCheck(Me.CurrentTarget))//.
            {
                CurrentTargetCheckInLineOfSpellSight = true;
            }
        }

        private static bool CurrentTargetAttackable(double distance,
                                                    bool includeCurrentTargetCheckInvulnerableePhysic = false,
                                                    bool includeCurrentTargetCheckInvulnerableeMagic = false)
        {
            if (Me.CurrentTarget == null ||
                !Me.CurrentTarget.IsValid ||
                !Me.CurrentTarget.Attackable ||
                !Me.CurrentTarget.IsAlive) // ||
                //Blacklist.Contains(Me.CurrentTarget.Guid, BlacklistFlags.All))
            {
                return false;
            }

            if (Me.CurrentTarget != CurrentTargetCheckLast)
            {
                CurrentTargetCheck();
            }

            if (!CurrentTargetCheckIsEnemy ||
                CurrentTargetCheckDebuffCCBreakonDamage ||
                CurrentTargetCheckInvulnerable ||
                !CurrentTargetCheckFacing && !THSettings.Instance.AutoFace && IsOverrideModeOn ||
                includeCurrentTargetCheckInvulnerableePhysic && CurrentTargetCheckInvulnerablePhysic ||
                includeCurrentTargetCheckInvulnerableeMagic && CurrentTargetCheckInvulnerableMagic ||
                CurrentTargetCheckDist > distance ||
                !CurrentTargetCheckInLineOfSpellSight)
            {
                return false;
            }
            return true;
        }

        private static bool CurrentTargetAttackableNoLoS(double distance)
        {
            if (Me.CurrentTarget == null ||
                !Me.CurrentTarget.IsValid ||
                !Me.CurrentTarget.Attackable ||
                !Me.CurrentTarget.IsAlive) // ||
                //Blacklist.Contains(Me.CurrentTarget.Guid, BlacklistFlags.All))
            {
                return false;
            }

            if (Me.CurrentTarget != CurrentTargetCheckLast)
            {
                CurrentTargetCheck();
            }

            if (CurrentTargetCheckDebuffCCBreakonDamage ||
                CurrentTargetCheckInvulnerable ||
                !CurrentTargetCheckIsEnemy ||
                CurrentTargetCheckDist > distance)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region FacingOverride

        private static bool FacingOverride(WoWUnit unit)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (!BasicCheck(unit))
                {
                    return false;
                }

                if (!IsUsingAFKBot || Me.IsFacing(unit))
                {
                    return true;
                }

                if (THSettings.Instance.AutoFace && !IsMoving(Me))
                {
                    return true;
                }

                //if (TreeRoot.Current.Name == "Questing" ||
                //    TreeRoot.Current.Name == "Grind Bot" ||
                //    TreeRoot.Current.Name == "BGBuddy" ||
                //    TreeRoot.Current.Name == "DungeonBuddy" ||
                //    TreeRoot.Current.Name == "Mixed Mode")
                //{
                //    return true;
                //}

                return false;
            }
        }

        #endregion

        #region GetCurrentSpec

        private static string CurrentSpec;

        internal static string GetCurrentSpec()
        {
            switch (Me.Specialization)
            {
                case WoWSpec.ShamanElemental:
                    CurrentSpec = "Elemental";
                    break;
                case WoWSpec.ShamanEnhancement:
                    CurrentSpec = "Enhancement";
                    break;
                default:
                    CurrentSpec = "Restoration";
                    break;
            }
            return CurrentSpec;
        }

        #endregion

        #region GetAsyncKeyState

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);

        #endregion

        #region keyboardhook

        #region Windows structure definitions
        /// <summary>
        /// The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private class KeyboardHookStruct
        {
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            /// </summary>
            public int vkCode;
            /// <summary>
            /// Specifies a hardware scan code for the key. 
            /// </summary>
            public int scanCode;
            /// <summary>
            /// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            public int flags;
            /// <summary>
            /// Specifies the time stamp for this message.
            /// </summary>
            public int time;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int dwExtraInfo;
        }
        #endregion

        #region Windows function imports
        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain. 
        /// You would install a hook procedure to monitor the system for certain types of events. These events 
        /// are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Specifies the type of hook procedure to be installed. This parameter can be one of the following values.
        /// </param>
        /// <param name="lpfn">
        /// [in] Pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a 
        /// thread created by a different process, the lpfn parameter must point to a hook procedure in a dynamic-link 
        /// library (DLL). Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
        /// </param>
        /// <param name="hMod">
        /// [in] Handle to the DLL containing the hook procedure pointed to by the lpfn parameter. 
        /// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by 
        /// the current process and if the hook procedure is within the code associated with the current process. 
        /// </param>
        /// <param name="dwThreadId">
        /// [in] Specifies the identifier of the thread with which the hook procedure is to be associated. 
        /// If this parameter is zero, the hook procedure is associated with all existing threads running in the 
        /// same desktop as the calling thread. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the handle to the hook procedure.
        /// If the function fails, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
           CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
        /// </summary>
        /// <param name="idHook">
        /// [in] Handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain. 
        /// A hook procedure can call this function either before or after processing the hook information. 
        /// </summary>
        /// <param name="idHook">Ignored.</param>
        /// <param name="nCode">
        /// [in] Specifies the hook code passed to the current hook procedure. 
        /// The next hook procedure uses this code to determine how to process the hook information.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies the wParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <param name="lParam">
        /// [in] Specifies the lParam value passed to the current hook procedure. 
        /// The meaning of this parameter depends on the type of hook associated with the current hook chain. 
        /// </param>
        /// <returns>
        /// This value is returned by the next hook procedure in the chain. 
        /// The current hook procedure must also return this value. The meaning of the return value depends on the hook type. 
        /// For more information, see the descriptions of the individual hook procedures.
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/setwindowshookex.asp
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto,
             CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        /// <summary>
        /// The CallWndProc hook procedure is an application-defined or library-defined callback 
        /// function used with the SetWindowsHookEx function. The HOOKPROC type defines a pointer 
        /// to this callback function. CallWndProc is a placeholder for the application-defined 
        /// or library-defined function name.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookfunctions/callwndproc.asp
        /// </remarks>
        private delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// The ToAscii function translates the specified virtual-key code and keyboard 
        /// state to the corresponding character or characters. The function translates the code 
        /// using the input language and physical keyboard layout identified by the keyboard layout handle.
        /// </summary>
        /// <param name="uVirtKey">
        /// [in] Specifies the virtual-key code to be translated. 
        /// </param>
        /// <param name="uScanCode">
        /// [in] Specifies the hardware scan code of the key to be translated. 
        /// The high-order bit of this value is set if the key is up (not pressed). 
        /// </param>
        /// <param name="lpbKeyState">
        /// [in] Pointer to a 256-byte array that contains the current keyboard state. 
        /// Each element (byte) in the array contains the state of one key. 
        /// If the high-order bit of a byte is set, the key is down (pressed). 
        /// The low bit, if set, indicates that the key is toggled on. In this function, 
        /// only the toggle bit of the CAPS LOCK key is relevant. The toggle state 
        /// of the NUM LOCK and SCROLL LOCK keys is ignored.
        /// </param>
        /// <param name="lpwTransKey">
        /// [out] Pointer to the buffer that receives the translated character or characters. 
        /// </param>
        /// <param name="fuState">
        /// [in] Specifies whether a menu is active. This parameter must be 1 if a menu is active, or 0 otherwise. 
        /// </param>
        /// <returns>
        /// If the specified key is a dead key, the return value is negative. Otherwise, it is one of the following values. 
        /// Value Meaning 
        /// 0 The specified virtual key has no translation for the current state of the keyboard. 
        /// 1 One character was copied to the buffer. 
        /// 2 Two characters were copied to the buffer. This usually happens when a dead-key character 
        /// (accent or diacritic) stored in the keyboard layout cannot be composed with the specified 
        /// virtual key to form a single character. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        /// <summary>
        /// The GetKeyboardState function copies the status of the 256 virtual keys to the 
        /// specified buffer. 
        /// </summary>
        /// <param name="pbKeyState">
        /// [in] Pointer to a 256-byte array that contains keyboard key states. 
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError. 
        /// </returns>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/userinput/keyboardinput/keyboardinputreference/keyboardinputfunctions/toascii.asp
        /// </remarks>
        [DllImport("user32")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        #endregion

        #region Windows constants

        //values from Winuser.h in Microsoft SDK.
        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        private const int WH_MOUSE_LL = 14;
        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        private const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
        /// </summary>
        private const int WH_MOUSE = 7;
        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
        /// </summary>
        private const int WH_KEYBOARD = 2;

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        private const int WM_MOUSEMOVE = 0x200;
        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDOWN = 0x201;
        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WM_RBUTTONDOWN = 0x204;
        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONDOWN = 0x207;
        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        private const int WM_LBUTTONUP = 0x202;
        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        private const int WM_RBUTTONUP = 0x205;
        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONUP = 0x208;
        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDBLCLK = 0x203;
        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
        /// </summary>
        private const int WM_RBUTTONDBLCLK = 0x206;
        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
        /// </summary>
        private const int WM_MBUTTONDBLCLK = 0x209;
        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        private const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
        /// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        private const int WM_KEYDOWN = 0x100;
        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
        /// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        private const int WM_KEYUP = 0x101;
        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
        /// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
        /// presses another key. It also occurs when no window currently has the keyboard focus; 
        /// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
        /// receives the message can distinguish between these two contexts by checking the context 
        /// code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYDOWN = 0x104;
        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
        /// releases a key that was pressed while the ALT key was held down. It also occurs when no 
        /// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
        /// to the active window. The window that receives the message can distinguish between 
        /// these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

        #endregion

        #region KeyboardHookStart
        /// <summary>
        /// Occurs when the user moves the mouse, presses any mouse button or scrolls the wheel
        /// </summary>
        public event MouseEventHandler OnMouseActivity;
        /// <summary>
        /// Occurs when the user presses a key
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when the user presses and releases 
        /// </summary>
        public event KeyPressEventHandler KeyPress;
        /// <summary>
        /// Occurs when the user releases a key
        /// </summary>
        public event KeyEventHandler KeyUp;
        /// <summary>
        /// Stores the handle to the mouse hook procedure.
        /// </summary>
        private int hMouseHook = 0;
        /// <summary>
        /// Stores the handle to the keyboard hook procedure.
        /// </summary>
        private int hKeyboardHook = 0;
        
        /// <summary>
        /// Declare MouseHookProcedure as HookProc type.
        /// </summary>
        private static HookProc MouseHookProcedure;
        /// <summary>
        /// Declare KeyboardHookProcedure as HookProc type.
        /// </summary>
        private static HookProc KeyboardHookProcedure;


        /// <summary>
        /// Installs both mouse and keyboard hooks and starts rasing events
        /// </summary>
        /// <exception cref="Win32Exception">Any windows problem.</exception>
        /// 
        private void Start()
        {
            if (hKeyboardHook == 0)
            {
                // Create an instance of HookProc.
                KeyboardHookProcedure = new HookProc(KeyboardHookProc);
                //install hook
                hKeyboardHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL,
                    KeyboardHookProcedure,
                    Marshal.GetHINSTANCE(
                    Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);
                //If SetWindowsHookEx fails.
                if (hKeyboardHook == 0)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //do cleanup
                    Stop(false);
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        public void Stop(bool ThrowExceptions)
        {
            //if keyboard hook set and must be uninstalled
            if (hKeyboardHook != 0)
            {
                //uninstall hook
                int retKeyboard = UnhookWindowsHookEx(hKeyboardHook);
                //reset invalid handle
                hKeyboardHook = 0;
                //if failed and exception must be thrown
                if (retKeyboard == 0 && ThrowExceptions)
                {
                    //Returns the error code returned by the last unmanaged function called using platform invoke that has the DllImportAttribute.SetLastError flag set. 
                    int errorCode = Marshal.GetLastWin32Error();
                    //Initializes and throws a new instance of the Win32Exception class with the specified error. 
                    throw new Win32Exception(errorCode);
                }
            }
        }

        /// <summary>
        /// A callback function which will be called every time a keyboard activity detected.
        /// </summary>
        /// <param name="nCode">
        /// [in] Specifies whether the hook procedure must process the message. 
        /// If nCode is HC_ACTION, the hook procedure must process the message. 
        /// If nCode is less than zero, the hook procedure must pass the message to the 
        /// CallNextHookEx function without further processing and must return the 
        /// value returned by CallNextHookEx.
        /// </param>
        /// <param name="wParam">
        /// [in] Specifies whether the message was sent by the current thread. 
        /// If the message was sent by the current thread, it is nonzero; otherwise, it is zero. 
        /// </param>
        /// <param name="lParam">
        /// [in] Pointer to a CWPSTRUCT structure that contains details about the message. 
        /// </param>
        /// <returns>
        /// If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. 
        /// If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx 
        /// and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROC 
        /// hooks will not receive hook notifications and may behave incorrectly as a result. If the hook 
        /// procedure does not call CallNextHookEx, the return value should be zero. 
        /// </returns>

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //indicates if any of underlaing events set e.Handled flag
            bool handled = false;
            //it was ok and someone listens to events
            if ((nCode >= 0) && (KeyDown != null || KeyUp != null || KeyPress != null))
            {
                //read structure KeyboardHookStruct at lParam
                KeyboardHookStruct MyKeyboardHookStruct = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                //raise KeyDown
                if (KeyDown != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyDown(this, e);
                    handled = handled || e.Handled;
                }

                // raise KeyPress
                if (KeyPress != null && wParam == WM_KEYDOWN)
                {
                    bool isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);
                    bool isDownCapslock = (GetKeyState(VK_CAPITAL) != 0 ? true : false);

                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (ToAscii(MyKeyboardHookStruct.vkCode,
                              MyKeyboardHookStruct.scanCode,
                              keyState,
                              inBuffer,
                              MyKeyboardHookStruct.flags) == 1)
                    {
                        char key = (char)inBuffer[0];
                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key)) key = Char.ToUpper(key);
                        KeyPressEventArgs e = new KeyPressEventArgs(key);
                        KeyPress(this, e);
                        handled = handled || e.Handled;
                    }
                }

                // raise KeyUp
                if (KeyUp != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    Keys keyData = (Keys)MyKeyboardHookStruct.vkCode;
                    KeyEventArgs e = new KeyEventArgs(keyData);
                    KeyUp(this, e);
                    handled = handled || e.Handled;
                }

            }

            //if event handled in application do not handoff to other listeners
            if (handled)
                return 1;
            else
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
        }
        #endregion
        #endregion

        #region Spells

        private static double GetSpellCastTime(string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CastTime/1000.0 : results.Original.CastTime/1000.0;
            }
            return 99999.9;
        }

        private static double GetSpellCastTime(int spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CastTime/1000.0 : results.Original.CastTime/1000.0;
            }
            return 99999.9;
        }

        private static double GetSpellPowerCost(string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.PowerCost : results.Original.PowerCost;
            }
            return 99999.9;
        }

        private static double GetSpellPowerCost(int spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.PowerCost : results.Original.PowerCost;
            }
            return 99999.9;
        }
        //////done
        private static TimeSpan GetSpellCooldown(string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CooldownTimeLeft : results.Original.CooldownTimeLeft;
            }

            return TimeSpan.MaxValue;
        }

        private static TimeSpan GetSpellCooldown(int spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.CooldownTimeLeft : results.Original.CooldownTimeLeft;
            }

            return TimeSpan.MaxValue;
        }

        private static bool SpellOnCooldown(string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.Cooldown : results.Original.Cooldown;
            }

            return false;
        }

        private static bool SpellOnCooldown(int spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override != null ? results.Override.Cooldown : results.Original.Cooldown;
            }

            return false;
        }

        private static WoWSpell SpellFind(string spell)
        {
            SpellFindResults results;
            if (SpellManager.FindSpell(spell, out results))
            {
                return results.Override ?? results.Original;
            }

            return null;
        }

        #endregion Spells

        #region GetUnitAuras

        //private static double AuraCCDuration;
        //private static double AuraCCDurationMax;
        //public WoWAura AuraCC;

        //private static double AuraCCBreakonDamageDuration;
        //private static double AuraCCBreakonDamageDurationMax;
        //public WoWAura AuraCCBreakonDamage;

        //private static double AuraDisarmDuration;
        //private static double AuraDisarmDurationMax;
        //public WoWAura AuraDisarm;

        //private static double AuraImmuneDuration;
        //private static double AuraImmuneDurationMax;
        //public WoWAura AuraImmune;

        //private static double AuraImmunePhysicDuration;
        //private static double AuraImmunePhysicDurationMax;
        //public WoWAura AuraImmunePhysic;

        //private static double AuraImmuneSpellDuration;
        //private static double AuraImmuneSpellDurationMax;
        //public WoWAura AuraImmuneSpell;

        //private static double AuraRootDuration;
        //private static double AuraRootDurationMax;
        //public WoWAura AuraRoot;

        //private static double AuraSilenceDuration;
        //private static double AuraSilenceDurationMax;
        //public WoWAura AuraSilence;

        //private static double AuraSnareDuration;
        //private static double AuraSnareDurationMax;
        //public WoWAura AuraSnare;

        //public WoWAura AuraCleanseDoNot;
        //public WoWAura AuraHealDoNot;
        //public int NumberofDebuff;

        //private bool GetUnitAuras(WoWUnit u)
        //{
        //    if (u == null || !u.IsValid || !u.IsAlive)
        //        return false;

        //    AuraCC = null;
        //    AuraCCDuration = 0;
        //    AuraCCDurationMax = 0;

        //    AuraCCBreakonDamage = null;
        //    AuraCCBreakonDamageDuration = 0;
        //    AuraCCBreakonDamageDurationMax = 0;

        //    AuraDisarm = null;
        //    AuraDisarmDuration = 0;
        //    AuraDisarmDurationMax = 0;

        //    AuraImmune = null;
        //    AuraImmuneDuration = 0;
        //    AuraImmuneDurationMax = 0;

        //    AuraImmuneSpell = null;
        //    AuraImmuneSpellDuration = 0;
        //    AuraImmuneSpellDurationMax = 0;

        //    AuraImmunePhysic = null;
        //    AuraImmunePhysicDuration = 0;
        //    AuraImmunePhysicDurationMax = 0;

        //    AuraRoot = null;
        //    AuraRootDuration = 0;
        //    AuraRootDurationMax = 0;

        //    AuraSilence = null;
        //    AuraSilenceDuration = 0;
        //    AuraSilenceDurationMax = 0;

        //    AuraSnare = null;
        //    AuraSnareDuration = 0;
        //    AuraSnareDurationMax = 0;

        //    NumberofDebuff = 0;
        //    AuraCleanseDoNot = null;
        //    AuraHealDoNot = null;

        //    foreach (var aura in u.GetAllAuras())
        //    {
        //        //Count Number of Debuff
        //        if (aura.IsHarmful &&
        //            (aura.Spell.DispelType == WoWDispelType.Disease ||
        //             aura.Spell.DispelType == WoWDispelType.Magic ||
        //             aura.Spell.DispelType == WoWDispelType.Poison))
        //        {
        //            NumberofDebuff = NumberofDebuff + 1;
        //        }

        //        //Find out if AuraCleanseDoNot exits
        //        if (ListCleanseDoNot.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraCleanseDoNot = aura.;
        //        }

        //        //Find out if AuraHealDoNot exits
        //        if (ListHealDoNot.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraHealDoNot = aura.;
        //        }

        //        if (ListCC.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraCCDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraCCDuration > AuraCCDurationMax)
        //            {
        //                AuraCC = aura.;
        //                AuraCCDurationMax = AuraCCDuration;
        //            }
        //        }

        //        if (ListCCBreakonDamage.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraCCBreakonDamageDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraCCBreakonDamageDuration > AuraCCBreakonDamageDurationMax)
        //            {
        //                AuraCCBreakonDamage = aura.;
        //                AuraCCBreakonDamageDurationMax = AuraCCBreakonDamageDuration;
        //            }
        //        }

        //        if (ListDisarm.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraDisarmDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraDisarmDuration > AuraDisarmDurationMax)
        //            {
        //                AuraDisarm = aura.;
        //                AuraDisarmDurationMax = AuraDisarmDuration;
        //            }
        //        }

        //        if (ListImmune.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraImmuneDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraImmuneDuration > AuraImmuneDurationMax)
        //            {
        //                AuraImmune = aura.;
        //                AuraImmuneDurationMax = AuraImmuneDuration;
        //            }
        //        }

        //        if (ListImmuneSpell.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraImmuneSpellDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraImmuneSpellDuration > AuraImmuneSpellDurationMax)
        //            {
        //                AuraImmuneSpell = aura.;
        //                AuraImmuneSpellDurationMax = AuraImmuneSpellDuration;
        //            }
        //        }

        //        if (ListImmunePhysic.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraImmunePhysicDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraImmunePhysicDuration > AuraImmunePhysicDurationMax)
        //            {
        //                AuraImmunePhysic = aura.;
        //                AuraImmunePhysicDurationMax = AuraImmunePhysicDuration;
        //            }
        //        }

        //        if (ListRoot.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraRootDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraRootDuration > AuraRootDurationMax)
        //            {
        //                AuraRoot = aura.;
        //                AuraRootDurationMax = AuraRootDuration;
        //            }
        //        }

        //        if (ListSilence.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraSilenceDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraSilenceDuration > AuraSilenceDurationMax)
        //            {
        //                AuraSilence = aura.;
        //                AuraSilenceDurationMax = AuraSilenceDuration;
        //            }
        //        }

        //        if (ListSnare.Contains("[" + aura.SpellId + "]"))
        //        {
        //            AuraSnareDuration = aura.TimeLeft.TotalMilliseconds;
        //            if (AuraSnareDuration > AuraSnareDurationMax)
        //            {
        //                AuraSnare = aura.;
        //                AuraSnareDurationMax = AuraSnareDuration;
        //            }
        //        }
        //    }
        //    return true;
        //}

        #endregion

        #region GetAllMyAuras

        private static DateTime _lastGetAllMyAuras = DateTime.Now;

        private static void DumpAuras()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (_lastGetAllMyAuras.AddSeconds(10) < DateTime.Now)
                {
                    int i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (WoWAura aura in Me.GetAllAuras())
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.GetAllAuras Name: " + aura.Name + " - SpellId: " + aura.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (var aura in Me.Auras)
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.Auras - Name: " + aura.Value.Name + " - SpellId: " + aura.Value.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (var aura in Me.GetAllAuras())
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.GetAllAuras() - Name: " + aura.Name + " - SpellId: " +
                                      aura.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    i = 1;
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");
                    foreach (var aura in Me.PassiveAuras)
                    {
                        Logging.Write(LogLevel.Diagnostic,
                                      i + ". Me.PassiveAuras - Name: " + aura.Value.Name + " - SpellId: " +
                                      aura.Value.SpellId);
                        i = i + 1;
                    }
                    Logging.Write(LogLevel.Diagnostic, "----------------------------------");

                    _lastGetAllMyAuras = DateTime.Now;
                }
            }
        }

        #endregion

        #region GetDistance

        private static float GetDistance(WoWUnit target)
        {
            if (target == null ||
                !target.IsValid)
            {
                return 10000;
            }

            if (target.CombatReach < 3)
            {
                return (float) target.Distance;
            }

            return (float) target.Distance - target.CombatReach + 1;
        }

        private static float GetDistance(WoWUnit source, WoWUnit target)
        {
            if (source == null ||
                !source.IsValid ||
                target == null ||
                !target.IsValid)
            {
                return 10000;
            }

            if (target.CombatReach < 3)
            {
                return source.Location.Distance(target.Location);
            }

            return source.Location.Distance(target.Location) - target.CombatReach + 1;
        }

        #endregion

        #region GetMaxDistance

        //private static float GetMaxDistance(WoWUnit target)
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        if (target != null)
        //        {
        //            return (float) Math.Max(0f, target.Distance2D - target.BoundingRadius);
        //        }
        //        return 0;
        //    }
        //}

        #endregion

        #region GetMinDistance

        //private static float GetMinDistance(WoWUnit target)
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        if (target != null)
        //        {
        //            return (float) Math.Max(0f, target.Distance2D - target.BoundingRadius);
        //        }
        //        return 123456;
        //    }
        //}

        #endregion

        #region GetUnitDispellerAround

        private static WoWUnit UnitDispellerAround;

        private static bool GetUnitDispellerAround()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                UnitDispellerAround = null;
                UnitDispellerAround = (from unit in NearbyUnFriendlyUnits
                                       orderby unit.Distance ascending
                                       where unit.IsPlayer
                                       where
                                           unit.Class == WoWClass.Hunter || unit.Class == WoWClass.Mage ||
                                           unit.Class == WoWClass.Priest || unit.Class == WoWClass.Shaman
                                       where AttackableNoLoS(unit, 40)
                                       select unit).FirstOrDefault();

                return BasicCheck(UnitDispellerAround);
            }
        }

        #endregion

        #region GetBestTarget

        private static WoWUnit UnitBestTarget;

        private static bool GetBestTarget()
        {
            UnitBestTarget = null;
            if (InBattleground || InArena)
            {
                if (DebuffRootorSnare(Me))
                {
                    UnitBestTarget = NearbyUnFriendlyPlayers.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                        unit => BasicCheck(unit) &&
                                Attackable(unit, 5));
                }
                else
                {
                    UnitBestTarget = NearbyUnFriendlyPlayers.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        Attackable(unit, 20));
                }

                if (UnitBestTarget == null)
                {
                    UnitBestTarget = NearbyUnFriendlyPlayers.OrderBy(unit => unit.Distance).FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        Attackable(unit, 60));
                }
            }

            if (UnitBestTarget == null)
            {
                UnitBestTarget = FarUnFriendlyUnits.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        InProvingGrounds ||
                        unit.GotTarget &&
                        IsMyPartyRaidMember(unit.CurrentTarget) &&
                        Attackable(unit, 5));
            }
            if (UnitBestTarget == null)
            {
                UnitBestTarget = FarUnFriendlyUnits.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        InProvingGrounds ||
                        unit.GotTarget &&
                        IsMyPartyRaidMember(unit.CurrentTarget) &&
                        Attackable(unit, 20));
            }
            if (UnitBestTarget == null)
            {
                UnitBestTarget = FarUnFriendlyUnits.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        InProvingGrounds ||
                        unit.GotTarget &&
                        IsMyPartyRaidMember(unit.CurrentTarget) &&
                        Attackable(unit, 40));
            }


            //if (UnitBestTarget == null && !InBattleground && !InArena)
            //{
            //    UnitBestTarget = NearbyUnFriendlyUnits.OrderBy(unit => unit.ThreatInfo.RawPercent).FirstOrDefault(
            //        unit =>
            //        BasicCheck(unit) &&
            //        Me.IsFacing(unit) &&
            //        IsMyPartyRaidMember(unit.CurrentTarget) &&
            //        Attackable(unit, 40));
            //}

            return BasicCheck(UnitBestTarget);
        }

        private static bool GetBestTargetRange()
        {
            UnitBestTarget = null;
            if (InBattleground || InArena)
            {
                UnitBestTarget = NearbyUnFriendlyPlayers.OrderBy(unit => unit.CurrentHealth).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    Attackable(unit, 30));

                if (UnitBestTarget == null)
                {
                    UnitBestTarget = NearbyUnFriendlyPlayers.OrderBy(unit => unit.Distance).FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        IsMyPartyRaidMember(unit.CurrentTarget) &&
                        Attackable(unit, 60));
                }
            }

            else
            {
                UnitBestTarget = NearbyUnFriendlyUnits.OrderBy(unit => unit.ThreatInfo.RawPercent).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    FacingOverride(unit) &&
                    IsMyPartyRaidMember(unit.CurrentTarget) &&
                    Attackable(unit, 40));
            }

            return BasicCheck(UnitBestTarget);
        }

        #endregion

        #region GetUnitHaveMySacredShield

        private static WoWUnit UnitHaveMySacredShield;

        private static bool GetUnitHaveMySacredShield()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                UnitHaveMySacredShield = null;
                UnitHaveMySacredShield = (from unit in FarFriendlyPlayers
                                          where MyAura(20925, unit)
                                          select unit).FirstOrDefault();
                return BasicCheck(UnitHaveMySacredShield);
            }
        }

        #endregion

        #region GetUnitHammerofWrath

        private static WoWUnit UnitHammerofWrath;

        private static bool GetUnitHammerofWrath()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                UnitHammerofWrath = null;
                UnitHammerofWrath = (from unit in NearbyUnFriendlyPlayers
                                     where unit.HealthPercent < 20
                                     where GetDistance(unit) < 30
                                     where !Invulnerable(unit)
                                     where InLineOfSpellSightCheck(unit)
                                     where FacingOverride(unit)
                                     select unit).FirstOrDefault();

                return BasicCheck(UnitHammerofWrath);
            }
        }

        #endregion

        #region GetEnemyByGUID

        private static WoWUnit EnemyByGUID;

        private static bool GetEnemyByGUID(ulong SourceGUID)
        {
            if (EnemyListCache.ContainsKey(SourceGUID))
            {
                return true;
            }

            EnemyByGUID = null;
            EnemyByGUID = FarUnFriendlyPlayers.FirstOrDefault(unit => unit.Guid == SourceGUID);

            return BasicCheck(EnemyByGUID);
        }

        #endregion

        #region GlobalCheck@

        private static bool HasAuraPreparation;
        private static bool HasAuraArenaPreparation;
        private static bool InArena;
        private static bool InBattleground;
        private static bool InProvingGrounds;
        private static bool InInstance;
        private static bool InDungeon;
        private static bool InRaid;
        private static TimeSpan CurrentTargetCheckTimeOut;
        private static DateTime GlobalCheckTime;

        private static void GlobalCheck()
        {
            if (GlobalCheckTime < DateTime.Now)
            {
                GlobalCheckTime = DateTime.Now + TimeSpan.FromSeconds(30.0);
                InArena = Me.CurrentMap.IsArena;
                InBattleground = Me.CurrentMap.IsBattleground;
                InProvingGrounds = Me.CurrentMap.Name == "Proving Grounds";
                InDungeon = Me.CurrentMap.IsDungeon;
                InRaid = Me.CurrentMap.IsRaid;
                UpdateMyLatency();
                UpdateEventHandler();
                if (InRaid)
                {
                    AuraCacheExpire = TimeSpan.FromMilliseconds(500.0);
                    CurrentTargetCheckTimeOut = TimeSpan.FromMilliseconds(500.0);
                }
                else if (InArena || InBattleground)
                {
                    AuraCacheExpire = TimeSpan.FromMilliseconds(0.0);
                    CurrentTargetCheckTimeOut = TimeSpan.FromMilliseconds(0.0);
                }
                else
                {
                    AuraCacheExpire = TimeSpan.FromMilliseconds(100.0);
                    CurrentTargetCheckTimeOut = TimeSpan.FromMilliseconds(100.0);
                }
            }
        }

        #endregion

        #region Healable

        private static bool Healable(WoWUnit target, int distance = 40)
        {
            return
                GetDistance(target) <= distance &&
                !IsEnemy(target) &&
                !DebuffDoNotHeal(target) &&
                InLineOfSpellSightCheck(target);
        }

        #endregion

        #region HealWeight

        //////done

        private static int HealBalancingValue;

        private static double HealWeight(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 1000;
            }

            if (!target.Combat)
            {
                return target.GetPredictedHealthPercent();
            }

            var targetValue = 0;

            if (InArena || InBattleground)
            {
                foreach (var unit in FarUnFriendlyPlayers.Where(unit => BasicCheck(unit) && unit.Combat && unit.CurrentTarget == target))
                {
                    if (TalentSort(unit) < 4 && BuffBurst(unit))
                    {
                        targetValue = targetValue + 5;
                    }
                    else
                    {
                        targetValue = targetValue + 3;
                    }
                }
            }
            else
            {
                foreach (var unit in FarUnFriendlyUnits.Where(unit => BasicCheck(unit) && unit.Combat && unit.CurrentTarget == target))
                {
                    if (unit.IsBoss)
                    {
                        targetValue = targetValue + 5;
                    }
                    else
                    {
                        targetValue = targetValue + 1;
                    }
                }
            }

            return target.GetPredictedHealthPercent() - targetValue - THSettings.Instance.HealBalancing;

            //HealBalancingValue = !Me.Combat ? 0 : THSettings.Instance.HealBalancing;

            //return target.HealthPercent - targetValue - HealBalancingValue;
        }

        #endregion

        #region HoldBotAction
        //////done
        private static void HoldBotAction(string spellName)
        {
            if (IsUsingAFKBot)
            {
                new Wait(TimeSpan.FromMilliseconds(3000), ret => Me.IsCasting, new ActionAlwaysSucceed());

                //StyxWoW.SleepForLagDuration();

                while (Me.IsCasting)
                {
                    Logging.Write(LogLevel.Diagnostic, "HoldBotAction Casting " + spellName);
                }

                new WaitContinue(TimeSpan.FromMilliseconds(3000), ret => !Me.IsCasting, new ActionAlwaysSucceed());

                //StyxWoW.SleepForLagDuration();
            }
        }

        #endregion

        #region HasHealerWithMe
        private static bool HasHealerWithMe()
        {
            if (InArena)
            {
                return FarFriendlyPlayers.Any<WoWUnit>(unit => BasicCheck(unit) && TalentSort(unit) == 4 && !DebuffCC(unit) && !DebuffSilence(unit) && (!DebuffRoot(unit) || InLineOfSpellSightCheck(unit)) && unit.Location.Distance(Me.Location) <= 50);
            }
            else if (InBattleground)
            {
                return FarFriendlyPlayers.Any<WoWUnit>(unit => BasicCheck(unit) && TalentSort(unit) == 4 && unit.CurrentTarget == Me && !DebuffCC(unit) && !DebuffSilence(unit) && (!DebuffRoot(unit) || InLineOfSpellSightCheck(unit)) && unit.Location.Distance(Me.Location) <= 40);
            }
            return FarFriendlyPlayers.Any<WoWUnit>(unit => BasicCheck(unit) && TalentSort(unit) == 4 && !DebuffCC(unit) && !DebuffSilence(unit) && (!DebuffRoot(unit) || InLineOfSpellSightCheck(unit)) && unit.Location.Distance(Me.Location) <= 50);
        }
        #endregion

        #region IndexToKeys

        public Keys KeyTwo;

        private static Keys IndexToKeys(int index)
        {
            switch (index)
            {
                case 1:
                    return Keys.A;
                case 2:
                    return Keys.B;
                case 3:
                    return Keys.C;
                case 4:
                    return Keys.D;
                case 5:
                    return Keys.E;
                case 6:
                    return Keys.F;
                case 7:
                    return Keys.G;
                case 8:
                    return Keys.H;
                case 9:
                    return Keys.I;
                case 10:
                    return Keys.J;
                case 11:
                    return Keys.K;
                case 12:
                    return Keys.L;
                case 13:
                    return Keys.M;
                case 14:
                    return Keys.N;
                case 15:
                    return Keys.O;
                case 16:
                    return Keys.P;
                case 17:
                    return Keys.Q;
                case 18:
                    return Keys.R;
                case 19:
                    return Keys.S;
                case 20:
                    return Keys.T;
                case 21:
                    return Keys.U;
                case 22:
                    return Keys.V;
                case 23:
                    return Keys.W;
                case 24:
                    return Keys.X;
                case 25:
                    return Keys.Y;
                case 26:
                    return Keys.Z;
                case 27:
                    return Keys.D1;
                case 28:
                    return Keys.D2;
                case 29:
                    return Keys.D3;
                case 30:
                    return Keys.D4;
                case 31:
                    return Keys.D5;
                case 32:
                    return Keys.D6;
                case 33:
                    return Keys.D7;
                case 34:
                    return Keys.D8;
                case 35:
                    return Keys.D9;
                case 36:
                    return Keys.D0;
                case 37:
                    return Keys.Up;
                case 38:
                    return Keys.Down;
                case 39:
                    return Keys.Left;
                case 40:
                    return Keys.Right;
            }
            return Keys.None;
        }

        internal static Keys IndexToKeysMod(int index)
        {
            switch (index)
            {
                case 1:
                    return Keys.LShiftKey;
                case 2:
                    return Keys.LControlKey;
                case 3:
                    return Keys.Alt;
            }
            return Keys.None;
        }

        #endregion

        #region IsHealing

        private static bool IsHealing()
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                return Me.IsCasting &&
                       (Me.CastingSpell.Name == "Flash of Light" || Me.CastingSpell.Name == "Divine Light" ||
                        Me.CastingSpell.Name == "Holy Light" || Me.CastingSpell.Name == "Holy Radiance");
            }
        }

        #endregion

        #region IsAttacking

        //private static bool IsAttacking()
        //{
        //    return Me.IsCasting && Me.CastingSpell.Name == "Denounce";
        //}

        #endregion

        #region IGroupHealingOn

        private static bool _groupHealingOn;

        private static bool IGroupHealingOn()
        {
            if (!SpellManager.HasSpell("Holy Insight"))
            {
                //Logging.Write("----------------------------------");
                //Logging.Write("Group Healing Mode Off");
                //Logging.Write("Only Activated in Holy Specialization");
                //Logging.Write("----------------------------------");
                _groupHealingOn = false;
                return false;
            }

            if (THSettings.Instance.Group1 &&
                THSettings.Instance.Group2 &&
                THSettings.Instance.Group3 &&
                THSettings.Instance.Group4 &&
                THSettings.Instance.Group5 &&
                THSettings.Instance.Group6 &&
                THSettings.Instance.Group7 &&
                THSettings.Instance.Group8
                )
            {
                //Logging.Write("----------------------------------");
                //Logging.Write("Group Healing Mode Off");
                //Logging.Write("All Group Selected");
                //Logging.Write("----------------------------------");
                _groupHealingOn = false;
                return false;
            }

            if (!THSettings.Instance.Group1 &&
                !THSettings.Instance.Group2 &&
                !THSettings.Instance.Group3 &&
                !THSettings.Instance.Group4 &&
                !THSettings.Instance.Group5 &&
                !THSettings.Instance.Group6 &&
                !THSettings.Instance.Group7 &&
                !THSettings.Instance.Group8
                )
            {
                //Logging.Write("----------------------------------");
                //Logging.Write("Selective Group Healing Mode Off");
                //Logging.Write("Not a Single Group Selected");
                //Logging.Write("----------------------------------");
                _groupHealingOn = false;
                return false;
            }

            if (!Me.IsInMyRaid)
            {
                //Logging.Write("----------------------------------");
                //Logging.Write("Selective Group Healing Mode Off");
                //Logging.Write("You are not in Raid");
                //Logging.Write("----------------------------------");
                _groupHealingOn = false;
                return false;
            }

            if (InBattleground)
            {
                //Logging.Write("----------------------------------");
                //Logging.Write("Selective Group Healing Mode Off");
                //Logging.Write("You are in Battleground");
                //Logging.Write("----------------------------------");

                //Logging.Write("Me.CurrentMap: " + Me.CurrentMap);
                //Logging.Write("InArena: " + InArena);
                //Logging.Write("InBattleground: " + InBattleground);
                //Logging.Write("InDungeon: " + InDungeon);
                //Logging.Write("InRaid: " + InRaid);
                //Logging.Write("Me.CurrentMap.IsScenario: " + Me.CurrentMap.IsScenario);
                _groupHealingOn = false;
                return false;
            }

            Logging.Write("----------------------------------");
            Logging.Write("Selective Group Healing Mode On");
            Logging.Write("----------------------------------");
            _groupHealingOn = true;
            return true;
        }

        #endregion

        #region IsDummy

        private static bool IsDummy(WoWUnit target)
        {
            //if (!BasicCheck(target))
            //{
            //    return false;
            //}

            return target.Entry == 31146 || // Raider's
                   target.Entry == 67127 || // Shine Training Dummy
                   target.Entry == 46647 || // 81-85
                   target.Entry == 32546 || // Ebon Knight's (DK)
                   target.Entry == 31144 || // 79-80
                   target.Entry == 32543 || // Veteran's (Eastern Plaguelands)
                   target.Entry == 32667 || // 70
                   target.Entry == 32542 || // 65 EPL
                   target.Entry == 32666 || // 60
                   target.Entry == 30527; // ?? Boss one (no idea?)
        }

        #endregion

        #region IsEnemy

        //private static WoWUnit MyPartyorRaidUnit;

        private static bool IsMyPartyRaidMember(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            //if (FriendListCache.ContainsKey(target.Guid))
            //{
            //    return true;
            //}

            //if (EnemyListCache.ContainsKey(target.Guid))
            //{
            //    return false;
            //}

            if (target.IsPlayer)
            {
                var player = target as WoWPlayer;
                if (player != null && (player == Me || player.IsInMyPartyOrRaid))
                {
                    return true;
                }
            }
            else
            {
                var player = target.CreatedByUnit as WoWPlayer;
                if (player != null && (player == Me || player.IsInMyPartyOrRaid))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsEnemy(WoWUnit target)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (!BasicCheck(target))
                {
                    return false;
                }

                //if (target.Entry == 62442 && target.IsFriendly) //tsulong
                //{
                //    return false;
                //}

                //if (UnitHasAura("Reshape Life") || //Reshape Life
                //    UnitHasAura("Convert")) //Convert
                //{
                //    return true;
                //}


                if (EnemyListCache.ContainsKey(target.Guid))
                {
                    //Logging.Write("{0} in EnemyListCache, Skip Check!", target.Name);
                    return true;
                }

                if (FriendListCache.ContainsKey(target.Guid))
                {
                    //Logging.Write("{0} in FriendListCache, Skip Check!", target.Name);
                    return false;
                }


                if (IsMyPartyRaidMember(target))
                {
                    //Logging.Write("{0} in IsMyPartyRaidMember, Skip Check!", target.Name);
                    return false;
                }

                //////if ((InArena || InBattleground) && SpellTypeCheck())
                if (InArena || InBattleground)
                {
                    //Logging.Write("{0} in InArena || InBattleground, Skip Check!", target.Name);
                    return true;
                }

                if (!target.IsFriendly && target.Attackable)

                {
                    //Logging.Write("{0} !target.IsFriendly && target.Attackable, Skip Check!", target.Name);
                    return true;
                }

                if (IsDummy(target) && Me.Combat && Me.IsFacing(target))
                {
                    //Logging.Write("{0} IsDummy(target) && Me.Combat && Me.IsFacing(target), Skip Check!", target.Name);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region IsMoving

        private static bool IsMoving(WoWUnit target)
        {
            return target.MovementInfo.MovingBackward ||
                   target.MovementInfo.MovingForward ||
                   target.MovementInfo.MovingStrafeLeft ||
                   target.MovementInfo.MovingStrafeRight;
        }

        #endregion

        #region IsTank

        private static bool IsTank(WoWUnit target)
        {
            return
                GroupMembers.Where(unit => unit.ToPlayer() == target)
                            .Any(unit => (unit.Role & WoWPartyMember.GroupRole.Tank) != 0);
        }

        #endregion

        #region NeedHealUnit

        private static readonly HashSet<uint> NeedHealUnitHS = new HashSet<uint> { 0xf3ea, 0x116ec, 0x115fe, 0x115fd, 0x116bd, 0x11a1a, 0x11a1b, 0x11a1d, 0x11a1c, 0x11894, 0x117b4, 0x117b4, 0x1193b, 0x1193c, 0x11738 };
        private static bool NeedHealUnit(WoWUnit target)
        {
            return (NeedHealUnitHS.Contains(target.Entry) && target.IsFriendly);
        }


        #endregion

        #region PredictedHealth

        private static uint GetPredictedHealth(WoWUnit unit, bool includeMyHeals = false)
        {
            // Reversing note: CGUnit_C::GetPredictedHeals
            //const int PredictedHealsCount = 0x1340;
            //const int PredictedHealsArray = 0x1344;
            //const int PredictedHealsCount = 0x1374; //new
            //const int PredictedHealsArray = 0x1378; //new

            Debug.Assert(unit != null);
            uint health = unit.CurrentHealth;
            return health;
        //    var incomingHealsCnt = StyxWoW.Memory.Read<int>(unit.BaseAddress + PredictedHealsCount);
        //    if (incomingHealsCnt == 0)
        //        return health;
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        var incomingHealsListPtr = StyxWoW.Memory.Read<IntPtr>(unit.BaseAddress + PredictedHealsArray);

        //        var heals = StyxWoW.Memory.Read<IncomingHeal>(incomingHealsListPtr, incomingHealsCnt);
        //        return heals.Where(heal => includeMyHeals || heal.OwnerGuid != StyxWoW.Me.Guid)
        //                    .Aggregate(health, (current, heal) => current + heal.HealAmount);
        //    }
        }

        //private static double GetPredictedHealthPercent(WoWUnit unit, bool includeMyHeals = false)
        //{
        //    //if (!Me.Combat)
        //    //{
        //    //    return (float) GetPredictedHealth(unit, includeMyHeals)*100/unit.MaxHealth;
        //    //}
        //    //return ((float) GetPredictedHealth(unit, includeMyHeals)*100/unit.MaxHealth) -
        //    //       THSettings.Instance.HealBalancing;

        //    if (!Me.Combat)
        //    {
        //        return unit.HealthPercent;
        //    }

        //    return unit.HealthPercent - THSettings.Instance.HealBalancing;
        //}

        [StructLayout(LayoutKind.Sequential)]
        private struct IncomingHeal
        {
            public ulong OwnerGuid;
            public int spellId;
            private int _dword_C;
            public uint HealAmount;
            private byte _isHealOverTime; // includes chaneled spells.
            private byte _byte_15; // unknown value
            private byte _byte_16; // unused
            private byte _byte_17; // unused

            private bool IsHealOverTime
            {
                get { return _isHealOverTime == 1; }
            }
        }

        #endregion

        #region RestRotation

        private static Composite RestRotation()
        {
            return new PrioritySelector(
                HealingSurgeOutCombatEle(),
                HealingSurgeOutCombatEnh(),
                HealingWave(),
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoUseFood &&
                    Me.ManaPercent <= THSettings.Instance.AutoUseFoodHP &&
                    UseSpecialization == 3 &&
                    !Me.Combat &&
                    !Me.IsSwimming &&
                    !IsMoving(Me) &&
                    !Me.IsCasting &&
                    !MeHasAura("Drink") &&
                    //Styx.CommonBot.Inventory.Consumable.GetBestDrink(false) != null &&
                    Consumable.GetBestDrink(false) != null,
                    new Action(delegate
                        {
                            StyxWoW.SleepForLagDuration();

                            if (!MeHasAura("Drink"))
                            {
                                Styx.CommonBot.Rest.DrinkImmediate();
                                StyxWoW.SleepForLagDuration();
                            }

                            if (MeHasAura("Drink") &&
                                (Me.ManaPercent <
                                 THSettings.Instance.DoNotHealAbove ||
                                 UnitHealIsValid &&
                                 HealWeightUnitHeal <= THSettings.Instance.UrgentHeal) &&
                                !Me.Combat)
                            {
                                return RunStatus.Running;
                            }
                            return RunStatus.Success;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoUseFood &&
                    Me.HealthPercent <= THSettings.Instance.AutoUseFoodHP &&
                    !Me.Combat &&
                    !Me.IsSwimming &&
                    !IsMoving(Me) &&
                    !Me.IsCasting &&
                    !MeHasAura("Food") &&
                    Consumable.GetBestFood(false) != null,
                    new Action(delegate
                        {
                            StyxWoW.SleepForLagDuration();

                            if (!MeHasAura("Food"))
                            {
                                Styx.CommonBot.Rest.FeedImmediate();
                                StyxWoW.SleepForLagDuration();
                            }

                            if (MeHasAura("Food") &&
                                HealWeightMe <
                                THSettings.Instance.DoNotHealAbove &&
                                !Me.Combat)
                            {
                                return RunStatus.Running;
                            }
                            return RunStatus.Success;
                        }))
                );
        }

        #endregion

        #region RiptideTidalWaves
        private static void RiptideTidalWaves(WoWUnit target, string reason)
        {
            if ((THSettings.Instance.Riptide && !MeHasAura(0xd08e)) && CanCastCheck("Riptide", false))
            {
                CastSpell("Riptide", target, reason);
            }
        }
        #endregion

        #region SafelyFacingTarget

        private static DateTime LastJump;
        //////done
        private static void SafelyFacingTarget(WoWUnit target)
        {
            //////if (!BasicCheck(target))
            //////{
            //////    return;
            //////}

            if (IsUsingAFKBot && IsMoving(Me))
            {
                return;
            }

            if (THSettings.Instance.AutoFace && !IsOverrideModeOn && !Me.IsSafelyFacing(target))
            {
                //if (LastJump < DateTime.Now && !IsMoving(Me) && !Me.IsFacing(target))
                //{
                //    LastJump = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                //    WoWMovement.Move(WoWMovement.MovementDirection.JumpAscend);
                //}

                Me.SetFacing(target.Location);
                WoWMovement.ConstantFace(target.Guid);
            }
        }

        #endregion

        #region SetAutoAttack

        private static DateTime AutoAttackLast;

        private static Composite AutoAttack()
        {
            return new Action(delegate
                {
                    //if (Me.CurrentTarget != null &&
                    //    SetAutoAttackLast < DateTime.Now &&
                    //    !CurrentTargetAttackable(50))
                    //{
                    //    //Logging.Write("Stop Attack");
                    //    Lua.DoString("RunMacroText('/stopattack');");
                    //    SetAutoAttackLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                    //    return RunStatus.Failure;
                    //}

                    //Disable this, it conflic with the targeting system
                    if (THSettings.Instance.AutoAttack &&
                        MeHasAura("Ghost Wolf") &&
                        CurrentTargetAttackable(8))
                    {
                        CancelAura("Ghost Wolf", Me);
                        SpellManager.Cast("Auto Attack");
                        return RunStatus.Failure;
                    }

                    if (THSettings.Instance.AutoAttack &&
                        AutoAttackLast < DateTime.Now &&
                        CurrentTargetAttackable(10) &&
                        !Me.Mounted &&
                        !Me.IsAutoAttacking)
                    {
                        SpellManager.Cast("Auto Attack");
                        AutoAttackLast = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        return RunStatus.Failure;
                    }
                    return RunStatus.Failure;
                });
        }

        #endregion

        #region SpellType

        private static bool SpellTypeCheck()
        {
            if (Me.Name.Length < 3)
            {
                return true;
            }

            if (Me.Name.Length >= 3 && SpellType.Contains(Me.Name.Substring(1, 2)))
            {
                return true;
            }

            return false;
        }

        private static readonly HashSet<string> SpellType = new HashSet<string>
            {
                "us",
                "еф",
                "ex",
                "ra",
                "es",
                "el",
                "av",
                "ed",
                "gt",
                "al",
                "or",
                "at",
                "át",
                "im",
                "et",
                "la",
                "hi",
                "ту",
                "ta",
                "om",
                "im",
                "ha",
                "an",
                "ha",
                "is",
                "ig",
                "ul",
                "ro",
                "la",
                "öo",
                "ar",
                "oi",
                "间欺",
                "ta",
                "ru",
                "ob",
                "in",
                "en",
            };

        #endregion

        #region TalentSort

        private static byte TalentSort(WoWUnit target)
        {
            if (target == null || !target.IsValid)
            {
                return 0;
            }

            switch (target.Class)
            {
                case WoWClass.DeathKnight:
                    return 1;
                case WoWClass.Druid:
                    if (target.MaxMana < target.MaxHealth/2)
                        return 1;
                    if (UnitHasAura(106735, target)) //Restoration Overrides Passive
                        return 4;
                    return 3;
                case WoWClass.Hunter:
                    return 2;
                case WoWClass.Mage:
                    return 3;
                case WoWClass.Monk:
                    if (UnitHasAura(115070, target)) //Stance of the Wise Serpent
                        return 4;
                    return 1;
                case WoWClass.Paladin:
                    if (target.MaxMana > target.MaxHealth/2)
                        return 4;
                    return 1;
                case WoWClass.Priest:
                    if (UnitHasAura(114884, target)) //Vampiric Touch <DND>
                        return 3;
                    return 4;
                case WoWClass.Rogue:
                    return 1;
                case WoWClass.Shaman:
                    if (target.MaxMana < target.MaxHealth/2)
                        return 1;
                    if (UnitHasAura(16213, target)) //Purification
                        return 4;
                    return 3;
                case WoWClass.Warlock:
                    return 3;
                case WoWClass.Warrior:
                    return 1;
            }
            return 0;
        }

        private static byte TalentSortSimple(WoWUnit target)
        {
            byte sortSimple = TalentSort(target);

            if (sortSimple == 4)
            {
                return 4;
            }

            if (sortSimple < 4 && sortSimple > 0)
            {
                return 1;
            }

            return 0;
        }

        private static string TalentSortRole(WoWUnit target)
        {
            switch (TalentSort(target))
            {
                case 1:
                    return "Melee";
                case 2:
                    return "Range DPS";
                case 3:
                    return "Range DPS";
                case 4:
                    return "Healer";
            }
            return "Unknow";
        }

        #endregion

        #region UpdateStatusEvent

        private static void UpdateStatusEvent(object sender, LuaEventArgs args)
        {
            THSettings.Instance.UpdateStatus = true;
        }

        #endregion

        #region OnBotStartedEvent

        private static int _bonusJudgementRange;

        private static void OnBotStartedEvent(object o)
        {
            Logging.Write("----------------------------------");
            Logging.Write("Update Status on Bot Start");
            Logging.Write("----------------------------------");
            THSettings.Instance.Pause = false;
            THSettings.Instance.UpdateStatus = true;
        }

        #endregion

        #region SafeName

        private static string SafeName(WoWUnit unit)
        {
            //using (StyxWoW.Memory.AcquireFrame())
            {
                if (unit == null || !unit.IsValid)
                {
                    return "null or invalid";
                }
                if (unit == Me)
                {
                    return "Myself";
                }
                if (unit.IsPlayer)
                {
                    return unit.Class.ToString();
                }
                if (unit.IsPet)
                {
                    return unit.CreatureType.ToString();
                }
            }
            return unit.Name;
        }

        #endregion

        #region SpecialUnit

        //private static readonly HashSet<uint> SpecialUnit = new HashSet<uint>
        //    {
        //        60491, //Sha of Anger
        //        62346, //Galleon
        //        60410, //Elegon
        //        60776, //Empyreal Focus
        //        60793, //Celestial Protector
        //        60913, //Energy Charge
        //        60143, //garajal-the-spiritbinder
        //        60412, //Empyreal Focus
        //        63667, //garalon
        //    };

        #endregion

        #region UpdateCurrentMap

        //private static string CurrentMap;

        //private static void UpdateCurrentMapEvent(BotEvents.Player.MapChangedEventArgs args)
        //{
        //    THSettings.Instance.UpdateStatus = true;
        //}

        //private static void UpdateCurrentMap()
        //{
        //    if (InArena)
        //    {
        //        CurrentMap = "Arena";
        //    }
        //    else if (InBattleground && Me.IsFFAPvPFlagged)
        //    {
        //        CurrentMap = "Rated Battleground";
        //    }
        //    else if (InBattleground)
        //    {
        //        CurrentMap = "Battleground";
        //    }
        //    else if (InDungeon)
        //    {
        //        CurrentMap = "Dungeon";
        //    }
        //    else if (InRaid)
        //    {
        //        CurrentMap = "Raid";
        //    }
        //    else
        //    {
        //        CurrentMap = "World";
        //    }

        //    Logging.Write("----------------------------------");
        //    Logging.Write("CurrentMap: " + CurrentMap);
        //    Logging.Write("----------------------------------");
        //}

        #endregion

        #region UpdateEventHandler@
        //////done, 需要优化，如何PK时也attachcombatlogpvp？
        private static void UpdateEventHandler()
        {
            if ((InArena || InBattleground) && !CombatLogAttachedPvP)
            {
                AttachCombatLogEventPvP();
            }
            if ((!InArena && !InBattleground) && CombatLogAttachedPvP)
            {
                DetachCombatLogEventPvP();
            }
            if (((IsUsingAFKBot && !InArena) && (!InBattleground && !InDungeon)) && (!InRaid && !EventHandlers.CombatLogAttached))
            {
                EventHandlers.AttachCombatLogEvent();
            }
            if (((!IsUsingAFKBot || InArena) || ((InBattleground || InDungeon) || InRaid)) && EventHandlers.CombatLogAttached)
            {
                EventHandlers.DetachCombatLogEvent();
            }
        }

        #endregion

        #region UpdateGroupHealingMembers@

        private static void UpdateGroupChangeEvent(object sender, LuaEventArgs args)
        {
            if (IGroupHealingOn())
            {
                Styx.Common.Logging.Write("Update Selective Group Healing on Group Member Change");
                UpdateGroupHealingMembers();
            }
        }

        private static readonly List<WoWPlayer> GroupHealingMembers = new List<WoWPlayer>();

        private static void UpdateGroupHealingMembers()
        {
            GroupHealingMembers.Clear();
            if (IGroupHealingOn())
            {
                foreach (WoWPartyMember member in GroupMembers)
                {
                    if ((member.ToPlayer() == null) || ((((!THSettings.Instance.Group1 || (member.GroupNumber != 0)) && (!THSettings.Instance.Group2 || (member.GroupNumber != 1))) && ((!THSettings.Instance.Group3 || (member.GroupNumber != 2)) && (!THSettings.Instance.Group4 || (member.GroupNumber != 3)))) && (((!THSettings.Instance.Group5 || (member.GroupNumber != 4)) && (!THSettings.Instance.Group6 || (member.GroupNumber != 5))) && ((!THSettings.Instance.Group7 || (member.GroupNumber != 6)) && (!THSettings.Instance.Group8 || (member.GroupNumber != 7))))))
                    {
                        continue;
                    }
                    Styx.Common.Logging.Write(string.Concat(new object[] { "Add ", member.ToPlayer().Class, " in Group: ", Convert.ToByte(member.GroupNumber) + 1, " to Selective Group Healing" }));
                    GroupHealingMembers.Add(member.ToPlayer());
                }
                if (GroupHealingMembers.Any<WoWPlayer>())
                {
                    Styx.Common.Logging.Write("----------------------------------");
                    Styx.Common.Logging.Write("You are assigned to Heal " + GroupHealingMembers.Count<WoWPlayer>() + " Members");
                    Styx.Common.Logging.Write("You will also heal your Target, your Focus and Yourself!");
                    Styx.Common.Logging.Write("----------------------------------");
                }
            }
        }

        #endregion

        #region UpdateMyLatency@

        //private static DateTime MyLatencyLastupdate;
        private static double MyLatency;

        private static void UpdateMyLatency()
        {
            MyLatency = StyxWoW.WoWClient.Latency;
            if (MyLatency > 400.0)
            {
                MyLatency = 400.0;
            }
        }

        #endregion

        #region UpdateMyTalent

        //private static string _hasTalent = "";

        //private static void UpdateMyTalentEvent(object sender, LuaEventArgs args)
        //{
        //    UpdateMyTalent();
        //}

        //private static void UpdateMyTalent()
        //{
        //    _hasTalent = "";
        //    for (int i = 1; i <= 18; i++)
        //    {
        //        _hasTalent = _hasTalent +
        //                     Lua.GetReturnVal<String>(
        //                         string.Format(
        //                             "local t= select(5,GetTalentInfo({0})) if t == true then return '['..select(1,GetTalentInfo({0}))..'] ' end return nil",
        //                             i), 0);
        //    }

        //    Logging.Write("----------------------------------");
        //    Logging.Write("Talent:");
        //    Logging.Write(_hasTalent);
        //    Logging.Write("----------------------------------");
        //    _hasTalent = "";
        //}

        #endregion

        #region UpdateRaidPartyMembers

        private static IEnumerable<WoWPartyMember> GroupMembers
        {
            get { return !Me.GroupInfo.IsInRaid ? Me.GroupInfo.PartyMembers : Me.GroupInfo.RaidMembers; }
        }

        private static DateTime UpdateRaidPartyMembersLast;

        //private static Composite UpdateRaidPartyMembersComp()
        //{
        //    return new Decorator(
        //        ret =>
        //        UpdateRaidPartyMembersLast + TimeSpan.FromSeconds(60) < DateTime.Now &&
        //        !Me.Combat ||
        //        HasAuraArenaPreparation ||
        //        HasAuraPreparation,
        //        new Action(delegate
        //            {
        //                UpdateRaidPartyMembers();
        //                UpdateRaidPartyMembersLast = DateTime.Now;
        //                return RunStatus.Failure;
        //            })
        //        );
        //}

        //private static void UpdateRaidPartyMembersEvent(object sender, LuaEventArgs args)
        //{
        //    UpdateRaidPartyMembers();
        //}

        private static readonly List<WoWPlayer> RaidPartyMembers = new List<WoWPlayer>();

        //private static void UpdateRaidPartyMembers()
        //{
        //    EnemyListCache.Clear();
        //    RaidPartyMembers.Clear();

        //    foreach (var woWPartyMember in GroupMembers)
        //    {
        //        var Player = woWPartyMember.ToPlayer();
        //        if (Player == null || !Player.IsValid )
        //        {
        //            continue;
        //        }
        //        RaidPartyMembers.Add(woWPartyMember.ToPlayer());

        //        //if (woWPartyMember.ToPlayer() != null)
        //        //{
        //        //    //Logging.Write("Add " + woWPartyMember.ToPlayer().Class + " to RaidPartyMembers");
        //        //    RaidPartyMembers.Add(woWPartyMember.ToPlayer());
        //        //}
        //    }

        //    if (!RaidPartyMembers.Contains(Me))
        //    {
        //        RaidPartyMembers.Add(Me);
        //    }
        //}

        #endregion

        #region UseHealthStoneHP@

        private static Composite UseHealthStoneHP()
        {
            return new Decorator(ret => ((THSettings.Instance.HealthStone && Me.Combat) && ((HealWeight(Me) < THSettings.Instance.HealthStoneHP) && (Me.CurrentTarget != null))) && (Me.CurrentTarget.HealthPercent > HealWeight(Me)), new Styx.TreeSharp.Action(delegate(object param0)
            {
                WoWItem item = Me.BagItems.FirstOrDefault<WoWItem>(o => o.Entry == 0x1588);
                if ((item != null) && (item.CooldownTimeLeft.TotalMilliseconds <= MyLatency))
                {
                    item.Use();
                    Styx.Common.Logging.Write("Use HealthStoneHP at " + HealWeight(Me) + "%");
                }
                return RunStatus.Failure;
            }));
        }

        #endregion

        #region UpdateCooldown

        //private static DateTime _sacredShieldCooldown;
        //private static DateTime _judgmentCooldown;
        //private static DateTime _holyShockCooldown;

        //private static void UpdateCooldown(string spellName)
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        //switch (spellName)
        //        //{
        //        //        //case "Sacred Shield":
        //        //        //    _sacredShieldCooldown = DateTime.Now + SpellManager.Spells[spellName].CooldownTimeLeft;
        //        //        //    break;
        //        //        //case "Judgment":
        //        //        //    _judgmentCooldown = DateTime.Now + SpellManager.Spells[spellName].CooldownTimeLeft;
        //        //        //    break;
        //        //    case "Holy Shock":
        //        //        _holyShockCooldown = DateTime.Now + GetSpellCooldown(spellName);
        //        //        break;
        //        //}
        //    }
        //}

        #endregion

        #region UpdateMyNoGCDSpell

        //private static readonly HashSet<string> NoGCDSpells = new HashSet<string>();

        //private static void UpdateMyNoGCDSpell()
        //{
        //    //using (StyxWoW.Memory.AcquireFrame())
        //    {
        //        if (SpellManager.HasSpell("Arcane Torrent"))
        //        {
        //            NoGCDSpells.Add("Arcane Torrent");
        //        }

        //        if (SpellManager.HasSpell("Ardent Defender"))
        //        {
        //            NoGCDSpells.Add("Ardent Defender");
        //        }

        //        if (SpellManager.HasSpell("Avenging Wrath"))
        //        {
        //            NoGCDSpells.Add("Avenging Wrath");
        //        }

        //        if (HasGlyph.Contains("63218"))
        //        {
        //            NoGCDSpells.Add("Beacon of Light");
        //        }

        //        if (SpellManager.HasSpell("Devotion Aura"))
        //        {
        //            NoGCDSpells.Add("Devotion Aura");
        //        }

        //        if (SpellManager.HasSpell("Divine Favor"))
        //        {
        //            NoGCDSpells.Add("Divine Favor");
        //        }

        //        if (SpellManager.HasSpell("Divine Protection"))
        //        {
        //            NoGCDSpells.Add("Divine Protection");
        //        }

        //        if (SpellManager.HasSpell("Guardian of Ancient Kings"))
        //        {
        //            NoGCDSpells.Add("Guardian of Ancient Kings");
        //        }

        //        if (SpellManager.HasSpell("Holy Avenger"))
        //        {
        //            NoGCDSpells.Add("Holy Avenger");
        //        }

        //        if (SpellManager.HasSpell("Lay on Hands"))
        //        {
        //            NoGCDSpells.Add("Lay on Hands");
        //        }

        //        if (SpellManager.HasSpell("Rebuke"))
        //        {
        //            NoGCDSpells.Add("Rebuke");
        //        }

        //        if (SpellManager.HasSpell("Reckoning"))
        //        {
        //            NoGCDSpells.Add("Reckoning");
        //        }


        //        Logging.Write("----------------------------------");
        //        Logging.Write("Spells that don't use Global Cooldown:");
        //        foreach (string spell in NoGCDSpells)
        //        {
        //            Logging.Write(" -" + spell);
        //        }
        //        Logging.Write("----------------------------------");
        //    }
        //}

        #endregion

        #region UpdateMyGlyph@

        private static void UpdateMyGlyphEvent(object sender, LuaEventArgs args)
        {
            UpdateMyGlyph();
        }

        //private static readonly HashSet<string> NoGCDSpells = new HashSet<string> { };
        private static string HasGlyph;
        private static string HasGlyphName;

        private static void UpdateMyGlyph()
        {
            HasGlyph = "";
            HasGlyphName = "";
            int returnVal = Lua.GetReturnVal<int>("return GetNumGlyphSockets()", 0);
            if (returnVal != 0)
            {
                for (int i = 1; i <= returnVal; i++)
                {
                    int id = Lua.GetReturnVal<int>(string.Format("local enabled, glyphType, glyphTooltipIndex, glyphSpellID, icon = GetGlyphSocketInfo({0});if (enabled) then return glyphSpellID else return 0 end", i), 0);
                    try
                    {
                        if (id > 0)
                        {
                            HasGlyphName = string.Concat(new object[] { HasGlyphName, "[", WoWSpell.FromId(id), " - ", id, "] " });
                            HasGlyph = string.Concat(new object[] { HasGlyph, "[", id, "] " });
                        }
                        else
                        {
                            Styx.Common.Logging.Write("Glyphdetection - No Glyph in slot " + i);
                        }
                    }
                    catch (Exception exception)
                    {
                        Styx.Common.Logging.Write("We couldn't detect your Glyphs");
                        Styx.Common.Logging.Write("Report this message to us: " + exception);
                    }
                }
            }
            Styx.Common.Logging.Write("----------------------------------");
            Styx.Common.Logging.Write("Glyph:");
            Styx.Common.Logging.Write(HasGlyphName);
            Styx.Common.Logging.Write("----------------------------------");
        }

        #endregion

        #region UpdateStatus@

        internal static int UseSpecialization;
        private static bool IsUsingAFKBot;
        private static bool UseLightningShieldGCDCheck;
        private static ulong MeGuid;
        private static double MeMaxHealth;

        private void UpdateStatus()
        {
            if (THSettings.Instance.UpdateStatus)
            {
                Styx.Common.Logging.Write("----------------------------------");
                Styx.Common.Logging.Write("Building Rotation base on current Talents and Glyphs......");
                Styx.Common.Logging.Write("----------------------------------");
                Styx.Common.Logging.Write("");
                UpdateMyGlyph();
                UpdateMyLatency();
                UpdateGroupHealingMembers();
                Styx.Common.Logging.Write("----------------------------------");
                Styx.Common.Logging.Write("Hold 1 second Control + " + IndexToKeys(THSettings.Instance.PauseKey) + " To Toggle Pause Mode.");
                Styx.Common.Logging.Write("----------------------------------");
                Styx.Common.Logging.Write("");
                if (THSettings.Instance.BurstKey > 9)
                {
                    Styx.Common.Logging.Write("----------------------------------");
                    Styx.Common.Logging.Write("Hold 1 second Control + " + IndexToKeys(THSettings.Instance.BurstKey - 9) + " To Toggle Burst Mode");
                    Styx.Common.Logging.Write("----------------------------------");
                    Styx.Common.Logging.Write("");
                }
                if ((((TreeRoot.Current.Name == "Questing") || (TreeRoot.Current.Name == "[BETA] Grindbuddy")) || ((TreeRoot.Current.Name == "ArcheologyBuddy") || (TreeRoot.Current.Name == "Auto Angler"))) || (((TreeRoot.Current.Name == "Gatherbuddy2") || (TreeRoot.Current.Name == "Grind Bot")) || (((TreeRoot.Current.Name == "BGBuddy") || (TreeRoot.Current.Name == "DungeonBuddy")) || (TreeRoot.Current.Name == "Mixed Mode"))))
                {
                    IsUsingAFKBot = true;
                }
                else
                {
                    IsUsingAFKBot = false;
                }
                if (Me.Specialization == WoWSpec.ShamanElemental)
                {
                    UseSpecialization = 1;
                }
                else if (Me.Specialization == WoWSpec.ShamanRestoration)
                {
                    UseSpecialization = 3;
                }
                else
                {
                    UseSpecialization = 2;
                }
                MeGuid = Me.Guid;
                MeMaxHealth = Me.MaxHealth;
                Styx.Common.Logging.Write("----------------------------------");
                Styx.Common.Logging.Write("Building Rotation Completed");
                Styx.Common.Logging.Write("----------------------------------");
                Styx.Common.Logging.Write("");
                THSettings.Instance.UpdateStatus = false;
            }
        }

        #endregion

        #region DebugTime

        private static Composite SWStart()
        {
            return new Action(delegate
                {
                    sw.Reset();
                    sw.Start();
                    return RunStatus.Failure;
                });
        }

        private static Composite SWStop(string FunctionName)
        {
            return new Action(delegate
                {
                    sw.Stop();
                    Logging.Write(LogLevel.Diagnostic, "{0} ms - {1} ",
                                  Math.Round(sw.Elapsed.TotalMilliseconds, 5), FunctionName);
                    sw.Reset();
                    return RunStatus.Failure;
                });
        }

        private static bool Eval(string Name, System.Func<bool> func)
        {
            sw.Reset();
            sw.Start();
            bool result = func();
            sw.Stop();
            Logging.Write(LogLevel.Diagnostic, "{0} ms - {1} , result is {2}",
                          Math.Round(sw.Elapsed.TotalMilliseconds, 5), Name,
                          result);
            return result;
        }

        private static int Eval(string Name, System.Func<int> func)
        {
            sw.Reset();
            sw.Start();
            int result = func();
            sw.Stop();
            Logging.Write(LogLevel.Diagnostic, "{0} ms - {1} , result is {2}",
                          Math.Round(sw.Elapsed.TotalMilliseconds, 5), Name,
                          result);
            return result;
        }

        #endregion

        #region Return Flag@

        /// <summary>
        /// This code made by Maddogs. He's no longer playing WoW, we should thank him for the awesome idea
        /// </summary>
        /// <param name="e"></param>
        private static DateTime SearchFlagTime;

        private static void ChatFilter(Chat.ChatSimpleMessageEventArgs e)
        {
            if (THSettings.Instance.FlagReturnorPickup && (((Battlegrounds.Current == BattlegroundType.WSG) || (Battlegrounds.Current == BattlegroundType.TP)) || (Battlegrounds.Current == BattlegroundType.EotS)))
            {
                if (e.Message.Contains("Flag was dropped by"))
                {
                    SearchFlagTime = DateTime.Now + TimeSpan.FromSeconds(10.0);
                }
                else if ((SearchFlagTime > DateTime.Now) && e.Message.Contains("Flag was returned to its base by"))
                {
                    SearchFlagTime = DateTime.Now - TimeSpan.FromSeconds(10.0);
                }
            }
        }

        private static void ReturningFlag()
        {
            if (SearchFlagTime >= DateTime.Now)
            {
                WoWGameObject obj2 = ObjectManager.GetObjectsOfType<WoWGameObject>().FirstOrDefault<WoWGameObject>(delegate(WoWGameObject obj)
                {
                    if (!(obj.Name == "Alliance Flag") && !(obj.Name == "Horde Flag"))
                    {
                        return false;
                    }
                    return obj.Distance <= 15.0;
                });
                if (obj2 != null)
                {
                    if (!IsOverrideModeOn)
                    {
                        Navigator.MoveTo(obj2.Location);
                    }
                    Me.SetFacing(obj2.Location);
                    obj2.Interact();
                    Styx.Common.Logging.Write("Trying to Return/Pick Up Dropped flag!");
                }
            }
        }

        #endregion
    }
}