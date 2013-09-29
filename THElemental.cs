using System;
using Styx.TreeSharp;
using Action = Styx.TreeSharp.Action;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region ElementalRotation

        private static Composite ElementalRotation()
        {
            return new PrioritySelector(
                new Action(delegate
                    {
                        if (LastAoESearch > DateTime.Now)
                        {
                            return RunStatus.Failure;
                        }

                        if (THSettings.Instance.AutoAoE &&
                            LastAoESearch < DateTime.Now &&
                            CurrentTargetAttackable(40) &&
                            CountEnemyNear(Me.CurrentTarget, 10) >= THSettings.Instance.UnittoStartAoE)
                        {
                            AoEModeOn = true;
                        }
                        else
                        {
                            AoEModeOn = false;
                        }

                        LastAoESearch = DateTime.Now + TimeSpan.FromMilliseconds(5000);
                        return RunStatus.Failure;
                    }),
                AutoTargetRange(),
                MovementMoveToLoS(ret => Me.CurrentTarget),
                MovementMoveStop(ret => Me.CurrentTarget, 30),
                TargetMyPetTarget(),
                //DPS Rotation Here
                GhostWolfHoldComp(),
                //Single Target Rotation
                new Decorator(
                    ret => !AoEModeOn && !Me.Mounted,
                    new PrioritySelector(
                        //SetAutoAttack(),
                        ShamanisticRage(),
                        AscendanceEle(),
                        ElementalMastery(),
                        AncestralGuidance(),
                        Windwalk(),
                        Stormlash(),
                        Hex(),
                        StoneBulwarkTotem(),
                        AstralShift(),
                        GroundingLow(),
                        Capacitor(),
                        ThunderstormCast(),
                        CleanseSpiritFriendlyASAPEle(),
                        ThunderstormMelee(),
                        HealingTideTotem(),
                        LightningShield(),
                        TemporaryEnchantmentEnhancement(),
                        FlameShockEle(),
                        LavaBustElementalProc(),
                        FlameShockRogueDruid(),
                        FireElemental(),
                        EarthElemental(),
                        HealingStreamTotem(),
                        Thunderstorm(),
                        FrostShockNearby(),
                        UnleashElementsEle(),
                        EarthShockElementalPvP(),
                        Earthbind(),
                        LavaBustElemental(),
                        ElementalBlastEle(),
                        PurgeASAPEleEnh(),
                        EarthShockElemental(),
                        BindElemental(),
                        SearingTotem(),
                        FlameShockAoEEle(),
                        HealingSurgeInCombatEle(),
                        ChainLightningEle(),
                        ThunderstormMana(),
                        LightningBoltFillerElemental(),
                        CleanseSpiritFriendlyEle(),
                        HealingSurgeOutCombatEle(),
                        WaterWalking(),
                        GhostWolfEle(),
                        AutoRez()
                        )),
                //AoE Target Rotation
                new Decorator(
                    ret => AoEModeOn && !Me.Mounted,
                    new PrioritySelector(
                        //SetAutoAttack(),
                        ShamanisticRage(),
                        AscendanceEle(),
                        ElementalMastery(),
                        AncestralGuidance(),
                        Windwalk(),
                        Stormlash(),
                        StoneBulwarkTotem(),
                        AstralShift(),
                        GroundingLow(),
                        Capacitor(),
                        ThunderstormCast(),
                        CleanseSpiritFriendlyASAPEle(),
                        ThunderstormMelee(),
                        EarthElemental(),
                        FireElemental(),
                        HealingTideTotem(),
                        HealingStreamTotem(),
                        MagmaTotemEle(),
                        Earthquake(),
                        Thunderstorm(),
                        LavaBustElementalProc(),
                        FlameShockEle(),
                        EarthShockElemental(),
                        FlameShockAoEEle(),
                        Earthbind(),
                        ChainLightningEleAoE(),
                        //LavaBustElemental(),
                        HealingSurgeInCombatEle(),
                        ElementalBlastEle(),
                        BindElemental(),
                        ThunderstormMana(),
                        LightningBoltFillerElementalAoE(),
                        Hex(),
                        CleanseSpiritFriendlyEle(),
                        HealingSurgeOutCombatEle()
                        )),
                WriteDebug("")
                );
        }

        #endregion
    }
}