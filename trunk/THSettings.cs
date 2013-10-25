using System;
using System.IO;
using Styx;
using Styx.Helpers;

namespace TuanHA_Combat_Routine
{
    public class THSettings : Settings
    {
        public static readonly THSettings Instance = new THSettings();

        #region DataSource

        //public readonly DataSet DCleanseASAP = new DataSet("CleanseASAP");

        //public string PathCastSpells = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        //                                            string.Format(
        //                                                @"Routines/shamanspecialedition/THListCastSpells.xml"));

        //public string PathChannelSpells = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        //                                               string.Format(
        //                                                   @"Routines/shamanspecialedition/THListChannelSpells.xml"));

        //public string PathCleanseASAP = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        //                                             string.Format(
        //                                                 @"Routines/shamanspecialedition/THListCleanseASAP.xml"));

        //public string PathCleanseDoNot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        //                                              string.Format(
        //                                                  @"Routines/shamanspecialedition/THListCleanseDoNot.xml"));

        //public string PathHealDoNot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
        //                                           string.Format(
        //                                               @"Routines/shamanspecialedition/THListHealDoNot.xml"));


        public bool UpdateStatus;

        private THSettings()
            : base(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                             string.Format(
                                 @"Routines/shamanspecialedition/TuanHA-Shaman-Settings-20130802-{0}.xml",
                                 StyxWoW.Me.Name)))
        {
            //DCleanseASAP.ReadXml(PathCleanseASAP);
        }

        #endregion

        #region General

        [Setting, DefaultValue(true)]
        public bool AncestralGuidanceCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool AncestralGuidanceBurst { get; set; }

        [Setting, DefaultValue(60)]
        public int AscendanceRestoHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool AncestralSwiftnessRes { get; set; }

        [Setting, DefaultValue(30)] //S
        public int AncestralSwiftnessResHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool AncestralSwiftnessLB { get; set; }

        [Setting, DefaultValue(true)]
        public bool AscendanceResto { get; set; }

        [Setting, DefaultValue(2)]
        public int AscendanceRestoUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool AscendanceEnhCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool AscendanceEnhBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool AscendanceEleCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool AscendanceEleBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool AstralShift { get; set; }

        [Setting, DefaultValue(40)]
        public int AstralShiftHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackResto { get; set; }

        [Setting, DefaultValue(70)]
        public int AttackRestoMana { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackRestoAny { get; set; }

        [Setting, DefaultValue(30)]
        public int AttackRestoAnyHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackRestoFlameShock { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackRestoEarthShock { get; set; }

        [Setting, DefaultValue(false)]
        public bool AttackRestoChainLightning { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackRestoElementalBlast { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackRestoLavaBurst { get; set; }

        [Setting, DefaultValue(false)]
        public bool AttackRestoLightningBolt { get; set; }

        [Setting, DefaultValue(true)]
        public bool AttackRestoLightningBoltGlyph { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoAoE { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoAttack { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoBuff { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoGhostWolf { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoGhostWolfCancel { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoMove { get; set; }

        [Setting, DefaultValue(false)]
        public bool AutoRez { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoRacial { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoTarget { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoFace { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoUseFood { get; set; }

        [Setting, DefaultValue(false)]
        public bool AutoWaterWalking { get; set; }

        [Setting, DefaultValue(50)] //S
        public int AutoUseFoodHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoWeaponImbues { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoWriteLog { get; set; }

        [Setting, DefaultValue(false)]
        public bool AutoSetFocus { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoAttackOutCombat { get; set; }

        [Setting, DefaultValue(true)]
        public bool AutoDetectManualCast { get; set; }

        [Setting, DefaultValue(100)]
        public int AutoDetectManualCastMS { get; set; }


        [Setting, DefaultValue(true)]
        public bool BattleStandard { get; set; }

        [Setting, DefaultValue(50)] //S
        public int BattleStandardHP { get; set; }

        [Setting, DefaultValue(19)] //S
        public int Backward { get; set; }

        [Setting, DefaultValue(false)]
        public bool BindElemental { get; set; }

        [Setting, DefaultValue(42)]
        public int BurstKey { get; set; }

        [Setting, DefaultValue(60)]
        public int BurstHP { get; set; }

        [Setting, DefaultValue(false)]
        public bool Burst { get; set; }

        [Setting, DefaultValue(true)]
        public bool CapacitorFriendLow { get; set; }

        [Setting, DefaultValue(40)]
        public int CapacitorFriendLowHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool CapacitorEnemyLow { get; set; }

        [Setting, DefaultValue(30)]
        public int CapacitorEnemyLowHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool CapacitorEnemyPack { get; set; }

        [Setting, DefaultValue(3)]
        public int CapacitorEnemyPackNumber { get; set; }

        [Setting, DefaultValue(true)]
        public bool CapacitorProjection { get; set; }

        [Setting, DefaultValue(300)]
        public int CapacitorProjectionMs { get; set; }

        [Setting, DefaultValue(true)]
        public bool ChainHeal { get; set; }

        [Setting, DefaultValue(3)]
        public int ChainHealUnit { get; set; }

        [Setting, DefaultValue(90)]
        public int ChainHealHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool ChainLightningEnh { get; set; }

        [Setting, DefaultValue(true)]
        public bool ChainLightningEle { get; set; }

        [Setting, DefaultValue(3)]
        public int ChainLightningEleUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool CleanseSpiritEnh { get; set; }

        [Setting, DefaultValue(true)]
        public bool CleanseSpiritEle { get; set; }

        [Setting, DefaultValue(true)]
        public bool PurifySpiritASAP { get; set; }

        [Setting, DefaultValue(true)]
        public bool PurifySpiritDebuff { get; set; }

        [Setting, DefaultValue(1)]
        public int PurifySpiritDebuffNumber { get; set; }

        [Setting, DefaultValue(95)]
        public int DoNotHealAbove { get; set; }

        [Setting, DefaultValue(true)]
        public bool EarthElementalCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool EarthElementalBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool ElementalBlastEle { get; set; }

        [Setting, DefaultValue(true)]
        public bool ElementalBlastEnh { get; set; }

        [Setting, DefaultValue(true)]
        public bool ElementalMasteryCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool ElementalMasteryBurst { get; set; }

        [Setting, DefaultValue(3)]
        public int ElementalBlastEnhStack { get; set; }

        [Setting, DefaultValue(true)]
        public bool Earthbind { get; set; }

        [Setting, DefaultValue(1)]
        public int EarthbindUnit { get; set; }

        [Setting, DefaultValue(20)]
        public int EarthbindDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool Earthquake { get; set; }

        [Setting, DefaultValue(5)]
        public int EarthquakeUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool EarthShock { get; set; }

        [Setting, DefaultValue(true)]
        public bool EarthShockElemental { get; set; }

        [Setting, DefaultValue(7)]
        public int EarthShockElementalCharge { get; set; }

        [Setting, DefaultValue(true)]
        public bool EarthShield { get; set; }

        [Setting, DefaultValue(false)]
        public bool EarthShieldAlways { get; set; }

        [Setting, DefaultValue(40)]
        public int EarthShieldAlwaysHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool FeralSpiritCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool FeralSpiritBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool FeralSpiritLow { get; set; }

        [Setting, DefaultValue(50)]
        public int FeralSpiritLowHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool FireElementalCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool FireElementalBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool FireNova { get; set; }

        [Setting, DefaultValue(false)]
        public bool FlagReturnorPickup { get; set; }

        [Setting, DefaultValue(false)]
        public bool FlameShockRogueDruid { get; set; }

        [Setting, DefaultValue(true)]
        public bool FlameShockEnh { get; set; }

        [Setting, DefaultValue(true)]
        public bool FlameShockEle { get; set; }

        [Setting, DefaultValue(30)]
        public int EleAoEMana { get; set; }

        [Setting, DefaultValue(false)]
        public bool FrostShockEnh { get; set; }

        [Setting, DefaultValue(6)]
        public int FrostShockEnhMinDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool FrostShockNearby { get; set; }

        [Setting, DefaultValue(true)]
        public bool FrostShockNearbyMelee { get; set; }

        [Setting, DefaultValue(false)]
        public bool FrostShockNearbyRange { get; set; }

        [Setting, DefaultValue(false)]
        public bool FrostShockNearbyHealer { get; set; }

        [Setting, DefaultValue(50)]
        public int FrostShockNearbyMana { get; set; }

        [Setting, DefaultValue(30)]
        public int FrostShockNearbyDist { get; set; }

        [Setting, DefaultValue(23)] //W
        public int Forward { get; set; }

        [Setting, DefaultValue(true)]
        public bool GreaterHealingWave { get; set; }

        [Setting, DefaultValue(60)]
        public int GreaterHealingWaveHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool GroundingLow { get; set; }

        [Setting, DefaultValue(60)]
        public int GroundingLowHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool GroundingCast { get; set; }

        [Setting, DefaultValue(3000)]
        public int GroundingCastMs { get; set; }

        [Setting, DefaultValue(true)]
        public bool GroundingTrap { get; set; }

        [Setting, DefaultValue(0)]
        public int HealBalancing { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingRain { get; set; }

        [Setting, DefaultValue(3)]
        public int HealingRainUnit { get; set; }

        [Setting, DefaultValue(90)]
        public int HealingRainHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingSurgeOutCombatEnh { get; set; }

        [Setting, DefaultValue(80)]
        public int HealingSurgeOutCombatEnhHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingSurgeOutCombatEle { get; set; }

        [Setting, DefaultValue(80)]
        public int HealingSurgeOutCombatEleHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingSurgeInCombatEnh { get; set; }

        [Setting, DefaultValue(3)]
        public int HealingSurgeInCombatEnhStack { get; set; }

        [Setting, DefaultValue(50)]
        public int HealingSurgeInCombatEnhHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingSurgeInCombatEnhFriend { get; set; }

        [Setting, DefaultValue(3)]
        public int HealingSurgeInCombatEnhStackFriend { get; set; }

        [Setting, DefaultValue(30)]
        public int HealingSurgeInCombatEnhHPFriend { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingSurgeInCombatEle { get; set; }

        [Setting, DefaultValue(false)]
        public bool HealingSurgeInCombatEleCC { get; set; }

        [Setting, DefaultValue(50)]
        public int HealingSurgeInCombatEleHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingSurgeInCombatEleFriend { get; set; }

        [Setting, DefaultValue(false)]
        public bool HealingSurgeInCombatEleFriendCC { get; set; }

        [Setting, DefaultValue(30)]
        public int HealingSurgeInCombatEleFriendHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingSurgeRes { get; set; }

        [Setting, DefaultValue(40)]
        public int HealingSurgeResHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingStreamTotem { get; set; }

        [Setting, DefaultValue(3)]
        public int HealingStreamTotemUnit { get; set; }

        [Setting, DefaultValue(80)]
        public int HealingStreamTotemHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingTideTotem { get; set; }

        [Setting, DefaultValue(3)]
        public int HealingTideTotemUnit { get; set; }

        [Setting, DefaultValue(60)]
        public int HealingTideTotemHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealthStone { get; set; }

        [Setting, DefaultValue(40)]
        public int HealthStoneHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool HealingWave { get; set; }

        [Setting, DefaultValue(80)]
        public int HealingWaveHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool Hex { get; set; }

        [Setting, DefaultValue(true)]
        public bool HexHealer { get; set; }

        [Setting, DefaultValue(true)]
        public bool HexDPS { get; set; }

        //Hotkey1
        [Setting, DefaultValue(0)]
        public int Hotkey1Target { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey1Mod { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey1Key { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey1Spell { get; set; }

        //Hotkey2
        [Setting, DefaultValue(0)]
        public int Hotkey2Target { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey2Mod { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey2Key { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey2Spell { get; set; }

        //Hotkey3
        [Setting, DefaultValue(0)]
        public int Hotkey3Target { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey3Mod { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey3Key { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey3Spell { get; set; }

        //Hotkey4
        [Setting, DefaultValue(0)]
        public int Hotkey4Target { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey4Mod { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey4Key { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey4Spell { get; set; }

        //Hotkey5
        [Setting, DefaultValue(0)]
        public int Hotkey5Target { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey5Mod { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey5Key { get; set; }

        [Setting, DefaultValue(0)]
        public int Hotkey5Spell { get; set; }

        [Setting, DefaultValue(true)]
        public bool InterruptAll { get; set; }

        [Setting, DefaultValue(false)]
        public bool InterruptTarget { get; set; }

        [Setting, DefaultValue(false)]
        public bool InterruptFocus { get; set; }

        [Setting, DefaultValue(true)]
        public bool LagTolerance { get; set; }

        [Setting, DefaultValue(true)]
        public bool LavaBustElemental { get; set; }

        [Setting, DefaultValue(true)]
        public bool LavaLash { get; set; }

        [Setting, DefaultValue(true)]
        public bool LightningBolt { get; set; }

        [Setting, DefaultValue(true)]
        public bool LightningBoltEnh { get; set; }

        [Setting, DefaultValue(5)]
        public int LightningBoltEnhMaelstromStack { get; set; }

        [Setting, DefaultValue(true)]
        public bool LightningBoltFiller { get; set; }

        [Setting, DefaultValue(3)]
        public int LightningBoltFillerMaelstromStack { get; set; }

        [Setting, DefaultValue(true)]
        public bool LightningBoltFillerElemental { get; set; }

        [Setting, DefaultValue(true)]
        public bool LightningShield { get; set; }

        [Setting, DefaultValue(true)]
        public bool ManaTideTotem { get; set; }

        [Setting, DefaultValue(30)]
        public int ManaTideTotemMN { get; set; }

        [Setting, DefaultValue(true)]
        public bool MagmaTotemEnh { get; set; }

        [Setting, DefaultValue(5)]
        public int MagmaTotemEnhUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool MagmaTotemEle { get; set; }

        [Setting, DefaultValue(5)]
        public int MagmaTotemEleUnit { get; set; }

        [Setting, DefaultValue(false)]
        public bool Pause { get; set; }

        [Setting, DefaultValue(75)]
        public int PriorityHeal { get; set; }

        [Setting, DefaultValue(true)]
        public bool PurgeASAP { get; set; }

        [Setting, DefaultValue(30)]
        public int PurgeASAPMana { get; set; }

        [Setting, DefaultValue(true)]
        public bool PurgeNormal { get; set; }

        [Setting, DefaultValue(80)]
        public int PurgeNormalMana { get; set; }

        [Setting, DefaultValue(false)]
        public bool OnlyHealMe { get; set; }

        [Setting, DefaultValue(false)]
        public bool OnlyHealTarget { get; set; }

        [Setting, DefaultValue(false)]
        public bool OnlyHealFocus { get; set; }

        [Setting, DefaultValue(16)] //P
        public int PauseKey { get; set; }

        [Setting, DefaultValue(true)]
        public bool PauseKeyUse { get; set; }

        [Setting, DefaultValue(0)]
        public int ProfBuff { get; set; }

        [Setting, DefaultValue(60)]
        public int ProfBuffHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool Riptide { get; set; }

        [Setting, DefaultValue(99)]
        public int RiptideHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShamanisticRageHP { get; set; }

        [Setting, DefaultValue(40)]
        public int ShamanisticRageHPHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool ShamanisticRageMN { get; set; }

        [Setting, DefaultValue(30)]
        public int ShamanisticRageMNMN { get; set; }


        [Setting, DefaultValue(true)]
        public bool ShamanisticCC { get; set; }

        [Setting, DefaultValue(2000)]
        public int ShamanisticCCDuration { get; set; }

        [Setting, DefaultValue(true)]
        public bool SearingTotem { get; set; }

        [Setting, DefaultValue(25)]
        public int SearingTotemDistance { get; set; }

        [Setting, DefaultValue(true)]
        public bool SpiritLink { get; set; }

        [Setting, DefaultValue(2)]
        public int SpiritLinkUnit { get; set; }

        [Setting, DefaultValue(40)]
        public int SpiritLinkHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool SpiritwalkersGrace { get; set; }

        [Setting, DefaultValue(50)]
        public int SpiritwalkersGraceHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool StoneBulwarkTotem { get; set; }

        [Setting, DefaultValue(40)]
        public int StoneBulwarkTotemHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool StormlashCooldown { get; set; }

        [Setting, DefaultValue(true)]
        public bool Stormstrike { get; set; }

        [Setting, DefaultValue(true)]
        public bool StormlashBurst { get; set; }

        [Setting, DefaultValue(true)]
        public bool StormlashEnemy { get; set; }

        [Setting, DefaultValue(40)]
        public int StormlashEnemyHP { get; set; }

        [Setting, DefaultValue(17)] //Q
        public int StrafleLeft { get; set; }

        [Setting, DefaultValue(5)] //E
        public int StrafleRight { get; set; }

        [Setting, DefaultValue(true)]
        public bool Tremor { get; set; }

        [Setting, DefaultValue(true)]
        public bool ThunderstormMelee { get; set; }

        [Setting, DefaultValue(1)]
        public int ThunderstormMeleeUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool Thunderstorm { get; set; }

        [Setting, DefaultValue(5)]
        public int ThunderstormUnit { get; set; }

        [Setting, DefaultValue(true)]
        public bool ThunderstormCast { get; set; }

        [Setting, DefaultValue(3000)]
        public int ThunderstormCastMs { get; set; }

        [Setting, DefaultValue(3000)]
        public int TremorDuration { get; set; }

        [Setting, DefaultValue(true)]
        public bool ThunderstormMana { get; set; }

        [Setting, DefaultValue(50)]
        public int ThunderstormManaMN { get; set; }

        [Setting, DefaultValue(true)]
        public bool TremorMe { get; set; }

        [Setting, DefaultValue(true)]
        public bool TremorHealer { get; set; }

        [Setting, DefaultValue(true)]
        public bool TremorDPS { get; set; }

        [Setting, DefaultValue(1)] //A
        public int TurnRight { get; set; }

        [Setting, DefaultValue(4)] //D
        public int TurnLeft { get; set; }

        [Setting, DefaultValue(0)]
        public int Trinket1 { get; set; }

        [Setting, DefaultValue(60)]
        public int Trinket1HP { get; set; }

        [Setting, DefaultValue(0)]
        public int Trinket2 { get; set; }

        [Setting, DefaultValue(60)]
        public int Trinket2HP { get; set; }

        [Setting, DefaultValue(40)]
        public int UrgentHeal { get; set; }

        [Setting, DefaultValue(3)]
        public int UnittoStartAoE { get; set; }

        [Setting, DefaultValue(true)]
        public bool UnleashElementsEle { get; set; }

        [Setting, DefaultValue(true)]
        public bool UnleashElementsEnh { get; set; }

        [Setting, DefaultValue(true)]
        public bool UnleashElementsRes { get; set; }

        [Setting, DefaultValue(40)]
        public int UnleashElementsResHP { get; set; }

        [Setting, DefaultValue(true)]
        public bool UnleashElementsCH { get; set; }

        [Setting, DefaultValue(true)]
        public bool UnleashElementsGHW { get; set; }

        [Setting, DefaultValue(true)]
        public bool UnleashElementsHR { get; set; }

        [Setting, DefaultValue(true)]
        public bool UnleashElementsHS { get; set; }

        [Setting, DefaultValue(true)]
        public bool WaterShield { get; set; }

        [Setting, DefaultValue(true)]
        public bool WaterShieldAlways { get; set; }

        [Setting, DefaultValue(30)]
        public int WaterShieldAlwaysMana { get; set; }

        [Setting, DefaultValue(1)]
        public int WeaponMainHand { get; set; }

        [Setting, DefaultValue(2)]
        public int WeaponOffHand { get; set; }

        [Setting, DefaultValue(true)]
        public bool WindShearInterrupt { get; set; }

        [Setting, DefaultValue(1000)]
        public int WindShearInterruptMs { get; set; }

        [Setting, DefaultValue(true)]
        public bool WindwalkRootMe { get; set; }

        [Setting, DefaultValue(false)]
        public bool WindwalkRootFriend { get; set; }

        #endregion

        #region Group Healing

        [Setting, DefaultValue(true)]
        public bool Group1 { get; set; }

        [Setting, DefaultValue(true)]
        public bool Group2 { get; set; }

        [Setting, DefaultValue(true)]
        public bool Group3 { get; set; }

        [Setting, DefaultValue(true)]
        public bool Group4 { get; set; }

        [Setting, DefaultValue(true)]
        public bool Group5 { get; set; }

        [Setting, DefaultValue(true)]
        public bool Group6 { get; set; }

        [Setting, DefaultValue(true)]
        public bool Group7 { get; set; }

        [Setting, DefaultValue(true)]
        public bool Group8 { get; set; }

        #endregion
    }
}