using System;
using Styx;
using Styx.Helpers;
using Styx.Pathing;
using Styx.TreeSharp;
using Styx.WoWInternals.WoWObjects;
using Action = Styx.TreeSharp.Action;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region Delegates

        public delegate float DynamicRangeRetriever(object context);

        public delegate WoWPoint LocationRetriever(object context);

        public delegate bool SimpleBooleanDelegate(object context);

        public delegate WoWUnit UnitSelectionDelegate(object context);

        #endregion

        private static DateTime DoNotMove;

        private static Composite MovementMoveStop(UnitSelectionDelegate toUnit, double range)
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoMove &&
                !IsOverrideModeOn &&
                toUnit != null &&
                toUnit(ret) != null &&
                toUnit(ret) != Me &&
                toUnit(ret).IsAlive &&
                IsMoving(Me) &&
                IsEnemy(toUnit(ret)) &&
                GetDistance(toUnit(ret)) <= range &&
                InLineOfSpellSightCheck(toUnit(ret)),
                new Action(ret =>
                    {
                        Navigator.PlayerMover.MoveStop();
                        return RunStatus.Failure;
                    }));
        }

        private static Composite MovementMoveToLoS(UnitSelectionDelegate toUnit)
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoMove &&
                DateTime.Now > DoNotMove &&
                !Me.Mounted &&
                !IsOverrideModeOn &&
                !Me.IsCasting &&
                //!Me.IsChanneling &&
                toUnit != null &&
                toUnit(ret) != null &&
                toUnit(ret) != Me &&
                toUnit(ret).IsAlive &&
                IsEnemy(toUnit(ret)) &&
                (GetDistance(toUnit(ret)) > 30 ||
                 !InLineOfSpellSightCheck(toUnit(ret))),
                new Action(ret =>
                    {
                        Navigator.MoveTo(toUnit(ret).Location);
                        return RunStatus.Failure;
                    }));
        }

        private static Composite MovementMoveToMelee(UnitSelectionDelegate toUnit)
        {
            return new Decorator(
                ret =>
                THSettings.Instance.AutoMove &&
                DateTime.Now > DoNotMove &&
                !Me.Mounted &&
                !IsOverrideModeOn &&
                !Me.IsCasting &&
                //!Me.IsChanneling &&
                toUnit != null &&
                toUnit(ret) != null &&
                toUnit(ret) != Me &&
                toUnit(ret).IsAlive &&
                IsEnemy(toUnit(ret)) &&
                GetDistance(toUnit(ret)) > 3,
                new Action(ret =>
                    {
                        Navigator.MoveTo(toUnit(ret).Location);
                        return RunStatus.Failure;
                    }));
        }

        public Composite MovementMoveBehind(UnitSelectionDelegate toUnit)
        {
            return
                new Decorator(
                    ret =>
                    THSettings.Instance.AutoMove &&
                    DateTime.Now > DoNotMove &&
                    !Me.Mounted &&
                    !IsOverrideModeOn &&
                    !Me.IsCasting &&
                    //!Me.IsChanneling &&
                    toUnit != null &&
                    toUnit(ret) != null &&
                    toUnit(ret) != Me &&
                    toUnit(ret).IsAlive &&
                    //only MovementMoveBehind if IsWithinMeleeRange
                    GetDistance(toUnit(ret)) <= 5 &&
                    !Me.IsBehind(toUnit(ret)) &&
                    //!IsTank(Me) &&
                    //Only Move again After a certain delay or target move 3 yard from original posision
                    (toUnit(ret).IsPlayer ||
                     !toUnit(ret).IsPlayer && toUnit(ret).CurrentTarget != Me && toUnit(ret).Combat),
                    new Action(ret =>
                        {
                            WoWPoint pointBehind =
                                toUnit(ret).Location.RayCast(
                                    toUnit(ret).Rotation + WoWMathHelper.DegreesToRadians(150), 3f);

                            Navigator.MoveTo(pointBehind);
                            return RunStatus.Failure;
                        }));
        }
    }
}