using Styx.TreeSharp;


namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region RestorationRotation

        private static Composite RestorationRotation()
        {
            return new PrioritySelector(
                MovementMoveToLoS(ret => Me.CurrentTarget),
                MovementMoveStop(ret => Me.CurrentTarget, 10),
                TargetMyPetTarget(),
                new Action(delegate { return Me.Mounted ? RunStatus.Success : RunStatus.Failure; }),
                //MovementMoveToLoS(ret => UnitHeal),
                //MovementMoveStop(ret => UnitHeal, 10),
                GhostWolfHoldComp(),
                AscendanceResto(),
                SpiritwalkersGrace(),
                ElementalMastery(),
                AncestralGuidance(),
                Stormlash(),
                Windwalk(),
                StoneBulwarkTotem(),
                AstralShift(),
                TemporaryEnchantmentEnhancement(),
                PurifySpiritFriendlyASAPComp(),
                SpiritLink(),
                GreaterHealingWaveAS(),
                GroundingLow(),
                Hex(),
                Capacitor(),
                PurgeASAPResto(),
                FlameShockRogueDruid(),
                EarthElemental(),
                FireElemental(),
                HealingTideTotem(),
                HealingSurgeResIsSafetoCast(),
                GreaterHealingWaveIsSafetoCast(),
                PurifySpiritMyRoot(),
                HealingStreamTotem(),
                Riptide(),
                EarthShield(),
                UnleashElementsRes(),
                FrostShockNearby(),
                HealingRain(),
                ChainHeal(),
                HealingSurgeRes(),
                WaterShieldAlways(),
                GreaterHealingWave(),
                BindElemental(),
                ManaTideTotem(),
                //Riptide63273(),
                WaterShield(),
                Earthbind(),
                SearingTotemResto(),
                AttackResto(),
                HealingWave(),
                LightningBoltTelluricCurrents(),
                PurifySpiritFriendlyComp(),
                PurgeNormal(),
                HealingWaveTopUpRaid(),
                HealingWaveBaitInterrupt(),
                WaterWalking(),
                GhostWolfResto(),
                AutoRez()
                //WriteDebug("RestorationRotation is Under Development")
                );
        }

        #endregion
    }
}