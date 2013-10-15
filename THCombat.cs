using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using JetBrains.Annotations;
using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.CommonBot.Routines;
using Styx.TreeSharp;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace TuanHA_Combat_Routine
{
    public partial class Classname : CombatRoutine
    {
        #region Basic Functions

        public delegate WoWPoint LocationRetriverDelegate(object context);

        public override bool WantButton
        {
            get { return true; }
        }

        public override WoWClass Class
        {
            get { return WoWClass.Shaman; }
        }

        public override string Name
        {
            get { return "TuanHA Shaman [Open Beta]"; }
        }

        public override Composite RestBehavior
        {
            get { return RestRotation(); }
        }

        public override Composite PreCombatBuffBehavior
        {
            get { return PreCombatRotation(); }
            //get { return MainRotation(); }
        }

        //public override Composite PullBuffBehavior
        //{
        //    get { return new CallTrace("PullBuffBehavior", PullBuffRotation()); }
        //}

        //public override Composite PullBehavior
        //{
        //    get { return new CallTrace("PullBehavior", PullBuffRotation()); }
        //}

        //public override Composite CombatBuffBehavior
        //{
        //    get { return MainRotation(); }
        //}

        public override Composite CombatBehavior
        {
            get { return MainRotation(); }
        }

        //public override Composite HealBehavior
        //{
        //    get { return new CallTrace("HealBehavior", PullBuffRotation()); }
        //}

        //public override Composite MoveToTargetBehavior
        //{
        //    get { return CreateMoveToLosBehavior(); }
        //}

        private static LocalPlayer Me
        {
            get { return StyxWoW.Me; }
        }

        public override void Initialize()
        {
            Logging.Write("");
            Logging.Write("Hello level " + Me.Level + " " + Me.Race + " " + Me.Class);
            Logging.Write("");
            Logging.Write("Thank you for using TuanHA Shaman Open Beta");
            Logging.Write("");
            Logging.Write("For best Combat Routine performance, please use Tyrael botbase.");
            Logging.Write("You can download Tyrael Here: http://goo.gl/51E0F6");
            Logging.Write("FRAMELOCK is now supported within Combat Routine");
            Logging.Write("Click Class Config - General Settings Tab - Check Enable Framelock");
            //Logging.Write("For detailed installation guide, visit our new website");
            //Logging.Write("www.tuanha.biz");
            Logging.Write("");

            //Lua.Events.AttachEvent("GROUP_ROSTER_UPDATE", UpdateRaidPartyMembersEvent);
            Lua.Events.AttachEvent("ACTIVE_TALENT_GROUP_CHANGED", UpdateStatusEvent);
            Lua.Events.AttachEvent("GLYPH_ADDED", UpdateMyGlyphEvent);
            //Lua.Events.AttachEvent("ZONE_CHANGED_NEW_AREA", UpdateRaidPartyMembersEvent);
            //Lua.Events.AttachEvent("PLAYER_ENTERING_WORLD", UpdateRaidPartyMembersEvent);
            //Lua.Events.AttachEvent("WORLD_MAP_NAME_UPDATE", UpdateRaidPartyMembersEvent);
            Lua.Events.AttachEvent("ZONE_CHANGED_NEW_AREA", ClearEnemyandFriendListCacheEvent);

            Chat.NeutralBattleground += ChatFilter;
            Chat.AllianceBattleground += ChatFilter;
            Chat.HordeBattleground += ChatFilter;

            BotEvents.OnBotStarted += OnBotStartedEvent;

            THSettings.Instance.UpdateStatus = true;
        }

        public override void OnButtonPress()
        {
            var gui = new THForm();
            gui.ShowDialog();
        }

        #endregion

        #region GetUnits

        private static IEnumerable<WoWUnit> GetAllUnits()
        {
            return ObjectManager.GetObjectsOfType<WoWUnit>(true, true)
                                .Where(
                                    unit =>
                                    !Blacklist.Contains(unit.Guid, BlacklistFlags.All) &&
                                    //unit.Attackable &&
                                    unit.CanSelect &&
                                    unit.IsAlive &&
                                    unit.Distance <= 60)
                                    //unit.DistanceSqr <= 3600)
                                .ToList();
        }

        private static readonly List<WoWPlayer> FarFriendlyPlayers = new List<WoWPlayer>();
        private static readonly List<WoWUnit> FarFriendlyUnits = new List<WoWUnit>();
        private static readonly List<WoWPlayer> NearbyFriendlyPlayers = new List<WoWPlayer>();
        private static readonly List<WoWUnit> NearbyFriendlyUnits = new List<WoWUnit>();
        private static readonly List<WoWPlayer> NearbyUnFriendlyPlayers = new List<WoWPlayer>();
        private static readonly List<WoWUnit> NearbyUnFriendlyUnits = new List<WoWUnit>();
        private static readonly List<WoWPlayer> FarUnFriendlyPlayers = new List<WoWPlayer>(); //Don't use in this CC
        private static readonly List<WoWUnit> FarUnFriendlyUnits = new List<WoWUnit>();

        private static Composite GetUnits()
        {
            return new Action(delegate
                {
                    //if (LastGetUnits + TimeSpan.FromMilliseconds(THSettings.Instance.SearchInterval) > DateTime.Now)
                    //{
                    //    return RunStatus.Failure;
                    //}
                    //LastGetUnits = DateTime.Now;

                    NearbyFriendlyPlayers.Clear();
                    NearbyUnFriendlyPlayers.Clear();
                    NearbyFriendlyUnits.Clear();
                    NearbyUnFriendlyUnits.Clear();
                    FarFriendlyPlayers.Clear();
                    FarFriendlyUnits.Clear();
                    FarUnFriendlyPlayers.Clear();
                    FarUnFriendlyUnits.Clear();

                    EnemyListCacheClear();
                    FriendListCacheClear();

                    //NearbyFriendlyUnits.Add(Me);
                    //NearbyFriendlyPlayers.Add(Me);
                    //FarFriendlyUnits.Add(Me);
                    //FarFriendlyPlayers.Add(Me);

                    //foreach (WoWUnit unit in GetAllUnits().Where(unit => BasicCheck(unit) && unit.Distance <= 60))
                    foreach (WoWUnit unit in GetAllUnits())
                    {
                        if (FriendListCache.ContainsKey(unit.Guid))
                        {
                            FarFriendlyUnits.Add(unit);
                            var player = unit as WoWPlayer;
                            if (player != null)
                            {
                                FarFriendlyPlayers.Add((WoWPlayer) unit);
                            }
                            if (unit.Distance <= 40)
                            {
                                NearbyFriendlyUnits.Add(unit);
                                if (player != null)
                                {
                                    NearbyFriendlyPlayers.Add(player);
                                }
                            }
                        }
                        else if (EnemyListCache.ContainsKey(unit.Guid))
                        {
                            FarUnFriendlyUnits.Add(unit);
                            var player = unit as WoWPlayer;
                            if (player != null)
                            {
                                FarUnFriendlyPlayers.Add((WoWPlayer) unit);
                            }
                            if (unit.Distance <= 40)
                            {
                                NearbyUnFriendlyUnits.Add(unit);
                                if (player != null)
                                {
                                    NearbyUnFriendlyPlayers.Add(player);
                                }
                            }
                        }
                        else if (!InArena && !InBattleground &&
                                 ((!unit.IsFriendly && unit.Attackable && !unit.IsQuestGiver) ||
                                  //////unit.Combat && !unit.IsFriendly ||
                                  Me.Combat && IsDummy(unit) && Me.IsFacing(unit)))
                        {
                            EnemyListCacheAdd(unit,10);
                            FarUnFriendlyUnits.Add(unit);

                            if (GetDistance(unit) < 40)
                            {
                                NearbyUnFriendlyUnits.Add(unit);
                            }

                            //Do not attack world player
                            var player = unit as WoWPlayer;

                            if (player != null && player.CurrentTarget != null && player.CurrentTarget == Me)
                                //Do not attack world player
                            {
                                NearbyUnFriendlyPlayers.Add(player);
                            }
                        }
                        else if (IsMyPartyRaidMember(unit))
                        {
                            FriendListCacheAdd(unit, 60);
                            FarFriendlyUnits.Add(unit);

                            var player = unit as WoWPlayer;

                            if (player != null)
                            {
                                FarFriendlyPlayers.Add((WoWPlayer) unit);
                            }

                            if (GetDistance(unit) < 40)
                            {
                                NearbyFriendlyUnits.Add(unit);

                                if (player != null)
                                {
                                    NearbyFriendlyPlayers.Add(player);
                                }
                            }
                        }
                        else if (InArena || InBattleground)
                        {
                            EnemyListCacheAdd(unit,10);
                            FarUnFriendlyUnits.Add(unit);

                            var player = unit as WoWPlayer;

                            if (player != null)
                            {
                                FarUnFriendlyPlayers.Add((WoWPlayer) unit);
                            }

                            if (GetDistance(unit) < 40)
                            {
                                NearbyUnFriendlyUnits.Add(unit);

                                if (player != null)
                                {
                                    NearbyUnFriendlyPlayers.Add(player);
                                }
                            }
                        }
                    }
                    return RunStatus.Failure;
                });
        }

        #endregion

        #region GetUnitHeal

        private static WoWUnit UnitHeal;
        private static double HealWeightUnitHeal;
        private static double HealWeightMe;
        private static bool UnitHealIsValid;

        private static Composite GetUnitHeal()
        {
            return new Action(delegate
                {
                    UnitHeal = null;
                    UnitHealIsValid = false;
                    HealWeightUnitHeal = 10000;

                    if (UnitHeal == null &&
                        BasicCheck(Me.CurrentTarget) &&
                        //////Me.CurrentTarget != null &&
                        //////Me.CurrentTarget.IsValid &&
                        //////Me.CurrentTarget.IsAlive &&
                        GetDistance(Me.CurrentTarget) < 40 &&
                        (Me.CurrentTarget.IsPlayer || Me.CurrentTarget.IsPet ||
                         NeedHealUnit.Contains(Me.CurrentTarget.Entry)) &&
                        Me.CurrentTarget.GetPredictedHealthPercent() <= THSettings.Instance.DoNotHealAbove &&
                        !IsEnemy(Me.CurrentTarget) &&
                        !DebuffDoNotHeal(Me.CurrentTarget) &&
                        InLineOfSpellSightCheck(Me.CurrentTarget))
                    {
                        UnitHeal = Me.CurrentTarget;
                    }

                    if (UnitHeal == null &&
                        BasicCheck(Me.FocusedUnit) &&
                        //Me.FocusedUnit != null &&
                        //Me.FocusedUnit.IsValid &&
                        //Me.FocusedUnit.IsAlive &&
                        GetDistance(Me.FocusedUnit) < 40 &&
                        (Me.FocusedUnit.IsPlayer || Me.FocusedUnit.IsPet || NeedHealUnit.Contains(Me.FocusedUnit.Entry)) &&
                        Me.FocusedUnit.GetPredictedHealthPercent() <= THSettings.Instance.PriorityHeal &&
                        !IsEnemy(Me.FocusedUnit) &&
                        !DebuffDoNotHeal(Me.FocusedUnit) &&
                        InLineOfSpellSightCheck(Me.FocusedUnit))
                    {
                        UnitHeal = Me.FocusedUnit;
                    }

                    if (!InArena && UnitHeal == null && Me.GetPredictedHealthPercent() < THSettings.Instance.PriorityHeal)
                    {
                        UnitHeal = Me;
                    }

                    if (InArena && UnitHeal == null)
                    {
                        UnitHeal = (from unit in NearbyFriendlyUnits.Where<WoWUnit>(new Func<WoWUnit, bool>(Classname.BasicCheck))
                            orderby unit.GetPredictedHealthPercent()
                            select unit).FirstOrDefault(
                                unit =>
                                //////BasicCheck(unit) &&
                                (unit.IsPlayer ||
                                 unit.IsBeast ||
                                 unit.IsDemon ||
                                 unit.IsUndead) &&
                                unit.MaxHealth > MeMaxHealth/2 &&
                                GetDistance(unit) <= 40 &&
                                !DebuffDoNotHeal(unit) &&
                                InLineOfSpellSightCheck(unit));
                    }

                    if (UnitHeal == null)
                    {
                        UnitHeal = (from unit in NearbyFriendlyPlayers.Where<WoWUnit>(new Func<WoWUnit, bool>(Classname.BasicCheck))
                            orderby unit.GetPredictedHealthPercent()
                            select unit).FirstOrDefault(
                                unit =>
                                //////BasicCheck(unit) &&
                                GetDistance(unit) <= 40 &&
                                !DebuffDoNotHeal(unit) &&
                                InLineOfSpellSightCheck(unit));
                    }

                    if (UnitHeal == null)
                    {
                        UnitHealIsValid = false;
                    }
                    else
                    {
                        UnitHealIsValid = true;
                        HealWeightUnitHeal = HealWeight(UnitHeal);
                    }

                    //HealWeightMe = UnitHeal == Me ? HealWeightUnitHeal : HealWeight(Me);

                    HealWeightMe = HealWeight(Me);

                    return RunStatus.Failure;
                });
        }

        #endregion

        #region Hold

        private static Composite Hold()
        {
            return new Decorator(
                ret =>
                !Me.IsValid ||
                !StyxWoW.IsInWorld ||
                !Me.IsAlive ||
                !THSettings.Instance.AutoAttackOutCombat && !Me.Combat ||
                MeHasAura("Food") && HealWeightMe < THSettings.Instance.DoNotHealAbove ||
                MeHasAura("Drink") && (Me.ManaPercent < THSettings.Instance.DoNotHealAbove ||
                                       Me.GetAuraByName("Drink").TimeLeft.TotalSeconds > 9) ||
                MeHasAura("Resurrection Sickness"),
                new Action(delegate
                    {
                        if (Me.IsAutoAttacking)
                        {
                            Lua.DoString("RunMacroText('/stopattack');");
                        }

                        Logging.Write("Hold");
                        return RunStatus.Success;
                    })
                );
        }

        #endregion

        #region Pulse

        private static DateTime LastSwitch;
        private static DateTime BurstLast;
        //private static DateTime LastPulseTime;

        public override void Pulse()
        {
            if (!BasicCheck(Me) ||
                !StyxWoW.IsInWorld ||
                Me.Mounted)
            {
                return;
            }

            if (THSettings.Instance.UpdateStatus)
            {
                UpdateStatus();
            }
            else
            {
                //////done
                GlobalCheck();
                //////done
                UseTrinketVoid();
                //////done
                ReturningFlag();
                if (!Me.Mounted &&
                    (THSettings.Instance.AutoGhostWolfCancel ||
                     !MeHasAura("Ghost Wolf")))
                {
                    //////done
                    WindShearInterruptVoid();
                    //////done
                    ///可优化逻辑，例如当通过hotkey使用电能图腾时，投掷目标为当前目标或者focus。另外队友血少，可增加队友被晕，切敌人目标为队友
                    CapacitorProjection(THSettings.Instance.CapacitorProjectionMs);
                    //////done
                    GroundingCastInterruptVoid();
                    //CapacitorTestPosition();
                    //////TotemicRestoration();
                    //////done
                    Tremor();
                    //////done
                    ShamanisticCC();
                    //////done
                    GhostWolfAvoidCC();

                    WarStompVoid();

                    StopCastingCheck();
                }

                //Pause
                if (THSettings.Instance.PauseKeyUse && THSettings.Instance.PauseKey != 0)
                {
                    //AoE Mode
                    if (GetAsyncKeyState(Keys.LControlKey) < 0 &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.PauseKey)) < 0 &&
                        LastSwitch.AddSeconds(1) < DateTime.Now) // && GetActiveWindowTitle() == "World of Warcraft")
                    {
                        if (THSettings.Instance.Pause)
                        {
                            THSettings.Instance.Pause = false;
                            LastSwitch = DateTime.Now;
                            Logging.Write("Pause Mode is OFF, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.PauseKey) +
                                          " to Override bot action.");
                            Lua.DoString("RunMacroText(\"/script msg='Pause Mode OFF' print(msg)\")");
                        }
                        else
                        {
                            THSettings.Instance.Pause = true;
                            THSettings.Instance.UpdateStatus = true;
                            LastSwitch = DateTime.Now;
                            Logging.Write("Pause Mode is ON, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.PauseKey) +
                                          " to resume bot action.");
                            Lua.DoString("RunMacroText(\"/script msg='Pause Mode ON' print(msg)\")");
                        }
                    }
                }

                //Auto Disactivate Burst after a Timer or Me get CC
                if (THSettings.Instance.Burst && BurstLast < DateTime.Now)
                {
                    THSettings.Instance.Burst = false;
                    BurstLast = DateTime.Now;
                    Logging.Write("Burst Mode is OFF");
                    Lua.DoString("RunMacroText(\"/script msg='Burst Mode OFF' print(msg)\")");
                }

                //Burst on Cooldown
                if (THSettings.Instance.BurstKey == 1 &&
                    THSettings.Instance.Burst == false &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated On Cooldown");
                    THSettings.Instance.Burst = true;
                }

                //Burst on Cooldown
                if (THSettings.Instance.BurstKey == 2 &&
                    THSettings.Instance.Burst == false &&
                    CurrentTargetAttackable(40) &&
                    Me.CurrentTarget.IsBoss &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated On Cooldown (Boss Only)");
                    THSettings.Instance.Burst = true;
                }

                //Burst on Bloodlust
                if (THSettings.Instance.BurstKey == 3 &&
                    THSettings.Instance.Burst == false &&
                    (MeHasAura("Bloodlust") || MeHasAura("Heroism") || MeHasAura("Time Warp") ||
                     MeHasAura("Ancient Hysteria")) &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated On Bloodlust/Heroism/Time Warp/Ancient Hysteria");
                    THSettings.Instance.Burst = true;
                }

                //Burst On Lose Control
                if (THSettings.Instance.BurstKey == 4 &&
                    THSettings.Instance.Burst == false &&
                    DebuffCCDuration(Me, 3000))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on Lose Control");
                    THSettings.Instance.Burst = true;
                }

                //Burst On My Health Below
                if (THSettings.Instance.BurstKey == 5 &&
                    THSettings.Instance.Burst == false &&
                    HealWeightMe < THSettings.Instance.BurstHP &&
                    ! DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on My Health Low");
                    THSettings.Instance.Burst = true;
                }

                //Burst On Friend Health Below
                if (THSettings.Instance.BurstKey == 6 &&
                    THSettings.Instance.Burst == false &&
                    BasicCheck(UnitHeal) &&
                    !UnitHeal.IsPet &&
                    HealWeightUnitHeal <= THSettings.Instance.BurstHP &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on Friend Health Low");
                    THSettings.Instance.Burst = true;
                }

                //Burst On Enemy Health Below
                if (THSettings.Instance.BurstKey == 7 &&
                    THSettings.Instance.Burst == false &&
                    CurrentTargetAttackable(40) &&
                    !Me.CurrentTarget.IsPet &&
                    HealWeightMe > THSettings.Instance.UrgentHeal &&
                    Me.CurrentTarget.HealthPercent <= THSettings.Instance.BurstHP &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on Enemy Health Low");
                    THSettings.Instance.Burst = true;
                }

                //Burst On My Mana Below
                if (THSettings.Instance.BurstKey == 8 &&
                    THSettings.Instance.Burst == false &&
                    Me.ManaPercent < THSettings.Instance.BurstHP &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on My Mana Low");
                    THSettings.Instance.Burst = true;
                }

                //Burst On Using Cooldown 使用升腾的时候，自动设置为burst
                if (THSettings.Instance.BurstKey == 9 &&
                    THSettings.Instance.Burst == false &&
                    BuffBurst(Me) &&
                    !DebuffCC(Me))
                {
                    BurstLast = DateTime.Now.AddSeconds(15);
                    Logging.Write("Burst Mode Activated on Using Cooldown");
                    THSettings.Instance.Burst = true;
                }

                //Burst by key press
                if (THSettings.Instance.BurstKey > 9)
                {
                    if(THSettings.Instance.Burst == false &&
                       CurrentTargetAttackable(30) &&
                       !Me.CurrentTarget.IsPet &&
                       Me.CurrentTarget.HealthPercent <= THSettings.Instance.BurstHP &&
                       !CurrentTargetCheckInvulnerablePhysic &&
                       !DebuffCC(Me) &&
                        //SSpellManager.HasSpell("Ascendance") &&
                       Me.ManaPercent > 20 &&
                       InArena &&
                       CanCastCheck("Ascendance", true) &&
                       CanCastCheck("Primal Strike")) //Suck to pop CD and no Mana to use seplls
                       {
                           BurstLast = DateTime.Now.AddSeconds(15);
                           Logging.Write("Burst Mode Activated on Enemy Health Low");
                           THSettings.Instance.Burst = true;
                       }

                    //Burst Mode
                    if (GetAsyncKeyState(Keys.LControlKey) < 0 &&
                        GetAsyncKeyState(IndexToKeys(THSettings.Instance.BurstKey - 9)) < 0 &&
                        LastSwitch.AddSeconds(1) < DateTime.Now) // && GetActiveWindowTitle() == "World of Warcraft")
                    {
                        if (THSettings.Instance.Burst)
                        {
                            THSettings.Instance.Burst = false;
                            LastSwitch = DateTime.Now;
                            Logging.Write("Burst Mode is OFF, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.BurstKey - 8) +
                                          " to Turn Burst Mode ON.");
                            Lua.DoString("RunMacroText(\"/script msg='Burst Mode OFF' print(msg)\")");
                        }
                        else if (!DebuffCC(Me))
                        {
                            THSettings.Instance.Burst = true;
                            LastSwitch = DateTime.Now;
                            BurstLast = DateTime.Now.AddSeconds(15);
                            Logging.Write("Burst Mode is ON, Hold 1 second Ctrl + " +
                                          IndexToKeys(THSettings.Instance.BurstKey - 8) +
                                          " to Turn Burst Mode OFF.");
                            Lua.DoString("RunMacroText(\"/script msg='Burst Mode ON' print(msg)\")");
                        }
                    }
                }
            }
        }

        #endregion

        #region LockSelector - FrameLock - AcquireFrame

        /// <summary>
        /// Thank you my friend Nomnomnom :D
        /// </summary>
        [UsedImplicitly]
        private class LockSelector : PrioritySelector
        {
            public LockSelector(params Composite[] children)
                : base(children)
            {
            }

            public override RunStatus Tick(object context)
            {
                using (StyxWoW.Memory.AcquireFrame())
                {
                    return base.Tick(context);
                }
            }
        }

        #endregion

        # region Trace 

        /// <summary>
        /// Copyright by Singular
        /// </summary>
        public class CallTrace : PrioritySelector
        {
            private static DateTime LastCall { get; set; }

            private static ulong CountCall { get; set; }

            private static bool TraceActive
            {
                get { return false; }
            }

            private string Name { get; set; }

            private static bool _init = false;

            private static void Initialize()
            {
                if (_init)
                    return;

                _init = true;
            }

            public CallTrace(string name, params Composite[] children)
                : base(children)
            {
                Initialize();

                Name = name;
                LastCall = DateTime.MinValue;
            }

            public override RunStatus Tick(object context)
            {
                RunStatus ret;
                CountCall++;

                if (!TraceActive)
                {
                    ret = base.Tick(context);
                }
                else
                {
                    DateTime started = DateTime.Now;
                    Logging.Write("... enter: {0}", Name);
                    ret = base.Tick(context);
                    Logging.Write("... leave: {0}, took {1} ms", Name,
                                  (ulong) (DateTime.Now - started).TotalMilliseconds);
                }

                return ret;
            }
        }

        #endregion

        #region MainRotation

        private static Composite PreCombatRotation()
        {
            return new Decorator(
                ret =>
                !Me.Combat,
                MainRotation());
        }

        private static bool AoEModeOn;
        private static readonly Stopwatch sw = Stopwatch.StartNew();
        private static DateTime LastRotationTime;
        private static bool IsOverrideModeOn;
        private static WoWUnit MyLastTarget;

        private static Composite MainRotation()
        {
            return new PrioritySelector(
                //SWStart(),
                new Action(delegate
                    {
                        //if (!Me.Combat)
                        //{
                        //    return RunStatus.Success;
                        //}
                        if (LastWriteDebug < DateTime.Now && GetAsyncKeyState(Keys.F10) != 0)
                        {
                            LastWriteDebug = DateTime.Now + TimeSpan.FromSeconds(5);
                            Logging.Write("=============================");
                            Logging.Write("SpellsCooldownCache");
                            Logging.Write("{1} GCDBack {0}",
                                          (DateTime.Now + GetSpellCooldown("Lightning Shield")).ToString("ss:fff"),
                                          DateTime.Now.ToString("ss:fff"));

                            foreach (var unit in SpellsCooldownCache)
                            {
                                Logging.Write("{2} unit.Key {0}, unit.Value {1}", unit.Key,
                                              unit.Value.ToString("ss:fff"), DateTime.Now.ToString("ss:fff"));
                            }
                            Logging.Write("=============================");
                        }

                        if (GetAsyncKeyState(Keys.F9) != 0)
                        {
                            Logging.Write("=============================");
                            Logging.Write("LastRotationTime take {0} ms",
                                          (DateTime.Now - LastRotationTime).TotalMilliseconds);
                            LastRotationTime = DateTime.Now;
                            Logging.Write("=============================");
                        }

                        if (LastWriteDebug < DateTime.Now && GetAsyncKeyState(Keys.F8) != 0)
                        {
                            LastWriteDebug = DateTime.Now + TimeSpan.FromSeconds(5);
                            //Logging.Write("=============================");
                            //Logging.Write("FriendListCache");
                            //foreach (var unit in FriendListCache)
                            //{
                            //    Logging.Write(unit.Key.ToString());
                            //}
                            //Logging.Write("=============================");
                            //Logging.Write("EnemyListCache");
                            //foreach (var unit in EnemyListCache)
                            //{
                            //    Logging.Write(unit.Key.ToString());
                            //}
                            Logging.Write("=============================");
                            Logging.Write("FarFriendlyPlayers");
                            foreach (var unit in FarFriendlyPlayers)
                            {
                                Logging.Write(unit.SafeName);
                            }
                            Logging.Write("=============================");
                            Logging.Write("FarFriendlyUnits");
                            foreach (var unit in FarFriendlyUnits)
                            {
                                Logging.Write(unit.SafeName);
                            }
                            Logging.Write("=============================");
                            Logging.Write("NearbyFriendlyPlayers");
                            foreach (var unit in NearbyFriendlyPlayers)
                            {
                                Logging.Write(unit.SafeName);
                            }
                            Logging.Write("=============================");
                            Logging.Write("NearbyFriendlyUnits");
                            foreach (var unit in NearbyFriendlyUnits)
                            {
                                Logging.Write(unit.SafeName);
                            }
                            Logging.Write("=============================");
                            Logging.Write("NearbyUnFriendlyPlayers");
                            foreach (var unit in NearbyUnFriendlyPlayers)
                            {
                                Logging.Write(unit.SafeName);
                            }
                            Logging.Write("=============================");
                            Logging.Write("NearbyUnFriendlyUnits");
                            foreach (var unit in NearbyUnFriendlyUnits)
                            {
                                Logging.Write(unit.SafeName);
                            }
                            Logging.Write("=============================");
                        }

                        if (LastWriteDebug < DateTime.Now && GetAsyncKeyState(Keys.F7) != 0)
                        {
                            LastWriteDebug = DateTime.Now + TimeSpan.FromSeconds(5);
                            Logging.Write("=============================");
                            foreach (var aura in Me.GetAllAuras())
                            {
                                Logging.Write("{0} {1}", aura.Name, aura.SpellId);
                            }
                            Logging.Write("=============================");
                        }
                        return RunStatus.Failure;
                    }),

                
                Hold(),//.
                //SWStop("Hold"),
                //Hotkey1 & Hotkey2增加了一个6秒的计时,计时内,根据不同计时器名称,判断图腾是丢给目标或者焦点
                Hotkey1(),//.
                //SWStop("Hotkey1"),
                Hotkey2(),//.
                //SWStop("Hotkey2"),
                Hotkey3(),//.
                //SWStop("Hotkey3"),
                Hotkey4(),//.
                //SWStop("Hotkey4"),
                Hotkey5(),//.
                //SWStop("Hotkey5"),

                //UpdateRaidPartyMembersComp(),

                new Decorator(
                    ret => THSettings.Instance.Pause,
                    new Action(delegate { return RunStatus.Success; })),//.

                //SWStop("Pause"),
                new Decorator(
                    ret =>
                    THSettings.Instance.UpdateStatus,
                    new Action(delegate { return RunStatus.Success; })),//.
                //SWStop("UpdateStatus"),
                GetUnits(),//.
                //SWStop("GetUnits"),
                GetUnitHeal(),//.
                //SWStop("GetUnitHeal"),
                new Action(delegate
                    {
                        if (!IsOverrideModeOn &&
                            //Do not stop movement in LazyRaider if AutoMove enabled
                            //TreeRoot.Current.Name != "LazyRaider" &&
                            (GetAsyncKeyState(Keys.LButton) < 0 &&
                             GetAsyncKeyState(Keys.RButton) < 0) ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleLeft)) < 0 &&
                            IndexToKeys(THSettings.Instance.StrafleLeft) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Forward)) < 0 &&
                            IndexToKeys(THSettings.Instance.Forward) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleRight)) < 0 &&
                            IndexToKeys(THSettings.Instance.StrafleRight) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnLeft)) < 0 &&
                            IndexToKeys(THSettings.Instance.TurnLeft) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Backward)) < 0 &&
                            IndexToKeys(THSettings.Instance.Backward) != Keys.None ||
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnRight)) < 0 &&
                            IndexToKeys(THSettings.Instance.TurnRight) != Keys.None)
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Override mode on. Stop all bot movement");
                            IsOverrideModeOn = true;
                        }

                        if (IsOverrideModeOn &&
                            //Do not stop movement in LazyRaider if AutoMove enabled
                            //TreeRoot.Current.Name != "LazyRaider" &&
                            (GetAsyncKeyState(Keys.LButton) >= 0 ||
                             GetAsyncKeyState(Keys.RButton) >= 0) &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleLeft)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Forward)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.StrafleRight)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnLeft)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.Backward)) >= 0 &&
                            GetAsyncKeyState(IndexToKeys(THSettings.Instance.TurnRight)) >= 0)
                        {
                            //Logging.Write(LogLevel.Diagnostic, "Override mode off. Bot movement resume");
                            IsOverrideModeOn = false;
                        }


                        //Get target back after a fear
                        //if (Me.CurrentTarget != null && MyLastTarget != Me.CurrentTarget)
                        //{
                        //    MyLastTarget = Me.CurrentTarget;
                        //}

                        //Clear Target if dead and still in combat
                        //if (Me.CurrentTarget != null && !Me.CurrentTarget.IsAlive && Me.Combat)
                        //{
                        //    Lua.DoString("RunMacroText(\"/cleartarget\")");
                        //}

                        //Clear Target if walk away from friendly
                        //if (Me.CurrentTarget != null && !Me.CurrentTarget.IsPlayer &&
                        //    Me.CurrentTarget.IsFriendly &&
                        //    Me.CurrentTarget.IsSafelyBehind(Me))
                        //{
                        //    Lua.DoString("RunMacroText(\"/cleartarget\")");
                        //}
                        return RunStatus.Failure;
                    }),
                //SWStop("IsOverrideModeOn"),
                //SWStop("SetAutoAttack"),

                new Action(delegate
                    {
                        CurrentTargetCheck();//.
                        return RunStatus.Failure;
                    }),
                //SWStop("CurrentTargetCheck"),

                UseRacial(),
                //SWStop("UseRacial"),
                UseProfession(),
                UseHealthStoneHP(),
                UseBattleStandard(),
                //SWStop("UseProfession"),
                AutoSetFocus(),
                new Decorator(
                    ret =>
                    UseSpecialization == 1 &&
                    !Me.Mounted,
                    ElementalRotation()
                    ),
                //SWStop("ElementalRotation"),
                new Decorator(
                    ret =>
                    UseSpecialization != 1 &&
                    UseSpecialization != 3 &&
                    !Me.Mounted,
                    EnhancementRotation()
                    ),
                //SWStop("EnhancementRotation"),
                new Decorator(
                    ret =>
                    UseSpecialization == 3 &&
                    !Me.Mounted,
                    RestorationRotation()
                    )
                //SWStop("RestorationRotation")
                //RestRotation()
                //Dismount()
                );
        }

        #endregion
    }
}