using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.WoWInternals.World;
using Action = Styx.TreeSharp.Action;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region AncestralGuidance

        private static Composite AncestralGuidance()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AncestralGuidanceCooldown ||
                 THSettings.Instance.AncestralGuidanceBurst &&
                 THSettings.Instance.Burst) &&
                //SSpellManager.HasSpell("Ancestral Guidance") &&
                //!Me.Mounted &&
                Me.Combat &&
                CanCastCheck("Ancestral Guidance", true),
                new Action(
                    ret =>
                        {
                            CastSpell("Ancestral Guidance", Me, "AncestralGuidance");
                            return RunStatus.Failure;
                        })
                );
        }

        #endregion

        #region Ascendance

        private static int CountUnitAscendanceResto(WoWUnit unitCenter)
        {
            //var i = 0;

            //foreach (var unit in NearbyFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        HealWeight(unit) < 80 &&
            //        HealWeight(unit) < THSettings.Instance.AscendanceRestoHP + 20 &&
            //        unitCenter.Location.Distance(unit.Location) <= 20)
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.AscendanceRestoUnit)
            //    {
            //        break;
            //    }
            //}
            //return i;

            return NearbyFriendlyPlayers.Count(
                unit =>
                BasicCheck(unit) &&
                HealWeight(unit) < 80 &&
                HealWeight(unit) < THSettings.Instance.AscendanceRestoHP + 20 &&
                unitCenter.Location.Distance(unit.Location) <= 20);
        }

        private static Composite AscendanceResto()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AscendanceResto &&
                HealWeightUnitHeal <= THSettings.Instance.AscendanceRestoHP &&
                UnitHealIsValid &&
                //!Me.Mounted &&
                UnitHeal.Combat &&
                !UnitHeal.IsPet &&
                !MeHasAura("Ascendance") &&
                //SSpellManager.HasSpell("Ascendance") &&
                !Invulnerable(UnitHeal) &&
                HaveDPSTarget(UnitHeal) &&
                CanCastCheck("Ascendance", true) &&
                CountUnitAscendanceResto(UnitHeal) >=
                THSettings.Instance.AscendanceRestoUnit,
                new Action(
                    ret =>
                        {
                            CastSpell("Ascendance", Me, "AscendanceResto");
                            AuraCacheUpdate(Me, true);
                            //Eval("CountUnitAscendanceResto(UnitHeal) >= THSettings.Instance.AscendanceRestoUnit",
                            //() => CountUnitAscendanceResto(UnitHeal) >= THSettings.Instance.AscendanceRestoUnit);
                            //Eval("CanCastCheck('Ascendance')", () => CanCastCheck("Ascendance"));
                            return RunStatus.Failure;
                        })
                );
        }

        private static Composite AscendanceEnh()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AscendanceEnhCooldown ||
                 THSettings.Instance.AscendanceEnhBurst &&
                 THSettings.Instance.Burst) &&
                //!Me.Mounted &&
                Me.Combat &&
                CurrentTargetAttackable(30) &&
                !MeHasAura("Ascendance") &&
                !CurrentTargetCheckInvulnerablePhysic &&
                //SSpellManager.HasSpell("Ascendance") &&
                Me.ManaPercent > 20 &&
                CanCastCheck("Ascendance", true) &&
                CanCastCheck("Primal Strike") && //Suck to pop CD and no Mana to use seplls
                (IsWorthyTarget(Me.CurrentTarget, 2, 0.5) ||
                 HaveWorthyTargetAttackingMe()),
                new Action(
                    ret =>
                        {
                            CastSpell("Ascendance", Me, "AscendanceEnh");
                            AuraCacheUpdate(Me, true);
                            return RunStatus.Failure;
                        })
                )
                ;
        }

        private static Composite AscendanceEle()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.AscendanceEleCooldown ||
                 THSettings.Instance.AscendanceEleBurst &&
                 THSettings.Instance.Burst) &&
                !MeHasAura("Ascendance") &&
                //!Me.Mounted &&
                Me.Combat &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                //SSpellManager.HasSpell("Ascendance") &&
                (IsWorthyTarget(Me.CurrentTarget, 2, 0.5) ||
                 HaveWorthyTargetAttackingMe()) &&
                MyAuraTimeLeft("Flame Shock", Me.CurrentTarget) > 15000 &&
                CanCastCheck("Ascendance", true) &&
                GetSpellCooldown("Lava Burst").TotalMilliseconds > 5000,
                new Action(
                    ret =>
                        {
                            CastSpell("Ascendance", Me, "AscendanceEle");
                            AuraCacheUpdate(Me, true);
                            return RunStatus.Failure;
                        })
                );
        }

        #endregion

        #region AstralShift

        private static Composite AstralShift()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AstralShift &&
                //!Me.Mounted &&
                HealWeightMe <= THSettings.Instance.AstralShiftHP &&
                Me.Combat &&
                CanCastCheck("Astral Shift") &&
                //SSpellManager.HasSpell("Astral Shift") &&
                HaveDPSTarget(Me),
                new Action(
                    ret => { CastSpell("Astral Shift", Me, "AstralShift"); })
                );
        }

        #endregion

        #region AttackResto

        private static WoWUnit UnitAttackRestoAny;

        private static bool GetUnitAttackRestoAny()
        {
            UnitAttackRestoAny = null;

            UnitAttackRestoAny = NearbyUnFriendlyPlayers.OrderBy(unit => unit.HealthPercent).FirstOrDefault(
                unit =>
                BasicCheck(unit) &&
                unit.HealthPercent <= THSettings.Instance.AttackRestoAnyHP &&
                !InvulnerableSpell(unit) &&
                FacingOverride(unit) &&
                Attackable(unit, 30));

            if (UnitAttackRestoAny == null)
            {
                UnitAttackRestoAny = NearbyUnFriendlyUnits.OrderBy(unit => unit.HealthPercent).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    unit.Combat &&
                    unit.CurrentTarget != null &&
                    FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                    unit.HealthPercent <= THSettings.Instance.AttackRestoAnyHP &&
                    !InvulnerableSpell(unit) &&
                    FacingOverride(unit) &&
                    Attackable(unit, 30));
            }

            return BasicCheck(UnitAttackRestoAny);
        }

        private static Composite AttackResto()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackResto &&
                    THSettings.Instance.AttackRestoFlameShock &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Flame Shock") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CurrentTargetAttackable(
                        (int) SpellManager.Spells["Flame Shock"].MaxRange) &&
                    !CurrentTargetCheckInvulnerableMagic &&
                    !MyAura("Flame Shock", Me.CurrentTarget) &&
                    CanCastCheck("Flame Shock"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Flame Shock", Me.CurrentTarget, "AttackResto");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackResto &&
                    THSettings.Instance.AttackRestoLavaBurst &&
                    //SSpellManager.HasSpell("Lava Burst") &&
                    //!Me.Mounted &&
                    //CanCastWhileMoving() &&
                    CurrentTargetAttackable(30) &&
                    !CurrentTargetCheckInvulnerableMagic &&
                    (HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                     Me.ManaPercent >= THSettings.Instance.AttackRestoMana ||
                     SpellManager.Spells["Healing Wave"].CooldownTimeLeft.TotalMilliseconds >
                     2000) &&
                    CanCastCheck("Lava Burst"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Lava Burst", Me.CurrentTarget, "AttackResto");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackResto &&
                    THSettings.Instance.AttackRestoElementalBlast &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Elemental Blast") &&
                    //!Me.Mounted &&
                    //CanCastWhileMoving() &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CurrentTargetAttackable(40) &&
                    !CurrentTargetCheckInvulnerableMagic &&
                    CanCastCheck("Elemental Blast"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Elemental Blast", Me.CurrentTarget, "AttackResto");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.FlameShockEnh &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    HasGlyph.Contains("55447") &&
                    //SSpellManager.HasSpell("Flame Shock") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CanCastCheck("Flame Shock") &&
                    GetUnitFlameShock55447(),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitFlameShock55447);
                                CastSpell("Flame Shock", UnitFlameShock55447, "FlameShock55447");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackResto &&
                    THSettings.Instance.AttackRestoEarthShock &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Earth Shock") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CurrentTargetAttackable(
                        (int) SpellManager.Spells["Earth Shock"].MaxRange) &&
                    !CurrentTargetCheckInvulnerableMagic &&
                    CanCastCheck("Earth Shock"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Earth Shock", Me.CurrentTarget, "AttackResto");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackResto &&
                    THSettings.Instance.AttackRestoChainLightning &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Chain Lightning") &&
                    //!Me.Mounted &&
                    //CanCastWhileMoving() &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CurrentTargetAttackable(30) &&
                    !CurrentTargetCheckInvulnerableMagic &&
                    CanCastCheck("Chain Lightning"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Chain Lightning", Me.CurrentTarget, "AttackResto");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackResto &&
                    THSettings.Instance.AttackRestoLightningBolt &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Lightning Bolt") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CurrentTargetAttackable(30) &&
                    !CurrentTargetCheckInvulnerableMagic &&
                    CanCastCheck("Lightning Bolt"),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Lightning Bolt", Me.CurrentTarget, "AttackResto");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackRestoAny &&
                    THSettings.Instance.AttackRestoFlameShock &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Flame Shock") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CanCastCheck("Flame Shock") &&
                    GetUnitAttackRestoAny() &&
                    !MyAura("Flame Shock", UnitAttackRestoAny),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackRestoAny);
                                CastSpell("Flame Shock", UnitAttackRestoAny, "AttackRestoAny");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackRestoAny &&
                    THSettings.Instance.AttackRestoLavaBurst &&
                    //SSpellManager.HasSpell("Lava Burst") &&
                    //!Me.Mounted &&
                    //CanCastWhileMoving() &&
                    CanCastCheck("Lava Burst") &&
                    GetUnitAttackRestoAny() &&
                    (
                        HealWeightUnitHeal >=
                        THSettings.Instance.PriorityHeal &&
                        Me.ManaPercent >=
                        THSettings.Instance.AttackRestoMana ||
                        SpellManager.Spells["Healing Wave"]
                            .CooldownTimeLeft.TotalMilliseconds > 2000),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackRestoAny);
                                CastSpell("Lava Burst", UnitAttackRestoAny, "AttackRestoAny");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackRestoAny &&
                    THSettings.Instance.AttackRestoElementalBlast &&
                    HealWeightUnitHeal >=
                    THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Elemental Blast") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >=
                    THSettings.Instance.AttackRestoMana &&
                    CanCastCheck("Elemental Blast") &&
                    GetUnitAttackRestoAny(),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackRestoAny);
                                CastSpell("Elemental Blast", UnitAttackRestoAny, "AttackResto");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackRestoAny &&
                    THSettings.Instance.AttackRestoEarthShock &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Earth Shock") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >= THSettings.Instance.AttackRestoMana &&
                    CanCastCheck("Earth Shock") &&
                    GetUnitAttackRestoAny(),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackRestoAny);
                                CastSpell("Earth Shock", UnitAttackRestoAny, "AttackRestoAny");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackRestoAny &&
                    THSettings.Instance.AttackRestoChainLightning &&
                    HealWeightUnitHeal >=
                    THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Chain Lightning") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >=
                    THSettings.Instance.AttackRestoMana &&
                    CanCastCheck("Chain Lightning") &&
                    GetUnitAttackRestoAny(),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackRestoAny);
                                CastSpell("Chain Lightning", UnitAttackRestoAny, "AttackRestoAny");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AttackRestoAny &&
                    THSettings.Instance.AttackRestoLightningBolt &&
                    HealWeightUnitHeal >=
                    THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Lightning Bolt") &&
                    //!Me.Mounted &&
                    Me.ManaPercent >=
                    THSettings.Instance.AttackRestoMana &&
                    CanCastCheck("Lightning Bolt") &&
                    GetUnitAttackRestoAny(),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(UnitAttackRestoAny);
                                CastSpell("Lightning Bolt", UnitAttackRestoAny, "AttackRestoAny");
                            }))
                );
        }

        #endregion

        #region AutoAttack

        private static WoWUnit UnitAutoAttack;

        private static bool GetUnitAutoAttack()
        {
            UnitAutoAttack = null;

            UnitAutoAttack = NearbyUnFriendlyUnits
                .FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    unit != Me.CurrentTarget &&
                    Me.IsFacing(unit) &&
                    GetDistance(unit) <= 5 &&
                    Attackable(unit, 5));

            return BasicCheck(UnitAutoAttack);
        }

        private static Composite AutoAttackOffTarget()
        {
            return new Decorator(
                ret =>
                UseSpecialization == 2 &&
                AutoAttackLast < DateTime.Now &&
                !CurrentTargetAttackable(10) &&
                Me.CurrentTarget != null &&
                //!Me.Mounted &&
                Me.Combat &&
                Me.Inventory.Equipped.MainHand != null &&
                Me.ManaPercent < 70 &&
                //SSpellManager.HasSpell("Auto Attack")&&
                CanCastCheck("Auto Attack") &&
                GetUnitAutoAttack(),
                new Action(
                    ret =>
                        {
                            //var currenttarget = Me.CurrentTarget;
                            CastSpell("Auto Attack", UnitAutoAttack, "AutoAttack Enh Gain Mana");
                            AutoAttackLast = DateTime.Now +
                                             TimeSpan.FromMilliseconds(
                                                 Me.Inventory.Equipped.MainHand.ItemInfo.WeaponSpeed);
                            Me.TargetLastTarget();
                            //while (Me.CurrentTarget != currenttarget)
                            //{
                            //    currenttarget.Target();
                            //}
                            return RunStatus.Failure;
                        })
                );
        }

        #endregion

        #region AutoFocus

        private static WoWUnit UnitBestFocus;

        private static bool GetBestFocus()
        {
            UnitBestFocus = null;

            UnitBestFocus = NearbyUnFriendlyPlayers.OrderByDescending(TalentSort).
                                                    ThenByDescending(unit => unit.HealthPercent).
                                                    FirstOrDefault(
                                                        unit =>
                                                        unit != null && unit.IsValid &&
                                                        //Me.CurrentTarget != null &&
                                                        unit != Me.CurrentTarget &&
                                                        AttackableNoLoS(unit, 50));

            return BasicCheck(UnitBestFocus);
        }

        private static DateTime LastAutoFocus;

        private static Composite AutoSetFocus()
        {
            return new Action(delegate
                {
                    if (!THSettings.Instance.AutoSetFocus || LastAutoFocus > DateTime.Now)
                    {
                        return RunStatus.Failure;
                    }


                    if (Me.CurrentTarget != null &&
                        Me.FocusedUnit == null &&
                        GetBestFocus())
                    {
                        //OriginalTarget = Me.CurrentTarget;
                        UnitBestFocus.Target();
                        Thread.Sleep(10);
                        Lua.DoString("RunMacroText('/focus');");
                        Me.SetFocus(UnitBestFocus);
                        Thread.Sleep(10);
                        Me.TargetLastTarget();
                        Logging.Write("Set Focus: " + SafeName(UnitBestFocus));
                        LastAutoFocus = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                        return RunStatus.Failure;
                    }

                    if (UseSpecialization != 3 &&
                        Me.CurrentTarget != null &&
                        Me.FocusedUnit != null &&
                        (Me.CurrentTarget == Me.FocusedUnit ||
                         !AttackableNoCCLoS(Me.FocusedUnit, 60) ||
                         TalentSort(Me.FocusedUnit) < 4 &&
                         GetBestFocus() &&
                         TalentSort(UnitBestFocus) == 4))
                    {
                        Logging.Write("Clear Focus");
                        //Logging.Write("Clear Focus: Focus = Target " +
                        //              "Target: " + SafeName(Me.CurrentTarget) +
                        //              " - Focus: " + SafeName(Me.FocusedUnit));
                        Lua.DoString("RunMacroText('/clearfocus');");
                        Me.SetFocus(0);
                        LastAutoFocus = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                        return RunStatus.Failure;
                    }
                    return RunStatus.Failure;
                });
        }

        #endregion

        #region AutoTarget

        private static DateTime LastAutoTarget;

        private static Composite AutoTargetMelee()
        {
            return new Action(delegate
                {
                    if (!THSettings.Instance.AutoTarget ||
                        LastAutoTarget > DateTime.Now)
                    {
                        return RunStatus.Failure;
                    }

                    if (BasicCheck(Me.CurrentTarget) &&
                        !CurrentTargetAttackableNoLoS(50) &&
                        GetBestTarget() &&//.
                        Me.CurrentTarget != UnitBestTarget)
                    {
                        UnitBestTarget.Target();
                        Logging.Write(LogLevel.Diagnostic, "Switch to Best Unit");
                        LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                        return RunStatus.Failure;
                    }

                    if (!BasicCheck(Me.CurrentTarget) &&
                        GetBestTarget())
                    {
                        UnitBestTarget.Target();
                        Logging.Write(LogLevel.Diagnostic, "Target  Best Unit");
                        LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                        return RunStatus.Failure;
                    }
                    LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                    return RunStatus.Failure;
                });
        }

        private static Composite AutoTargetRange()
        {
            return new Action(delegate
                {
                    if (!THSettings.Instance.AutoTarget ||
                        LastAutoTarget > DateTime.Now)
                    {
                        return RunStatus.Failure;
                    }

                    if (InArena || InBattleground)
                    {
                        if (BasicCheck(Me.CurrentTarget) &&
                            !CurrentTargetAttackableNoLoS(50) &&
                            GetBestTarget() &&
                            Me.CurrentTarget != UnitBestTarget)
                        {
                            UnitBestTarget.Target();
                            Logging.Write(LogLevel.Diagnostic, "Switch to Best Unit");
                            LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                            return RunStatus.Failure;
                        }

                        if (!BasicCheck(Me.CurrentTarget) &&
                            GetBestTarget())
                        {
                            UnitBestTarget.Target();
                            Logging.Write(LogLevel.Diagnostic, "Target  Best Unit");
                            LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                            return RunStatus.Failure;
                        }
                    }
                    else
                    {
                        if (GetBestTargetRange() &&
                            Me.CurrentTarget != UnitBestTarget)
                        {
                            UnitBestTarget.Target();
                            Logging.Write(LogLevel.Diagnostic, "Switch to Best Unit Ele");
                            LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                            return RunStatus.Failure;
                        }
                    }

                    LastAutoTarget = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                    return RunStatus.Failure;
                });
        }

        #endregion

        #region AutoRez

        private static readonly Dictionary<ulong, DateTime> AutoRezListCache = new Dictionary<ulong, DateTime>();

        private static void AutoRezListCacheClear()
        {
            var indexToRemove = AutoRezListCache.Where(
                unit =>
                unit.Value < DateTime.Now).Select(unit => unit.Key).ToList();

            foreach (var index in indexToRemove)
            {
                //Logging.Write("Remove {0} from AutoRezList", AutoRezList[index]);
                AutoRezListCache.Remove(index);
            }
        }

        private static void AutoRezListCacheAdd(WoWUnit unit, int expireSeconds = 20)
        {
            if (AutoRezListCache.ContainsKey(unit.Guid)) return;
            AutoRezListCache.Add(unit.Guid, DateTime.Now + TimeSpan.FromSeconds(expireSeconds));
        }

        private static WoWPlayer UnitAutoRez;

        private static bool GetUnitAutoRez()
        {
            AutoRezListCacheClear();
            UnitAutoRez = null;
            UnitAutoRez =
                ObjectManager.GetObjectsOfType<WoWPlayer>().
                              OrderBy(unit => unit.Distance).
                              FirstOrDefault(
                                  p =>
                                  !AutoRezListCache.ContainsKey(p.Guid) &&
                                  p.IsPlayer &&
                                  !p.IsAlive &&
                                  p.Distance < 100 &&
                                  p.IsInMyPartyOrRaid);

            return UnitAutoRez != null && UnitAutoRez.IsValid;
        }

        private static Composite AutoRez()
        {
            {
                return new Decorator(
                    ret =>
                    THSettings.Instance.AutoRez &&
                    //(InDungeon || InRaid) &&
                    !Me.Combat &&
                    //SSpellManager.HasSpell("Ancestral Spirit") &&
                    CanCastCheck("Ancestral Spirit") &&
                    GetUnitAutoRez(),
                    new PrioritySelector(
                        new Action(delegate
                            {
                                if (UnitAutoRez.Distance > 40 || !InLineOfSpellSightCheck(UnitAutoRez))
                                {
                                    Navigator.MoveTo(UnitAutoRez.Location);
                                }
                                else
                                {
                                    if (IsMoving(Me))
                                    {
                                        Navigator.PlayerMover.MoveStop();
                                    }
                                    CastSpell("Ancestral Spirit", UnitAutoRez, "Ancestral Spirit");
                                    AutoRezListCacheAdd(UnitAutoRez);
                                }
                            })
                        ))
                    ;
            }
        }

        #endregion

        #region BindElemental

        private static WoWUnit UnitBindElemental;
        //////done
        private static bool GetUnitUnitBindElemental()
        {
            UnitBindElemental = null;

            UnitBindElemental = NearbyFriendlyUnits.OrderByDescending(unit => unit.CurrentHealth)
                                                   .FirstOrDefault(
                                                       unit =>
                                                       unit.IsElemental &&
                                                       !UnitHasAura("Bind Elemental", unit) &&
                                                       !DebuffCC(unit) &&
                                                       Attackable(unit, 30));

            return BasicCheck(UnitBindElemental);
        }

        private static bool HasUnitBindElemental()
        {
            UnitBindElemental = null;

            UnitBindElemental = NearbyFriendlyUnits.OrderByDescending(unit => unit.CurrentHealth)
                                                   .FirstOrDefault(
                                                       unit =>
                                                       unit.IsElemental &&
                                                       MyAura("Bind Elemental", unit) &&
                                                       Attackable(unit, 30));

            return BasicCheck(UnitBindElemental);
        }

        private static Composite BindElemental()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.BindElemental &&
                (UseSpecialization != 2 ||
                 UseSpecialization == 2 &&
                 HealWeightUnitHeal >= THSettings.Instance.PriorityHeal) &&
                //(InArena || InBattleground) &&
                CanCastCheck("Bind Elemental") &&
                !HasUnitBindElemental() &&
                GetUnitUnitBindElemental(),
                new Action(
                    ret => { CastSpell("Bind Elemental", UnitBindElemental, "BindElemental"); })
                );
        }

        #endregion

        #region CalculateDropLocation

        private static float HeightOffTheGround(WoWPoint Point, int HoverZ)
        {
            var listMeshZ = Navigator.FindHeights(Point.X, Point.Y).Where(h => h <= Point.Z);
            if (listMeshZ.Any())
            {
                return Point.Z - listMeshZ.Max();
            }
            return HoverZ;
        }

        private static WoWPoint FindGround(WoWPoint Point, int HoverZ)
        {
            //if (!InArena || !InBattleground)
            //{
            //    return Point;
            //}

            var PointHover = new WoWPoint(Point.X, Point.Y, Point.Z + HoverZ);
            var CalculatedPoint = new WoWPoint(Point.X, Point.Y, PointHover.Z - HeightOffTheGround(PointHover, HoverZ));

            //Logging.Write("Point X: {0} - Y: {1} - Z: {2}", Point.X, Point.Y, Point.Z);
            //Logging.Write("CalculatedPoint X: {0} - Y: {1} - Z: {2}", CalculatedPoint.X, CalculatedPoint.Y,
            //              CalculatedPoint.Z);

            return CalculatedPoint;
        }

        private static WoWPoint CalculateDropLocation(WoWUnit target, float x, float y)
        {
            WoWPoint dropLocation;

            if (!target.IsMoving || target.MovementInfo.MovingBackward || DebuffRoot(target))
            {
                dropLocation = target.Location.Add(x,y,0);
            }
            else if (DebuffSnare(target))
            {
                dropLocation = WoWMathHelper.CalculatePointBehind(target.Location, target.Rotation, -3).Add(x,y,0);
                dropLocation = FindGround(dropLocation, 10);
            }
            else
            {
                dropLocation = WoWMathHelper.CalculatePointBehind(target.Location, target.Rotation, -6).Add(x,y,0);
                dropLocation = FindGround(dropLocation, 10);
            }

            if (dropLocation.Distance(Me.Location) > 40)
            {
                dropLocation = WoWMathHelper.CalculatePointFrom(Me.Location, dropLocation, 39).Add(x,y,0);
                dropLocation = FindGround(dropLocation, 10);
            }
            return dropLocation;
        }

        #endregion

        #region CanCastWhileMoving

        //private static bool CanCastWhileMoving()
        //{
        //    if (!IsMoving(Me) ||
        //        //MeHasAura(131558) || //Spiritwalker's Aegis
        //        MeHasAura(79206) || //Spiritwalker's Grace
        //        MeHasAura(19188) || // Ancestral Swiftness
        //        MyAuraStackCount(53817, Me) > 4) //Maelstrom Weapon
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        #endregion

        #region Capacitor
        //////done
        private static WoWPlayer UnitCapacitorFriendLow;

        private static bool GetUnitCapacitorFriendLow()
        {
            UnitCapacitorFriendLow = null;

            if (THSettings.Instance.CapacitorProjection &&
                SpellManager.HasSpell("Totemic Projection"))
            {
                UnitCapacitorFriendLow = NearbyUnFriendlyPlayers.Where(BasicCheck)
                    .OrderByDescending(unit => unit.HealthPercent)
                    .FirstOrDefault(
                        unit =>
                        TalentSort(unit) < 4 &&
                        unit.GotTarget &&
                        NearbyFriendlyPlayers.Contains(unit.CurrentTarget) &&
                        HealWeight(unit.CurrentTarget) <=
                        THSettings.Instance.CapacitorFriendLowHP &&
                        !InvulnerableSpell(unit) &&
                        !DebuffCC(unit) &&
                        Attackable(unit, 40));
            }
            else
            {
                UnitCapacitorFriendLow = NearbyUnFriendlyPlayers.Where(BasicCheck)
                    .OrderByDescending(unit => unit.HealthPercent)
                    .FirstOrDefault(
                        unit =>
                        TalentSort(unit) < 4 &&
                        unit.GotTarget &&
                        NearbyFriendlyPlayers.Contains(unit.CurrentTarget) &&
                        HealWeight(unit.CurrentTarget) <=
                        THSettings.Instance.CapacitorFriendLowHP &&
                        !InvulnerableSpell(unit) &&
                        !DebuffCC(unit) &&
                        Attackable(unit, 8));
            }
            return BasicCheck(UnitCapacitorFriendLow);
        }

        private static WoWPlayer UnitCapacitorEnemyLow;

        private static bool GetUnitCapacitorEnemyLow()
        {
            UnitCapacitorEnemyLow = null;

            if (THSettings.Instance.CapacitorProjection &&
                SpellManager.HasSpell("Totemic Projection"))
            {
                UnitCapacitorEnemyLow = NearbyUnFriendlyPlayers.Where(BasicCheck)
                    .OrderBy(unit => unit.HealthPercent)
                    .FirstOrDefault(
                        unit =>
                        unit.GetPredictedHealthPercent() <= THSettings.Instance.CapacitorEnemyLowHP &&
                        !InvulnerableSpell(unit) &&
                        !DebuffCC(unit) &&
                        Attackable(unit, 40));
            }
            else
            {
                UnitCapacitorEnemyLow = NearbyUnFriendlyPlayers
                    .OrderBy(unit => unit.GetPredictedHealthPercent())
                    .FirstOrDefault(
                        unit =>
                        unit.GetPredictedHealthPercent() <= THSettings.Instance.CapacitorEnemyLowHP &&
                        !InvulnerableSpell(unit) &&
                        !DebuffCC(unit) &&
                        Attackable(unit, 8));
            }
            return BasicCheck(UnitCapacitorEnemyLow);
        }

        private static bool GetUnitCapacitorEnemyLowNoThreadhold()
        {
            UnitCapacitorEnemyLow = null;

            if (THSettings.Instance.CapacitorProjection &&
                SpellManager.HasSpell("Totemic Projection"))
            {
                UnitCapacitorEnemyLow = NearbyUnFriendlyPlayers
                    .OrderBy(unit => unit.HealthPercent)
                    .FirstOrDefault(
                        unit =>
                        //unit.HealthPercent <= THSettings.Instance.CapacitorEnemyLowHP &&
                        !InvulnerableSpell(unit) &&
                        !DebuffCC(unit) &&
                        Attackable(unit, 40));
            }
            return BasicCheck(UnitCapacitorEnemyLow);
        }

        private static double CountEnemyPlayerNearCapacitor(WoWObject unitCenter)
        {
            //var i = 0;

            //foreach (var unit in NearbyFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        unitCenter.Location.Distance(unit.Location) <= 8 &&
            //        !Invulnerable(unit) &&
            //        !InvulnerableSpell(unit) &&
            //        !DebuffCC(unit))
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.CapacitorEnemyPackNumber)
            //    {
            //        break;
            //    }
            //}
            //return i;

            return NearbyUnFriendlyPlayers.Count(
                unit =>
                BasicCheck(unit) &&
                unitCenter.Location.Distance(unit.Location) <= 8 &&
                !Invulnerable(unit) &&
                !InvulnerableSpell(unit) &&
                !DebuffCC(unit));
        }

        private static WoWPlayer UnitCapacitorEnemyPack;

        private static bool GetUnitCapacitorEnemyPack()
        {
            UnitCapacitorEnemyPack = null;

            if (THSettings.Instance.CapacitorProjection &&
                SpellManager.HasSpell("Totemic Projection"))
            {
                UnitCapacitorEnemyPack = NearbyUnFriendlyPlayers.Where(BasicCheck)
                    .OrderBy(CountEnemyPlayerNearCapacitor)
                    .FirstOrDefault(
                        unit =>
                        !InvulnerableSpell(unit) &&
                        CountEnemyPlayerNearCapacitor(unit) >= THSettings.Instance.CapacitorEnemyPackNumber &&
                        Attackable(unit, 40));
            }
            else if (CountEnemyPlayerNearCapacitor(Me) > THSettings.Instance.CapacitorEnemyPackNumber)
            {
                UnitCapacitorEnemyPack = Me;
            }

            return BasicCheck(UnitCapacitorEnemyPack);
        }

        private static WoWUnit CapacitorTotem;
        private static WoWUnit CapacitorTarget;
        private static string CapacitorProjectionPurpose;

        private static bool HasCapacitorTotem()
        {
            CapacitorTotem = FarFriendlyUnits.FirstOrDefault(
                unit =>
                BasicCheck(unit) &&
                unit.Entry == 61245 &&
                unit.CreatedByUnit == Me);

            return CapacitorTotem != null && CapacitorTotem.IsValid;
        }
        //////done, 可增加使用hotkey用电能图腾时，丢的目标为当前目标或者focus
        private static void CapacitorProjection(int MsLeft)
        {
            if (!THSettings.Instance.CapacitorProjection ||
                !SpellManager.HasSpell("Totemic Projection") ||
                !HasCapacitorTotem() ||
                HasCapacitorTotem() && CapacitorTotem.CurrentCastTimeLeft.TotalMilliseconds > MsLeft + MyLatency ||
                SpellManager.Spells["Totemic Projection"].CooldownTimeLeft.TotalMilliseconds > MsLeft + MyLatency)
            {
                return;
            }

            CapacitorTarget = null;
            CapacitorProjectionPurpose = null;

            if (LastHotKey1Press >= DateTime.Now)
            {
                CapacitorTarget = HotkeyTargettoUnit(THSettings.Instance.Hotkey1Target);
                CapacitorProjectionPurpose = "Capacitor on Target due to Hotkey1";
            }
            else if (LastHotKey2Press >= DateTime.Now)
            {
                CapacitorTarget = HotkeyTargettoUnit(THSettings.Instance.Hotkey2Target);
                CapacitorProjectionPurpose = "Capacitor on Target due to Hotkey2";
            }
            else if (GetUnitCapacitorFriendLow())
            {
                CapacitorTarget = UnitCapacitorFriendLow;
                CapacitorProjectionPurpose = "Capacitor Help Friend Low HP";
            }
            else if (GetUnitCapacitorEnemyLow())
            {
                CapacitorTarget = UnitCapacitorEnemyLow;
                CapacitorProjectionPurpose = "Capacitor on Enemy Low HP";
            }
            else if (GetUnitCapacitorEnemyPack())
            {
                CapacitorTarget = UnitCapacitorEnemyPack;
                CapacitorProjectionPurpose = "Capacitor on Enemy Group";
            }
            else if (GetUnitCapacitorEnemyLowNoThreadhold())
            {
                CapacitorTarget = UnitCapacitorEnemyLow;
                CapacitorProjectionPurpose = "Capacitor on Enemy No Threadhold";
            }

            if (!BasicCheck(CapacitorTarget))
            {
                Logging.Write(LogLevel.Diagnostic, "Can't Find Suitable Capacitor Target");
                return;
            }

            if (CapacitorTotem.Location.Distance(CapacitorTarget.Location) < 5)
            {
                Logging.Write(LogLevel.Diagnostic, "Capacitor Totem Already Near Target. No Projection needed");
                return;
            }


            Logging.Write("Trying to Capacitor Projection on {0} purpose {1}",
                          CapacitorTarget.SafeName,
                          CapacitorProjectionPurpose);

            if (Me.IsCasting)
            {
                SpellManager.StopCasting();
            }

            while (CapacitorTotem.CurrentCastTimeLeft.TotalMilliseconds > MsLeft)
            {
                //waiting fo the right time;
            }

            SpamUntil = DateTime.Now + SpellManager.Spells["Totemic Projection"].CooldownTimeLeft +
                        TimeSpan.FromMilliseconds(100);

            while (SpamUntil > DateTime.Now)
            {
                //Logging.Write("Trying to cast Totemic Projection");
                CastSpell("Totemic Projection", Me, "Totemic Projection");
                //Logging.Write("Me.CurrentPendingCursorSpell.Id {0}", Me.CurrentPendingCursorSpell.Id);
                //Logging.Write("ClickRemoteLocation {0}", UnitProject.Location);
                ObjectManager.Update();
                var DropLocation = new WoWPoint(CapacitorTarget.Location.X + (LastHotKey2PressPosition.X-CapacitorTotem.Location.X),
                                                CapacitorTarget.Location.Y + (LastHotKey2PressPosition.Y - CapacitorTotem.Location.Y),
                                                CapacitorTarget.Location.Z);
                //SpellManager.ClickRemoteLocation(CapacitorTarget.Location);
                Logging.Write("My location:" + LastHotKey2PressPosition.X + "," + LastHotKey2PressPosition.Y);
                Logging.Write("Totem location:" + CapacitorTotem.Location.X + "," + CapacitorTotem.Location.Y);
                Logging.Write("Totem location:" + CapacitorTarget.Location.X + "," + CapacitorTarget.Location.Y);
                Logging.Write("Totem location:" + DropLocation.X + "," + DropLocation.Y);


                SpellManager.ClickRemoteLocation(CalculateDropLocation(CapacitorTarget,LastHotKey2PressPosition.X - CapacitorTotem.Location.X, LastHotKey2PressPosition.Y - CapacitorTotem.Location.Y));
            }
        }

        //////done
        private static Composite Capacitor()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.CapacitorFriendLow &&
                    UnitHealIsValid &&
                    HealWeightUnitHeal <= THSettings.Instance.CapacitorEnemyLowHP &&
                    //SSpellManager.HasSpell("Capacitor Totem") &&
                    //!Me.Mounted &&
                    !MyTotemAirCheck(Me, 60) &&
                    CanCastCheck("Capacitor Totem") &&
                    GetUnitCapacitorFriendLow(),
                    new Action(
                        ret => { CastSpell("Capacitor Totem", UnitCapacitorFriendLow, "CapacitorFriendLow"); })),
                new Decorator(
                    ret =>
                    THSettings.Instance.CapacitorEnemyLow &&
                    //SSpellManager.HasSpell("Capacitor Totem") &&
                    //!Me.Mounted &&
                    !MyTotemAirCheck(Me, 60) &&
                    CanCastCheck("Capacitor Totem") &&
                    GetUnitCapacitorEnemyLow(),
                    new Action(
                        ret => { CastSpell("Capacitor Totem", UnitCapacitorEnemyLow, "CapacitorEnemyLow"); })),
                new Decorator(
                    ret =>
                    THSettings.Instance.CapacitorEnemyPack &&
                    //SSpellManager.HasSpell("Capacitor Totem") &&
                    //!Me.Mounted &&
                    !MyTotemAirCheck(Me, 60) &&
                    CanCastCheck("Capacitor Totem") &&
                    GetUnitCapacitorEnemyPack(),
                    new Action(
                        ret => { CastSpell("Capacitor Totem", UnitCapacitorEnemyPack, "CapacitorEnemyPack"); }))
                );
        }

        #endregion

        #region CapacitorTestPosition

        private static void CapacitorTestPosition()
        {
            if (!BasicCheck(Me.CurrentTarget) ||
                !MyTotemCheck("Searing Totem", Me, 40) ||
                SpellManager.Spells["Totemic Projection"].CooldownTimeLeft.TotalMilliseconds > MyLatency)
            {
                return;
            }

            if (Me.IsCasting)
            {
                SpellManager.StopCasting();
            }

            SpamUntil = DateTime.Now + SpellManager.Spells["Totemic Projection"].CooldownTimeLeft +
                        TimeSpan.FromMilliseconds(100);

            while (SpamUntil > DateTime.Now)
            {
                //Logging.Write("Trying to cast Totemic Projection");
                CastSpell("Totemic Projection", Me, "Totemic Projection");
                //Logging.Write("Me.CurrentPendingCursorSpell.Id {0}", Me.CurrentPendingCursorSpell.Id);
                //Logging.Write("ClickRemoteLocation {0}", UnitProject.Location);
                var DropLocation = new WoWPoint(Me.CurrentTarget.X + THSettings.Instance.SpiritLinkUnit,
                                                Me.CurrentTarget.Y + THSettings.Instance.AscendanceRestoUnit,
                                                Me.CurrentTarget.Z);
                SpellManager.ClickRemoteLocation(DropLocation);
            }
        }

        #endregion

        #region ChainHeal

        private static double CountUnitChainHeal()
        {
            //var i = 0;

            //foreach (var unit in NearbyFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        (unit.Location.Distance(UnitHeal.Location) < 20 ||
            //         HasGlyph.Contains("55452") && //Glyph of Chaining
            //         unit.Location.Distance(UnitHeal.Location) < 40) &&
            //        HealWeight(unit) < THSettings.Instance.ChainHealHP)
            //    {
            //        i = i + 1;
            //    }

            //    if (i >= THSettings.Instance.ChainHealUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyFriendlyPlayers.Count(
                unit => BasicCheck(unit) &&
                        (unit.Location.Distance(UnitHeal.Location) < 20 ||
                         HasGlyph.Contains("55452") && //Glyph of Chaining
                         unit.Location.Distance(UnitHeal.Location) < 40) &&
                        HealWeight(unit) < THSettings.Instance.ChainHealHP);
        }

        private static Composite ChainHeal()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ChainHeal &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.ChainHealHP &&
                //!Me.Mounted &&
                SpellManager.HasSpell("Chain Heal") &&
                Me.CurrentMana > SpellManager.Spells["Chain Heal"].PowerCost*2 &&
                CanCastCheck("Chain Heal") &&
                CountUnitChainHeal() >= THSettings.Instance.ChainHealUnit,
                new Action(
                    ret =>
                        {
                            UnleaseElementEarthLiving(THSettings.Instance.UnleashElementsCH, UnitHeal);
                            CastSpell("Chain Heal", UnitHeal, "ChainHeal");
                            //Eval("CountUnitChainHeal() >= THSettings.Instance.ChainHealUnit",
                            //() => CountUnitChainHeal() >= THSettings.Instance.ChainHealUnit);
                            //Eval("CanCastCheck('Chain Heal')", () => CanCastCheck("Chain Heal"));
                        })
                );
        }

        #endregion

        #region ChainLightning

        private static Composite ChainLightning5MW()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ChainLightningEnh &&
                //SSpellManager.HasSpell("Chain Lightning") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerableMagic &&
                MyAuraStackCount(53817, Me) > 4 && //Maelstrom Weapon
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Chain Lightning"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Chain Lightning", Me.CurrentTarget, "ChainLightning5MW");
                        })
                );
        }

        private static Composite ChainLightningEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ChainLightningEle &&
                //SSpellManager.HasSpell("Chain Lightning") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                Me.ManaPercent >= THSettings.Instance.EleAoEMana &&
                CountEnemyNear(Me.CurrentTarget, 15) >= THSettings.Instance.ChainLightningEleUnit &&
                CanCastCheck("Chain Lightning") &&
                !CanCastCheck("Lava Burst") &&
                !CanCastCheck("Elemental Blast"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Chain Lightning", Me.CurrentTarget, "ChainLightningEle");
                        })
                );
        }

        private static Composite ChainLightningEleAoE()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ChainLightningEle &&
                //SSpellManager.HasSpell("Chain Lightning") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                Me.ManaPercent >= THSettings.Instance.EleAoEMana &&
                CountEnemyNear(Me.CurrentTarget, 15) >= THSettings.Instance.ChainLightningEleUnit &&
                CanCastCheck("Chain Lightning"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Chain Lightning", Me.CurrentTarget, "ChainLightningEleAoE");
                        })
                );
        }

        #endregion

        #region CleanseSpirit

        private static WoWUnit PlayerFriendlyCleanseSpiritASAP;

        private static bool GetPlayerFriendlyCleanseSpiritASAP()
        {
            PlayerFriendlyCleanseSpiritASAP = null;

            PlayerFriendlyCleanseSpiritASAP = NearbyFriendlyPlayers.Where(BasicCheck).FirstOrDefault(
                unit => //.BasicCheck(unit) &&
                        !DebuffDoNotCleanse(unit) &&
                        UnitHasAura("Hex", unit) &&
                        Healable(unit));

            return BasicCheck(PlayerFriendlyCleanseSpiritASAP);
        }

        private static Composite CleanseSpiritFriendlyASAPEnh()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.CleanseSpiritEnh &&
                    //SSpellManager.HasSpell("Cleanse Spirit") &&
                    GetPlayerFriendlyCleanseSpiritASAP() &&
                    CanCastCheck("Cleanse Spirit"),
                    new Action(delegate
                        {
                            SpellManager.StopCasting();
                            CastSpell("Cleanse Spirit", PlayerFriendlyCleanseSpiritASAP, "CleanseSpiritFriendlyASAPEnh");
                        })));
        }

        private static Composite CleanseSpiritFriendlyASAPEle()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.CleanseSpiritEle &&
                    //SSpellManager.HasSpell("Cleanse Spirit") &&
                    GetPlayerFriendlyCleanseSpiritASAP() &&
                    CanCastCheck("Cleanse Spirit"),
                    new Action(delegate
                        {
                            SpellManager.StopCasting();
                            CastSpell("Cleanse Spirit", PlayerFriendlyCleanseSpiritASAP, "CleanseSpiritFriendlyASAPEle");
                        })));
        }

        private static WoWUnit PlayerFriendlyCleanseSpirit;

        private static double CountDebuffCurse(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 0;
            }

            int numberofDebuff =
                target.Debuffs.Values.Count(
                    debuff =>
                    debuff.Spell.DispelType == WoWDispelType.Curse);

            return numberofDebuff;
        }
        //////done
        private static bool GetPlayerFriendlyCleanseSpirit()
        {
            PlayerFriendlyCleanseSpirit = null;

            PlayerFriendlyCleanseSpirit = NearbyFriendlyPlayers.
                OrderByDescending(CountDebuffCurse).
                FirstOrDefault(
                    //////unit => BasicCheck(unit) &&
                        unit =>
                            CountDebuffCurse(unit) > 0 &&
                            !DebuffDoNotCleanse(unit) &&
                            Healable(unit,40));

            return BasicCheck(PlayerFriendlyCleanseSpirit);
        }
        //////done
        private static Composite CleanseSpiritFriendlyEnh()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.CleanseSpiritEnh &&
                    Me.ManaPercent > THSettings.Instance.PriorityHeal &&
                    CanCastCheck("Cleanse Spirit") &&
                    //SSpellManager.HasSpell("Cleanse Spirit") &&
                    GetPlayerFriendlyCleanseSpirit(),
                    new Action(delegate
                        {
                            SpellManager.StopCasting();
                            CastSpell("Cleanse Spirit", PlayerFriendlyCleanseSpirit, "CleanseSpiritEnh");
                        })));
        }

        private static Composite CleanseSpiritFriendlyEle()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.CleanseSpiritEle &&
                    Me.ManaPercent > THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Cleanse Spirit") &&
                    GetPlayerFriendlyCleanseSpirit() &&
                    CanCastCheck("Cleanse Spirit"),
                    new Action(delegate
                        {
                            SpellManager.StopCasting();
                            CastSpell("Cleanse Spirit", PlayerFriendlyCleanseSpirit, "CleanseSpiritEle");
                        })));
        }

        #endregion

        #region Dismount

        //private static Composite Dismount()
        //{
        //    return new Decorator(
        //        ret =>
        //        InBattleground &&
        //        IsUsingAFKBot &&
        //        Me.Mounted &&
        //        (HaveDPSTarget(Me) ||
        //         UnitHealIsValid &&
        //         HealWeightUnitHeal < THSettings.Instance.PriorityHeal &&
        //         UnitHeal.Distance < 20),
        //        new Action(
        //            ret =>
        //                {
        //                    Logging.Write("Manual Dismount in BG due to BGBuddy Bug");
        //                    Lua.DoString("RunMacroText(\"/Dismount\")");
        //                    return RunStatus.Failure;
        //                })
        //        );
        //}

        #endregion

        #region Earthbind

        private static int CountNearEnemyEarthbindNoRooted(WoWUnit target, int dist)
        {
            //var i = 0;

            //foreach (var unit in NearbyUnFriendlyUnits)
            //{
            //    if (BasicCheck(unit) &&
            //        (InArena || InBattleground) &&
            //        unit.Combat &&
            //        TalentSort(unit) == 1 &&
            //        unit.CurrentTarget != null &&
            //        NearbyFriendlyPlayers.Contains(unit.CurrentTarget) &&
            //        unit.MaxHealth > MeMaxHealth*0.5 &&
            //        GetDistance(target, unit) <= dist &&
            //        !InvulnerableRootandSnare(unit) &&
            //        !DebuffRoot(unit))
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.EarthbindUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyUnFriendlyUnits.Count(
                unit =>
                BasicCheck(unit) &&
                unit.Combat &&
                TalentSort(unit) == 1 &&
                unit.CurrentTarget != null &&
                NearbyFriendlyPlayers.Contains(unit.CurrentTarget) &&
                unit.MaxHealth > MeMaxHealth*0.5 &&
                GetDistance(target, unit) <= dist &&
                !InvulnerableRootandSnare(unit) &&
                !DebuffRoot(unit));
        }

        private static int CountNearEnemyEarthbind(WoWUnit target, int dist)
        {
            //var i = 0;

            //foreach (var unit in NearbyUnFriendlyUnits)
            //{
            //    if (BasicCheck(unit) &&
            //        (InArena || InBattleground) &&
            //        unit.Combat &&
            //        TalentSort(unit) == 1 &&
            //        unit.CurrentTarget != null &&
            //        NearbyFriendlyPlayers.Contains(unit.CurrentTarget) &&
            //        unit.MaxHealth > MeMaxHealth*0.5 &&
            //        (GetDistance(target, unit) <= dist ||
            //         GetDistance(target, unit) <= dist + 10 &&
            //         unit.IsMoving &&
            //         unit.IsSafelyFacing(target)) &&
            //        !InvulnerableRootandSnare(unit) &&
            //        !DebuffRootorSnare(unit))
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.EarthbindUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyUnFriendlyUnits.Count(
                unit =>
                BasicCheck(unit) &&
                unit.Combat &&
                TalentSort(unit) == 1 &&
                unit.CurrentTarget != null &&
                NearbyFriendlyPlayers.Contains(unit.CurrentTarget) &&
                unit.MaxHealth > MeMaxHealth*0.5 &&
                (GetDistance(target, unit) <= dist ||
                 GetDistance(target, unit) <= dist + 10 &&
                 unit.IsMoving &&
                 unit.IsSafelyFacing(target)) &&
                !InvulnerableRootandSnare(unit) &&
                !DebuffRootorSnare(unit));
        }

        private static Composite Earthbind()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Earthbind &&
                    (InArena || InBattleground) &&
                    SpellManager.HasSpell("Earthgrab Totem") &&
                    //!Me.Mounted &&
                    !MyTotemEarthCheck(Me, 40) &&
                    CanCastCheck("Earthgrab Totem") &&
                    CountNearEnemyEarthbindNoRooted(Me, THSettings.Instance.EarthbindDistance) >=
                    THSettings.Instance.EarthbindUnit,
                    new Action(
                        ret => { CastSpell("Earthgrab Totem", Me, "Earthgrab"); })
                    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.Earthbind &&
                    (InArena || InBattleground) &&
                    //
                    //SpellManager.HasSpell("Earthbind Totem") &&
                    //!Me.Mounted &&
                    !SpellManager.HasSpell("Earthgrab Totem") &&
                    !MyTotemEarthCheck(Me, 40) &&
                    CanCastCheck("Earthbind Totem") &&
                    CountNearEnemyEarthbind(Me, THSettings.Instance.EarthbindDistance) >=
                    THSettings.Instance.EarthbindUnit,
                    new Action(
                        ret => { CastSpell("Earthbind Totem", Me, "Earthbind"); })
                    )
                );
        }

        #endregion

        #region EarthElemental

        private static Composite EarthElemental()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.EarthElementalCooldown ||
                 THSettings.Instance.EarthElementalBurst &&
                 THSettings.Instance.Burst) &&
                //SSpellManager.HasSpell("Earth Elemental Totem") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerablePhysic &&
                (IsWorthyTarget(Me.CurrentTarget, 2, 0.5) ||
                 HaveWorthyTargetAttackingMe()) &&
                !HasElementalAround() &&
                CanCastCheck("Earth Elemental Totem"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Earth Elemental Totem", Me.CurrentTarget, "EarthElemental");
                        })
                );
        }

        #endregion

        #region Earthquake

        private static WoWUnit UnitEarthquake;

        private static double CountUnitEarthquake(WoWUnit unitCenter)
        {
            //var i = 0;

            //foreach (var unit in NearbyUnFriendlyUnits)
            //{
            //    if (BasicCheck(unit) &&
            //        (IsDummy(unit) ||
            //         unit.CurrentTarget != null &&
            //         FarFriendlyPlayers.Contains(unit.CurrentTarget) && unit.MaxHealth > MeMaxHealth*0.5) &&
            //        unitCenter.Location.Distance(unit.Location) <= 8)
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.EarthquakeUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyUnFriendlyUnits.Count(
                unit =>
                BasicCheck(unit) &&
                (IsDummy(unit) ||
                 unit.CurrentTarget != null &&
                 FarFriendlyPlayers.Contains(unit.CurrentTarget) && unit.MaxHealth > MeMaxHealth*0.5) &&
                unitCenter.Location.Distance(unit.Location) <= 8);
        }

        private static bool GetUnitEarthquake()
        {
            UnitEarthquake = null;

            UnitEarthquake = NearbyUnFriendlyUnits.
                OrderByDescending(CountUnitEarthquake).
                ThenByDescending(unit => unit.HealthPercent).
                FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    unit.Distance <= 40 &&
                    CountUnitEarthquake(unit) >=
                    THSettings.Instance.EarthquakeUnit &&
                    Attackable(unit, 40));

            return BasicCheck(UnitEarthquake);
        }

        private static Composite Earthquake()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.Earthquake &&
                //SSpellManager.HasSpell("Earthquake") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                Me.ManaPercent >= THSettings.Instance.EleAoEMana &&
                CanCastCheck("Earthquake") &&
                GetUnitEarthquake(),
                new Action(
                    ret =>
                        {
                            CastSpell("Earthquake", UnitEarthquake, "Earthquake");
                            //while (Me.IsCasting)
                            //{
                            //    //Waiting Healing Rain Finish Casting
                            //}
                            LastClickRemoteLocation = DateTime.Now + TimeSpan.FromMilliseconds(300);
                            while (LastClickRemoteLocation > DateTime.Now)
                            {
                                SpellManager.ClickRemoteLocation(UnitEarthquake.Location);
                            }
                        })
                );
        }

        #endregion

        #region EarthShock

        private static Composite EarthShock()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.EarthShock &&
                //SSpellManager.HasSpell("Earth Shock") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Earth Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                CanCastCheck("Earth Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Earth Shock", Me.CurrentTarget, "EarthShock");
                        })
                );
        }

        private static Composite EarthShockElemental()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.EarthShockElemental &&
                //SSpellManager.HasSpell("Earth Shock") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Earth Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                !MeHasAura(77762) && //Lava Surge
                MyAuraStackCount("Lightning Shield", Me) >= THSettings.Instance.EarthShockElementalCharge &&
                CanCastCheck("Earth Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Earth Shock", Me.CurrentTarget, "EarthShockElemental");
                        })
                );
        }

        private static Composite EarthShockElementalPvP()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.EarthShockElemental &&
                (InArena || InBattleground) &&
                //SSpellManager.HasSpell("Earth Shock") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Earth Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                !MeHasAura(77762) &&
                CanCastCheck("Earth Shock") && //Lava Surge
                MyAuraStackCount("Lightning Shield", Me) >= THSettings.Instance.EarthShockElementalCharge,
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Earth Shock", Me.CurrentTarget, "EarthShockElemental");
                        })
                );
        }

        #endregion

        #region EarthShield

        private static WoWPlayer UnitTankEarthShield;

        private static bool GetUnitTankEarthShield()
        {
            UnitTankEarthShield = null;

            UnitTankEarthShield = NearbyFriendlyPlayers.
                OrderBy(HealWeight).
                FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    UnitHasAura("Vengeance", unit) &&
                    !UnitHasAura("Earth Shield", unit) &&
                    HaveDPSTarget(unit));

            if (UnitTankEarthShield == null &&
                !Me.Combat &&
                (!GetUnitHasMyEarthShield() ||
                 GetUnitHasMyEarthShield() &&
                 !IsTank(UnitHasMyEarthShield)))
            {
                UnitTankEarthShield = NearbyFriendlyPlayers.
                    OrderByDescending(unit => unit.MaxHealth).
                    FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        !UnitHasAura("Earth Shield", unit) &&
                        IsTank(unit));
            }
            return BasicCheck(UnitTankEarthShield);
        }

        private static WoWUnit UnitEarthShield;

        private static bool GetUnitEarthShield()
        {
            UnitEarthShield = null;

            UnitEarthShield = NearbyFriendlyPlayers.OrderBy(HealWeight).FirstOrDefault(
                unit =>
                BasicCheck(unit) &&
                !UnitHasAura("Earth Shield", unit) &&
                !MyAura("Water Shield", unit) &&
                !MyAura("Lightning Shield", unit) &&
                Healable(unit));

            //if (UnitEarthShield != null)
            //{
            //    Logging.Write("UnitEarthShield {0}", UnitEarthShield.SafeName);
            //}
            return BasicCheck(UnitEarthShield);
        }

        private static WoWUnit UnitHasMyEarthShield;
        private static DateTime GetUnitHasMyEarthShieldLast;

        private static bool GetUnitHasMyEarthShield()
        {
            if (GetUnitHasMyEarthShieldLast > DateTime.Now &&
                BasicCheck(UnitHasMyEarthShield))
            {
                return true;
            }

            UnitHasMyEarthShield = null;
            GetUnitHasMyEarthShieldLast = DateTime.Now + TimeSpan.FromMilliseconds(3000);

            UnitHasMyEarthShield = FarFriendlyUnits.FirstOrDefault(
                unit =>
                BasicCheck(unit) &&
                MyAura("Earth Shield", unit));

            //if (UnitHasMyEarthShield != null)
            //{
            //    Logging.Write("UnitHasMyEarthShield {0}", UnitHasMyEarthShield.SafeName);
            //}
            return BasicCheck(UnitHasMyEarthShield);
        }

        private static DateTime LastEarthShield;

        private static bool GetMyShieldonUnit(WoWUnit target)
        {
            AuraCacheUpdate(target);

            return AuraCacheList.Any(
                aura =>
                aura.AuraCacheUnit == target.Guid &&
                aura.AuraCacheAura.CreatorGuid == Me.Guid &&
                (aura.AuraCacheId == 974 || aura.AuraCacheId == 324 ||
                 aura.AuraCacheId == 52127));
        }

        private static Composite EarthShield()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.EarthShield &&
                    LastEarthShield < DateTime.Now &&
                    //SSpellManager.HasSpell("Earth Shield") &&
                    (InDungeon || InRaid) &&
                    CanCastCheck("Earth Shield") &&
                    //!Me.Mounted &&
                    GetUnitTankEarthShield(),
                    new Action(
                        ret =>
                            {
                                LastEarthShield = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                CastSpell("Earth Shield", UnitTankEarthShield, "EarthShieldTank");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.EarthShieldAlways &&
                    LastEarthShield < DateTime.Now &&
                    //SSpellManager.HasSpell("Earth Shield") &&
                    //!Me.Mounted &&
                    Me.ManaPercent > THSettings.Instance.WaterShieldAlwaysMana &&
                    HealWeightMe <= THSettings.Instance.EarthShieldAlwaysHP &&
                    !MeHasAura("Earth Shield") &&
                    CanCastCheck("Earth Shield") &&
                    HaveDPSTarget(Me),
                    new Action(
                        ret =>
                            {
                                LastEarthShield = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                CastSpell("Earth Shield", Me, "EarthShieldAlways");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.EarthShield &&
                    LastEarthShield < DateTime.Now &&
                    //SSpellManager.HasSpell("Earth Shield") &&
                    !InDungeon &&
                    !InRaid &&
                    CanCastCheck("Earth Shield") &&
                    //!Me.Mounted &&
                    GetUnitEarthShield() &&
                    (!GetUnitHasMyEarthShield() ||
                     GetUnitHasMyEarthShield() &&
                     HealWeight(UnitHasMyEarthShield) - 10 > HealWeight(UnitEarthShield)),
                    new Action(
                        ret =>
                            {
                                LastEarthShield = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                CastSpell("Earth Shield", UnitEarthShield, "EarthShield Swith Earth Shield Unit");
                            }))
                );
        }

        #endregion

        #region ElementalBlast

        private static Composite ElementalBlastEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ElementalBlastEle &&
                //SSpellManager.HasSpell("Elemental Blast") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                //CanCastWhileMoving() &&
                !MeHasAura(77762) && //Lava Surge
                CanCastCheck("Elemental Blast",false),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Elemental Blast", Me.CurrentTarget, "ElementalBlastEle");
                        })
                );
        }

        private static Composite ElementalBlastEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ElementalBlastEnh &&
                //SSpellManager.HasSpell("Elemental Blast") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                //CanCastWhileMoving() &&
                MyAuraStackCount(53817, Me) >= THSettings.Instance.ElementalBlastEnhStack &&
                CanCastCheck("Elemental Blast"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Elemental Blast", Me.CurrentTarget, "ElementalBlastEnh");
                        })
                );
        }

        #endregion

        #region ElementalMastery

        private static Composite ElementalMastery()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.ElementalMasteryCooldown ||
                 THSettings.Instance.ElementalMasteryBurst &&
                 THSettings.Instance.Burst) &&
                //SSpellManager.HasSpell("Elemental Mastery") &&
                //!Me.Mounted &&
                Me.Combat &&
                CanCastCheck("Elemental Mastery", true),
                new Action(
                    ret =>
                        {
                            CastSpell("Elemental Mastery", Me, "ElementalMastery");
                            AuraCacheUpdate(Me, true);
                            return RunStatus.Failure;
                        })
                );
        }

        #endregion

        #region FireElemental

        //61029 Prime Fire Elemental
        //61056 Prime Earth Elemental
        //15352 Greater Earth Elemental
        //15438 Greater Fire Elemental
        private static bool HasElementalAround()
        {
            return FarFriendlyUnits.Any(
                unit =>
                BasicCheck(unit) &&
                (unit.Entry == 61029 ||
                 unit.Entry == 61056 ||
                 unit.Entry == 15352 ||
                 unit.Entry == 15438) &&
                unit.CreatedByUnit == Me);
        }

        private static Composite FireElemental()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.FireElementalCooldown ||
                 THSettings.Instance.FireElementalBurst &&
                 THSettings.Instance.Burst) &&
                //SSpellManager.HasSpell("Fire Elemental Totem") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerablePhysic &&
                CanCastCheck("Fire Elemental Totem") &&
                (IsWorthyTarget(Me.CurrentTarget, 2, 0.5) ||
                 HaveWorthyTargetAttackingMe()) &&
                !HasElementalAround(),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Fire Elemental Totem", Me.CurrentTarget, "FireElemental");
                        })
                );
        }

        #endregion

        #region FireNova

        private static bool HasEnemyFlameShock()
        {
            return FarUnFriendlyUnits.FirstOrDefault(unit => MyAura("Flame Shock", unit)) != null;
        }

        private static Composite FireNovaAoE()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FireNova &&
                //SSpellManager.HasSpell("Fire Nova") &&
                //!Me.Mounted &&
                Me.ManaPercent > 20 &&
                CanCastCheck("Fire Nova") &&
                HasEnemyFlameShock(),
                new Action(
                    ret => { CastSpell("Fire Nova", Me, "FireNovaAoE"); })
                );
        }

        private static Composite FireNovaLoS()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FireNova &&
                !CurrentTargetAttackable(30) &&
                //SSpellManager.HasSpell("Fire Nova") &&
                Me.ManaPercent > 20 &&
                CanCastCheck("Fire Nova") &&
                HasEnemyFlameShock(),
                new Action(
                    ret => { CastSpell("Fire Nova", Me, "FireNovaLoS"); })
                );
        }

        #endregion

        #region FlameShock

        private static Composite FlameShockEnhNoFlametongue()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FlameShockEnh &&
                //SSpellManager.HasSpell("Flame Shock") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Flame Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                (Me.Inventory.Equipped.OffHand == null || Me.Inventory.Equipped.OffHand != null &&
                 Me.Inventory.Equipped.OffHand.TemporaryEnchantment.Id != 5) && //Flametongue
                MyAuraTimeLeft("Flame Shock", Me.CurrentTarget) < 3000 &&
                CanCastCheck("Flame Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Flame Shock", Me.CurrentTarget, "FlameShockEnhNoFlametongue");
                        })
                );
        }

        private static Composite FlameShockUnleashFlame()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FlameShockEnh &&
                //SSpellManager.HasSpell("Flame Shock") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Flame Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                IsWorthyTarget(Me.CurrentTarget) &&
                (MyAura("Unleash Flame", Me) ||
                 !MyAura("Flame Shock", Me.CurrentTarget) ||
                 MyAuraTimeLeft("Flame Shock", Me.CurrentTarget) < 3000 &&
                 (!SpellManager.HasSpell("Unleash Elements") ||
                  SpellManager.HasSpell("Unleash Elements") &&
                  SpellManager.Spells["Unleash Elements"].CooldownTimeLeft.TotalMilliseconds > 5000)) &&
                CanCastCheck("Flame Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Flame Shock", Me.CurrentTarget, "FlameShockUnleashFlame");
                        })
                );
        }

        private static Composite FlameShockEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FlameShockEnh &&
                //SSpellManager.HasSpell("Flame Shock") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Flame Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                MyAuraTimeLeft("Flame Shock", Me.CurrentTarget) < 5000 &&
                CanCastCheck("Flame Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Flame Shock", Me.CurrentTarget, "FlameShockEnh");
                        })
                );
        }

        private static Composite FlameShockEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FlameShockEle &&
                //SSpellManager.HasSpell("Flame Shock") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Flame Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                MyAuraTimeLeft("Flame Shock", Me.CurrentTarget) < 5000 &&
                CanCastCheck("Flame Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Flame Shock", Me.CurrentTarget, "FlameShockEle");
                        })
                );
        }

        private static WoWUnit UnitFlameShockRogueDruid;

        private static bool GetUnitFlameShockRogueDruid()
        {
            UnitFlameShockRogueDruid = null;

            UnitFlameShockRogueDruid = NearbyUnFriendlyPlayers
                .OrderBy(unit => MyAuraTimeLeft("Flame Shock", unit))
                .FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    //FacingOverride(unit) &&
                    TalentSort(unit) < 2 &&
                    (unit.Class == WoWClass.Rogue || unit.Class == WoWClass.Druid) &&
                    (unit.CurrentTarget == null ||
                     unit.CurrentTarget != null &&
                     unit.CurrentTarget != Me ||
                     //SSpellManager.HasSpell("Frozen Power") &&
                     unit.CurrentTarget != null &&
                     unit.CurrentTarget == Me &&
                     DebuffRoot(unit)) &&
                    (!unit.Combat || !DebuffDotDuration(unit, 6000)) &&
                    !InvulnerableSpell(unit) &&
                    Attackable(unit, (int) SpellManager.Spells["Flame Shock"].MaxRange));

            return BasicCheck(UnitFlameShockRogueDruid);
        }

        private static Composite FlameShockRogueDruid()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FlameShockRogueDruid &&
                HealWeightUnitHeal > THSettings.Instance.UrgentHeal &&
                (InArena || InBattleground) &&
                //SSpellManager.HasSpell("Flame Shock") &&
                //!Me.Mounted &&
                CanCastCheck("Flame Shock") &&
                GetUnitFlameShockRogueDruid(),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(UnitFlameShockRogueDruid);
                            CastSpell("Flame Shock", UnitFlameShockRogueDruid, "FlameShockRogueDruid");
                        })
                );
        }

        private static WoWUnit UnitFlameShock55447;

        private static bool GetUnitFlameShock55447()
        {
            UnitFlameShock55447 = null;

            UnitFlameShock55447 = NearbyUnFriendlyUnits
                .OrderByDescending(unit => unit.HealthPercent)
                .FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    (IsDummy(unit) ||
                     unit.CurrentTarget != null &&
                     FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                     unit.MaxHealth > MeMaxHealth*0.5) &&
                    FacingOverride(unit) &&
                    !MyAura("Flame Shock", unit) &&
                    !InvulnerableSpell(unit) &&
                    Attackable(unit, (int) SpellManager.Spells["Flame Shock"].MaxRange));

            return BasicCheck(UnitFlameShock55447);
        }

        private static Composite FlameShock55447()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FlameShockEnh &&
                HealWeightUnitHeal > THSettings.Instance.PriorityHeal &&
                HasGlyph.Contains("55447") &&
                //SSpellManager.HasSpell("Flame Shock") &&
                //!Me.Mounted &&
                CanCastCheck("Flame Shock") &&
                GetUnitFlameShock55447(),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(UnitFlameShock55447);
                            CastSpell("Flame Shock", UnitFlameShock55447, "FlameShock55447");
                        })
                );
        }

        private static WoWUnit UnitFlameShockAoE;

        private static bool GetUnitFlameShockAoE()
        {
            UnitFlameShockAoE = null;

            if (InArena || InBattleground)
            {
                UnitFlameShockAoE = NearbyUnFriendlyUnits
                    .OrderBy(unit => unit.HealthPercent)
                    .FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        (IsDummy(unit) ||
                         unit.CurrentTarget != null &&
                         FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                         unit.MaxHealth > MeMaxHealth*0.5) &&
                        Me.IsFacing(unit) &&
                        !MyAura("Flame Shock", unit) &&
                        !InvulnerableSpell(unit) &&
                        Attackable(unit, (int) SpellManager.Spells["Flame Shock"].MaxRange - 3));
            }
            else
            {
                UnitFlameShockAoE = NearbyUnFriendlyUnits
                    .OrderByDescending(unit => unit.HealthPercent)
                    .FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        (IsDummy(unit) ||
                         unit.CurrentTarget != null &&
                         FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                         unit.CurrentHealth > MeMaxHealth*0.5 &&
                         unit.Combat) &&
                        Me.IsFacing(unit) &&
                        !MyAura("Flame Shock", unit) &&
                        !InvulnerableSpell(unit) &&
                        Attackable(unit, (int) SpellManager.Spells["Flame Shock"].MaxRange - 3));
            }
            return BasicCheck(UnitFlameShockAoE);
        }

        private static bool ShouldFlameShockAoE()
        {
            if (MyAuraTimeLeft("Flame Shock", Me.CurrentTarget) < 7000 ||
                MyAuraStackCount("Lightning Shield", Me) > 5)
            {
                return false;
            }
            return true;
        }

        private static Composite FlameShockAoEEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FlameShockEle &&
                //SSpellManager.HasSpell("Flame Shock") &&
                //!Me.Mounted &&
                Me.ManaPercent >= THSettings.Instance.EleAoEMana - 10 &&
                ShouldFlameShockAoE() &&
                CanCastCheck("Flame Shock") &&
                GetUnitFlameShockAoE(),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(UnitFlameShockAoE);
                            CastSpell("Flame Shock", UnitFlameShockAoE, "FlameShockAoEEle");
                        })
                );
        }

        #endregion

        #region FrostShock

        private static WoWUnit UnitFrostShockNearby;

        private static bool GetUnitFrostShockNearby()
        {
            UnitFrostShockNearby = null;

            UnitFrostShockNearby = NearbyUnFriendlyPlayers.
                OrderBy(TalentSort).
                ThenBy(unit => unit.Distance).
                FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    (THSettings.Instance.FrostShockNearbyMelee &&
                     TalentSort(unit) < 2 ||
                     THSettings.Instance.FrostShockNearbyRange &&
                     TalentSort(unit) < 4 &&
                     TalentSort(unit) > 1 ||
                     THSettings.Instance.FrostShockNearbyHealer &&
                     TalentSort(unit) > 3) &&
                    //unit.CurrentTarget != null &&
                    //unit.CurrentTarget == Me &&
                    ( //SSpellManager.HasSpell("Frozen Power") &&
                        !DebuffRoot(unit) ||
                        !DebuffRootorSnare(unit)) &&
                    !InvulnerableSpell(unit) &&
                    !InvulnerableRootandSnare(unit) &&
                    Attackable(unit, (int) SpellManager.Spells["Frost Shock"].MaxRange));

            //if (UnitFrostShockNearby == null)
            //{
            //    UnitFrostShockNearby = NearbyUnFriendlyPlayers
            //        .OrderBy(unit => TalentSort(unit))
            //        .FirstOrDefault(
            //            unit =>
            //            BasicCheck(unit) &&
            //            !DebuffRootorSnare(unit) &&
            //            !InvulnerableSpell(unit) &&
            //            Attackable(unit, (int) SpellManager.Spells["Frost Shock"].MaxRange));
            //}

            return BasicCheck(UnitFrostShockNearby);
        }

        private static Composite FrostShockNearby()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FrostShockNearby &&
                HealWeightUnitHeal > THSettings.Instance.PriorityHeal &&
                (InArena || InBattleground) &&
                //SSpellManager.HasSpell("Frost Shock") &&
                //!Me.Mounted &&
                Me.ManaPercent >= THSettings.Instance.FrostShockNearbyMana &&
                CanCastCheck("Frost Shock") &&
                GetUnitFrostShockNearby(),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(UnitFrostShockNearby);
                            CastSpell("Frost Shock", UnitFrostShockNearby, "FrostShockNearby");
                        })
                );
        }

        private static Composite FrostShockEnhRoot()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FrostShockEnh &&
                //SSpellManager.HasSpell("Frost Shock") &&
                //SSpellManager.HasSpell("Frozen Power") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Frost Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                CurrentTargetCheckDist >= THSettings.Instance.FrostShockEnhMinDistance &&
                !DebuffRoot(Me.CurrentTarget) &&
                !InvulnerableRootandSnare(Me.CurrentTarget) &&
                CanCastCheck("Frost Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Frost Shock", Me.CurrentTarget, "FrostShockEnhRoot");
                        })
                );
        }

        private static Composite FrostShockEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.FrostShockEnh &&
                //SSpellManager.HasSpell("Frost Shock") &&
                //SSpellManager.HasSpell("Frozen Power") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Frost Shock"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                CurrentTargetCheckDist >= THSettings.Instance.FrostShockEnhMinDistance &&
                !DebuffSnare(Me.CurrentTarget) &&
                !InvulnerableRootandSnare(Me.CurrentTarget) &&
                CanCastCheck("Frost Shock"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Frost Shock", Me.CurrentTarget, "FrostShockEnh");
                        })
                );
        }

        #endregion

        #region FeralSpirit

        private static Composite FeralSpirit()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    (THSettings.Instance.FeralSpiritCooldown ||
                     THSettings.Instance.FeralSpiritBurst &&
                     THSettings.Instance.Burst) &&
                    //SSpellManager.HasSpell("Feral Spirit") &&
                    //!Me.Mounted &&
                    CurrentTargetAttackable(30) &&
                    !CurrentTargetCheckInvulnerablePhysic &&
                    CanCastCheck("Feral Spirit") &&
                    (IsWorthyTarget(Me.CurrentTarget, 2, 0.5) ||
                     HaveWorthyTargetAttackingMe()),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Feral Spirit", Me.CurrentTarget, "FeralSpirit");
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.FeralSpiritLow &&
                    HealWeightMe <= THSettings.Instance.FeralSpiritLowHP &&
                    //SSpellManager.HasSpell("Feral Spirit") &&
                    //!Me.Mounted &&
                    CurrentTargetAttackable(30) &&
                    !CurrentTargetCheckInvulnerablePhysic &&
                    CanCastCheck("Feral Spirit") &&
                    (IsWorthyTarget(Me.CurrentTarget, 2, 0.5) ||
                     HaveWorthyTargetAttackingMe()),
                    new Action(
                        ret =>
                            {
                                SafelyFacingTarget(Me.CurrentTarget);
                                CastSpell("Feral Spirit", Me.CurrentTarget, "FeralSpiritLow");
                            }))
                );
        }

        #endregion

        #region GhostWolf

        private static Composite GhostWolfHoldComp()
        {
            return new Decorator(
                ret =>
                !THSettings.Instance.AutoGhostWolfCancel &&
                MeHasAura("Ghost Wolf"),
                new Action(
                    ret =>
                        {
                            Logging.Write("Auto Cancel Ghost Wolf Disabled");
                            return RunStatus.Success;
                        })
                );
        }

        private static WoWUnit UnitGhostWolfCC;

        private static bool GetUnitGhostWolfCastingCC()
        {
            UnitGhostWolfCC = null;

            if (InBattleground || InArena)
            {
                UnitGhostWolfCC = NearbyUnFriendlyUnits.FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    unit.Distance <= 30 &&
                    IsCastingCCGhostWolfImmune(unit) &&
                    InLineOfSpellSightCheck(unit));
            }

            return BasicCheck(UnitGhostWolfCC);
        }

        private static void GhostWolfAvoidCC()
        {
            if (THSettings.Instance.AutoGhostWolf &&
                (InArena || InBattleground) &&
                !MeHasAura("Ghost Wolf") &&
                LastInterrupt < DateTime.Now &&
                //SSpellManager.HasSpell("Ghost Wolf") &&
                //!Me.Mounted &&
                //////SpellTypeCheck() &&
                !InvulnerableSpell(Me) &&
                GetUnitGhostWolfCastingCC() &&
                GetSpellCooldown("Ghost Wolf").TotalMilliseconds <= MyLatency)
            {
                if (Me.IsCasting)
                {
                    SpellManager.StopCasting();
                }

                while (UnitGhostWolfCC.CurrentCastTimeLeft.TotalMilliseconds > 1500)
                {
                    Logging.Write("INCOMING " + UnitGhostWolfCC.SafeName + " is Casting " +
                                  UnitGhostWolfCC.CastingSpell.Name + " - " + UnitGhostWolfCC.CastingSpellId);
                }

                LastInterrupt = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                SpellManager.StopCasting();
                CastSpell("Ghost Wolf", Me,
                          "GhostWolfAvoidCC: " + UnitGhostWolfCC.SafeName + " is Casting " +
                          UnitGhostWolfCC.CastingSpell.Name + " - " + UnitGhostWolfCC.CastingSpellId);
            }
        }

        private static Composite GhostWolfEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoGhostWolf &&
                !MeHasAura("Ghost Wolf") &&
                !MeHasAura("Spiritwalker's Grace") &&
                Me.IsMoving &&
                CanCastCheck("Ghost Wolf") &&
                (!CurrentTargetAttackable(30) ||
                 !SpellManager.HasSpell("Stormblast") &&
                 (!CurrentTargetAttackable(10) ||
                  (GetDistance(Me.CurrentTarget) > 10 &&
                   Me.CurrentTarget.IsSafelyBehind(Me)))),
                //(!CurrentTargetAttackable(10) ||
                // GetDistance(Me.CurrentTarget) > 7 &&
                // Me.CurrentTarget.IsSafelyBehind(Me) ||
                // GetDistance(Me.CurrentTarget) > 5 &&
                // DebuffSnare(Me)),
                new Action(
                    ret => { CastSpell("Ghost Wolf", Me, "GhostWolfEnh"); }));
        }

        private static bool HasMeleeonMe()
        {
            return
                NearbyUnFriendlyPlayers.Any(
                    unit => unit.Distance < 15 && unit.CurrentTarget == Me && TalentSort(unit) == 1);
        }

        private static Composite GhostWolfResto()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoGhostWolf &&
                !MeHasAura("Ghost Wolf") &&
                !MeHasAura("Spiritwalker's Grace") &&
                Me.IsMoving &&
                CanCastCheck("Ghost Wolf") &&
                (DebuffSnare(Me) || HasMeleeonMe()),
                new Action(
                    ret => { CastSpell("Ghost Wolf", Me, "GhostWolfResto"); }));
        }


        private static Composite GhostWolfEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoGhostWolf &&
                !MeHasAura("Ghost Wolf") &&
                !MeHasAura("Spiritwalker's Grace") &&
                Me.IsMoving &&
                CanCastCheck("Ghost Wolf") &&
                (!CurrentTargetAttackable(40) ||
                 GetDistance(Me.CurrentTarget) > 7 &&
                 Me.CurrentTarget.IsSafelyBehind(Me)),
                new Action(
                    ret => { CastSpell("Ghost Wolf", Me, "GhostWolfEle"); }));
        }

        #endregion

        #region GreaterHealingWave

        private static Composite GreaterHealingWaveAS()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AncestralSwiftnessRes &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.AncestralSwiftnessResHP &&
                //SSpellManager.HasSpell("Ancestral Swiftness") &&
                //!Me.Mounted &&
                !MeHasAura(16188) && //Ancestral Swiftness
                MyAura("Tidal Waves", Me) &&
                UnitHeal.Combat &&
                HaveDPSTarget(UnitHeal) &&
                !Invulnerable(UnitHeal) &&
                Me.CurrentMana >= SpellManager.Spells["Greater Healing Wave"].PowerCost &&
                (MeHasAura(16188) || CanCastCheck("Ancestral Swiftness")) &&
                CanCastCheck("Greater Healing Wave"),
                new Action(
                    ret =>
                        {
                            UnleaseElementEarthLiving(THSettings.Instance.UnleashElementsGHW, UnitHeal);
                            CastSpell("Ancestral Swiftness", Me, "AncestralSwiftnessRes");
                            CastSpell("Greater Healing Wave", UnitHeal, "GreaterHealingWaveAS");
                        }));
        }

        private static Composite GreaterHealingWave()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.GreaterHealingWave &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.GreaterHealingWaveHP &&
                //SSpellManager.HasSpell("Greater Healing Wave") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                //MyAura("Tidal Waves", Me) &&
                UnitHeal.Combat &&
                CanCastCheck("Greater Healing Wave"),
                new Action(
                    ret =>
                        {
                            UnleaseElementEarthLiving(THSettings.Instance.UnleashElementsGHW, UnitHeal);
                            CastSpell("Greater Healing Wave", UnitHeal, "GreaterHealingWave");
                        })
                );
        }

        private static Composite GreaterHealingWaveIsSafetoCast()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.GreaterHealingWave &&
                UnitHealIsValid &&
                HealWeightUnitHeal < 100 &&
                HealWeightUnitHeal <= THSettings.Instance.GreaterHealingWaveHP + 10 &&
                (InBattleground ||
                 InArena) &&
                //SSpellManager.HasSpell("Greater Healing Wave") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                UnitHeal.Combat &&
                CanCastCheck("Greater Healing Wave") &&
                IsSafetoCast(),
                new Action(
                    ret =>
                        {
                            UnleaseElementEarthLiving(THSettings.Instance.UnleashElementsGHW, UnitHeal);
                            CastSpell("Greater Healing Wave", UnitHeal, "GreaterHealingWaveIsSafetoCast");
                        })
                );
        }

        #endregion

        #region GroundingCast

        private static WoWUnit UnitGroundingCast;

        private static bool GetUnitGroundingCast()
        {
            UnitGroundingCast = null;

            if (InBattleground || InArena)
            {
                UnitGroundingCast = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    unit.Distance < 30 &&
                    TalentSort(unit) < 4 &&
                    !unit.IsCastingHealingSpell &&
                    InterruptCheckGrounding(unit, THSettings.Instance.GroundingCastMs + MyLatency));
            }

            return BasicCheck(UnitGroundingCast);
        }

        private static WoWObject EnemyFreezingTrap;

        private static bool GetEnemyFreezingTrap()
        {
            EnemyFreezingTrap = null;

            EnemyFreezingTrap = ObjectManager.GetObjectsOfType<WoWDynamicObject>()
                                             .FirstOrDefault(
                                                 p =>
                                                 IsEnemy(p.Caster) &&
                                                 p.Entry == 2561);

            return EnemyFreezingTrap != null && EnemyFreezingTrap.IsValid;
        }

        private static bool IsFriendlyHunterNearby()
        {
            return FarFriendlyPlayers.Any(
                player =>
                BasicCheck(player) &&
                player.Class == WoWClass.Hunter);
        }

        private static WoWPlayer GroundingTrapPlayer;
        private static DateTime LastTrapEvent;

        private static bool NeedGroudingTrap()
        {
            GroundingTrapPlayer =
                NearbyFriendlyPlayers.FirstOrDefault(
                    player =>
                    !BasicCheck(player) &&
                    player.Location.Distance(EnemyFreezingTrap.Location) < 8);

            return BasicCheck(GroundingTrapPlayer);
        }

        private static void GroundingCastInterruptVoid()
        {
            if (THSettings.Instance.GroundingCast &&
                //SSpellManager.HasSpell("Grounding Totem") &&
                LastInterrupt < DateTime.Now &&
                //!Me.Mounted &&
                GetUnitGroundingCast() &&
                GetSpellCooldown("Grounding Totem").TotalMilliseconds <= MyLatency)
            {
                if (Me.IsCasting)
                {
                    SpellManager.StopCasting();
                }

                if (UnitGroundingCast.IsCasting || UnitGroundingCast.IsChanneling)
                {
                    CastSpell("Grounding Totem", UnitGroundingCast,
                              "Casting " +
                              UnitGroundingCast.CastingSpell.Name + " - " + UnitGroundingCast.CastingSpellId);
                    LastInterrupt = DateTime.Now + TimeSpan.FromMilliseconds(1500);
                }
            }

            if (THSettings.Instance.GroundingTrap &&
                InArena &&
                //SSpellManager.HasSpell("Grounding Totem") &&
                LastInterrupt < DateTime.Now &&
                //////SpellTypeCheck() &&
                //!Me.Mounted &&
                !IsFriendlyHunterNearby() &&
                GetEnemyFreezingTrap() &&
                NeedGroudingTrap() &&
                GetSpellCooldown("Grounding Totem").TotalMilliseconds <= MyLatency)
            {
                if (Me.IsCasting)
                {
                    SpellManager.StopCasting();
                }

                CastSpell("Grounding Totem", GroundingTrapPlayer,
                          "Grounding Totem Freezing Trap base on Trap Scan and Friend Distance to trap <=" +
                          GroundingTrapPlayer.Location.Distance(EnemyFreezingTrap.Location));

                LastInterrupt = DateTime.Now + TimeSpan.FromMilliseconds(1500);
            }

            if (THSettings.Instance.GroundingTrap &&
                InArena &&
                //SSpellManager.HasSpell("Grounding Totem") &&
                //////SpellTypeCheck() &&
                //!Me.Mounted &&
                LastTrapEvent + TimeSpan.FromMilliseconds(3000) > DateTime.Now &&
                GetSpellCooldown("Grounding Totem").TotalMilliseconds <= MyLatency)
            {
                if (Me.IsCasting)
                {
                    SpellManager.StopCasting();
                }

                CastSpell("Grounding Totem", Me,
                          "Grounding Totem Freezing Trap base on SPELL_CAST_SUCCESS event");
            }
        }

        #endregion

        #region GroundingLow
        //////done
        private static WoWUnit UnitGroundingLow;

        private static bool GetUnitGroundingLow(WoWUnit target)
        {
            UnitGroundingLow = null;

            UnitGroundingLow = NearbyUnFriendlyPlayers.Where(BasicCheck).FirstOrDefault(
                unit =>
                //////BasicCheck(unit) &&
                //////unit.CurrentTarget != null &&
                unit.GotTarget &&
                unit.CurrentTarget == target &&
                unit.Distance < 30 &&
                TalentSort(unit) < 4 &&
                unit.Class != WoWClass.Rogue &&
                unit.Class != WoWClass.Warrior);

            return BasicCheck(UnitGroundingLow);
        }
        //////done
        private static Composite GroundingLow()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.GroundingLow &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.GroundingLowHP &&
                //SSpellManager.HasSpell("Grounding Totem")  &&
                CanCastCheck("Grounding Totem") &&
                //!Me.Mounted &&
                GetUnitGroundingLow(UnitHeal),
                new Action(
                    ret => { CastSpell("Grounding Totem", Me, "GroundingLow"); })
                );
        }

        #endregion

        #region HealingRain

        private static WoWUnit PlayerHealingRain;

        private static double CountUnitHealingRain(WoWUnit unitCenter)
        {
            //var i = 0;

            //foreach (var unit in FarFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        unitCenter.Location.Distance(unit.Location) <= 10 &&
            //        HealWeight(unit) <= THSettings.Instance.HealingRainHP)
            //    {
            //        i = i + 1;
            //    }

            //    if (i >= THSettings.Instance.HealingRainUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return FarFriendlyPlayers.Count(
                unit =>
                BasicCheck(unit) &&
                unitCenter.Location.Distance(unit.Location) <= 10 &&
                HealWeight(unit) <= THSettings.Instance.HealingRainHP);
        }

        private static bool GetPlayerHealingRain()
        {
            PlayerHealingRain = null;

            PlayerHealingRain = NearbyFriendlyPlayers.OrderByDescending(CountUnitHealingRain)
                                                     .ThenBy(HealWeight)
                                                     .FirstOrDefault(
                                                         unit =>
                                                         BasicCheck(unit) &&
                                                         unit.Distance <= 40 &&
                                                         CountUnitHealingRain(unit) >=
                                                         THSettings.Instance.HealingRainUnit &&
                                                         Healable(unit));

            return BasicCheck(PlayerHealingRain);
        }

        private static DateTime LastClickRemoteLocation;

        private static Composite HealingRain()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingRain &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.HealingRainHP &&
                //SSpellManager.HasSpell("Healing Rain") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                CanCastCheck("Healing Rain") &&
                GetPlayerHealingRain(),
                new Action(
                    ret =>
                        {
                            UnleaseElementEarthLiving(THSettings.Instance.UnleashElementsHR, PlayerHealingRain);

                            CastSpell("Healing Rain", PlayerHealingRain, "HealingRain");
                            //while (Me.IsCasting)
                            //{
                            //    //Waiting Healing Rain Finish Casting
                            //}
                            LastClickRemoteLocation = DateTime.Now + TimeSpan.FromMilliseconds(300);
                            while (LastClickRemoteLocation > DateTime.Now)
                            {
                                SpellManager.ClickRemoteLocation(PlayerHealingRain.Location);
                            }
                            //Eval("CanCastCheck Healing", () => CanCastCheck("Healing"));
                            //Eval("GetPlayerHealingRain", () => GetPlayerHealingRain());
                        })
                );
        }

        #endregion

        #region HealingSurge

        private static DateTime LastHealingSurge;

        private static Composite HealingSurgeOutCombatEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingSurgeOutCombatEnh &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.HealingSurgeOutCombatEnhHP &&
                UseSpecialization == 2 &&
                //LastHealingSurge < DateTime.Now &&
                //SSpellManager.HasSpell("Healing Surge") &&
                //!Me.Mounted &&
                Me.ManaPercent > 50 &&
                CanCastCheck("Healing Surge") &&
                //CanCastWhileMoving() &&
                !HaveDPSTarget(Me),
                new Action(
                    ret =>
                        {
                            CastSpell("Healing Surge", UnitHeal, "HealingSurgeOutCombatEnh");
                            if (IsUsingAFKBot)
                            {
                                DoNotMove = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                                LastHealingSurge = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                HoldBotAction("Healing Surge");
                            }
                        })
                );
        }

        private static Composite HealingSurgeOutCombatEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingSurgeOutCombatEle &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.HealingSurgeOutCombatEleHP &&
                UseSpecialization == 1 &&
                CanCastCheck("Healing Surge") &&
                //LastHealingSurge < DateTime.Now &&
                //SSpellManager.HasSpell("Healing Surge") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                !HaveDPSTarget(UnitHeal),
                new Action(
                    ret =>
                        {
                            CastSpell("Healing Surge", UnitHeal, "HealingSurgeOutCombatEle");
                            if (IsUsingAFKBot)
                            {
                                DoNotMove = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                                LastHealingSurge = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                HoldBotAction("Healing Surge");
                            }
                        })
                );
        }
        //////done
        private static Composite HealingSurgeInCombatEnh()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.HealingSurgeInCombatEnh &&
                    LastHealingSurge < DateTime.Now &&
                    //SSpellManager.HasSpell("Healing Surge") &&
                    //!Me.Mounted &&
                    (MyAuraStackCount(53817, Me) > 4 ||
                     !CurrentTargetAttackable(20) ||
                     !CurrentTargetAttackable(5) &&
                     DebuffRoot(Me) ||
                     CurrentTargetAttackable(20) &&
                     Me.CurrentTarget.GetPredictedHealthPercent() > THSettings.Instance.UrgentHeal &&
                     Me.ManaPercent > 40 ||
                     CurrentTargetAttackable(20) &&
                     Me.CurrentTarget.GetPredictedHealthPercent() > Me.GetPredictedHealthPercent() &&
                     Me.ManaPercent > 40) &&
                    HealWeightMe <= THSettings.Instance.HealingSurgeInCombatEnhHP &&
                    //CanCastWhileMoving() &&
                    MyAuraStackCount(53817, Me) >= THSettings.Instance.HealingSurgeInCombatEnhStack &&
                    CanCastCheck("Healing Surge"),
                    new Action(
                        ret =>
                            {
                                CastSpell("Healing Surge", Me, "HealingSurgeInCombatEnh");
                                if (IsUsingAFKBot)
                                {
                                    //DoNotMove = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                                    //LastHealingSurge = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                    HoldBotAction("Healing Surge");
                                }
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.HealingSurgeInCombatEnhFriend &&
                    UnitHealIsValid &&
                    HealWeightUnitHeal <= THSettings.Instance.HealingSurgeInCombatEnhHPFriend &&
                    LastHealingSurge < DateTime.Now &&
                    //SSpellManager.HasSpell("Healing Surge") &&
                    //!Me.Mounted &&
                    (MyAuraStackCount(53817, Me) > 4 ||
                     !CurrentTargetAttackable(20) &&
                     Me.ManaPercent > 40 ||
                     CurrentTargetAttackable(20) &&
                     Me.CurrentTarget.GetPredictedHealthPercent() >= THSettings.Instance.UrgentHeal &&
                     Me.ManaPercent > 40) &&
                    //CanCastWhileMoving() &&
                    MyAuraStackCount(53817, Me) >= THSettings.Instance.HealingSurgeInCombatEnhStackFriend &&
                    CanCastCheck("Healing Surge"),
                    new Action(
                        ret =>
                            {
                                CastSpell("Healing Surge", UnitHeal, "HealingSurgeInCombatEnhFriend");
                                if (IsUsingAFKBot)
                                {
                                    //DoNotMove = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                                    //LastHealingSurge = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                    HoldBotAction("Healing Surge");
                                }
                            }))
                );
        }

        private static Composite HealingSurgeInCombatEle()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.HealingSurgeInCombatEle &&
                    LastHealingSurge < DateTime.Now &&
                    //SSpellManager.HasSpell("Healing Surge") &&
                    //!Me.Mounted &&
                    (!CurrentTargetAttackable(40) ||
                     CurrentTargetAttackable(40) &&
                     Me.CurrentTarget.HealthPercent > THSettings.Instance.UrgentHeal ||
                     CurrentTargetAttackable(40) &&
                     Me.CurrentTarget.HealthPercent > Me.HealthPercent) &&
                    HealWeightMe <= THSettings.Instance.HealingSurgeInCombatEleHP &&
                    (!THSettings.Instance.HealingSurgeInCombatEleCC ||
                     MeHasAura(16246)) && //Clearcasting 
                    //CanCastWhileMoving() &&
                    CanCastCheck("Healing Surge"),
                    new Action(
                        ret =>
                            {
                                CastSpell("Healing Surge", Me, "HealingSurgeInCombatEle");
                                if (IsUsingAFKBot)
                                {
                                    DoNotMove = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                                    LastHealingSurge = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                    HoldBotAction("Healing Surge");
                                }
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.HealingSurgeInCombatEleFriend &&
                    UnitHealIsValid &&
                    HealWeightUnitHeal <= THSettings.Instance.HealingSurgeInCombatEleFriendHP &&
                    LastHealingSurge < DateTime.Now &&
                    //SSpellManager.HasSpell("Healing Surge") &&
                    //!Me.Mounted &&
                    (!CurrentTargetAttackable(40) ||
                     CurrentTargetAttackable(40) &&
                     Me.CurrentTarget.HealthPercent > THSettings.Instance.UrgentHeal) &&
                    (!THSettings.Instance.HealingSurgeInCombatEleFriendCC ||
                     MeHasAura(16246)) && //Clearcasting 
                    //CanCastWhileMoving() &&
                    CanCastCheck("Healing Surge"),
                    new Action(
                        ret =>
                            {
                                CastSpell("Healing Surge", UnitHeal, "HealingSurgeInCombatEleFriend");
                                if (IsUsingAFKBot)
                                {
                                    DoNotMove = DateTime.Now + TimeSpan.FromMilliseconds(3000);
                                    LastHealingSurge = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                                    HoldBotAction("Healing Surge");
                                }
                            }))
                );
        }

        private static Composite HealingSurgeRes()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingSurgeRes &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.HealingSurgeResHP &&
                //SSpellManager.HasSpell("Healing Surge") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                //MyAura("Tidal Waves", Me) &&
                UnitHeal.Combat &&
                //CanCastWhileMoving() &&
                CanCastCheck("Healing Surge"),
                new Action(
                    ret =>
                        {
                            UnleaseElementEarthLiving(THSettings.Instance.UnleashElementsHS, UnitHeal);
                            CastSpell("Healing Surge", UnitHeal, "HealingSurgeRes");
                        })
                );
        }

        private static Composite HealingSurgeResIsSafetoCast()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingSurgeRes &&
                UnitHealIsValid &&
                HealWeightUnitHeal < 100 &&
                HealWeightUnitHeal <= THSettings.Instance.HealingSurgeResHP + 10 &&
                (InBattleground ||
                 InArena) &&
                //SSpellManager.HasSpell("Healing Surge") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                IsSafetoCast() &&
                UnitHeal.Combat &&
                //CanCastWhileMoving() &&
                CanCastCheck("Healing Surge"),
                new Action(
                    ret =>
                        {
                            UnleaseElementEarthLiving(THSettings.Instance.UnleashElementsHS, UnitHeal);
                            CastSpell("Healing Surge", UnitHeal, "HealingSurgeResIsSafetoCast");
                        })
                );
        }

        #endregion

        #region HealingWave

        private static Composite HealingWave()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingWave &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.HealingWaveHP &&
                //(//SSpellManager.HasSpell("Earth Shield") ||
                // //SSpellManager.HasSpell("Earth Shield") &&
                //!Me.Mounted &&
                SpellManager.HasSpell("Healing Wave") && //Need it here
                Me.CurrentMana > SpellManager.Spells["Healing Wave"].PowerCost*4 && //Save Mana for ESsss
                //CanCastWhileMoving() &&
                (HealWeightUnitHeal >= THSettings.Instance.GreaterHealingWaveHP ||
                 !UnitHeal.Combat && !Me.Combat) &&
                CanCastCheck("Healing Wave"),
                new Action(
                    ret => { CastSpell("Healing Wave", UnitHeal, "HealingWave"); })
                )
                ;
        }

        private static Composite HealingWaveTopUpRaid()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingWave &&
                InRaid &&
                UnitHealIsValid &&
                Me.Combat &&
                UnitHeal.Combat &&
                //HealWeightUnitHeal <= 100 &&
                SpellManager.HasSpell("Healing Wave") && //Need it here
                Me.CurrentMana > SpellManager.Spells["Healing Wave"].PowerCost*4 && //Save Mana for ESsss
                //CanCastWhileMoving() &&
                CanCastCheck("Healing Wave"),
                new Action(
                    ret => { CastSpell("Healing Wave", UnitHeal, "HealingWaveTopUpRaid"); })
                );
        }

        private static DateTime HealingWaveBaitInterruptLast;
        private static readonly Random rnd = new Random();

        private static Composite HealingWaveBaitInterrupt()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingWave &&
                (InArena || InBattleground) &&
                UnitHealIsValid &&
                HealWeightUnitHeal > THSettings.Instance.HealingWaveHP &&
                HealingWaveBaitInterruptLast < DateTime.Now &&
                Me.Combat &&
                UnitHeal.Combat &&
                //HealWeightUnitHeal <= 100 &&
                SpellManager.HasSpell("Healing Wave") && //Need it here
                CanCastCheck("Healing Wave"),
                new Action(
                    ret =>
                        {
                            CastSpell("Healing Wave", UnitHeal, "HealingWaveBaitInterrupt");
                            HealingWaveBaitInterruptLast = DateTime.Now +
                                                           TimeSpan.FromMilliseconds(rnd.Next(2000, 3000));
                        })
                );
        }

        #endregion

        #region HealingStreamTotem

        private static double CountUnitHealingStreamTotem()
        {
            //var i = 0;

            //foreach (var unit in NearbyFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        unit.Combat &&
            //        unit.Distance <= 35 &&
            //        HealWeight(unit) < THSettings.Instance.HealingStreamTotemHP)
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.HealingStreamTotemUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyFriendlyPlayers.Count(
                unit => BasicCheck(unit) &&
                        unit.Combat &&
                        unit.Distance <= 35 &&
                        HealWeight(unit) < THSettings.Instance.HealingStreamTotemHP);
        }

        private static Composite HealingStreamTotem()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingStreamTotem &&
                UnitHealIsValid &&
                //.HealWeightUnitHeal > THSettings.Instance.UrgentHeal &&
                HealWeightUnitHeal < THSettings.Instance.HealingStreamTotemHP &&
                UnitHeal.Distance < 35 &&
                //SSpellManager.HasSpell("Healing Stream Totem") &&
                //!Me.Mounted &&
                Me.Combat &&
                !MyTotemWaterCheck(Me, 40) &&
                CanCastCheck("Healing Stream Totem") &&
                CountUnitHealingStreamTotem() >= THSettings.Instance.HealingStreamTotemUnit,
                new Action(
                    ret =>
                        {
                            CastSpell("Healing Stream Totem", Me, "HealingStreamTotem");
                            //Eval("CanCastCheck Healing Stream Totem", () => CanCastCheck("Healing Stream Totem"));
                            //Eval("CountUnitHealingStreamTotem",
                            //() => CountUnitHealingStreamTotem() >= THSettings.Instance.HealingTideTotemUnit);
                        })
                );
        }

        #endregion

        #region HealthTideTotem

        private static double CountUnitHealingTideTotem()
        {
            //var i = 0;

            //foreach (var unit in NearbyFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        unit.Combat &&
            //        unit.Distance <= 35 &&
            //        HealWeight(unit) < THSettings.Instance.HealingTideTotemHP &&
            //        !Invulnerable(unit))
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.HealingTideTotemUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyFriendlyPlayers.Count(
                unit => BasicCheck(unit) &&
                        unit.Combat &&
                        unit.Distance <= 35 &&
                        HealWeight(unit) < THSettings.Instance.HealingTideTotemHP &&
                        !Invulnerable(unit));
        }

        private static Composite HealingTideTotem()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.HealingTideTotem &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.HealingTideTotemHP &&
                UnitHeal.Distance < 30 &&
                //SSpellManager.HasSpell("Healing Tide Totem") &&
                //!Me.Mounted &&
                !MyTotemCheck("Spirit Link Totem", Me, 40) &&
                !MyTotemCheck("Mana Tide Totem", Me, 40) &&
                CanCastCheck("Healing Tide Totem") &&
                CountUnitHealingTideTotem() >= THSettings.Instance.HealingTideTotemUnit,
                new Action(
                    ret =>
                        {
                            CastSpell("Healing Tide Totem", Me, "HealingTideTotem");
                            //Eval("CanCastCheck Healing Tide Totem", () => CanCastCheck("Healing Tide Totem"));
                            //Eval("CountUnitHealingTideTotem",
                            //() => CountUnitHealingTideTotem() >= THSettings.Instance.HealingTideTotemUnit);
                        })
                );
        }

        #endregion

        #region Hex

        private static readonly HashSet<int> HexImmuneAura = new HashSet<int>
            {
                768,
                5487,
                783,
                1066,
                33943,
                40120,
                24858,
                102560,
                102543,
                102558,
                5420,
                81097,
                81098,
                2645,
                46924,
            };

        private static bool CanHex(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            return
                AuraCacheList.Where(aura => aura.AuraCacheUnit == target.Guid)
                             .All(aura => !HexImmuneAura.Contains(aura.AuraCacheId));

            //foreach (var aura in AuraCacheList)
            //{
            //    if (aura.AuraCacheUnit != target.Guid)
            //    {
            //        continue;
            //    }

            //    if (HexImmuneAura.Contains(aura.AuraCacheId))
            //    {
            //        return false;
            //    }
            //}

            //return true;

            //return AuraCacheList.Where(aura => aura.AuraCacheUnit == target.Guid)
            //                    .All(
            //                        aura =>
            //                        aura.AuraCacheId != 768 &&
            //                        aura.AuraCacheId != 5487 &&
            //                        aura.AuraCacheId != 783 &&
            //                        aura.AuraCacheId != 1066 &&
            //                        aura.AuraCacheId != 33943 &&
            //                        aura.AuraCacheId != 40120 &&
            //                        aura.AuraCacheId != 24858 &&
            //                        aura.AuraCacheId != 102560 &&
            //                        aura.AuraCacheId != 102543 &&
            //                        aura.AuraCacheId != 102558 &&
            //                        aura.AuraCacheId != 5420 &&
            //                        aura.AuraCacheId != 81097 &&
            //                        aura.AuraCacheId != 81098 &&
            //                        aura.AuraCacheId != 2645 &&
            //                        aura.AuraCacheId != 46924);
        }

        private static WoWUnit UnitHex;

        private static bool GetUnitUnitHex()
        {
            UnitHex = null;

            if (InArena || InBattleground)
            {
                UnitHex = NearbyUnFriendlyPlayers.Where(BasicCheck)
                                                 .OrderByDescending(TalentSortSimple)
                                                 .ThenBy(CountFriendDPSTarget)
                                                 .ThenByDescending(unit => unit.HealthPercent)
                                                 .FirstOrDefault(
                                                     unit =>
                                                     //////BasicCheck(unit) &&
                                                     (THSettings.Instance.HexHealer && TalentSort(unit) > 3 ||
                                                      THSettings.Instance.HexDPS && TalentSort(unit) < 4) &&
                                                     (UseSpecialization == 3 ||
                                                      UseSpecialization != 3 && unit != Me.CurrentTarget) &&
                                                     CanHex(unit) &&
                                                     !DebuffCCDuration(unit, 2000) &&
                                                     !InvulnerableSpell(unit) &&
                                                     Attackable(unit, 30));
            }
            else
            {
                UnitHex = FarUnFriendlyUnits.Where(BasicCheck)
                                               .OrderByDescending(unit => unit.CurrentHealth)
                                               .ThenBy(CountFriendDPSTarget)
                                               .FirstOrDefault(
                                                   unit =>
                                                   //.BasicCheck(unit) &&
                                                   unit.Combat &&
                                                   !unit.IsBoss &&
                                                   (unit.IsHumanoid || unit.IsBeast) &&
                                                   InProvingGrounds ||
                                                   unit.GotTarget &&
                                                   FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                                                   unit != Me.CurrentTarget &&
                                                   Attackable(unit, 30));
            }
            return BasicCheck(UnitHex);
        }

        private static Composite Hex()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.Hex &&
                HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                //SSpellManager.HasSpell("Hex") &&
                //!Me.Mounted &&
                //CanCastWhileMoving() &&
                CanCastCheck("Hex") &&
                GetUnitUnitHex(),
                new Action(
                    ret => { CastSpell("Hex", UnitHex, "Hex"); })
                );
        }

        #endregion

        #region Hotkeys

        private static WoWUnit HotkeyTargettoUnit(int HotkeyTarget)
        {
            switch (HotkeyTarget)
            {
                case 1:
                    if (BasicCheck(Me.CurrentTarget))
                    {
                        return Me.CurrentTarget;
                    }
                    break;
                case 2:
                    if (BasicCheck(Me.FocusedUnit))
                    {
                        return Me.FocusedUnit;
                    }
                    break;
                case 3:
                    return Me;
            }
            return null;
        }

        private static bool HotKeyTargetValidate(int HotkeyTarget)
        {
            if (HotkeyTargettoUnit(HotkeyTarget) != null)
            {
                return true;
            }
            return false;
        }

        private static string HotKeySpelltoName(int HotkeySpell)
        {
            switch (HotkeySpell)
            {
                case 1:
                    return "Ghost Wolf";
                case 2:
                    return "Grounding Totem";
                case 3:
                    return "Hex";
                case 4:
                    return "Tremor Totem";
                case 5:
                    return "Wind Shear";
                case 6:
                    return "Capacitor Totem";
            }
            return "None Exist Spell";
        }

        private static bool HotKeySpellValidate(int HotkeySpell)
        {
            return true;

            //if ( //SSpellManager.HasSpell(HotKeySpelltoName(HotkeySpell)) &&
            //    SpellManager.Spells[HotKeySpelltoName(HotkeySpell)].CooldownTimeLeft.TotalMilliseconds < 400)
            //{
            //    return true;
            //}

            //return false;
        }

        private static DateTime LastHotKey1Press;


        private static Composite Hotkey1()
        {
            //////return new Action(delegate
            //////    {
            //////        if (THSettings.Instance.Hotkey1Target != 0 &&
            //////            THSettings.Instance.Hotkey1Key != 0 &&
            //////            THSettings.Instance.Hotkey1Spell != 0 &&
            //////            (THSettings.Instance.Hotkey1Mod == 0 ||
            //////             THSettings.Instance.Hotkey1Mod != 0 &&
            //////             GetAsyncKeyState(IndexToKeysMod(THSettings.Instance.Hotkey1Mod)) < 0) &&
            //////            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey1Key)) < 0 &&
            //////            HotKeyTargetValidate(THSettings.Instance.Hotkey1Target) &&
            //////            HotKeySpellValidate(THSettings.Instance.Hotkey1Spell))
            //////        {
            //////            SafelyFacingTarget(HotkeyTargettoUnit(THSettings.Instance.Hotkey1Target));

            //////            if (Me.IsCasting && Me.CastingSpell.Name != HotKeySpelltoName(THSettings.Instance.Hotkey1Spell))
            //////            {
            //////                SpellManager.StopCasting();
            //////            }

            //////            CastSpell(HotKeySpelltoName(THSettings.Instance.Hotkey1Spell),
            //////                      HotkeyTargettoUnit(THSettings.Instance.Hotkey1Target),
            //////                      "Hotkey: Cast " + HotKeySpelltoName(THSettings.Instance.Hotkey1Spell) + " on " +
            //////                      SafeName(HotkeyTargettoUnit(THSettings.Instance.Hotkey1Target)));

            //////            if (GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey1Key)) < 0)
            //////            {
            //////                return RunStatus.Success;
            //////            }
            //////        }
            //////        return RunStatus.Failure;
            //////    });
            return new Action(delegate
                {
                    if (THSettings.Instance.Hotkey1Target != 0 &&
                        THSettings.Instance.Hotkey1Key != 0 &&
                        THSettings.Instance.Hotkey1Spell != 0 &&
                        (THSettings.Instance.Hotkey1Mod == 0 ||
                         THSettings.Instance.Hotkey1Mod != 0 &&
                         GetAsyncKeyState(IndexToKeysMod(THSettings.Instance.Hotkey1Mod)) < 0) &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey1Key)) < 0 &&
                        HotKeyTargetValidate(THSettings.Instance.Hotkey1Target) &&
                        HotKeySpellValidate(THSettings.Instance.Hotkey1Spell) &&
                        CanCastCheck(HotKeySpelltoName(THSettings.Instance.Hotkey2Spell)))
                    {
                        if (Me.IsCasting && Me.CastingSpell.Name != HotKeySpelltoName(THSettings.Instance.Hotkey1Spell))
                        {
                            SpellManager.StopCasting();
                        }

                        LastHotKey1Press = DateTime.Now.AddSeconds(6);

                        CastSpell(HotKeySpelltoName(THSettings.Instance.Hotkey1Spell),
                                  HotkeyTargettoUnit(THSettings.Instance.Hotkey1Target),
                                  "Hotkey: Cast " + HotKeySpelltoName(THSettings.Instance.Hotkey1Spell) + " on " +
                                  SafeName(HotkeyTargettoUnit(THSettings.Instance.Hotkey1Target)));

                        if (GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey1Key)) < 0)
                        {
                            return RunStatus.Success;
                        }
                    }
                    return RunStatus.Failure;
                });

        }

        private static DateTime LastHotKey2Press;
        private static WoWPoint LastHotKey2PressPosition;
        private static Composite Hotkey2()
        {
            return new Action(delegate
                {
                    if (THSettings.Instance.Hotkey2Target != 0 &&
                        THSettings.Instance.Hotkey2Key != 0 &&
                        THSettings.Instance.Hotkey2Spell != 0 &&
                        (THSettings.Instance.Hotkey2Mod == 0 ||
                         THSettings.Instance.Hotkey2Mod != 0 &&
                         GetAsyncKeyState(IndexToKeysMod(THSettings.Instance.Hotkey2Mod)) < 0) &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey2Key)) < 0 &&
                        HotKeyTargetValidate(THSettings.Instance.Hotkey2Target) &&
                        HotKeySpellValidate(THSettings.Instance.Hotkey2Spell) &&
                        CanCastCheck(HotKeySpelltoName(THSettings.Instance.Hotkey2Spell)))
                    {
                        //SafelyFacingTarget(HotkeyTargettoUnit(THSettings.Instance.Hotkey2Target));

                        if (Me.IsCasting && Me.CastingSpell.Name != HotKeySpelltoName(THSettings.Instance.Hotkey2Spell))
                        {
                            SpellManager.StopCasting();
                        }
                        LastHotKey2Press = DateTime.Now.AddSeconds(6);
                        LastHotKey2PressPosition = Me.GetPosition();
                        CastSpell(HotKeySpelltoName(THSettings.Instance.Hotkey2Spell),
                                  HotkeyTargettoUnit(THSettings.Instance.Hotkey2Target),
                                  "Hotkey: Cast " + HotKeySpelltoName(THSettings.Instance.Hotkey2Spell) + " on " +
                                  SafeName(HotkeyTargettoUnit(THSettings.Instance.Hotkey2Target)));

                        if (GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey2Key)) < 0)
                        {
                            return RunStatus.Success;
                        }
                    }
                    return RunStatus.Failure;
                });
        }

        private static DateTime LastHotKey3Press;
        private static Composite Hotkey3()
        {
            return new Action(delegate
                {
                    if (THSettings.Instance.Hotkey3Target != 0 &&
                        THSettings.Instance.Hotkey3Key != 0 &&
                        THSettings.Instance.Hotkey3Spell != 0 &&
                        (THSettings.Instance.Hotkey3Mod == 0 ||
                         THSettings.Instance.Hotkey3Mod != 0 &&
                         GetAsyncKeyState(IndexToKeysMod(THSettings.Instance.Hotkey3Mod)) < 0) &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey3Key)) < 0 &&
                        HotKeyTargetValidate(THSettings.Instance.Hotkey3Target) &&
                        HotKeySpellValidate(THSettings.Instance.Hotkey3Spell))
                    {
                        SafelyFacingTarget(HotkeyTargettoUnit(THSettings.Instance.Hotkey3Target));

                        if (Me.IsCasting && Me.CastingSpell.Name != HotKeySpelltoName(THSettings.Instance.Hotkey3Spell))
                        {
                            SpellManager.StopCasting();
                        }
                        LastHotKey3Press = DateTime.Now.AddSeconds(5);
                        CastSpell(HotKeySpelltoName(THSettings.Instance.Hotkey3Spell),
                                  HotkeyTargettoUnit(THSettings.Instance.Hotkey3Target),
                                  "Hotkey: Cast " + HotKeySpelltoName(THSettings.Instance.Hotkey3Spell) + " on " +
                                  SafeName(HotkeyTargettoUnit(THSettings.Instance.Hotkey3Target)));

                        if (GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey3Key)) < 0)
                        {
                            return RunStatus.Success;
                        }
                    }
                    return RunStatus.Failure;
                });
        }

        private static DateTime LastHotKey4Press;
        private static Composite Hotkey4()
        {
            return new Action(delegate
                {
                    if (THSettings.Instance.Hotkey4Target != 0 &&
                        THSettings.Instance.Hotkey4Key != 0 &&
                        THSettings.Instance.Hotkey4Spell != 0 &&
                        (THSettings.Instance.Hotkey4Mod == 0 ||
                         THSettings.Instance.Hotkey4Mod != 0 &&
                         GetAsyncKeyState(IndexToKeysMod(THSettings.Instance.Hotkey4Mod)) < 0) &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey4Key)) < 0 &&
                        HotKeyTargetValidate(THSettings.Instance.Hotkey4Target) &&
                        HotKeySpellValidate(THSettings.Instance.Hotkey4Spell))
                    {
                        SafelyFacingTarget(HotkeyTargettoUnit(THSettings.Instance.Hotkey4Target));

                        if (Me.IsCasting && Me.CastingSpell.Name != HotKeySpelltoName(THSettings.Instance.Hotkey4Spell))
                        {
                            SpellManager.StopCasting();
                        }
                        LastHotKey4Press = DateTime.Now.AddSeconds(5);
                        CastSpell(HotKeySpelltoName(THSettings.Instance.Hotkey4Spell),
                                  HotkeyTargettoUnit(THSettings.Instance.Hotkey4Target),
                                  "Hotkey: Cast " + HotKeySpelltoName(THSettings.Instance.Hotkey4Spell) + " on " +
                                  SafeName(HotkeyTargettoUnit(THSettings.Instance.Hotkey4Target)));

                        if (GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey4Key)) < 0)
                        {
                            return RunStatus.Success;
                        }
                    }
                    return RunStatus.Failure;
                });
        }

        private static Composite Hotkey5()
        {
            return new Action(delegate
                {
                    if (THSettings.Instance.Hotkey5Target != 0 &&
                        THSettings.Instance.Hotkey5Key != 0 &&
                        THSettings.Instance.Hotkey5Spell != 0 &&
                        (THSettings.Instance.Hotkey5Mod == 0 ||
                         THSettings.Instance.Hotkey5Mod != 0 &&
                         GetAsyncKeyState(IndexToKeysMod(THSettings.Instance.Hotkey5Mod)) < 0) &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey5Key)) < 0 &&
                        HotKeyTargetValidate(THSettings.Instance.Hotkey5Target) &&
                        HotKeySpellValidate(THSettings.Instance.Hotkey5Spell))
                    {
                        SafelyFacingTarget(HotkeyTargettoUnit(THSettings.Instance.Hotkey5Target));

                        if (Me.IsCasting && Me.CastingSpell.Name != HotKeySpelltoName(THSettings.Instance.Hotkey5Spell))
                        {
                            SpellManager.StopCasting();
                        }

                        CastSpell(HotKeySpelltoName(THSettings.Instance.Hotkey5Spell),
                                  HotkeyTargettoUnit(THSettings.Instance.Hotkey5Target),
                                  "Hotkey: Cast " + HotKeySpelltoName(THSettings.Instance.Hotkey5Spell) + " on " +
                                  SafeName(HotkeyTargettoUnit(THSettings.Instance.Hotkey5Target)));

                        if (GetAsyncKeyState(IndexToKeys(THSettings.Instance.Hotkey5Key)) < 0)
                        {
                            return RunStatus.Success;
                        }
                    }
                    return RunStatus.Failure;
                });
        }

        #endregion

        #region LavaBust

        private static Composite LavaBustElemental()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LavaBustElemental &&
                //SSpellManager.HasSpell("Lava Burst") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                //CanCastWhileMoving() &&
                (MyAura("Flame Shock", Me.CurrentTarget) ||
                 !CanCastCheck("Flame Shock")) &&
                CanCastCheck("Lava Burst"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lava Burst", Me.CurrentTarget, "LavaBustElemental");
                        })
                );
        }

        private static Composite LavaBustElementalProc()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LavaBustElemental &&
                //SSpellManager.HasSpell("Lava Burst") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                MeHasAura(77762) &&
                (MyAura("Flame Shock", Me.CurrentTarget) ||
                 !CanCastCheck("Flame Shock")) &&
                CanCastCheck("Lava Burst"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lava Burst", Me.CurrentTarget, "LavaBustElementalProc");
                        })
                );
        }

        #endregion

        #region LavaLash

        private static Composite LavaLash()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LavaLash &&
                //SSpellManager.HasSpell("Lava Lash") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(5) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Lava Lash"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lava Lash", Me.CurrentTarget, "LavaLash");
                        })
                );
        }

        #endregion

        #region LightningBolt

        private static Composite LightningBoltAncestralSwiftness()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LightningBolt &&
                THSettings.Instance.AncestralSwiftnessLB &&
                //SSpellManager.HasSpell("Lightning Bolt") &&
                //SSpellManager.HasSpell("Ancestral Swiftness") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerableMagic &&
                MyAuraStackCount(53817, Me) <= 0 && //Maelstrom Weapon
                (IsWorthyTarget(Me.CurrentTarget, 2, 0.5) ||
                 HaveWorthyTargetAttackingMe()) &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Ancestral Swiftness") &&
                CanCastCheck("Lightning Bolt"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Ancestral Swiftness", Me.CurrentTarget,
                                      "AncestralSwiftnessLightningBolt");
                            CastSpell("Lightning Bolt", Me.CurrentTarget, "LightningBoltAncestralSwiftness");
                        })
                );
        }

        private static Composite LightningBoltEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LightningBoltEnh &&
                //SSpellManager.HasSpell("Lightning Bolt") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerableMagic &&
                MyAuraStackCount(53817, Me) >= THSettings.Instance.LightningBoltEnhMaelstromStack && //Maelstrom Weapon
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Lightning Bolt"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lightning Bolt", Me.CurrentTarget, "LightningBoltEnh");
                        })
                );
        }

        private static DateTime LastLightningBoltFiller;
        //////done
        private static Composite LightningBoltFillerEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LightningBoltFiller &&
                LastLightningBoltFiller < DateTime.Now &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerableMagic &&
                !Me.IsCasting &&
                FacingOverride(Me.CurrentTarget) &&
                SpellManager.HasSpell("Lightning Bolt") &&
                Me.CurrentMana > SpellManager.Spells["Lightning Bolt"].PowerCost*3 &&
                (MyAuraStackCount(53817, Me) >= THSettings.Instance.LightningBoltFillerMaelstromStack ||
                 !SpellManager.HasSpell("Maelstrom Weapon")) &&
                //Maelstrom Weapon
                CanCastCheck("Lightning Bolt"),
                //////(!UseLightningShieldGCDCheck ||
                ////// UseLightningShieldGCDCheck &&
                ////// SpellManager.Spells["Lightning Shield"].CooldownTimeLeft.TotalMilliseconds <= MyLatency),
                //!CanCastCheck("Stormstrike") &&
                //!CanCastCheck("Lava Lash") &&
                //!CanCastCheck("Unleash Elements"),
                new Action(
                    ret =>
                        {
                            LastLightningBoltFiller = DateTime.Now + TimeSpan.FromSeconds(3);
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lightning Bolt", Me.CurrentTarget, "LightningBoltFiller");
                        })
                );
        }

        private static Composite LightningBoltFillerEnhRange()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LightningBolt &&
                (InBattleground || InArena) &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetAttackable(7) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                SpellManager.HasSpell("Lightning Bolt") &&
                Me.CurrentMana > SpellManager.Spells["Lightning Bolt"].PowerCost*3 &&
                CanCastCheck("Lightning Bolt") &&
                (!MeHasAura(53817) || //Maelstrom Weapon
                 MyAuraStackCount(53817, Me) < 2 &&
                 DebuffRoot(Me)),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lightning Bolt", Me.CurrentTarget, "LightningBoltFillerEnhRange");
                        })
                );
        }

        private static Composite LightningBoltFillerElemental()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LightningBoltFillerElemental &&
                //SSpellManager.HasSpell("Lightning Bolt") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Lightning Bolt"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                !Me.IsCasting &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Lightning Bolt") &&
                ( //SSpellManager.HasSpell("Flame Shock") ||
                    MyAuraTimeLeft("Flame Shock", Me.CurrentTarget) > 2000 ||
                    !UseLightningShieldGCDCheck ||
                    UseLightningShieldGCDCheck &&
                    SpellManager.Spells["Lightning Shield"].CooldownTimeLeft.TotalMilliseconds <= MyLatency),
                //!CanCastCheck("Lava Burst") &&
                //!CanCastCheck("Elemental Blast") &&
                //(Me.ManaPercent >= THSettings.Instance.EleAoEMana ||
                // !CanCastCheck("Flame Shock")),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lightning Bolt", Me.CurrentTarget, "LightningBoltFillerElemental");
                        })
                );
        }

        private static Composite LightningBoltFillerElementalAoE()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LightningBoltFillerElemental &&
                //SSpellManager.HasSpell("Lightning Bolt") &&
                //!Me.Mounted &&
                CurrentTargetAttackable((int) SpellManager.Spells["Lightning Bolt"].MaxRange) &&
                !CurrentTargetCheckInvulnerableMagic &&
                !Me.IsCasting &&
                FacingOverride(Me.CurrentTarget) &&
                !MeHasAura(77762) && //Lava Surge
                CanCastCheck("Lightning Bolt") &&
                (!UseLightningShieldGCDCheck ||
                 UseLightningShieldGCDCheck &&
                 SpellManager.Spells["Lightning Shield"].CooldownTimeLeft.TotalMilliseconds <= MyLatency),
                //!CanCastCheck("Lava Burst") &&
                //!CanCastCheck("Elemental Blast") &&
                //(Me.ManaPercent >= THSettings.Instance.EleAoEMana ||
                // !CanCastCheck("Flame Shock")),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Lightning Bolt", Me.CurrentTarget, "LightningBoltFillerElementalAoE");
                        })
                )
                ;
        }

        //Glyph of Telluric Currents - 55453
        private static WoWUnit UnitAttackRTelluricCurrents;

        private static bool GetUnitAttackRTelluricCurrents()
        {
            UnitAttackRTelluricCurrents = null;

            if (InArena || InBattleground)
            {
                UnitAttackRTelluricCurrents = NearbyUnFriendlyPlayers.
                    OrderBy(unit => unit.HealthPercent).
                    FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        unit.MaxHealth > MeMaxHealth*0.5 &&
                        !InvulnerableSpell(unit) &&
                        FacingOverride(unit) &&
                        Attackable(unit, 30));
            }

            if (UnitAttackRTelluricCurrents == null)
            {
                UnitAttackRTelluricCurrents = NearbyUnFriendlyUnits.
                    OrderByDescending(unit => unit.HealthPercent).
                    FirstOrDefault(
                        unit =>
                        BasicCheck(unit) &&
                        (IsDummy(unit) ||
                         unit.Combat &&
                         unit.MaxHealth > MeMaxHealth*0.5) &&
                        !InvulnerableSpell(unit) &&
                        FacingOverride(unit) &&
                        unit.CurrentTarget != null &&
                        FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                        Attackable(unit, 30));
            }

            return BasicCheck(UnitAttackRTelluricCurrents);
        }

        private static Composite LightningBoltTelluricCurrents()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AttackRestoLightningBoltGlyph &&
                //SSpellManager.HasSpell("Lightning Bolt") &&
                HasGlyph.Contains("55453") && //Glyph of Telluric Currents - 55453
                //!Me.Mounted &&
                Me.Combat &&
                !Me.IsCasting &&
                Me.ManaPercent < THSettings.Instance.DoNotHealAbove &&
                (Me.ManaPercent < 20 ||
                 HealWeightUnitHeal > THSettings.Instance.PriorityHeal) &&
                CanCastCheck("Lightning Bolt") &&
                GetUnitAttackRTelluricCurrents(),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(UnitAttackRTelluricCurrents);
                            CastSpell("Lightning Bolt", UnitAttackRTelluricCurrents,
                                      "AttackRestoLightningBoltGlyph");
                        }));
        }

        #endregion

        #region LightningShield

        private static Composite LightningShield()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.LightningShield &&
                //SSpellManager.HasSpell("Lightning Shield") &&
                //!Me.Mounted &&
                //!MeHasAura(52127) &&
                //(!MeHasAura(52127) ||
                // MeHasAura(52127) &&
                // Me.ManaPercent > THSettings.Instance.PriorityHeal) && //Water Shield
                MyAuraTimeLeft(324, Me) < 1800000 &&
                CanCastCheck("Lightning Shield"),
                new Action(
                    ret => { CastSpell("Lightning Shield", Me, "LightningShield"); })
                );
        }

        #endregion

        #region MagmaTotem

        private static Composite MagmaTotemEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.MagmaTotemEnh &&
                //LastDropTotem < DateTime.Now &&
                //SSpellManager.HasSpell("Magma Totem") &&
                //!Me.Mounted &&
                !MyTotemCheck("Fire Elemental Totem", Me, 40) &&
                !MyTotemCheck("Magma Totem", Me, 40) &&
                CanCastCheck("Magma Totem") &&
                CountEnemyNear(Me, 10) >= THSettings.Instance.MagmaTotemEnhUnit,
                new Action(
                    ret =>
                        {
                            //LastDropTotem = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                            //SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Magma Totem", Me, "MagmaTotemEnh");
                        })
                );
        }

        private static Composite MagmaTotemEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.MagmaTotemEle &&
                //LastDropTotem < DateTime.Now &&
                //SSpellManager.HasSpell("Magma Totem") &&
                //!Me.Mounted &&
                !MyTotemCheck("Fire Elemental Totem", Me, 40) &&
                !MyTotemCheck("Magma Totem", Me, 40) &&
                CanCastCheck("Magma Totem") &&
                CountEnemyNear(Me, 8) >= THSettings.Instance.MagmaTotemEleUnit,
                new Action(
                    ret =>
                        {
                            //LastDropTotem = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                            //SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Magma Totem", Me, "MagmaTotemEle");
                        })
                );
        }

        #endregion

        #region ManaTideTotem

        private static Composite ManaTideTotem()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ManaTideTotem &&
                //SSpellManager.HasSpell("Mana Tide Totem") &&
                //!Me.Mounted &&
                Me.Combat &&
                Me.ManaPercent <= THSettings.Instance.ManaTideTotemMN &&
                !MyTotemWaterCheck(Me, 40) &&
                !MeHasAura("Mana Tide") &&
                CanCastCheck("Mana Tide Totem"),
                new Action(
                    ret => { CastSpell("Mana Tide Totem", Me, "ManaTideTotem"); })
                );
        }

        #endregion

        #region Purge

        private static readonly HashSet<string> NeedPurgeASAPRestoHS = new HashSet<string>
            {
                //Death Knight
                //"Unholy Frenzy",
                //Druid
                "Innervate",
                "Nature's Swiftness",
                "Predator's Swiftness",
                //Hunter
                "Master's Call",
                //Mage
                "Arcane Power",
                "Alter Time",
                //"Ice Barrier",
                "Icy Veins",
                //"Mana Shield",
                "Fingers of Frost",
                "Presence of Mind",
                //Monk
                //"Life Cocoon", Not dispellable
                //"Enveloping Mist",
                //"Touch of Karma",//Thank cedricdu94. No, your healer can dispell you when you touch monk with touch of karma but you cant dispell enemy buff touch of karma
                //Paladin
                "Divine Plea",
                "Hand of Freedom",
                //"Avenging Wrath",
                "Hand of Protection",
                "Hand of Sacrifice",
                //"Sacred Shield",
                //Priest
                "Inner Focus",
                "Fear Ward",
                "Power Infusion",
                //"Power Word: Shield",
                //Rogue
                //??
                //Shaman
                //"Earth Shield",
                "Elemental Mastery",
                //"Ghost Wolf",
                //"Nature's Swiftness", Same as druid
                "Spiritwalker's Grace",
                //Warlock
                "Sacrifice",
                //Warrior
                //"Berserker Rage",
                //"Enrage",
                //Warlock
                //"Dark Soul: Instability", Dark Soul is not dispellable
                //"Dark Soul: Knowledge", Dark Soul is not dispellable
                //"Dark Soul: Misery", Dark Soul is not dispellable
            };

        private static bool NeedPurgeASAPResto(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            if (AuraCacheList.Any(
                a => a.AuraCacheUnit == target.Guid && NeedPurgeASAPRestoHS.Contains(a.AuraCacheAura.Name)))
            {
                //Logging.Write(target.Name + " got DebuffRoot");
                return true;
            }
            return false;
        }

        private static WoWUnit UnitPurgeASAPResto;

        private static bool GetUnitUnitPurgeASAPResto()
        {
            UnitPurgeASAPResto = null;

            UnitPurgeASAPResto = NearbyUnFriendlyPlayers.OrderBy(HealWeight).FirstOrDefault(
                unit =>
                BasicCheck(unit) &&
                NeedPurgeASAPResto(unit) &&
                Attackable(unit, 30));

            //if (BasicCheck(UnitPurgeASAPResto))
            //{
            //    Logging.Write("Found UnitPurgeASAPResto {0}", UnitPurgeASAPResto.SafeName);
            //}

            return BasicCheck(UnitPurgeASAPResto);
        }

        private static Composite PurgeASAPResto()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.PurgeASAP &&
                HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                //SSpellManager.HasSpell("Purge") &&
                //!Me.Mounted &&
                Me.ManaPercent >= THSettings.Instance.PurgeASAPMana &&
                CanCastCheck("Purge") &&
                GetUnitUnitPurgeASAPResto(),
                new Action(
                    ret => { CastSpell("Purge", UnitPurgeASAPResto, "PurgeASAP"); })
                );
        }

        private static WoWUnit UnitPurgeNormalResto;

        private static int CountMagicBuffPurge(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return 0;
            }

            AuraCacheUpdate(target);

            return AuraCacheList.Count(
                a =>
                a.AuraCacheUnit == target.Guid &&
                a.AuraCacheAura.IsActive &&
                !a.AuraCacheAura.IsHarmful &&
                a.AuraCacheAura.Spell.DispelType == WoWDispelType.Magic);
        }

        private static bool GetUnitUnitPurgeNormalResto()
        {
            UnitPurgeNormalResto = null;

            if (InArena || InBattleground)
            {
                UnitPurgeNormalResto = NearbyUnFriendlyPlayers.OrderBy(CountFriendDPSTarget).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    CountMagicBuffPurge(unit) > 0 &&
                    Attackable(unit, 30));
            }

            if (UnitPurgeNormalResto == null)
            {
                UnitPurgeNormalResto = NearbyUnFriendlyUnits.OrderBy(CountFriendDPSTarget).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    unit.CurrentTarget != null &&
                    FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                    CountMagicBuffPurge(unit) > 0 &&
                    Attackable(unit, 30));
            }

            //if (BasicCheck(UnitPurgeNormalResto))
            //{
            //    Logging.Write("Found UnitPurgeNormalResto {0}", UnitPurgeNormalResto.SafeName);
            //}

            return BasicCheck(UnitPurgeNormalResto);
        }

        private static Composite PurgeNormal()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.PurgeNormal &&
                HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                //SSpellManager.HasSpell("Purge") &&
                //!Me.Mounted &&
                Me.ManaPercent >= THSettings.Instance.PurgeNormalMana &&
                CanCastCheck("Purge") &&
                GetUnitUnitPurgeNormalResto(),
                new Action(
                    ret => { CastSpell("Purge", UnitPurgeNormalResto, "PurgeNormal"); })
                );
        }

        private static readonly HashSet<string> NeedPurgeASAPEleEnhHS = new HashSet<string>
            {
                //Death Knight
                //"Unholy Frenzy",
                //Druid
                "Innervate",
                "Nature's Swiftness",
                "Predator's Swiftness",
                //Hunter
                "Master's Call",
                //Mage
                "Arcane Power",
                "Alter Time",
                "Ice Barrier",
                "Icy Veins",
                "Mana Shield",
                "Fingers of Frost",
                "Presence of Mind",
                //Monk
                //"Life Cocoon", Not dispellable
                "Enveloping Mist",
                //"Touch of Karma",//Thank cedricdu94. No, your healer can dispell you when you touch monk with touch of karma but you cant dispell enemy buff touch of karma
                //Paladin
                "Divine Plea",
                "Hand of Freedom",
                //"Avenging Wrath",
                "Hand of Protection",
                "Hand of Sacrifice",
                //"Sacred Shield",
                //Priest
                "Inner Focus",
                //"Fear Ward",
                "Power Infusion",
                "Power Word: Shield",
                //Rogue
                //??
                //Shaman
                //"Earth Shield",
                "Elemental Mastery",
                "Ghost Wolf",
                "Ancestral Swiftness",
                "Spiritwalker's Grace",
                //Warlock
                "Sacrifice",
                //Warrior
                //"Berserker Rage",
                //"Enrage",
                //Warlock
                //"Dark Soul: Instability", Dark Soul is not dispellable
                //"Dark Soul: Knowledge", Dark Soul is not dispellable
                //"Dark Soul: Misery", Dark Soul is not dispellable
            };

        private static bool NeedPurgeASAPEleEnh(WoWUnit target)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            if (AuraCacheList.Any(
                a => a.AuraCacheUnit == target.Guid && NeedPurgeASAPEleEnhHS.Contains(a.AuraCacheAura.Name)))
            {
                //Logging.Write(target.Name + " got DebuffRoot");
                return true;
            }
            return false;
        }

        private static Composite PurgeASAPEleEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.PurgeASAP &&
                //SSpellManager.HasSpell("Purge") &&
                //!Me.Mounted &&
                Me.ManaPercent >= THSettings.Instance.PurgeASAPMana &&
                BasicCheck(Me.CurrentTarget) &&
                Me.CurrentTarget.IsPlayer &&
                !CurrentTargetCheckInvulnerable &&
                CurrentTargetCheckIsEnemy &&
                CurrentTargetCheckDist <= 30 &&
                !CurrentTargetCheckInvulnerableMagic &&
                NeedPurgeASAPEleEnh(Me.CurrentTarget) &&
                CanCastCheck("Purge"),
                new Action(
                    ret => { CastSpell("Purge", Me.CurrentTarget, "PurgeASAPEleEnh"); })
                );
        }

        #endregion

        #region PurifySpiritMyRoot

        private static Composite PurifySpiritMyRoot()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.PurifySpiritASAP &&
                    HealWeightUnitHeal >= THSettings.Instance.UrgentHeal &&
                    //SSpellManager.HasSpell("Purify Spirit") &&
                    Me.ManaPercent > 30 &&
                    CanCastCheck("Cleanse Spirit") &&
                    DebuffRootCanCleanse(Me),
                    new Action(
                        delegate { CastSpell("Cleanse Spirit", Me, "PurifySpiritMyRoot"); }))
                );
        }

        #endregion

        #region PurifySpiritFriendlyComp

        private static WoWUnit PlayerFriendlyPurifySpirit;
        private static DateTime GetPlayerFriendlyPurifySpiritLast;

        private static bool GetPlayerFriendlyPurifySpirit()
        {
            if (GetPlayerFriendlyPurifySpiritLast > DateTime.Now)
            {
                return true;
            }

            GetPlayerFriendlyPurifySpiritLast = DateTime.Now + TimeSpan.FromMilliseconds(1000);
            PlayerFriendlyPurifySpirit = null;

            PlayerFriendlyPurifySpirit = NearbyFriendlyPlayers.OrderByDescending(CountDebuff).FirstOrDefault(
                unit =>
                BasicCheck(unit) &&
                CountDebuff(unit) >= THSettings.Instance.PurifySpiritDebuffNumber &&
                !DebuffDoNotCleanse(unit) &&
                Healable(unit));

            return true;
        }

        private static Composite PurifySpiritFriendlyComp()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.PurifySpiritDebuff &&
                    HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                    //SSpellManager.HasSpell("Purify Spirit") &&
                    Me.ManaPercent > 30 &&
                    CanCastCheck("Cleanse Spirit") &&
                    GetPlayerFriendlyPurifySpirit(),
                    new Action(
                        delegate
                            { CastSpell("Cleanse Spirit", PlayerFriendlyPurifySpirit, "PurifySpiritFriendlyComp"); }))
                );
        }

        #endregion

        #region PurifySpiritFriendlyASAPComp

        private static WoWUnit PlayerFriendlyPurifySpiritASAP;

        private static bool GetPlayerFriendlyPurifySpiritASAP()
        {
            PlayerFriendlyPurifySpiritASAP = null;

            if (!THSettings.Instance.PurifySpiritASAP || !Me.Combat)
            {
                return false;
            }

            PlayerFriendlyPurifySpiritASAP = NearbyFriendlyPlayers.FirstOrDefault(
                unit => BasicCheck(unit) &&
                        DebuffCCleanseASAP(unit) &&
                        !DebuffDoNotCleanse(unit) &&
                        Healable(unit));


            return BasicCheck(PlayerFriendlyPurifySpiritASAP);
        }

        private static Composite PurifySpiritFriendlyASAPComp()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.PurifySpiritASAP &&
                    HealWeightUnitHeal >= THSettings.Instance.UrgentHeal &&
                    CanCastCheck("Cleanse Spirit") &&
                    //SSpellManager.HasSpell("Purify Spirit") &&
                    GetPlayerFriendlyPurifySpiritASAP(),
                    //Me.CurrentMana >= SpellManager.Spells["Purify Spirit"].PowerCost &&
                    //SpellManager.Spells["Purify Spirit"].CooldownTimeLeft.TotalMilliseconds <= MyLatency,
                    new Action(delegate
                        {
                            SpellManager.StopCasting();
                            CastSpell("Cleanse Spirit", PlayerFriendlyPurifySpiritASAP, "PurifySpiritFriendlyASAPComp");
                        })));
        }

        #endregion

        #region Riptide

        private static WoWUnit UnitRiptide;

        private static bool GetUnitUnitRiptide()
        {
            UnitRiptide = null;

            if (InDungeon || InRaid || HasGlyph.Contains("63273"))
            {
                UnitRiptide = NearbyFriendlyPlayers.OrderBy(HealWeight).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    HealWeight(unit) <= THSettings.Instance.RiptideHP &&
                    !MyAura(61295, unit) &&
                    Healable(unit));
            }
            else if (InArena)
            {
                UnitRiptide = NearbyFriendlyUnits.OrderBy(HealWeight).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    (unit.IsPlayer || unit.IsPet && unit.MaxHealth > MeMaxHealth*0.5) &&
                    HealWeight(unit) <= THSettings.Instance.RiptideHP &&
                    (!MyAura(61295, unit) || HealWeight(unit) <= THSettings.Instance.PriorityHeal) &&
                    Healable(unit));
            }
            else
            {
                UnitRiptide = NearbyFriendlyPlayers.OrderBy(HealWeight).FirstOrDefault(
                    unit =>
                    BasicCheck(unit) &&
                    HealWeight(unit) <= THSettings.Instance.RiptideHP &&
                    (!MyAura(61295, unit) || HealWeight(unit) <= THSettings.Instance.PriorityHeal) &&
                    Healable(unit));
            }
            return BasicCheck(UnitRiptide);
        }

        private static Composite Riptide()
        {
            return new PrioritySelector(
                //new Decorator(
                //    ret =>
                //    THSettings.Instance.Riptide &&
                //    //SSpellManager.HasSpell("Riptide") &&
                //    (InArena || InBattleground) &&
                //    //!Me.Mounted &&
                //    HealWeightUnitHeal <= THSettings.Instance.RiptideHP &&
                //    MyAuraTimeLeft("Riptide", UnitHeal) < 13000 &&
                //    //Spiritwalker's Aegis Need Faster Heal during this
                //    (!MeHasAura(131558) || !MyAura("Tidal Waves", Me)) &&
                //    CanCastCheck("Riptide"),
                //    new Action(
                //        ret => { CastSpell("Riptide", UnitHeal, "Riptide"); })
                //    ),
                new Decorator(
                    ret =>
                    THSettings.Instance.Riptide && //Glyph of Riptide - 63273
                    //SSpellManager.HasSpell("Riptide") &&
                    //!Me.Mounted &&
                    //HasGlyph.Contains("63273") &&
                    HealWeightUnitHeal <= THSettings.Instance.RiptideHP &&
                    CanCastCheck("Riptide") &&
                    GetUnitUnitRiptide(),
                    //Spiritwalker's Aegis Need Faster Heal during this
                    //!MeHasAura(131558) ,
                    new Action(
                        ret =>
                            {
                                CastSpell("Riptide", UnitRiptide, "Riptide");
                                //Eval("CanCastCheck(Riptide)", () => CanCastCheck("Riptide"));
                                //Eval("GetUnitUnitRiptide()", () => GetUnitUnitRiptide());
                            })
                    ));
        }

        //private static Composite Riptide63273()
        //{
        //    return new Decorator(
        //        ret =>
        //        THSettings.Instance.Riptide && //Glyph of Riptide - 63273
        //        //SSpellManager.HasSpell("Riptide") &&
        //        HasGlyph.Contains("63273") &&
        //        //!Me.Mounted &&
        //        //Spiritwalker's Aegis Need Faster Heal during this
        //        (!MeHasAura(131558) || !MyAura("Tidal Waves", Me)) &&
        //        GetUnitUnitRiptide() &&
        //        CanCastCheck("Riptide"),
        //        new Action(
        //            ret => { CastSpell("Riptide", UnitRiptide, "Riptide63273"); })
        //        );
        //}

        #endregion

        #region SearingTotem

        private static DateTime LastDropTotem;

        private static Composite SearingTotem()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.SearingTotem &&
                //LastDropTotem < DateTime.Now &&
                //SSpellManager.HasSpell("Searing Totem") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(THSettings.Instance.SearingTotemDistance) &&
                !CurrentTargetCheckInvulnerableMagic &&
                Me.ManaPercent > 20 &&
                !MyTotemFireCheck(Me, 40) &&
                CanCastCheck("Searing Totem") &&
                (!MyTotemCheck("Searing Totem", Me.CurrentTarget, THSettings.Instance.SearingTotemDistance) ||
                 Me.CurrentTarget.IsPlayer &&
                 MyTotemCheck("Searing Totem", Me.CurrentTarget, THSettings.Instance.SearingTotemDistance) &&
                 GetDistance(Me.Totems[0].Unit, Me.CurrentTarget) > 10 &&
                 !Styx.WoWInternals.World.GameWorld.TraceLine(Me.Totems[0].Unit.Location, Me.CurrentTarget.Location,
                                                              GameWorld.CGWorldFrameHitFlags.HitTestLOS)),
                new Action(
                    ret =>
                        {
                            //if (Me.Totems[0] != null)
                            //{
                            //    Logging.Write("MyTotemCheck {0}", MyTotemCheck("Searing Totem", Me.CurrentTarget, 15));
                            //    Logging.Write("Range {0}",
                            //                  GetDistance(Me.Totems[0].Unit, Me.CurrentTarget));
                            //    Logging.Write("TraceLine {0}",
                            //                  Styx.WoWInternals.World.GameWorld.TraceLine(Me.Totems[0].Unit.Location,
                            //                                                              Me.CurrentTarget.Location,
                            //                                                              GameWorld.CGWorldFrameHitFlags
                            //                                                                       .HitTestLOS));
                            //}
                            //LastDropTotem = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Searing Totem", Me.CurrentTarget, "SearingTotem");
                        })
                );
        }

        private static bool NeedSearingTotemResto(int dist)
        {
            return NearbyUnFriendlyUnits.Any(
                unit =>
                BasicCheck(unit) &&
                (IsDummy(unit) ||
                 unit.CurrentTarget != null &&
                 FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                 unit.MaxHealth > MeMaxHealth*0.5) &&
                GetDistance(unit) <= dist &&
                //!DebuffCCBreakonDamage(unit) &&
                InLineOfSpellSightCheck(unit));
        }

        private static Composite SearingTotemResto()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.SearingTotem &&
                HealWeightUnitHeal >= THSettings.Instance.PriorityHeal &&
                //LastDropTotem < DateTime.Now &&
                //SSpellManager.HasSpell("Searing Totem") &&
                //!Me.Mounted &&
                !MyTotemFireCheck(Me, 60) &&
                CanCastCheck("Searing Totem") &&
                NeedSearingTotemResto(THSettings.Instance.SearingTotemDistance),
                new Action(
                    ret =>
                        {
                            //LastDropTotem = DateTime.Now + TimeSpan.FromMilliseconds(2000);
                            CastSpell("Searing Totem", Me, "SearingTotemResto");
                            //Eval("MyTotemFireCheck(Me, 60)", () => MyTotemFireCheck(Me, 60));
                            //Eval("NeedSearingTotemResto(THSettings.Instance.SearingTotemDistance) &&",
                            //     () => NeedSearingTotemResto(THSettings.Instance.SearingTotemDistance));
                        })
                )
                ;
        }

        #endregion

        #region ShamanisticRageGlyph

        private static readonly HashSet<int> Debuff63280DurationHS = new HashSet<int>
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

        private static bool Debuff63280Duration(WoWUnit target, double duration, bool LogSpell = false)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            AuraCacheUpdate(target);

            var DebuffAura = AuraCacheList.OrderByDescending(a => a.AuraCacheAura.TimeLeft).FirstOrDefault(
                a =>
                a.AuraCacheUnit == target.Guid &&
                Debuff63280DurationHS.Contains(a.AuraCacheId) &&
                a.AuraCacheAura.TimeLeft.TotalMilliseconds > duration);


            if (DebuffAura != null)
            {
                if (LogSpell)
                {
                    Logging.Write("Debuff63280Duration on {0} {1} SpellID {2} Duration: {3}",
                                  target.SafeName,
                                  DebuffAura.AuraCacheAura.Name,
                                  DebuffAura.AuraCacheId,
                                  DebuffAura.AuraCacheAura.TimeLeft.TotalMilliseconds);
                }
                return true;
            }
            return false;
        }

        private static void ShamanisticCC()
        {
            if (!THSettings.Instance.ShamanisticCC ||
                !HasGlyph.Contains("63280") ||
                LastBreakCC >= DateTime.Now ||
                SpellsCooldownCache.ContainsKey("Shamanistic Rage") ||
                //SSpellManager.HasSpell("Shamanistic Rage") ||
                Me.Mounted ||
                !Debuff63280Duration(Me, THSettings.Instance.ShamanisticCCDuration) ||
                SpellManager.Spells["Shamanistic Rage"].CooldownTimeLeft.TotalMilliseconds > MyLatency)
            {
                return;
            }
            LastBreakCC = DateTime.Now + TimeSpan.FromMilliseconds(1500);
            CastSpell("Shamanistic Rage", Me, "ShamanisticCC");
        }

        #endregion

        #region ShamanisticRage

        private static Composite ShamanisticRage()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.ShamanisticRageHP &&
                    //SSpellManager.HasSpell("Shamanistic Rage") &&
                    //!Me.Mounted &&
                    Me.Combat &&
                    HealWeightMe <= THSettings.Instance.ShamanisticRageHPHP &&
                    CanCastCheck("Shamanistic Rage", true) &&
                    HasEnemyTargettingUnit(Me, 40),
                    new Action(
                        ret =>
                            {
                                CastSpell("Shamanistic Rage", Me, "ShamanisticRageHP");
                                return RunStatus.Failure;
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.ShamanisticRageMN &&
                    //SSpellManager.HasSpell("Shamanistic Rage") &&
                    //!Me.Mounted &&
                    Me.Combat &&
                    Me.ManaPercent <= THSettings.Instance.ShamanisticRageMNMN &&
                    CanCastCheck("Shamanistic Rage", true) &&
                    HasEnemyTargettingUnit(Me, 40),
                    new Action(
                        ret =>
                            {
                                CastSpell("Shamanistic Rage", Me, "ShamanisticRageHP");
                                return RunStatus.Failure;
                            }))
                );
        }

        #endregion

        #region SpiritwalkersGrace

        private static bool ShouldSpiritwalkersGrace()
        {
            var ShouldSpiritwalkersGrace = NearbyFriendlyPlayers.FirstOrDefault(
                unit =>
                BasicCheck(unit) &&
                (TalentSort(unit) == 1 && unit.Distance <= 10 ||
                 TalentSort(unit) > 1 && TalentSort(unit) < 4 && unit.Distance <= 40 &&
                 !DebuffCC(unit) &&
                 InLineOfSpellSightCheck(unit)));

            return (BasicCheck(ShouldSpiritwalkersGrace));
        }


        private static Composite SpiritwalkersGrace()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.SpiritwalkersGrace &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.SpiritwalkersGraceHP &&
                //SSpellManager.HasSpell("Spiritwalker's Grace") &&
                //!Me.Mounted &&
                UnitHeal.Combat &&
                !UnitHeal.IsPet &&
                !Invulnerable(UnitHeal) &&
                CanCastCheck("Spiritwalker's Grace", true) &&
                ShouldSpiritwalkersGrace(),
                new Action(
                    ret =>
                        {
                            CastSpell("Spiritwalker's Grace", Me, "SpiritwalkersGrace");
                            AuraCacheUpdate(Me, true);
                            return RunStatus.Failure;
                        })
                );
        }

        #endregion

        #region SpiritLink

        private static int CountUnitSpiritLink(WoWUnit unitCenter)
        {
            //var i = 0;

            //foreach (var unit in NearbyFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        unitCenter.Location.Distance(unit.Location) <= 10)
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.SpiritLinkUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyFriendlyPlayers.Count(
                unit =>
                BasicCheck(unit) &&
                unitCenter.Location.Distance(unit.Location) <= 10);
        }

        private static DateTime SpamUntil;

        private static Composite SpiritLink()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.SpiritLink &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.SpiritLinkHP &&
                //SSpellManager.HasSpell("Spirit Link Totem") &&
                //!Me.Mounted &&
                !MyTotemCheck("Capacitor Totem", Me, 40) &&
                UnitHeal.Combat &&
                !UnitHeal.IsPet &&
                !Invulnerable(UnitHeal) &&
                (UnitHeal.Distance <= 10 || CanCastCheck("Totemic Projection")) &&
                HaveDPSTarget(UnitHeal) &&
                CanCastCheck("Spirit Link Totem") &&
                CountUnitSpiritLink(UnitHeal) >= THSettings.Instance.SpiritLinkUnit,
                new Action(
                    ret =>
                        {
                            CastSpell("Spirit Link Totem", Me, "Spirit Link Totem");
                            StyxWoW.SleepForLagDuration();
                            if ( //SSpellManager.HasSpell("Totemic Projection") &&
                                UnitHeal.Distance > 10 &&
                                CanCastCheck("Totemic Projection"))
                            {
                                SpamUntil = DateTime.Now + SpellManager.Spells["Totemic Projection"].CooldownTimeLeft +
                                            TimeSpan.FromMilliseconds(100);

                                while (SpamUntil > DateTime.Now)
                                {
                                    Logging.Write("Trying to cast Totemic Projection");
                                    CastSpell("Totemic Projection", Me, "Totemic Projection");
                                    //Logging.Write("Me.CurrentPendingCursorSpell.Id {0}", Me.CurrentPendingCursorSpell.Id);
                                    Logging.Write("ClickRemoteLocation {0}", UnitHeal.Location);
                                    SpellManager.ClickRemoteLocation(UnitHeal.Location);
                                }
                            }
                            //Eval("CountUnitSpiritLink(UnitHeal) >= THSettings.Instance.SpiritLinkUnit",
                            //() => CountUnitSpiritLink(UnitHeal) >= THSettings.Instance.SpiritLinkUnit);
                            //Eval("CanCastCheck('Spirit Link Totem')", () => CanCastCheck("Spirit Link Totem"));
                        })
                );
        }

        #endregion

        #region StoneBulwarkTotem

        private static Composite StoneBulwarkTotem()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.StoneBulwarkTotem &&
                //SSpellManager.HasSpell("Stone Bulwark Totem") &&
                //!Me.Mounted &&
                Me.Combat &&
                HealWeightMe <= THSettings.Instance.StoneBulwarkTotemHP &&
                !MyTotemCheck("Earthgrab Totem", Me, 40) &&
                !MyTotemCheck("Earth Elemental Totem", Me, 40) &&
                CanCastCheck("Stone Bulwark Totem") &&
                HasEnemyNear(Me, 35),
                new Action(
                    ret => { CastSpell("Stone Bulwark Totem", Me, "StoneBulwarkTotemHP"); })
                );
        }

        #endregion

        #region StopCastingCheck

        //Healing Surge 8004
        //Healing Wave 331
        //Healing Rain 73920
        //Greater Healing Wave 77472
        //Hex 51514
        //Chain Heal 1064
        //Lightning Bolt 403
        //Elemental Blash 117014
        //Lava Burst 15105
        //Chain Lightning 421
        private static bool HaveInterrupterRound(WoWUnit target)
        {
            return FarFriendlyPlayers.Any(
                unit =>
                BasicCheck(unit) &&
                TalentSort(unit) == 1 &&
                GetDistance(target, unit) < 10);
        }

        private void StopCastingCheck()
        {
            if (!Me.IsCasting)
            {
                return;
            }

            //if (Me.CastingSpellId != 8004 &&
            //    Me.CastingSpellId != 331 &&
            //    Me.CastingSpellId != 73920 &&
            //    Me.CastingSpellId != 77472 &&
            //    Me.CastingSpellId != 51514 &&
            //    Me.CastingSpellId != 1064 &&
            //    Me.CastingSpellId != 403 &&
            //    Me.CastingSpellId != 117014 &&
            //    Me.CastingSpellId != 15105 &&
            //    Me.CastingSpellId != 421)
            //{
            //    return;
            //}

            if (Me.CastingSpell.Id == 51514 && //Hex
                (DebuffCCDuration(LastCastUnit, 3000) ||
                 !CanHex(LastCastUnit)))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting Hex: Unit is CC/Sharpshifted");
            }

            if (UseSpecialization == 3 &&
                Me.CastingSpell.Id == 403 && //Lightning Bolt 403
                UnitHealIsValid &&
                HasGlyph.Contains("55453") && //Glyph of Telluric Currents - 55453
                Me.ManaPercent > THSettings.Instance.UrgentHeal &&
                UnitHeal.HealthPercent < THSettings.Instance.UrgentHeal)
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") +
                              "Stop Casting Telluric Currents. Someone Need Urgent Heal!");
                return;
            }

            if (UseSpecialization == 2 &&
                (InArena || InBattleground) &&
                Me.CastingSpell.Id == 403 &&
                CurrentTargetAttackable(5))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting Lightning Bolt: Enemy in Melee Range");
            }

            if (UseSpecialization == 1 &&
                (Me.CastingSpell.Id == 403 ||
                 Me.CastingSpell.Id == 421) &&
                MeHasAura(77762) &&
                Me.CurrentCastTimeLeft.TotalMilliseconds > MyLatency)
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") +
                              "Stop Casting Lightning Bolt/Chain Lightning: Lava Burst Proc");
            }

            if (UseSpecialization == 1 &&
                (InArena || InBattleground) &&
                Me.CastingSpell.Id == 117014 &&
                Me.CurrentCastTimeLeft.TotalMilliseconds > MyLatency &&
                MeHasAura(77762) &&
                HaveInterrupterRound(Me))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") +
                              "Stop Casting Elemental Blash: Lava Burst Proc and Interrupter Nearby");
            }

            if (UseSpecialization == 3 &&
                Me.CastingSpell.Id == 77472 && //"Greater Healing Wave" &&
                (LastCastUnit.HealthPercent >
                 THSettings.Instance.DoNotHealAbove ||
                 LastCastUnit.HealthPercent > THSettings.Instance.GreaterHealingWaveHP + 20))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: Save Mana Greater Healing Wave");
                return;
            }

            if (UseSpecialization == 3 &&
                Me.CastingSpell.Id == 1064 && //"Chain Heal" &&
                (LastCastUnit.HealthPercent >
                 THSettings.Instance.DoNotHealAbove ||
                 LastCastUnit.HealthPercent > THSettings.Instance.ChainHealHP + 20))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: Save Mana Chain Heal");
                return;
            }


            if (Me.CastingSpell.Id == 8004 && //"Healing Surge" &&
                (LastCastUnit.HealthPercent >
                 THSettings.Instance.DoNotHealAbove ||
                 LastCastUnit.HealthPercent > THSettings.Instance.HealingSurgeResHP + 20))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: Save Mana Healing Surge");
                return;
            }

            if (UseSpecialization == 3 &&
                !InDungeon &&
                !InRaid &&
                Me.CastingSpell.Id == 331 && //== "Healing Wave" &&
                Me.ManaPercent > 30 &&
                Me.CurrentCastTimeLeft.TotalMilliseconds > 1300 &&
                LastCastUnit.HealthPercent < THSettings.Instance.GreaterHealingWaveHP &&
                MyAura("Tidal Waves", Me) &&
                (LastCastUnit.Combat || Me.Combat))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting Healing Wave. Need Faster Heal!");
                return;
            }

            if (UseSpecialization == 3 &&
                InRaid &&
                Me.CastingSpell.Id == 331 && //Healing Wave
                (LastCastUnit.HealthPercent > THSettings.Instance.DoNotHealAbove ||
                 LastCastUnit.HealthPercent > THSettings.Instance.HealingWaveHP + 10))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: Save Mana Healing Wave");
                return;
            }

            if (UseSpecialization == 3 &&
                (InArena || InBattleground) &&
                Me.CastingSpell.Id == 331 && //Healing Wave
                LastCastUnit.HealthPercent < THSettings.Instance.DoNotHealAbove &&
                Me.CurrentCastTimeLeft.TotalMilliseconds > rnd.Next(800, 1200))
            {
                SpellManager.StopCasting();
                Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: HealingWaveBaitInterrupt");
                return;
            }

            //Stop Casting Healing Spells
            //if ((LastCastSpell == "Healing Wave" ||
            //     LastCastSpell == "Healing Surge" ||
            //     LastCastSpell == "Chain Heal" ||
            //     LastCastSpell == "Healing Rain" ||
            //     LastCastSpell == "Greater Healing Wave") &&
            //    (Me.CastingSpell.Id == 331 ||
            //     Me.CastingSpell.Id == 8004 ||
            //     Me.CastingSpell.Id == 1064 ||
            //     Me.CastingSpell.Id == 73920 ||
            //     Me.CastingSpell.Id == 77472))
            //{
            //    var HealWeightLastCastUnit = LastCastUnit.HealthPercent;

            //    if (Me.CastingSpell.Id == 77472 && //"Greater Healing Wave" &&
            //        (HealWeightLastCastUnit >=
            //         THSettings.Instance.DoNotHealAbove - THSettings.Instance.HealBalancing ||
            //         HealWeightLastCastUnit >= THSettings.Instance.GreaterHealingWaveHP + 20))
            //    {
            //        SpellManager.StopCasting();
            //        Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: Save Mana Greater Healing Wave");
            //        return;
            //    }

            //    if (UseSpecialization == 3 &&
            //        Me.CastingSpell.Id == 8004 && //SSpellManager.HasSpell("Healing Surge") &&
            //        (HealWeightLastCastUnit >=
            //         THSettings.Instance.DoNotHealAbove - THSettings.Instance.HealBalancing
            //         || HealWeightLastCastUnit >= THSettings.Instance.HealingSurgeResHP + 20))
            //    {
            //        SpellManager.StopCasting();
            //        Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: Save Mana Healing Surge");
            //        return;
            //    }

            //    if (!InDungeon &&
            //        !InRaid &&
            //        Me.CastingSpell.Id == 331 && //== "Healing Wave" &&
            //        Me.ManaPercent > 30 &&
            //        Me.CurrentCastTimeLeft.TotalMilliseconds > 1300 &&
            //        HealWeightLastCastUnit < THSettings.Instance.GreaterHealingWaveHP &&
            //        MyAura("Tidal Waves", Me) &&
            //        (LastCastUnit.Combat || Me.Combat))
            //    {
            //        SpellManager.StopCasting();
            //        Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting Healing Wave. Need Faster Heal!");
            //        return;
            //    }

            //    if (Me.CastingSpell.Id != 331 && //Healing Wave
            //        Me.CastingSpell.Id != 73920 && //Healing Rain
            //        HealWeightLastCastUnit >
            //        THSettings.Instance.DoNotHealAbove - THSettings.Instance.HealBalancing)
            //    {
            //        SpellManager.StopCasting();
            //        Logging.Write(DateTime.Now.ToString("ss:fff ") + "Stop Casting: Unit Full HP");
            //    }
            //}
        }

        #endregion

        #region Stormlash

        private static bool StormlashEnemy()
        {
            return NearbyUnFriendlyPlayers.Where(BasicCheck).Any(
                unit =>
                unit.HealthPercent <= THSettings.Instance.StormlashEnemyHP &&
                HasFriendDPSTarget(unit));
        }

        private static Composite Stormlash()
        {
            return new Decorator(
                ret =>
                (THSettings.Instance.StormlashCooldown ||
                 THSettings.Instance.StormlashBurst &&
                 THSettings.Instance.Burst ||
                 THSettings.Instance.StormlashEnemy &&
                 StormlashEnemy()) &&
                //SSpellManager.HasSpell("Stormlash Totem") &&
                //!Me.Mounted &&
                Me.Combat &&
                !MyTotemAirCheck(Me, 40) &&
                CanCastCheck("Stormlash Totem") &&
                HasEnemyNear(Me, 30),
                new Action(
                    ret => { CastSpell("Stormlash Totem", Me, "Stormlash"); })
                );
        }

        #endregion

        #region Stormstrike

        private static Composite Stormblast()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.Stormstrike &&
                //!Me.Mounted &&
                CurrentTargetAttackable(30) &&
                !CurrentTargetCheckInvulnerablePhysic &&
                //MeHasAura("Ascendence") &&
                SpellManager.HasSpell("Stormblast") &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Primal Strike"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Primal Strike", Me.CurrentTarget, "Stormblast");
                        }));
        }

        private static Composite Stormstrike()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.Stormstrike &&
                //SSpellManager.HasSpell("Primal Strike") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(5) &&
                !CurrentTargetCheckInvulnerablePhysic &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Primal Strike"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Primal Strike", Me.CurrentTarget, "PrimalStrike/Stormstrike");
                        }));
        }

        #endregion

        #region TargetMyPetTarget

        private static WoWUnit UnitAttackingMyPet;
        //61029 Prime Fire Elemental
        //61056 Prime Earth Elemental
        //15352 Greater Earth Elemental
        //15438 Greater Fire Elemental
        //29264 Spirit Wolf
        private static bool GetUnitAttackingMyPet()
        {
            UnitAttackingMyPet = null;
            UnitAttackingMyPet = NearbyUnFriendlyUnits.
                OrderByDescending(unit => unit.ThreatInfo.RawPercent).
                FirstOrDefault(
                    unit => BasicCheck(unit) &&
                            unit.Combat &&
                            unit.CurrentTarget != null &&
                            unit.CurrentTarget.IsValid &&
                            (unit.CurrentTarget == Me ||
                             unit.CurrentTarget.CreatedByUnit == Me));

            return UnitAttackingMyPet != null;
        }

        private static Composite TargetMyPetTarget()
        {
            return new Decorator(
                ret =>
                IsUsingAFKBot &&
                (Me.CurrentTarget == null ||
                 !CurrentTargetAttackable(40)) &&
                Me.Combat &&
                GetUnitAttackingMyPet(),
                new Action(delegate
                    {
                        Logging.Write(LogLevel.Diagnostic, "TargetMyPetTarget");
                        UnitAttackingMyPet.Target();
                        return RunStatus.Failure;
                    }))
                ;
        }

        #endregion

        #region TemporaryEnchantment

        private static string ImbuesIdToName(int ImbuesId)
        {
            switch (ImbuesId)
            {
                case 1:
                    return "Windfury Weapon";
                case 2:
                    return "Flametongue Weapon";
                case 3:
                    return "Frostbrand Weapon";
                case 4:
                    return "Rockbiter Weapon";
                case 5:
                    return "Earthliving Weapon";
                default:
                    return "Invalid Weapon";
            }
        }

        private static int ImbuesIdToTemporaryEnchantmentId(int ImbuesId)
        {
            switch (ImbuesId)
            {
                case 1:
                    return 283;
                case 2:
                    return 5;
                case 3:
                    return 2;
                case 4:
                    return 3021;
                case 5:
                    return 3345;
                default:
                    return 1234;
            }
        }

        //[Windfury Weapon] [Flametongue Weapon] [Frostbrand Weapon] [Rockbiter Weapon]
        private static Composite TemporaryEnchantmentEnhancement()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.WeaponMainHand > 0 &&
                    //SSpellManager.HasSpell(ImbuesIdToName(THSettings.Instance.WeaponMainHand)) &&
                    //!Me.Mounted &&
                    Me.Inventory.Equipped.MainHand != null &&
                    Me.Inventory.Equipped.MainHand.TemporaryEnchantment.Id !=
                    ImbuesIdToTemporaryEnchantmentId(THSettings.Instance.WeaponMainHand) &&
                    CanCastCheck(ImbuesIdToName(THSettings.Instance.WeaponMainHand)),
                    new Action(
                        ret =>
                            {
                                //StyxWoW.SleepForLagDuration();
                                Lua.DoString("RunMacroText(\"/click TempEnchant1\")");
                                //StyxWoW.SleepForLagDuration();
                                Lua.DoString("RunMacroText(\"/click TempEnchant2\")");
                                StyxWoW.SleepForLagDuration();
                                CastSpell(ImbuesIdToName(THSettings.Instance.WeaponMainHand), Me,
                                          "MainHand: " + ImbuesIdToName(THSettings.Instance.WeaponMainHand));
                                //StyxWoW.SleepForLagDuration();
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoWeaponImbues &&
                    UseSpecialization != 1 &&
                    UseSpecialization != 3 &&
                    //SSpellManager.HasSpell(ImbuesIdToName(THSettings.Instance.WeaponOffHand)) &&
                    //!Me.Mounted &&
                    Me.Inventory.Equipped.OffHand != null &&
                    Me.Inventory.Equipped.OffHand.TemporaryEnchantment.Id !=
                    ImbuesIdToTemporaryEnchantmentId(THSettings.Instance.WeaponOffHand) &&
                    CanCastCheck(ImbuesIdToName(THSettings.Instance.WeaponOffHand)),
                    new Action(
                        ret =>
                            {
                                if (Me.Inventory.Equipped.OffHand.TemporaryEnchantment.Id == 0)
                                {
                                    StyxWoW.SleepForLagDuration();
                                    CastSpell(ImbuesIdToName(THSettings.Instance.WeaponOffHand), Me,
                                              "OffHand: " + ImbuesIdToName(THSettings.Instance.WeaponOffHand));
                                    StyxWoW.SleepForLagDuration();
                                }
                                else
                                {
                                    StyxWoW.SleepForLagDuration();
                                    Lua.DoString("RunMacroText(\"/click TempEnchant1\")");
                                    StyxWoW.SleepForLagDuration();
                                    Lua.DoString("RunMacroText(\"/click TempEnchant2\")");
                                    StyxWoW.SleepForLagDuration();
                                }
                            }))
                );
        }

        #endregion

        #region Thunderstorm

        private static double CountUnitThunderstorm()
        {
            //var i = 0;

            //foreach (var unit in NearbyUnFriendlyUnits)
            //{
            //    if (BasicCheck(unit) &&
            //        (IsDummy(unit) ||
            //         unit.CurrentTarget != null &&
            //         FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
            //         unit.MaxHealth > MeMaxHealth*0.5) &&
            //        GetDistance(unit) <= 10)
            //    {
            //        i = i + 1;
            //    }

            //    if (i >= THSettings.Instance.ThunderstormUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyUnFriendlyUnits.Count(
                unit =>
                BasicCheck(unit) &&
                (IsDummy(unit) ||
                 unit.CurrentTarget != null &&
                 FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                 unit.MaxHealth > MeMaxHealth*0.5) &&
                GetDistance(unit) <= 10);
        }

        private static Composite Thunderstorm()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Thunderstorm &&
                    CanCastCheck("Thunderstorm") &&
                    //SSpellManager.HasSpell("Thunderstorm") &&
                    //!Me.Mounted &&
                    CountUnitThunderstorm() >= THSettings.Instance.ThunderstormUnit,
                    new Action(
                        ret => { CastSpell("Thunderstorm", Me, "Thunderstorm"); })
                    ));
        }

        private static double CountUnitThunderstormMelee()
        {
            //var i = 0;

            //foreach (var unit in NearbyUnFriendlyPlayers)
            //{
            //    if (BasicCheck(unit) &&
            //        TalentSort(unit) < 2 &&
            //        GetDistance(unit) <= 10 &&
            //        !DebuffCC(unit) &&
            //        !DebuffRoot(unit) &&
            //        !unit.IsStealthed)
            //    {
            //        i = i + 1;
            //    }
            //    if (i >= THSettings.Instance.ThunderstormMeleeUnit)
            //    {
            //        break;
            //    }
            //}

            //return i;

            return NearbyUnFriendlyPlayers.Count(
                unit =>
                BasicCheck(unit) &&
                TalentSort(unit) < 2 &&
                GetDistance(unit) <= 10 &&
                !unit.IsStealthed &&
                !DebuffCC(unit) &&
                !DebuffRoot(unit));
        }

        private static Composite ThunderstormMelee()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.ThunderstormMelee &&
                    CanCastCheck("Thunderstorm") &&
                    //SSpellManager.HasSpell("Thunderstorm") &&
                    //!Me.Mounted &&
                    CountUnitThunderstormMelee() >= THSettings.Instance.ThunderstormMeleeUnit,
                    new Action(
                        ret => { CastSpell("Thunderstorm", Me, "ThunderstormMelee"); })
                    ));
        }

        private static Composite ThunderstormMana()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ThunderstormMana &&
                //SSpellManager.HasSpell("Thunderstorm") &&
                //!Me.Mounted &&
                Me.Combat &&
                Me.ManaPercent <= THSettings.Instance.ThunderstormManaMN &&
                CanCastCheck("Thunderstorm"),
                new Action(
                    ret => { CastSpell("Thunderstorm", Me, "ThunderstormMana"); })
                );
        }

        private static WoWUnit UnitThuderstormInterrupt;

        private static bool GetUnitThuderstormInterrupt()
        {
            UnitThuderstormInterrupt = null;

            if (InBattleground || InArena)
            {
                UnitThuderstormInterrupt = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => BasicCheck(unit) &&
                            (THSettings.Instance.InterruptAll ||
                             THSettings.Instance.InterruptTarget &&
                             Me.CurrentTarget != null &&
                             unit == Me.CurrentTarget ||
                             THSettings.Instance.InterruptFocus &&
                             Me.FocusedUnit != null &&
                             unit == Me.FocusedUnit) &&
                            //FacingOverride(unit) &&
                            InterruptCheck(unit, THSettings.Instance.ThunderstormCastMs + MyLatency + 1000, false) &&
                            //Attackable(unit, 30));
                            Attackable(unit, 10));
            }
                //PvE Search
            else
            {
                UnitThuderstormInterrupt = NearbyUnFriendlyUnits.FirstOrDefault(
                    unit => BasicCheck(unit) &&
                            (THSettings.Instance.InterruptAll ||
                             THSettings.Instance.InterruptTarget &&
                             Me.CurrentTarget != null &&
                             unit == Me.CurrentTarget ||
                             THSettings.Instance.InterruptFocus &&
                             Me.FocusedUnit != null &&
                             unit == Me.FocusedUnit) &&
                            unit.Combat &&
                            unit.CurrentTarget != null &&
                            //FacingOverride(unit) &&
                            FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                            InterruptCheck(unit, THSettings.Instance.ThunderstormCastMs + MyLatency + 1000, false) &&
                            //Attackable(unit, 30));
                            Attackable(unit, 10));
            }

            return BasicCheck(UnitThuderstormInterrupt);
        }

        private static Composite ThunderstormCast()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ThunderstormCast &&
                LastInterrupt < DateTime.Now &&
                //SSpellManager.HasSpell("Thunderstorm") &&
                //!Me.Mounted &&
                Me.Combat &&
                CanCastCheck("Thunderstorm") &&
                GetUnitThuderstormInterrupt(),
                new Action(
                    ret =>
                        {
                            LastInterrupt = DateTime.Now + TimeSpan.FromMilliseconds(1500);
                            CastSpell("Thunderstorm", Me, "ThunderstormCast");
                        })
                );
        }

        #endregion

        #region Totem

        //Air Totem
        //Capacitor Totem 61245 
        //Grounding Totem 5925
        //Stormlash Totem 32002
        //Windwalk Totem 59717
        //Earth Totem
        //Earthbind Totem 2630
        //Earthgrab Totem 60561
        //Earth Elemental Totem 15430
        //Tremor Totem 5913
        //Stone Bulwark Totem 59715
        //Fire Totem
        //Fire Elemental Totem 15439
        //Magma Totem 5929
        //Searing Totem 2523
        //Water Totem
        //Healing Stream Totem 3527
        //Spirit Link Totem 53006
        //Mana Tide Totem 10467
        //Healing Tide Totem 59764 

        private static int TotemNametoEntry(string totemName)
        {
            switch (totemName)
            {
                case "Capacitor Totem":
                    return 61245;
                case "Grounding Totem":
                    return 5925;
                case "Stormlash Totem":
                    return 32002;
                case "Windwalk Totem":
                    return 59717;
                case "Earthbind Totem":
                    return 2630;
                case "Earthgrab Totem":
                    return 60561;
                case "Earth Elemental Totem":
                    return 15430;
                case "Tremor Totem Totem":
                    return 5913;
                case "Stone Bulwark Totem":
                    return 59715;
                case "Fire Elemental Totem":
                    return 15439;
                case "Searing Totem":
                    return 2523;
                case "Magma Totem":
                    return 5929;
                case "Spirit Link Totem":
                    return 53006;
                case "Mana Tide Totem":
                    return 10467;
                case "Healing Tide Totem":
                    return 59764;
                case "Healing Stream Totem":
                    return 3527;
                default:
                    return 0;
            }
        }

        private static bool MyTotemCheck(string totemName, WoWUnit target, int distance)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            for (var i = 0; i < 4; i++)
            {
                if (Me.Totems[i].Unit != null &&
                    Me.Totems[i].Unit.Entry == TotemNametoEntry(totemName) &&
                    GetDistance(Me.Totems[i].Unit, target) <= distance)
                {
                    return true;
                }
            }

            return false;

            //var totem = FarFriendlyUnits.FirstOrDefault(
            //    unit =>
            //    BasicCheck(unit) &&
            //    unit.Entry == TotemNametoEntry(totemName) &&
            //    unit.CreatedByUnit == Me);

            //if (totem == null)
            //{
            //    return false;
            //}

            ////Logging.Write("Name {0} Get Distance {1} Distance {2}", totem.Name, GetDistance(totem, target),
            ////              totem.Location.Distance(target.Location));
            //return GetDistance(totem, target) <= distance;
        }

        private static bool MyTotemAirCheck(WoWUnit target, int distance)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            return Me.Totems[3].Unit != null &&
                   GetDistance(Me.Totems[3].Unit, target) <= distance;

            //var totem = FarFriendlyUnits.OrderByDescending(GetDistance).FirstOrDefault(
            //    unit =>
            //    BasicCheck(unit) &&
            //    (unit.Entry == 5925 ||
            //     unit.Entry == 61245 ||
            //     unit.Entry == 32002 ||
            //     unit.Entry == 59717) &&
            //    unit.CreatedByUnit == Me);

            //if (totem == null)
            //{
            //    return false;
            //}

            //return GetDistance(totem, target) <= distance;
        }

        private static bool MyTotemEarthCheck(WoWUnit target, int distance)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            return Me.Totems[1].Unit != null &&
                   GetDistance(Me.Totems[1].Unit, target) <= distance;

            //var totem = FarFriendlyUnits.OrderByDescending(GetDistance).FirstOrDefault(
            //    unit =>
            //    BasicCheck(unit) &&
            //    (unit.Entry == 2630 ||
            //     unit.Entry == 60561 ||
            //     unit.Entry == 15430 ||
            //     unit.Entry == 59715 ||
            //     unit.Entry == 5913) &&
            //    unit.CreatedByUnit == Me);

            //if (totem == null)
            //{
            //    return false;
            //}

            //return GetDistance(totem, target) <= distance;
        }

        private static bool MyTotemFireCheck(WoWUnit target, int distance)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            return Me.Totems[0].Unit != null &&
                   GetDistance(Me.Totems[0].Unit, target) <= distance;

            //var totem = FarFriendlyUnits.OrderByDescending(GetDistance).FirstOrDefault(
            //    unit =>
            //    BasicCheck(unit) &&
            //    (unit.Entry == 15439 ||
            //     unit.Entry == 5929 ||
            //     unit.Entry == 2523) &&
            //    unit.CreatedByUnit == Me);

            //if (totem == null)
            //{
            //    return false;
            //}

            //return GetDistance(totem, target) <= distance;
        }

        private static bool MyTotemWaterCheck(WoWUnit target, int distance)
        {
            if (!BasicCheck(target))
            {
                return false;
            }

            return Me.Totems[2].Unit != null &&
                   GetDistance(Me.Totems[2].Unit, target) <= distance;

            //var totem = FarFriendlyUnits.OrderByDescending(GetDistance).FirstOrDefault(
            //    unit =>
            //    BasicCheck(unit) &&
            //    (unit.Entry == 3527 ||
            //     unit.Entry == 53006 ||
            //     unit.Entry == 10467 ||
            //     unit.Entry == 59764) &&
            //    unit.CreatedByUnit == Me);

            //if (totem == null)
            //{
            //    return false;
            //}

            //return GetDistance(totem, target) <= distance;
        }

        //private static WoWUnit MyTotem(string totemName)
        //{
        //    var totem = FarFriendlyUnits.FirstOrDefault(
        //        unit =>
        //        BasicCheck(unit) &&
        //        unit.Entry == TotemNametoEntry(totemName) &&
        //        unit.CreatedByUnit == Me);

        //    return totem;
        //}

        #endregion

        #region TotemicRestoration

        private static int CountEnemyNearTotem(WoWUnit unitCenter, float distance)
        {
            return FarUnFriendlyUnits.Count(
                unit =>
                BasicCheck(unit) &&
                GetDistance(unitCenter, unit) <= distance);
        }

        private static void TotemicRestoration()
        {
            //return;

            if (!SpellManager.HasSpell("Totemic Restoration"))
            {
                return;
            }

            for (var i = 0; i < 4; i++)
            {
                if (Me.Totems[i].Spell == null)
                {
                    continue;
                }

                if (Me.Totems[i].Spell != null && Me.Totems[i].Spell.Id == 8143 &&
                    Me.Totems[i].StartTime + TimeSpan.FromMilliseconds(1000) < DateTime.Now &&
                    !NeedTremor(Me, THSettings.Instance.TremorDuration))
                {
                    Logging.Write(LogLevel.Diagnostic, "Totemic Restoration Remove Tremor Totem, No One Feared");
                    //Logging.Write(LogLevel.Diagnostic,
                    //              "click TotemFrameTotem" + 1 + " RightButton");
                    SpellsCooldownCache.Clear();
                    Lua.DoString("RunMacroText(\"/click TotemFrameTotem" + 1 + " RightButton\")");
                }

                if (Me.Totems[i].Spell != null && Me.Totems[i].Spell.Id == 42734 && MeHasAura(114893))
                {
                    Logging.Write(LogLevel.Diagnostic, "Totemic Restoration Remove Stone Bulwark Totem, You Got Shield");
                    //Logging.Write(LogLevel.Diagnostic,
                    //              "click TotemFrameTotem" + 1 + " RightButton");
                    SpellsCooldownCache.Clear();
                    Lua.DoString("RunMacroText(\"/click TotemFrameTotem" + 1 + " RightButton\")");
                }

                if (Me.Totems[i].Spell != null && Me.Totems[i].Spell.Id == 8177 && FarUnFriendlyUnits.Count < 1)
                {
                    Logging.Write(LogLevel.Diagnostic, "Totemic Restoration Remove Grounding Totem, No Enemy Nearby");
                    //Logging.Write(LogLevel.Diagnostic,
                    //              "click TotemFrameTotem" + 4 + " RightButton");
                    SpellsCooldownCache.Clear();
                    Lua.DoString("RunMacroText(\"/click TotemFrameTotem" + 4 + " RightButton\")");
                }

                //if (Me.Totems[i].Spell != null && Me.Totems[i].Spell.Id == 2484 &&
                //    Me.Totems[i].StartTime + TimeSpan.FromMilliseconds(5000) < DateTime.Now &&
                //    CountEnemyNearTotem(MyTotem("Earthbind Totem"), THSettings.Instance.EarthbindDistance + 20) < 1)
                //{
                //    Logging.Write(LogLevel.Diagnostic, "Totemic Restoration Remove Earthbind Totem, No Enemy Nearby");
                //    //Logging.Write(LogLevel.Diagnostic,
                //    //              "click TotemFrameTotem" + 1 + " RightButton");
                //    Lua.DoString("RunMacroText(\"/click TotemFrameTotem" + 1 + " RightButton\")");
                //}

                //if (Me.Totems[i].Spell != null && Me.Totems[i].Spell.Id == 51485 &&
                //    Me.Totems[i].StartTime + TimeSpan.FromMilliseconds(5000) < DateTime.Now &&
                //    CountEnemyNearTotem(MyTotem("Earthgrab Totem"),
                //                        THSettings.Instance.EarthbindDistance + 20) < 1)
                //{
                //    Logging.Write(LogLevel.Diagnostic,
                //                  "Totemic Restoration Remove Earthgrab Totem, No Enemy Nearby or all Rooted");
                //    //Logging.Write(LogLevel.Diagnostic,
                //    //              "click TotemFrameTotem" + 1 + " RightButton");
                //    Lua.DoString("RunMacroText(\"/click TotemFrameTotem" + 1 + " RightButton\")");
                //}
            }
        }

        #endregion

        #region Tremor

        private static readonly HashSet<int> DebuffNeedTremor = new HashSet<int>
            {
                5782, //Fear
                118699, //Fear
                130616, //Fear (Glyph of Fear)
                2637, //Hibernate
                5484, //Howl of Terror
                113056, //Intimidating Roar [Cowering in fear] (Warrior)
                113004, //Intimidating Roar [Fleeing in fear] (Warrior)
                5246, //Intimidating Shout (aoe)
                20511, //Intimidating Shout (targeted)
                126246, //Lullaby (Crane)
                115268, //Mesmerize (Shivarra)
                6789, //Mortal Coil
                64044, //Psychic Horror
                8122, //Psychic Scream
                113792, //Psychic Terror (Psyfiend)
                132412, //Seduction (Grimoire of Sacrifice)
                6358, //Seduction (Succubus)
                30283, //Shadowfury
                132168, //Shockwave
                87204, //Sin and Punishment
                104045, //Sleep (Metamorphosis)
            };

        private static bool NeedTremor(WoWUnit target, int duration, bool writelog = true)
        {
            //if (!BasicCheck(target))
            //{
            //    return false;
            //}

            //foreach (var aura in target.GetAllAuras())
            //{
            //    if (aura.TimeLeft.TotalMilliseconds < duration ||
            //        aura.SpellId == 30283 || // shadowfury
            //        aura.SpellId == 132168) // shadowfury
            //    {
            //        continue;
            //    }

            //    if (DebuffNeedTremor.Contains(aura.SpellId) ||
            //        aura.Spell.Mechanic == WoWSpellMechanic.Asleep ||
            //        aura.Spell.Mechanic == WoWSpellMechanic.Charmed ||
            //        aura.Spell.Mechanic == WoWSpellMechanic.Fleeing ||
            //        aura.Spell.Mechanic == WoWSpellMechanic.Horrified)
            //    {
            //        if (writelog)
            //        {
            //            Logging.Write("NeedTremor {0} {1} ID: {2}", target.SafeName, aura.Name,
            //                          aura.SpellId);
            //        }
            //        return true;
            //    }
            //}
            //return false;

            AuraCacheUpdate(target);

            foreach (var aura in AuraCacheList)
            {
                if (aura.AuraCacheUnit != target.Guid ||
                    aura.AuraCacheAura.TimeLeft.TotalMilliseconds < duration ||
                    aura.AuraCacheId == 30283 || // shadowfury
                    aura.AuraCacheId == 132168) // shadowfury
                {
                    continue;
                }

                if (DebuffNeedTremor.Contains(aura.AuraCacheId) ||
                    aura.AuraCacheAura.Spell.Mechanic == WoWSpellMechanic.Asleep ||
                    aura.AuraCacheAura.Spell.Mechanic == WoWSpellMechanic.Charmed ||
                    aura.AuraCacheAura.Spell.Mechanic == WoWSpellMechanic.Fleeing ||
                    aura.AuraCacheAura.Spell.Mechanic == WoWSpellMechanic.Horrified)
                {
                    if (writelog)
                    {
                        Logging.Write("NeedTremor {0} {1} ID: {2}", target.SafeName, aura.AuraCacheAura.Name,
                                      aura.AuraCacheId);
                    }
                    return true;
                }
            }
            return false;
        }

        private static bool GetUnitNeedTremor()
        {
            return NearbyFriendlyPlayers.Any(
                unit =>
                BasicCheck(unit) &&
                unit.Distance <= 29 &&
                (THSettings.Instance.TremorMe && unit == Me ||
                 THSettings.Instance.TremorHealer && TalentSort(unit) > 3 ||
                 THSettings.Instance.TremorDPS && TalentSort(unit) < 4) &&
                NeedTremor(unit, THSettings.Instance.TremorDuration));
        }

        private static void Tremor()
        {
            if (!THSettings.Instance.Tremor ||
                LastBreakCC > DateTime.Now ||
                Me.Mounted ||
                !CanCastCheck("Tremor Totem") ||
                !GetUnitNeedTremor())
            {
                return;
            }
            LastBreakCC = DateTime.Now + TimeSpan.FromMilliseconds(1500);
            CastSpell("Tremor Totem", Me, "Tremor");
        }

        #endregion

        #region UnleashElements

        private static Composite UnleashElementsEle()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.UnleashElementsEle &&
                //SSpellManager.HasSpell("Unleash Elements") &&
                (InArena || InBattleground) &&
                //!Me.Mounted &&
                IsMoving(Me) &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Unleash Elements"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Unleash Elements", Me.CurrentTarget, "UnleashElementsEle");
                        })
                );
        }

        private static Composite UnleashElementsEnh()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.UnleashElementsEnh &&
                //SSpellManager.HasSpell("Unleash Elements") &&
                //SSpellManager.HasSpell("Unleashed Fury") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Unleash Elements"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Unleash Elements", Me.CurrentTarget, "UnleashElementsEnh");
                        })
                );
        }

        private static Composite UnleashElementsEnhSnare()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.UnleashElementsEnh &&
                //SSpellManager.HasSpell("Unleash Elements") &&
                //SSpellManager.HasSpell("Unleashed Fury") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                Me.Inventory.Equipped.OffHand != null &&
                Me.Inventory.Equipped.OffHand.TemporaryEnchantment.Id == 2 && //Frostband
                !InvulnerableRootandSnare(Me.CurrentTarget) &&
                CanCastCheck("Unleash Elements"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Unleash Elements", Me.CurrentTarget, "UnleashElementsEnhSnare");
                        })
                );
        }

        private static Composite UnleashElementsEnhUnleashedFury()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.UnleashElementsEnh &&
                //SSpellManager.HasSpell("Unleashed Fury") &&
                //!Me.Mounted &&
                CurrentTargetAttackable(40) &&
                !CurrentTargetCheckInvulnerableMagic &&
                FacingOverride(Me.CurrentTarget) &&
                CanCastCheck("Unleash Elements"),
                new Action(
                    ret =>
                        {
                            SafelyFacingTarget(Me.CurrentTarget);
                            CastSpell("Unleash Elements", Me.CurrentTarget, "UnleashElementsEnhUnleashedFury");
                        })
                );
        }

        private static Composite UnleashElementsRes()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.UnleashElementsRes &&
                UnitHealIsValid &&
                HealWeightUnitHeal <= THSettings.Instance.UnleashElementsResHP &&
                //SSpellManager.HasSpell("Unleash Elements") &&
                //!Me.Mounted &&
                CanCastCheck("Unleash Elements"),
                new Action(
                    ret => { CastSpell("Unleash Elements", UnitHeal, "UnleashElementsRes"); })
                );
        }

        private static void UnleaseElementEarthLiving(bool Enabled, WoWUnit UnitToUnleaseElementEarthLiving)
        {
            if (Enabled &&
                Me.Inventory.Equipped.MainHand != null &&
                Me.Inventory.Equipped.MainHand.TemporaryEnchantment.Id == 3345 &&
                //SSpellManager.HasSpell("Unleash Elements") &&
                BasicCheck(UnitToUnleaseElementEarthLiving) &&
                !MeHasAura(73685) && //Unleash Life
                CanCastCheck("Unleash Elements"))
            {
                CastSpell("Unleash Elements", UnitToUnleaseElementEarthLiving, "Unleash Elements before CH/GHW/HR/HS");
            }
        }

        #endregion

        #region UseRacial

        private static Composite UseRacial()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoRacial &&
                    //SSpellManager.HasSpell("Every Man for Himself") &&
                    Me.Combat &&
                    !MeHasAura("Sap") &&
                    !MeHasAura("Scatter Shot") &&
                    CanCastCheck("Every Man for Himself", true) &&
                    DebuffCCDuration(Me, 4000),
                    new Action(delegate
                        {
                            if (THSettings.Instance.AutoTarget && Me.CurrentTarget == null &&
                                MyLastTarget != null &&
                                MyLastTarget.IsValid)
                            {
                                MyLastTarget.Target();
                            }

                            Logging.Write("Use: Every Man for Himself");
                            CastSpell("Every Man for Himself", Me);
                            return RunStatus.Failure;
                        })),
                //Stoneform
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoRacial && HealWeightMe < THSettings.Instance.UrgentHeal &&
                    //SSpellManager.HasSpell("Stoneform") &&
                    Me.Combat &&
                    CanCastCheck("Stoneform", true),
                    new Action(delegate
                        {
                            {
                                //Logging.Write("Stoneform");
                                CastSpell("Stoneform", Me);
                            }
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoRacial &&
                    HealWeightMe < THSettings.Instance.UrgentHeal &&
                    //SSpellManager.HasSpell("Gift of the Naaru") &&
                    Me.Combat &&
                    CanCastCheck("Gift of the Naaru", true),
                    new Action(delegate
                        {
                            {
                                //Logging.Write("Gift of the Naaru");
                                CastSpell("Gift of the Naaru", Me);
                            }
                            return RunStatus.Failure;
                        })) //,
                //new Decorator(
                //    ret =>
                //    THSettings.Instance.AutoRacial && Me.ManaPercent < THSettings.Instance.PriorityHeal &&
                //    //SSpellManager.HasSpell("Arcane Torrent") && //SSpellManager.HasSpell("Holy Insight") &&
                //    Me.Combat &&
                //    (InDungeon || InRaid) &&
                //    CanCastCheck("Arcane Torrent", true),
                //    new Action(delegate
                //        {
                //            {
                //                Logging.Write("Arcane Torrent");
                //                CastSpell("Arcane Torrent", Me);
                //            }
                //            return RunStatus.Failure;
                //        }))
                );
        }

        #endregion

        #region UseTrinket

        private static DateTime LastBreakCC;

        private static Composite UseTrinket()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 1 &&
                    //LastCastSpell != "STrinket1" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Cooldown");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 2 &&
                    //LastCastSpell != "STrinket1" &&
                    Me.Combat &&
                    CurrentTargetAttackable(30) &&
                    Me.CurrentTarget.IsBoss &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Cooldown (Boss Only");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 3 &&
                    //LastCastSpell != "STrinket1" &&
                    THSettings.Instance.Burst &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Burst Mode");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 4 &&
                    LastBreakCC < DateTime.Now &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    !MeHasAura("Scatter Shot") &&
                    //!MeHasAura("Cheap Shot") &&
                    !MeHasAura("Sap") &&
                    DebuffCCDuration(Me, 3000, true) &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            //if (CanCastCheck("Tremor Totem") && NeedTremor(Me, 3000))
                            //{
                            //    CastSpell("Tremor Totem", Me, "Tremor Totem instead of Trinket");
                            //}
                            //else
                            //{
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Lose Control");
                            LastCastSpell = "STrinket1";
                            //}
                            LastBreakCC = DateTime.Now + TimeSpan.FromMilliseconds(1500);
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 5 &&
                    //LastCastSpell != "STrinket1" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    HealWeightMe < THSettings.Instance.Trinket1HP &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 6 &&
                    UnitHealIsValid &&
                    HealWeightUnitHeal <= THSettings.Instance.Trinket1HP &&
                    //LastCastSpell != "STrinket1" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Friendly Unit Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 7 &&
                    //LastCastSpell != "STrinket1" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    CurrentTargetAttackable(20) &&
                    !Me.CurrentTarget.IsPet &&
                    Me.CurrentTarget.HealthPercent <= THSettings.Instance.Trinket1HP &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Enemy Unit Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 8 &&
                    //LastCastSpell != "STrinket1" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    Me.ManaPercent <= THSettings.Instance.Trinket1HP &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Low Mana");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket1 == 9 &&
                    //LastCastSpell != "STrinket1" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket1 != null &&
                    BuffBurst(Me) &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket1),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                          " on Using Cooldown");
                            StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                            LastCastSpell = "STrinket1";
                            return RunStatus.Failure;
                        })),
                //Trinket 2
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 1 &&
                    //LastCastSpell != "STrinket2" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Cooldown");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 2 &&
                    //LastCastSpell != "STrinket2" &&
                    Me.Combat &&
                    CurrentTargetAttackable(30) &&
                    Me.CurrentTarget.IsBoss &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Cooldown (Boss Only");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 3 &&
                    //LastCastSpell != "STrinket2" &&
                    THSettings.Instance.Burst &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Burst Mode");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 4 &&
                    //LastCastSpell != "STrinket2" &&
                    LastBreakCC < DateTime.Now &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    !MeHasAura("Scatter Shot") &&
                    !MeHasAura("Sap") &&
                    DebuffCCDuration(Me, 3000, true) &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            //if (CanCastCheck("Tremor Totem") && NeedTremor(Me, 3000))
                            //{
                            //    CastSpell("Tremor Totem", Me, "Tremor Totem instead of Trinket");
                            //}
                            //else
                            //{
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Lose Control");
                            LastCastSpell = "STrinket2";
                            //}
                            LastBreakCC = DateTime.Now + TimeSpan.FromMilliseconds(1500);
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 5 &&
                    //LastCastSpell != "STrinket2" &&.0.
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    HealWeightMe <= THSettings.Instance.Trinket2HP &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 6 &&
                    UnitHealIsValid &&
                    HealWeightUnitHeal <= THSettings.Instance.Trinket2HP &&
                    //LastCastSpell != "STrinket2" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Friendly Unit Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 7 &&
                    //LastCastSpell != "STrinket2" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    CurrentTargetAttackable(20) &&
                    !Me.CurrentTarget.IsPet &&
                    Me.CurrentTarget.HealthPercent <= THSettings.Instance.Trinket2HP &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Enemy Unit Low HP");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 8 &&
                    //LastCastSpell != "STrinket2" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    Me.ManaPercent <= THSettings.Instance.Trinket2HP &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Low Mana");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        })),
                new Decorator(
                    ret =>
                    THSettings.Instance.Trinket2 == 9 &&
                    //LastCastSpell != "STrinket2" &&
                    Me.Combat &&
                    Me.Inventory.Equipped.Trinket2 != null &&
                    BuffBurst(Me) &&
                    CanUseCheck(Me.Inventory.Equipped.Trinket2),
                    new Action(delegate
                        {
                            Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                          " on Using Cooldown");
                            StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                            LastCastSpell = "STrinket2";
                            return RunStatus.Failure;
                        }))
                )
                ;
        }

        private static void UseTrinketVoid()
        {
            if (!Me.Combat || Me.Mounted)
            {
                return;
            }

            switch (THSettings.Instance.Trinket1)
            {
                case 1:
                    if (Me.Inventory.Equipped.Trinket1 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Cooldown");
                    }
                    break;
                case 2:
                    if (CurrentTargetAttackable(30) &&
                        Me.CurrentTarget.IsBoss &&
                        Me.Inventory.Equipped.Trinket1 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Cooldown (Boss Only");
                    }
                    break;
                case 3:
                    if (THSettings.Instance.Burst &&
                        Me.Inventory.Equipped.Trinket1 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Burst Mode");
                    }
                    break;
                case 4:
                    if (LastBreakCC < DateTime.Now &&
                        Me.Inventory.Equipped.Trinket1 != null &&
                        !MeHasAura("Scatter Shot") &&
                        //!MeHasAura("Cheap Shot") &&
                        !MeHasAura("Sap") &&
                        DebuffCCDuration(Me, 3000, true) &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Lose Control");
                    }
                    break;
                case 5:
                    if (Me.Inventory.Equipped.Trinket1 != null &&
                        HealWeightMe < THSettings.Instance.Trinket1HP &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Low HP");
                    }
                    break;
                case 6:
                    if (UnitHealIsValid &&
                        HealWeightUnitHeal <= THSettings.Instance.Trinket1HP &&
                        Me.Combat &&
                        Me.Inventory.Equipped.Trinket1 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Friendly Unit Low HP");
                    }
                    break;
                case 7:
                    if (Me.Inventory.Equipped.Trinket1 != null &&
                        CurrentTargetAttackable(20) &&
                        !Me.CurrentTarget.IsPet &&
                        Me.CurrentTarget.HealthPercent <= THSettings.Instance.Trinket1HP &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Enemy Unit Low HP");
                    }
                    break;
                case 8:
                    if (Me.Inventory.Equipped.Trinket1 != null &&
                        Me.ManaPercent <= THSettings.Instance.Trinket1HP &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Low Mana");
                    }
                    break;
                case 9:
                    if (Me.Combat &&
                        Me.Inventory.Equipped.Trinket1 != null &&
                        BuffBurst(Me) &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket1))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket1.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket1.Name +
                                      " on Using Cooldown");
                    }
                    break;
            }

            switch (THSettings.Instance.Trinket2)
            {
                case 1:
                    if (Me.Inventory.Equipped.Trinket2 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Cooldown");
                    }
                    break;
                case 2:
                    if (CurrentTargetAttackable(30) &&
                        Me.CurrentTarget.IsBoss &&
                        Me.Inventory.Equipped.Trinket2 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Cooldown (Boss Only");
                    }
                    break;
                case 3:
                    if (THSettings.Instance.Burst &&
                        Me.Inventory.Equipped.Trinket2 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Burst Mode");
                    }
                    break;
                case 4:
                    if (LastBreakCC < DateTime.Now &&
                        Me.Inventory.Equipped.Trinket2 != null &&
                        !MeHasAura("Scatter Shot") &&
                        //!MeHasAura("Cheap Shot") &&
                        !MeHasAura("Sap") &&
                        DebuffCCDuration(Me, 3000, true) &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Lose Control");
                    }
                    break;
                case 5:
                    if (Me.Combat &&
                        Me.Inventory.Equipped.Trinket2 != null &&
                        HealWeightMe < THSettings.Instance.Trinket2HP &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Low HP");
                    }
                    break;
                case 6:
                    if (UnitHealIsValid &&
                        HealWeightUnitHeal <= THSettings.Instance.Trinket2HP &&
                        Me.Inventory.Equipped.Trinket2 != null &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Friendly Unit Low HP");
                    }
                    break;
                case 7:
                    if (Me.Inventory.Equipped.Trinket2 != null &&
                        CurrentTargetAttackable(20) &&
                        !Me.CurrentTarget.IsPet &&
                        Me.CurrentTarget.HealthPercent <= THSettings.Instance.Trinket2HP &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Enemy Unit Low HP");
                    }
                    break;
                case 8:
                    if (Me.Inventory.Equipped.Trinket2 != null &&
                        Me.ManaPercent <= THSettings.Instance.Trinket2HP &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Low Mana");
                    }
                    break;
                case 9:
                    if (Me.Inventory.Equipped.Trinket2 != null &&
                        BuffBurst(Me) &&
                        CanUseCheck(Me.Inventory.Equipped.Trinket2))
                    {
                        StyxWoW.Me.Inventory.Equipped.Trinket2.Use();
                        Logging.Write("Activate " + StyxWoW.Me.Inventory.Equipped.Trinket2.Name +
                                      " on Using Cooldown");
                    }
                    break;
            }
        }

        #endregion

        #region UseProfession

        private static Composite UseProfession()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.ProfBuff == 1 &&
                //LastCastSpell != "SProfBuff" &&
                Me.Combat &&
                !Me.Mounted,
                new PrioritySelector(
                    //Engineering
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 1 &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Cooldown");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 2 &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        CurrentTargetAttackable(30) &&
                        Me.CurrentTarget.IsBoss &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Cooldown (Boss Only");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 3 &&
                        //LastCastSpell != "SProfBuff" &&
                        THSettings.Instance.Burst &&
                        Me.Inventory.Equipped.Hands != null &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Burst Mode");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 4 &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        CurrentTargetAttackable(30) &&
                        DebuffCCDuration(Me, 3000) &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Lose Control");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 5 &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        HealWeightMe < THSettings.Instance.ProfBuffHP &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: GLoves Buff Activated on Low HP");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 6 &&
                        UnitHealIsValid &&
                        HealWeightUnitHeal <= THSettings.Instance.ProfBuffHP &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Friendly Unit Low HP");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 7 &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        CurrentTargetAttackable(20) &&
                        !Me.CurrentTarget.IsPet &&
                        Me.CurrentTarget.HealthPercent <= THSettings.Instance.ProfBuffHP &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Enemy Unit Low HP");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 8 &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        Me.ManaPercent <= THSettings.Instance.ProfBuffHP &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Low Mana");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 9 &&
                        //LastCastSpell != "SProfBuff" &&
                        Me.Inventory.Equipped.Hands != null &&
                        BuffBurst(Me) &&
                        CanUseCheck(Me.Inventory.Equipped.Hands),
                        new Action(delegate
                            {
                                Logging.Write("Use: Gloves Buff Activated on Using Cooldown");
                                StyxWoW.Me.Inventory.Equipped.Hands.Use();
                                LastCastSpell = "SProfBuff";
                                return RunStatus.Failure;
                            })),
                    //Herbalism
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 1 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Cooldown");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 2 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        CurrentTargetAttackable(30) &&
                        Me.CurrentTarget.IsBoss &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Cooldown (Boss Only");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 3 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        THSettings.Instance.Burst &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Burst Mode");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 4 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        Me.Combat &&
                        DebuffCCDuration(Me, 3000) &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Lose Control");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 5 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        HealWeightMe <= THSettings.Instance.ProfBuffHP &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Low HP");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 6 &&
                        UnitHealIsValid &&
                        HealWeightUnitHeal <= THSettings.Instance.ProfBuffHP &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Friendly Unit Low HP");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 7 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        CurrentTargetAttackable(20) &&
                        !Me.CurrentTarget.IsPet &&
                        Me.CurrentTarget.HealthPercent <= THSettings.Instance.ProfBuffHP &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Enemy Unit Low HP");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 8 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        Me.ManaPercent <= THSettings.Instance.ProfBuffHP &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Low Mana");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })),
                    new Decorator(
                        ret =>
                        THSettings.Instance.ProfBuff == 9 &&
                        //SSpellManager.HasSpell("Lifeblood") &&
                        //LastCastSpell != "Lifeblood" &&
                        BuffBurst(Me) &&
                        CanCastCheck("Lifeblood", true),
                        new Action(delegate
                            {
                                Logging.Write("Use: Lifeblood Activated on Using Cooldown");
                                CastSpell("Lifeblood", Me);
                                return RunStatus.Failure;
                            })))
                );
        }

        #endregion

        #region WarStomp

        private static WoWUnit UnitWarStomp;
        private static DateTime LastInterrupt;

        private static bool GetUnitWarStomp()
        {
            UnitWarStomp = null;

            if (InBattleground || InArena)
            {
                UnitWarStomp = NearbyUnFriendlyPlayers.FirstOrDefault(
                    unit => BasicCheck(unit) &&
                            (THSettings.Instance.InterruptAll ||
                             THSettings.Instance.InterruptTarget &&
                             Me.CurrentTarget != null &&
                             unit == Me.CurrentTarget ||
                             THSettings.Instance.InterruptFocus &&
                             Me.FocusedUnit != null &&
                             unit == Me.FocusedUnit) &&
                            InterruptCheck(unit, THSettings.Instance.WindShearInterruptMs + MyLatency) &&
                            Attackable(unit, 8));
            }
                //PvE Search
            else
            {
                UnitWarStomp = NearbyUnFriendlyUnits.FirstOrDefault(
                    unit => BasicCheck(unit) &&
                            (THSettings.Instance.InterruptAll ||
                             THSettings.Instance.InterruptTarget &&
                             Me.CurrentTarget != null &&
                             unit == Me.CurrentTarget ||
                             THSettings.Instance.InterruptFocus &&
                             Me.FocusedUnit != null &&
                             unit == Me.FocusedUnit) &&
                            FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                            InterruptCheck(unit, THSettings.Instance.WindShearInterruptMs + MyLatency) &&
                            Attackable(unit, 8));
            }

            return BasicCheck(UnitWarStomp);
        }

        private void WarStompVoid()
        {
            if (THSettings.Instance.AutoRacial &&
                HealWeightUnitHeal > THSettings.Instance.PriorityHeal &&
                //SSpellManager.HasSpell("War Stomp") &&
                LastInterrupt < DateTime.Now &&
                UnitHeal != null &&
                UnitHeal.IsValid &&
                !IsMoving(Me) &&
                CanCastCheck("War Stomp") &&
                GetUnitWarStomp())
            {
                if (Me.IsCasting || Me.IsChanneling)
                {
                    SpellManager.StopCasting();
                }


                while (BasicCheck(UnitWarStomp) &&
                       UnitWarStomp.CurrentCastTimeLeft.TotalMilliseconds >
                       THSettings.Instance.WindShearInterruptMs + 500)
                {
                    Logging.Write("Waiting for War Stomp");
                }

                if (UnitWarStomp.IsCasting || UnitWarStomp.IsChanneling)
                {
                    CastSpell("War Stomp", Me, "War Stomp");
                    LastInterrupt = DateTime.Now + TimeSpan.FromMilliseconds(1500);
                }
            }
        }

        #endregion

        #region WaterShield

        private static Composite WaterShield()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.WaterShield &&
                //SSpellManager.HasSpell("Water Shield") &&
                //!Me.Mounted &&
                (!GetMyShieldonUnit(Me) ||
                 (InRaid || InDungeon) &&
                 !MyAura("Water Shield", Me)) &&
                CanCastCheck("Water Shield"),
                new Action(
                    ret => { CastSpell("Water Shield", Me, "WaterShield"); })
                );
        }

        private static Composite WaterShieldAlways()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.WaterShieldAlways &&
                //SSpellManager.HasSpell("Water Shield") &&
                //!Me.Mounted &&
                Me.ManaPercent <= THSettings.Instance.WaterShieldAlwaysMana &&
                !MyAura("Water Shield", Me) &&
                (Me.ManaPercent <= 10 ||
                 !MyAura("Earth Shield", Me) ||
                 MyAura("Earth Shield", Me) &&
                 Me.HealthPercent >= THSettings.Instance.EarthShieldAlwaysHP) &&
                CanCastCheck("Water Shield"),
                new Action(
                    ret => { CastSpell("Water Shield", Me, "WaterShieldAlways"); })
                );
        }

        #endregion

        #region WaterWalking

        private static DateTime WaterWalkingLast;

        private static Composite WaterWalking()
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoWaterWalking &&
                HealWeightUnitHeal >= THSettings.Instance.DoNotHealAbove &&
                WaterWalkingLast < DateTime.Now &&
                //SSpellManager.HasSpell("Water Walking") &&
                //!Me.Mounted &&
                !Me.Combat &&
                Me.ManaPercent >= THSettings.Instance.UrgentHeal &&
                !MeHasAura("Water Walking") &&
                CanCastCheck("Water Walking") &&
                !DebuffDot(Me),
                new Action(
                    ret =>
                        {
                            WaterWalkingLast = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                            CastSpell("Water Walking", Me, "WaterWalking");
                        })
                );
        }

        #endregion

        #region WindShear

        private static WoWUnit UnitWindShear;
        //////done
        private static bool GetUnitWindShear()
        {
            UnitWindShear = null;

            if (InBattleground || InArena)
            {
                UnitWindShear = NearbyUnFriendlyPlayers.FirstOrDefault(
                    //////unit => BasicCheck(unit) &&
                    unit => (THSettings.Instance.InterruptAll ||
                             THSettings.Instance.InterruptTarget &&
                             Me.CurrentTarget != null &&
                             unit == Me.CurrentTarget ||
                             THSettings.Instance.InterruptFocus &&
                             Me.FocusedUnit != null &&
                             unit == Me.FocusedUnit) &&
                            //FacingOverride(unit) &&
                            InterruptCheck(unit, THSettings.Instance.WindShearInterruptMs + MyLatency + 1000, false) &&
                            //Attackable(unit, 30));
                            Attackable(unit, (int) SpellManager.Spells["Wind Shear"].MaxRange));
            }
                //PvE Search
            else
            {
                UnitWindShear = NearbyUnFriendlyUnits.FirstOrDefault(
                    unit => BasicCheck(unit) &&
                            (THSettings.Instance.InterruptAll ||
                             THSettings.Instance.InterruptTarget &&
                             Me.CurrentTarget != null &&
                             unit == Me.CurrentTarget ||
                             THSettings.Instance.InterruptFocus &&
                             Me.FocusedUnit != null &&
                             unit == Me.FocusedUnit) &&
                            unit.Combat &&
                            unit.CurrentTarget != null &&
                            //FacingOverride(unit) &&
                            FarFriendlyPlayers.Contains(unit.CurrentTarget) &&
                            InterruptCheck(unit, THSettings.Instance.WindShearInterruptMs + MyLatency + 1000, false) &&
                            //Attackable(unit, 30));
                            Attackable(unit, (int) SpellManager.Spells["Wind Shear"].MaxRange));
            }

            //if (BasicCheck(UnitWindShear))
            //{
            //    Logging.Write("Fround UnitWindShear {0}", UnitWindShear.SafeName);
            //}
            return BasicCheck(UnitWindShear);
        }

        private static void WindShearInterruptVoid()
        {
            if (THSettings.Instance.WindShearInterrupt &&
                (UseSpecialization != 3 ||
                 UseSpecialization == 3 && !UnitHealIsValid ||
                 UseSpecialization == 3 && UnitHealIsValid && HealWeightUnitHeal > THSettings.Instance.UrgentHeal) &&
                //SSpellManager.HasSpell("Wind Shear") &&
                LastInterrupt < DateTime.Now &&
                CanCastCheck("Wind Shear", true) &&
                //!Me.Mounted &&
                GetUnitWindShear())
            {
                if (Me.IsCasting || Me.IsChanneling)
                {
                    SpellManager.StopCasting();
                }

                SafelyFacingTarget(UnitWindShear);

                while (BasicCheck(UnitWindShear) &&
                       UnitWindShear.CurrentCastTimeLeft.TotalMilliseconds >
                       THSettings.Instance.WindShearInterruptMs + MyLatency)
                {
                    Logging.Write("Waiting for Wind Shear");
                }

                if (UnitWindShear.IsCasting || UnitWindShear.IsChanneling)
                {
                    LastInterrupt = DateTime.Now + TimeSpan.FromMilliseconds(1500);
                    CastSpell("Wind Shear", UnitWindShear, "Casting " +
                                                           UnitWindShear.CastingSpell.Name + " - " +
                                                           UnitWindShear.CastingSpellId);
                }
            }
        }

        #endregion

        #region Windwalk

        private static bool NeedFriendWindwalk()
        {
            return NearbyFriendlyPlayers.Where(BasicCheck).Any(
                unit =>
                unit.Combat &&
                IsEnemy(unit.CurrentTarget) &&
                DebuffRoot(unit));
        }

        private static Composite Windwalk()
        {
            return new PrioritySelector(
                new Decorator(
                    ret =>
                    THSettings.Instance.WindwalkRootMe &&
                    HealWeightUnitHeal >= THSettings.Instance.UrgentHeal &&
                    UseSpecialization == 3 &&
                    Me.Combat &&
                    //SSpellManager.HasSpell("Windwalk Totem") &&
                    !MyTotemCheck("Earth Elemental Totem", Me, 40) &&
                    CanCastCheck("Windwalk Totem", true) &&
                    DebuffRoot(Me) &&
                    HaveDPSTarget(Me),
                    new Action(
                        delegate
                            {
                                CastSpell("Windwalk Totem", Me, "WindwalkRootMeResto");
                                return RunStatus.Failure;
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.WindwalkRootMe &&
                    UseSpecialization != 3 &&
                    Me.Combat &&
                    //SSpellManager.HasSpell("Windwalk Totem") &&
                    !MyTotemCheck("Earth Elemental Totem", Me, 40) &&
                    CurrentTargetAttackable(40) &&
                    CanCastCheck("Windwalk Totem", true) &&
                    DebuffRoot(Me),
                    new Action(
                        delegate
                            {
                                CastSpell("Windwalk Totem", Me, "WindwalkRootMeEleEnh");
                                return RunStatus.Failure;
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.WindwalkRootFriend &&
                    HealWeightUnitHeal >= THSettings.Instance.UrgentHeal &&
                    UseSpecialization == 3 &&
                    //SSpellManager.HasSpell("Windwalk Totem") &&
                    !MyTotemCheck("Earth Elemental Totem", Me, 40) &&
                    CanCastCheck("Windwalk Totem", true) &&
                    NeedFriendWindwalk(),
                    new Action(
                        delegate
                            {
                                CastSpell("Windwalk Totem", Me, "WindwalkRootFriendResto");
                                return RunStatus.Failure;
                            })),
                new Decorator(
                    ret =>
                    THSettings.Instance.WindwalkRootFriend &&
                    UseSpecialization != 3 &&
                    //SSpellManager.HasSpell("Windwalk Totem") &&
                    !MyTotemCheck("Earth Elemental Totem", Me, 40) &&
                    CanCastCheck("Windwalk Totem", true) &&
                    NeedFriendWindwalk(),
                    new Action(
                        delegate
                            {
                                CastSpell("Windwalk Totem", Me, "WindwalkRootFriendEleEnh");
                                return RunStatus.Failure;
                            }))
                );
        }

        #endregion

        #region WorthyTarget

        private static WoWUnit WorthyTargetAttackingMe;

        private static bool HaveWorthyTargetAttackingMe()
        {
            //////WorthyTargetAttackingMe = NearbyUnFriendlyUnits.
            WorthyTargetAttackingMe = FarUnFriendlyUnits.Where(BasicCheck).
                FirstOrDefault(
                    unit =>
                    //////BasicCheck(unit) &&
                    unit.Combat &&
                    unit.GotTarget &&
                    //////unit.CurrentTarget != null &&
                    unit.CurrentTarget == Me &&
                    IsWorthyTarget(unit,1,0.3));

            return BasicCheck(WorthyTargetAttackingMe);
        }

        private static bool IsWorthyTarget(WoWUnit target, double pvEPercent = 1, double pvPPercent = 0.3)
        {
            //////if (!BasicCheck(target) || Me.GetPredictedHealthPercent() < 20)
            if (Me.HealthPercent < 20)
            {
                return false;
            }

            if (IsDummy(target) ||
                !target.IsPet &&
                target.CurrentHealth > Me.CurrentHealth*pvEPercent ||
                target.IsPlayer &&
                target.CurrentHealth > Me.CurrentHealth*pvPPercent)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region WriteDebug

        private static DateTime LastWriteDebug;

        private static Composite WriteDebug(string message)
        {
            return new Action(delegate
                {
                    if (message == "")
                    {
                        return RunStatus.Failure;
                    }

                    Logging.Write("(" +
                                  Math.Round(
                                      (DateTime.Now - LastWriteDebug).TotalMilliseconds,
                                      0) +
                                  ") " + message);
                    LastWriteDebug = DateTime.Now;
                    return RunStatus.Failure;
                });
        }

        #endregion
    }
}