using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Styx;
using Styx.Common;

namespace TuanHA_Combat_Routine
{
    public partial class THForm : Form
    {
        public THForm()
        {
            InitializeComponent();
        }

        private static int TalentValue(int holy, int protection, int retribution)
        {
            return Classname.GetCurrentSpec() == "Holy"
                       ? holy
                       : (Classname.GetCurrentSpec() == "Protection" ? protection : retribution);
        }

        public bool TalentValue(bool holy, bool protection, bool retribution)
        {
            return Classname.GetCurrentSpec() == "Holy"
                       ? holy
                       : (Classname.GetCurrentSpec() == "Protection" ? protection : retribution);
        }

        private void THForm_Load(object sender, EventArgs e)
        {
            #region TabPages.Remove

            if (StyxWoW.Me.Specialization == WoWSpec.ShamanElemental)
            {
                if (!StyxWoW.Me.Name.Contains("ustt"))
                {
                    tabControl1.TabPages.Remove(tabPage3);
                    tabControl1.TabPages.Remove(tabPage5);
                }
            }
            else if (StyxWoW.Me.Specialization == WoWSpec.ShamanRestoration)
            {
                if (!StyxWoW.Me.Name.Contains("ustt"))
                {
                    tabControl1.TabPages.Remove(tabPage4);
                    tabControl1.TabPages.Remove(tabPage5);
                }
            }
            else
            {
                if (!StyxWoW.Me.Name.Contains("ustt"))
                {
                    tabControl1.TabPages.Remove(tabPage3);
                    tabControl1.TabPages.Remove(tabPage4);
                }
            }

            #endregion

            #region Load Files

            if (
                File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHAShamanPublicRelease\TuanHA-Shaman-Picture.jpg"))
            {
                pictureBox1.ImageLocation = Utilities.AssemblyDirectory +
                                            @"\Routines\TuanHAShamanPublicRelease\TuanHA-Shaman-Picture.jpg";
            }

            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHAShamanPublicRelease\SpecialThanks.rtf"))
            {
                richTextBox1.LoadFile(Utilities.AssemblyDirectory +
                                      @"\Routines\TuanHAShamanPublicRelease\SpecialThanks.rtf");
            }

            #endregion

            #region Load General

            AncestralGuidanceCooldown.Checked = THSettings.Instance.AncestralGuidanceCooldown;
            AncestralGuidanceBurst.Checked = THSettings.Instance.AncestralGuidanceBurst;
            AscendanceEleCooldown.Checked = THSettings.Instance.AscendanceEleCooldown;
            AscendanceEleBurst.Checked = THSettings.Instance.AscendanceEleBurst;
            AscendanceEnhCooldown.Checked = THSettings.Instance.AscendanceEnhCooldown;
            AscendanceEnhBurst.Checked = THSettings.Instance.AscendanceEnhBurst;
            AscendanceResto.Checked = THSettings.Instance.AscendanceResto;
            AscendanceRestoUnit.Value = THSettings.Instance.AscendanceRestoUnit;
            AscendanceRestoHP.Value = THSettings.Instance.AscendanceRestoHP;
            AncestralSwiftnessRes.Checked = THSettings.Instance.AncestralSwiftnessRes;
            AncestralSwiftnessResHP.Value = THSettings.Instance.AncestralSwiftnessResHP;
            AstralShift.Checked = THSettings.Instance.AstralShift;
            AstralShiftHP.Value = THSettings.Instance.AstralShiftHP;
            AttackResto.Checked = THSettings.Instance.AttackResto;
            AttackRestoMana.Value = THSettings.Instance.AttackRestoMana;
            AttackRestoAny.Checked = THSettings.Instance.AttackRestoAny;
            AttackRestoAnyHP.Value = THSettings.Instance.AttackRestoAnyHP;
            AttackRestoFlameShock.Checked = THSettings.Instance.AttackRestoFlameShock;
            AttackRestoEarthShock.Checked = THSettings.Instance.AttackRestoEarthShock;
            AttackRestoChainLightning.Checked = THSettings.Instance.AttackRestoChainLightning;
            AttackRestoLavaBurst.Checked = THSettings.Instance.AttackRestoLavaBurst;
            AttackRestoLightningBolt.Checked = THSettings.Instance.AttackRestoLightningBolt;
            AttackRestoLightningBoltGlyph.Checked = THSettings.Instance.AttackRestoLightningBoltGlyph;
            AutoAoE.Checked = THSettings.Instance.AutoAoE;
            AutoBuff.Checked = THSettings.Instance.AutoBuff;
            AutoUseFood.Checked = THSettings.Instance.AutoUseFood;
            AutoUseFoodHP.Value = THSettings.Instance.AutoUseFoodHP;
            AutoFace.Checked = THSettings.Instance.AutoFace;
            AutoGhostWolf.Checked = THSettings.Instance.AutoGhostWolf;
            AutoGhostWolfCancel.Checked = THSettings.Instance.AutoGhostWolfCancel;
            AutoMove.Checked = THSettings.Instance.AutoMove;
            AutoRacial.Checked = THSettings.Instance.AutoRacial;
            AutoRez.Checked = THSettings.Instance.AutoRez;
            AutoTarget.Checked = THSettings.Instance.AutoTarget;
            AutoAttackOutCombat.Checked = THSettings.Instance.AutoAttackOutCombat;
            AutoSetFocus.Checked = THSettings.Instance.AutoSetFocus;
            AutoWaterWalking.Checked = THSettings.Instance.AutoWaterWalking;
            AutoWriteLog.Checked = THSettings.Instance.AutoWriteLog;
            Backward.SelectedIndex = THSettings.Instance.Backward;
            BattleStandard.Checked = THSettings.Instance.BattleStandard;
            BattleStandardHP.Value = THSettings.Instance.BattleStandardHP;
            BindElemental.Checked = THSettings.Instance.BindElemental;
            BurstHP.Value = THSettings.Instance.BurstHP;
            BurstKey.SelectedIndex = THSettings.Instance.BurstKey;
            CapacitorFriendLow.Checked = THSettings.Instance.CapacitorFriendLow;
            CapacitorFriendLowHP.Value = THSettings.Instance.CapacitorFriendLowHP;
            CapacitorEnemyLow.Checked = THSettings.Instance.CapacitorEnemyLow;
            CapacitorEnemyLowHP.Value = THSettings.Instance.CapacitorEnemyLowHP;
            CapacitorEnemyPack.Checked = THSettings.Instance.CapacitorEnemyPack;
            CapacitorEnemyPackNumber.Value = THSettings.Instance.CapacitorEnemyPackNumber;
            CapacitorProjection.Checked = THSettings.Instance.CapacitorProjection;
            CapacitorProjectionMs.Value = THSettings.Instance.CapacitorProjectionMs;
            ChainHeal.Checked = THSettings.Instance.ChainHeal;
            ChainHealUnit.Value = THSettings.Instance.ChainHealUnit;
            ChainHealHP.Value = THSettings.Instance.ChainHealHP;
            ChainLightningEle.Checked = THSettings.Instance.ChainLightningEle;
            ChainLightningEleUnit.Value = THSettings.Instance.ChainLightningEleUnit;
            CleanseSpiritEnh.Checked = THSettings.Instance.CleanseSpiritEnh;
            CleanseSpiritEle.Checked = THSettings.Instance.CleanseSpiritEle;
            PurifySpiritASAP.Checked = THSettings.Instance.PurifySpiritASAP;
            PurifySpiritDebuff.Checked = THSettings.Instance.PurifySpiritDebuff;
            PurifySpiritDebuffNumber.Value = THSettings.Instance.PurifySpiritDebuffNumber;
            ElementalBlastEnh.Checked = THSettings.Instance.ElementalBlastEnh;
            ElementalBlastEnhStack.Value = THSettings.Instance.ElementalBlastEnhStack;
            Earthbind.Checked = THSettings.Instance.Earthbind;
            EarthbindUnit.Value = THSettings.Instance.EarthbindUnit;
            EarthbindDistance.Value = THSettings.Instance.EarthbindDistance;
            EarthElementalCooldown.Checked = THSettings.Instance.EarthElementalCooldown;
            EarthElementalBurst.Checked = THSettings.Instance.EarthElementalBurst;
            Earthquake.Checked = THSettings.Instance.Earthquake;
            EarthquakeUnit.Value = THSettings.Instance.EarthquakeUnit;
            EarthShield.Checked = THSettings.Instance.EarthShield;
            EarthShieldAlways.Checked = THSettings.Instance.EarthShieldAlways;
            EarthShieldAlwaysHP.Value = THSettings.Instance.EarthShieldAlwaysHP;
            EarthShockElemental.Checked = THSettings.Instance.EarthShockElemental;
            EarthShockElementalCharge.Value = THSettings.Instance.EarthShockElementalCharge;
            ElementalMasteryCooldown.Checked = THSettings.Instance.ElementalMasteryCooldown;
            ElementalMasteryBurst.Checked = THSettings.Instance.ElementalMasteryBurst;
            FeralSpiritCooldown.Checked = THSettings.Instance.FeralSpiritCooldown;
            FeralSpiritBurst.Checked = THSettings.Instance.FeralSpiritBurst;
            FeralSpiritLow.Checked = THSettings.Instance.FeralSpiritLow;
            FeralSpiritLowHP.Value = THSettings.Instance.FeralSpiritLowHP;
            FireElementalCooldown.Checked = THSettings.Instance.FireElementalCooldown;
            FireElementalBurst.Checked = THSettings.Instance.FireElementalBurst;
            Forward.SelectedIndex = THSettings.Instance.Forward;
            FlagReturnorPickup.Checked = THSettings.Instance.FlagReturnorPickup;
            FlameShockRogueDruid.Checked = THSettings.Instance.FlameShockRogueDruid;
            FlameShockAoEEleMana.Value = THSettings.Instance.EleAoEMana;
            FrostShockEnh.Checked = THSettings.Instance.FrostShockEnh;
            FrostShockEnhMinDistance.Value = THSettings.Instance.FrostShockEnhMinDistance;
            FrostShockNearby.Checked = THSettings.Instance.FrostShockNearby;
            FrostShockNearbyMelee.Checked = THSettings.Instance.FrostShockNearbyMelee;
            FrostShockNearbyRange.Checked = THSettings.Instance.FrostShockNearbyRange;
            FrostShockNearbyHealer.Checked = THSettings.Instance.FrostShockNearbyHealer;
            FrostShockNearbyMana.Value = THSettings.Instance.FrostShockNearbyMana;
            FrostShockNearbyDist.Value = THSettings.Instance.FrostShockNearbyDist;
            GreaterHealingWave.Checked = THSettings.Instance.GreaterHealingWave;
            GreaterHealingWaveHP.Value = THSettings.Instance.GreaterHealingWaveHP;
            Group1.Checked = THSettings.Instance.Group1;
            Group2.Checked = THSettings.Instance.Group2;
            Group3.Checked = THSettings.Instance.Group3;
            Group4.Checked = THSettings.Instance.Group4;
            Group5.Checked = THSettings.Instance.Group5;
            Group6.Checked = THSettings.Instance.Group6;
            Group7.Checked = THSettings.Instance.Group7;
            Group8.Checked = THSettings.Instance.Group8;
            GroundingLow.Checked = THSettings.Instance.GroundingLow;
            GroundingLowHP.Value = THSettings.Instance.GroundingLowHP;
            GroundingCast.Checked = THSettings.Instance.GroundingCast;
            GroundingCastMs.Value = THSettings.Instance.GroundingCastMs;
            GroundingTrap.Checked = THSettings.Instance.GroundingTrap;
            HealBalancing.Value = THSettings.Instance.HealBalancing;
            HealingRain.Checked = THSettings.Instance.HealingRain;
            HealingRainUnit.Value = THSettings.Instance.HealingRainUnit;
            HealingRainHP.Value = THSettings.Instance.HealingRainHP;
            HealingStreamTotem.Checked = THSettings.Instance.HealingStreamTotem;
            HealingStreamTotemUnit.Value = THSettings.Instance.HealingStreamTotemUnit;
            HealingStreamTotemHP.Value = THSettings.Instance.HealingStreamTotemHP;
            HealingSurgeOutCombatEnh.Checked = THSettings.Instance.HealingSurgeOutCombatEnh;
            HealingSurgeOutCombatEnhHP.Value = THSettings.Instance.HealingSurgeOutCombatEnhHP;
            HealingSurgeInCombatEnhFriend.Checked = THSettings.Instance.HealingSurgeInCombatEnhFriend;
            HealingSurgeInCombatEnhStackFriend.Value = THSettings.Instance.HealingSurgeInCombatEnhStackFriend;
            HealingSurgeInCombatEnhHPFriend.Value = THSettings.Instance.HealingSurgeInCombatEnhHPFriend;
            HealingSurgeInCombatEnh.Checked = THSettings.Instance.HealingSurgeInCombatEnh;
            HealingSurgeInCombatEnhStack.Value = THSettings.Instance.HealingSurgeInCombatEnhStack;
            HealingSurgeInCombatEnhHP.Value = THSettings.Instance.HealingSurgeInCombatEnhHP;
            HealingSurgeOutCombatEle.Checked = THSettings.Instance.HealingSurgeOutCombatEle;
            HealingSurgeOutCombatEleHP.Value = THSettings.Instance.HealingSurgeOutCombatEleHP;
            HealingSurgeInCombatEle.Checked = THSettings.Instance.HealingSurgeInCombatEle;
            HealingSurgeInCombatEleCC.Checked = THSettings.Instance.HealingSurgeInCombatEleCC;
            HealingSurgeInCombatEleHP.Value = THSettings.Instance.HealingSurgeInCombatEleHP;
            HealingSurgeInCombatEleFriend.Checked = THSettings.Instance.HealingSurgeInCombatEleFriend;
            HealingSurgeInCombatEleFriendCC.Checked = THSettings.Instance.HealingSurgeInCombatEleFriendCC;
            HealingSurgeInCombatEleFriendHP.Value = THSettings.Instance.HealingSurgeInCombatEleFriendHP;
            HealingSurgeRes.Checked = THSettings.Instance.HealingSurgeRes;
            HealingSurgeResHP.Value = THSettings.Instance.HealingSurgeResHP;
            HealingWave.Checked = THSettings.Instance.HealingWave;
            HealingWaveHP.Value = THSettings.Instance.HealingWaveHP;
            HealthStone.Checked = THSettings.Instance.HealthStone;
            HealthStoneHP.Value = THSettings.Instance.HealthStoneHP;
            HealingTideTotem.Checked = THSettings.Instance.HealingTideTotem;
            HealingTideTotemUnit.Value = THSettings.Instance.HealingTideTotemUnit;
            HealingTideTotemHP.Value = THSettings.Instance.HealingTideTotemHP;
            Hex.Checked = THSettings.Instance.Hex;
            HexHealer.Checked = THSettings.Instance.HexHealer;
            HexDPS.Checked = THSettings.Instance.HexDPS;
            //Hotkey1
            Hotkey1Target.SelectedIndex = THSettings.Instance.Hotkey1Target;
            Hotkey1Mod.SelectedIndex = THSettings.Instance.Hotkey1Mod;
            Hotkey1Key.SelectedIndex = THSettings.Instance.Hotkey1Key;
            Hotkey1Spell.SelectedIndex = THSettings.Instance.Hotkey1Spell;
            //Hotkey2
            Hotkey2Target.SelectedIndex = THSettings.Instance.Hotkey2Target;
            Hotkey2Mod.SelectedIndex = THSettings.Instance.Hotkey2Mod;
            Hotkey2Key.SelectedIndex = THSettings.Instance.Hotkey2Key;
            Hotkey2Spell.SelectedIndex = THSettings.Instance.Hotkey2Spell;
            //Hotkey3
            Hotkey3Target.SelectedIndex = THSettings.Instance.Hotkey3Target;
            Hotkey3Mod.SelectedIndex = THSettings.Instance.Hotkey3Mod;
            Hotkey3Key.SelectedIndex = THSettings.Instance.Hotkey3Key;
            Hotkey3Spell.SelectedIndex = THSettings.Instance.Hotkey3Spell;
            //Hotkey4
            Hotkey4Target.SelectedIndex = THSettings.Instance.Hotkey4Target;
            Hotkey4Mod.SelectedIndex = THSettings.Instance.Hotkey4Mod;
            Hotkey4Key.SelectedIndex = THSettings.Instance.Hotkey4Key;
            Hotkey4Spell.SelectedIndex = THSettings.Instance.Hotkey4Spell;
            //Hotkey5
            Hotkey5Target.SelectedIndex = THSettings.Instance.Hotkey5Target;
            Hotkey5Mod.SelectedIndex = THSettings.Instance.Hotkey5Mod;
            Hotkey5Key.SelectedIndex = THSettings.Instance.Hotkey5Key;
            Hotkey5Spell.SelectedIndex = THSettings.Instance.Hotkey5Spell;

            InterruptAll.Checked = THSettings.Instance.InterruptAll;
            InterruptTarget.Checked = THSettings.Instance.InterruptTarget;
            InterruptFocus.Checked = THSettings.Instance.InterruptFocus;
            label66.Text = "" + THSettings.Instance.HealBalancing;
            LightningBoltEnh.Checked = THSettings.Instance.LightningBoltEnh;
            LightningBoltEnhMaelstromStack.Value = THSettings.Instance.LightningBoltEnhMaelstromStack;
            LightningBoltFiller.Checked = THSettings.Instance.LightningBoltFiller;
            LightningBoltFillerMaelstromStack.Value = THSettings.Instance.LightningBoltFillerMaelstromStack;
            MagmaTotemEle.Checked = THSettings.Instance.MagmaTotemEle;
            MagmaTotemEleUnit.Value = THSettings.Instance.MagmaTotemEleUnit;
            ManaTideTotem.Checked = THSettings.Instance.ManaTideTotem;
            ManaTideTotemMN.Value = THSettings.Instance.ManaTideTotemMN;
            OnlyHealFocus.Checked = THSettings.Instance.OnlyHealFocus;
            OnlyHealMe.Checked = THSettings.Instance.OnlyHealMe;
            OnlyHealTarget.Checked = THSettings.Instance.OnlyHealTarget;
            PauseKey.SelectedIndex = THSettings.Instance.PauseKey;
            ProfBuff.SelectedIndex = THSettings.Instance.ProfBuff;
            ProfBuffHP.Value = THSettings.Instance.ProfBuffHP;
            PurgeASAP.Checked = THSettings.Instance.PurgeASAP;
            PurgeASAPMana.Value = THSettings.Instance.PurgeASAPMana;
            PurgeNormal.Checked = THSettings.Instance.PurgeNormal;
            PurgeNormalMana.Value = THSettings.Instance.PurgeNormalMana;
            Riptide.Checked = THSettings.Instance.Riptide;
            RiptideHP.Value = THSettings.Instance.RiptideHP;
            SearingTotem.Checked = THSettings.Instance.SearingTotem;
            SearingTotemDistance.Value = THSettings.Instance.SearingTotemDistance;
            ShamanisticRageHP.Checked = THSettings.Instance.ShamanisticRageHP;
            ShamanisticRageHPHP.Value = THSettings.Instance.ShamanisticRageHPHP;
            ShamanisticRageMN.Checked = THSettings.Instance.ShamanisticRageMN;
            ShamanisticRageMNMN.Value = THSettings.Instance.ShamanisticRageMNMN;
            ShamanisticCC.Checked = THSettings.Instance.ShamanisticCC;
            ShamanisticCCDuration.Value = THSettings.Instance.ShamanisticCCDuration;
            SpiritLink.Checked = THSettings.Instance.SpiritLink;
            SpiritLinkUnit.Value = THSettings.Instance.SpiritLinkUnit;
            SpiritLinkHP.Value = THSettings.Instance.SpiritLinkHP;
            SpiritwalkersGrace.Checked = THSettings.Instance.SpiritwalkersGrace;
            SpiritwalkersGraceHP.Value = THSettings.Instance.SpiritwalkersGraceHP;
            StormlashCooldown.Checked = THSettings.Instance.StormlashCooldown;
            StormlashBurst.Checked = THSettings.Instance.StormlashBurst;
            StormlashEnemy.Checked = THSettings.Instance.StormlashEnemy;
            StormlashEnemyHP.Value = THSettings.Instance.StormlashEnemyHP;
            StoneBulwarkTotem.Checked = THSettings.Instance.StoneBulwarkTotem;
            StoneBulwarkTotemHP.Value = THSettings.Instance.StoneBulwarkTotemHP;
            StrafleLeft.SelectedIndex = THSettings.Instance.StrafleLeft;
            StrafleRight.SelectedIndex = THSettings.Instance.StrafleRight;
            Thunderstorm.Checked = THSettings.Instance.Thunderstorm;
            ThunderstormUnit.Value = THSettings.Instance.ThunderstormUnit;
            ThunderstormCast.Checked = THSettings.Instance.ThunderstormCast;
            ThunderstormCastMs.Value = THSettings.Instance.ThunderstormCastMs;
            ThunderstormMana.Checked = THSettings.Instance.ThunderstormMana;
            ThunderstormManaMN.Value = THSettings.Instance.ThunderstormManaMN;
            ThunderstormMelee.Checked = THSettings.Instance.ThunderstormMelee;
            ThunderstormMeleeUnit.Value = THSettings.Instance.ThunderstormMeleeUnit;
            Tremor.Checked = THSettings.Instance.Tremor;
            TremorDuration.Value = THSettings.Instance.TremorDuration;
            TremorMe.Checked = THSettings.Instance.TremorMe;
            TremorHealer.Checked = THSettings.Instance.TremorHealer;
            TremorDPS.Checked = THSettings.Instance.TremorDPS;
            Trinket1.SelectedIndex = THSettings.Instance.Trinket1;
            Trinket1HP.Value = THSettings.Instance.Trinket1HP;
            Trinket2.SelectedIndex = THSettings.Instance.Trinket2;
            Trinket2HP.Value = THSettings.Instance.Trinket2HP;
            THSettings.Instance.Pause = false;
            TurnLeft.SelectedIndex = THSettings.Instance.TurnLeft;
            TurnRight.SelectedIndex = THSettings.Instance.TurnRight;
            UnittoStartAoE.Value = THSettings.Instance.UnittoStartAoE;
            UnleashElementsRes.Checked = THSettings.Instance.UnleashElementsRes;
            UnleashElementsResHP.Value = THSettings.Instance.UnleashElementsResHP;
            UnleashElementsCH.Checked = THSettings.Instance.UnleashElementsCH;
            UnleashElementsGHW.Checked = THSettings.Instance.UnleashElementsGHW;
            UnleashElementsHR.Checked = THSettings.Instance.UnleashElementsHR;
            UnleashElementsHS.Checked = THSettings.Instance.UnleashElementsHS;
            WaterShieldAlways.Checked = THSettings.Instance.WaterShieldAlways;
            WaterShieldAlwaysMana.Value = THSettings.Instance.WaterShieldAlwaysMana;
            WeaponMainHand.SelectedIndex = THSettings.Instance.WeaponMainHand;
            WeaponOffHand.SelectedIndex = THSettings.Instance.WeaponOffHand;
            WindShearInterrupt.Checked = THSettings.Instance.WindShearInterrupt;
            WindShearInterruptMs.Value = THSettings.Instance.WindShearInterruptMs;
            WindwalkRootMe.Checked = THSettings.Instance.WindwalkRootMe;
            WindwalkRootFriend.Checked = THSettings.Instance.WindwalkRootFriend;

            #endregion
        }

        #region Buttons

        #region OK

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start("http://www.tuanha.biz/");
        }

        private void BOK_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("TuanHA Shaman Settings have been saved", "Save");
            THSettings.Instance.Save();
            Logging.Write("----------------------------------");
            Logging.Write("TuanHA " + Classname.GetCurrentSpec() + " Shaman Settings have been saved");

            Logging.Write(LogLevel.Diagnostic, "Your Setting for Debug Purpose Only");
            foreach (var var in THSettings.Instance.GetSettings())
            {
                Logging.Write(LogLevel.Diagnostic, var.ToString());
            }

            Close();
            THSettings.Instance.UpdateStatus = true;
        }

        #endregion

        #region Save

        private void bSave_Click(object sender, EventArgs e)
        {
            THSettings.Instance.Save();
            Logging.Write("TuanHA " + Classname.GetCurrentSpec() + " Shaman Settings have been saved");
            THSettings.Instance.UpdateStatus = true;
        }

        #endregion

        #region Dungeon

        private void BDungeon_Click(object sender, EventArgs e)
        {
            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                            " Shaman Default Dungeon.xml"))
            {
                MessageBox.Show(
                    "Dungeon Mode: Work best with Tyrael in Dungeon.",
                    "Important Note",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " +
                                                              Classname.GetCurrentSpec() +
                                                              " Shaman Default Dungeon.xml"));
                Logging.Write("----------------------------------");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Dungeon Settings from a file complete");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Dungeon.xml");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Dungeon Settings from a file fail.");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
        }

        #endregion

        #region Raid

        private void BRaid_Click(object sender, EventArgs e)
        {
            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                            " Shaman Default Raid.xml"))
            {
                MessageBox.Show(
                    "Raid Mode: Work best with Tyrael in Raid.",
                    "Important Note",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " +
                                                              Classname.GetCurrentSpec() + " Shaman Default Raid.xml"));
                Logging.Write("----------------------------------");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Raid Settings from a file complete");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Raid.xml");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Raid Settings from a file fail.");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
        }

        #endregion

        #region PVP

        private void BPvPHelper_Click(object sender, EventArgs e)
        {
            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                            " Shaman Default PvP.xml"))
            {
                MessageBox.Show(
                    "PvP Mode: Work best with Tyrael in PvP.",
                    "Important Note",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " +
                                                              Classname.GetCurrentSpec() + " Shaman Default PvP.xml"));
                Logging.Write("----------------------------------");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default PvP Settings from a file complete");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default PvP.xml");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default PvP Settings from a file fail.");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
        }

        #endregion

        #region Quest

        private void BQuestHelper_Click(object sender, EventArgs e)
        {
            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                            " Shaman Default Quest.xml"))
            {
                MessageBox.Show(
                    "Quest Mode: Work best with Tyrael in Quest.",
                    "Important Note",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " +
                                                              Classname.GetCurrentSpec() + " Shaman Default Quest.xml"));
                Logging.Write("----------------------------------");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Quest Settings from a file complete");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Quest.xml");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default Quest Settings from a file fail.");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
        }

        #endregion

        #region FullAFK

        private void BFullAfk_Click(object sender, EventArgs e)
        {
            if (File.Exists(Utilities.AssemblyDirectory +
                            @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                            " Shaman Default AFK.xml"))
            {
                MessageBox.Show(
                    "Full AFK Mode: Work best with BGBuddy, ArcheologyBuddy, DungeonBuddy, Grind Bot, Questing...",
                    "Important Note",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                THSettings.Instance.LoadFromXML(XElement.Load(Utilities.AssemblyDirectory +
                                                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " +
                                                              Classname.GetCurrentSpec() + " Shaman Default AFK.xml"));
                Logging.Write("----------------------------------");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default AFK Settings from a file complete");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
            else
            {
                Logging.Write("----------------------------------");
                Logging.Write("File not exists: " + Utilities.AssemblyDirectory +
                              @"\Routines\TuanHAShamanPublicRelease\Preset\TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default AFK.xml");
                Logging.Write("Load TuanHA " + Classname.GetCurrentSpec() +
                              " Shaman Default AFK Settings from a file fail.");
                Logging.Write("----------------------------------");
                THForm_Load(null, null);
            }
        }

        #endregion

        #region SaveSettings

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Setting File|*.xml";
            saveFileDialog.Title = "Save Setting to a File";
            saveFileDialog.InitialDirectory = Utilities.AssemblyDirectory +
                                              @"\Routines\TuanHAShamanPublicRelease\User Settings\";
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.FileName = "TuanHA " + Classname.GetCurrentSpec() + " Shaman";

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName.Contains(".xml"))
            {
                //Logging.Write(DialogResult.ToString());
                THSettings.Instance.SaveToFile(saveFileDialog.FileName);
                Logging.Write("----------------------------------");
                Logging.Write("Save Setting to: " + saveFileDialog.FileName);
                Logging.Write("----------------------------------");
            }
            else
            {
                return;
            }
        }

        #endregion

        #region LoadSettings

        private void LoadSettings_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
                {
                    Filter = "Setting File|*.xml",
                    Title = "Load Setting from a File",
                    InitialDirectory =
                        Utilities.AssemblyDirectory + @"\Routines\TuanHAShamanPublicRelease\User Settings\"
                };
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName.Contains(".xml"))
            {
                //Logging.Write(DialogResult.ToString());
                THSettings.Instance.LoadFromXML(XElement.Load(openFileDialog.FileName));
                Logging.Write("----------------------------------");
                Logging.Write("Load Setting from: " + openFileDialog.FileName);
                Logging.Write("----------------------------------");

                THForm_Load(null, null);
            }
            else
            {
                return;
            }
        }

        #endregion

        #endregion

        private void Trinket1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 10; i++)
            {
                if (Trinket1.SelectedIndex == i)
                {
                    THSettings.Instance.Trinket1 = i;
                }
            }
        }

        private void Trinket1HP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Trinket1HP = (int) Trinket1HP.Value;
        }

        private void Trinket2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 10; i++)
            {
                if (Trinket2.SelectedIndex == i)
                {
                    THSettings.Instance.Trinket2 = i;
                }
            }
        }

        private void Trinket2HP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Trinket2HP = (int) Trinket2HP.Value;
        }

        private void ProfBuff_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 10; i++)
            {
                if (ProfBuff.SelectedIndex == i)
                {
                    THSettings.Instance.ProfBuff = i;
                }
            }
        }

        private void ProfBuffHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ProfBuffHP = (int) ProfBuffHP.Value;
        }

        private void BurstKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 60; i++)
            {
                if (BurstKey.SelectedIndex == i)
                {
                    THSettings.Instance.BurstKey = i;
                }
            }
        }

        private void BurstHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BurstHP = (int) BurstHP.Value;
        }

        private void AutoBuff_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoBuff = AutoBuff.Checked;
        }

        private void AutoFace_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoFace = AutoFace.Checked;
        }

        private void AutoMove_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoMove = AutoMove.Checked;
        }


        private void AutoRacial_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoRacial = AutoRacial.Checked;
        }


        private void AutoTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoTarget = AutoTarget.Checked;
        }


        private void AutoUseFoodHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoUseFoodHP = (int) AutoUseFoodHP.Value;
        }

        private void PauseKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (PauseKey.SelectedIndex == i)
                {
                    THSettings.Instance.PauseKey = i;
                }
            }
        }

        private void StrafleLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (StrafleLeft.SelectedIndex == i)
                {
                    THSettings.Instance.StrafleLeft = i;
                }
            }
        }

        private void Forward_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Forward.SelectedIndex == i)
                {
                    THSettings.Instance.Forward = i;
                }
            }
        }

        private void StrafleRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (StrafleRight.SelectedIndex == i)
                {
                    THSettings.Instance.StrafleRight = i;
                }
            }
        }

        private void TurnLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (TurnLeft.SelectedIndex == i)
                {
                    THSettings.Instance.TurnLeft = i;
                }
            }
        }

        private void Backward_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Backward.SelectedIndex == i)
                {
                    THSettings.Instance.Backward = i;
                }
            }
        }

        private void TurnRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (TurnRight.SelectedIndex == i)
                {
                    THSettings.Instance.TurnRight = i;
                }
            }
        }

        private void OnlyHealMe_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.OnlyHealMe = OnlyHealMe.Checked;
        }

        private void OnlyHealTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.OnlyHealTarget = OnlyHealTarget.Checked;
        }

        private void OnlyHealFocus_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.OnlyHealFocus = OnlyHealFocus.Checked;
        }

        private void Group1_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group1 = Group1.Checked;
        }

        private void Group2_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group2 = Group2.Checked;
        }

        private void Group3_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group3 = Group3.Checked;
        }

        private void Group4_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group4 = Group4.Checked;
        }

        private void Group5_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group5 = Group5.Checked;
        }

        private void Group6_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group6 = Group6.Checked;
        }

        private void Group7_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group7 = Group7.Checked;
        }

        private void Group8_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Group8 = Group8.Checked;
        }

        private void HealBalancing_Scroll(object sender, EventArgs e)
        {
            label66.Text = "" + HealBalancing.Value;
            THSettings.Instance.HealBalancing = (int) HealBalancing.Value;
        }

        private void AutoRez_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoRez = AutoRez.Checked;
        }

        private void InterruptAll_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.InterruptAll = InterruptAll.Checked;
        }

        private void InterruptTarget_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.InterruptTarget = InterruptTarget.Checked;
        }

        private void InterruptFocus_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.InterruptFocus = InterruptFocus.Checked;
        }


        private void AutoUseFood_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoUseFood = AutoUseFood.Checked;
        }

        private void BattleStandard_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BattleStandard = BattleStandard.Checked;
        }

        private void BattleStandardHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BattleStandardHP = (int) BattleStandardHP.Value;
        }


        //Hotkey1
        private void Hotkey1Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 3; i++)
            {
                if (Hotkey1Target.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey1Target = i;
                }
            }
        }

        private void Hotkey1Mod_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 2; i++)
            {
                if (Hotkey1Mod.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey1Mod = i;
                }
            }
        }

        private void Hotkey1Key_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey1Key.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey1Key = i;
                }
            }
        }

        private void Hotkey1Spell_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey1Spell.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey1Spell = i;
                }
            }
        }

        //Hotkey2
        private void Hotkey2Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 3; i++)
            {
                if (Hotkey2Target.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey2Target = i;
                }
            }
        }

        private void Hotkey2Mod_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 2; i++)
            {
                if (Hotkey2Mod.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey2Mod = i;
                }
            }
        }

        private void Hotkey2Key_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey2Key.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey2Key = i;
                }
            }
        }

        private void Hotkey2Spell_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey2Spell.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey2Spell = i;
                }
            }
        }

        //Hotkey3
        private void Hotkey3Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 3; i++)
            {
                if (Hotkey3Target.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey3Target = i;
                }
            }
        }

        private void Hotkey3Mod_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 2; i++)
            {
                if (Hotkey3Mod.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey3Mod = i;
                }
            }
        }

        private void Hotkey3Key_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey3Key.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey3Key = i;
                }
            }
        }

        private void Hotkey3Spell_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey3Spell.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey3Spell = i;
                }
            }
        }

        //Hotkey4
        private void Hotkey4Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 3; i++)
            {
                if (Hotkey4Target.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey4Target = i;
                }
            }
        }

        private void Hotkey4Mod_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 2; i++)
            {
                if (Hotkey4Mod.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey4Mod = i;
                }
            }
        }

        private void Hotkey4Key_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey4Key.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey4Key = i;
                }
            }
        }

        private void Hotkey4Spell_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey4Spell.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey4Spell = i;
                }
            }
        }

        //Hotkey5
        private void Hotkey5Target_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 3; i++)
            {
                if (Hotkey5Target.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey5Target = i;
                }
            }
        }

        private void Hotkey5Mod_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 2; i++)
            {
                if (Hotkey5Mod.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey5Mod = i;
                }
            }
        }

        private void Hotkey5Key_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey5Key.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey5Key = i;
                }
            }
        }

        private void Hotkey5Spell_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 40; i++)
            {
                if (Hotkey5Spell.SelectedIndex == i)
                {
                    THSettings.Instance.Hotkey5Spell = i;
                }
            }
        }

        private void AutoAoE_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoAoE = AutoAoE.Checked;
        }

        private void UnittoStartAoE_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnittoStartAoE = (int) UnittoStartAoE.Value;
        }

        private void LightningBoltFiller_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.LightningBoltFiller = LightningBoltFiller.Checked;
        }

        private void LightningBoltFillerMaelstromStack_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.LightningBoltFillerMaelstromStack = (int) LightningBoltFillerMaelstromStack.Value;
        }

        private void HealingSurgeInCombatEnh_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEnh = HealingSurgeInCombatEnh.Checked;
        }

        private void HealingSurgeInCombatEnhStack_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEnhStack = (int) HealingSurgeInCombatEnhStack.Value;
        }

        private void HealingSurgeInCombatEnhHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEnhHP = (int) HealingSurgeInCombatEnhHP.Value;
        }

        private void HealingSurgeOutCombatEnh_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeOutCombatEnh = HealingSurgeOutCombatEnh.Checked;
        }

        private void HealingSurgeOutCombatEnhHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeOutCombatEnhHP = (int) HealingSurgeOutCombatEnhHP.Value;
        }

        private void WindShearInterrupt_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.WindShearInterrupt = WindShearInterrupt.Checked;
        }

        private void WindShearInterruptMs_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.WindShearInterruptMs = (int) WindShearInterruptMs.Value;
        }

        private void ShamanisticRageHP_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShamanisticRageHP = ShamanisticRageHP.Checked;
        }

        private void ShamanisticRageHPHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShamanisticRageHPHP = (int) ShamanisticRageHPHP.Value;
        }

        private void ShamanisticRageMN_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShamanisticRageMN = ShamanisticRageMN.Checked;
        }

        private void ShamanisticRageMNMN_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShamanisticRageMNMN = (int) ShamanisticRageMNMN.Value;
        }

        private void FeralSpirit_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FeralSpiritCooldown = FeralSpiritCooldown.Checked;
        }

        private void HealthTideTotem_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingTideTotem = HealingTideTotem.Checked;
        }

        private void HealthTideTotemUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingTideTotemUnit = (int) HealingTideTotemUnit.Value;
        }

        private void HealthTideTotemHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingTideTotemHP = (int) HealingTideTotemHP.Value;
        }

        private void EarthShield_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthShield = EarthShield.Checked;
        }

        private void GreaterHealingWave_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GreaterHealingWave = GreaterHealingWave.Checked;
        }

        private void GreaterHealingWaveHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GreaterHealingWaveHP = (int) GreaterHealingWaveHP.Value;
        }

        private void HealingSurgeRes_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeRes = HealingSurgeRes.Checked;
        }

        private void HealingSurgeResHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeResHP = (int) HealingSurgeResHP.Value;
        }

        private void HealingWave_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingWave = HealingWave.Checked;
        }

        private void HealingWaveHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingWaveHP = (int) HealingWaveHP.Value;
        }

        private void UnleashElementsRes_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnleashElementsRes = UnleashElementsRes.Checked;
        }

        private void UnleashElementsResHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnleashElementsResHP = (int) UnleashElementsResHP.Value;
        }

        private void AncestralSwiftnessRes_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AncestralSwiftnessRes = AncestralSwiftnessRes.Checked;
        }

        private void AncestralSwiftnessResHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AncestralSwiftnessResHP = (int) AncestralSwiftnessResHP.Value;
        }

        private void ChainHeal_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ChainHeal = ChainHeal.Checked;
        }

        private void ChainHealUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ChainHealUnit = (int) ChainHealUnit.Value;
        }

        private void ChainHealHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ChainHealHP = (int) ChainHealHP.Value;
        }

        private void Riptide_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Riptide = Riptide.Checked;
        }

        private void RiptideHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.RiptideHP = (int) RiptideHP.Value;
        }

        private void HealingStreamTotem_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingStreamTotem = HealingStreamTotem.Checked;
        }

        private void HealingStreamTotemHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingStreamTotemHP = (int) HealingStreamTotemHP.Value;
        }

        private void HealingStreamTotemUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingStreamTotemUnit = (int) HealingStreamTotemUnit.Value;
        }

        private void ManaTideTotem_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ManaTideTotem = ManaTideTotem.Checked;
        }

        private void ManaTideTotemMN_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ManaTideTotemMN = (int) ManaTideTotemMN.Value;
        }

        private void HealingRain_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingRain = HealingRain.Checked;
        }

        private void HealingRainUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingRainUnit = (int) HealingRainUnit.Value;
        }

        private void HealingRainHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingRainHP = (int) HealingRainHP.Value;
        }

        private void WeaponMainHand_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 10; i++)
            {
                if (WeaponMainHand.SelectedIndex == i)
                {
                    THSettings.Instance.WeaponMainHand = i;
                }
            }
        }

        private void WeaponOffHand_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (var i = 0; i <= 10; i++)
            {
                if (WeaponOffHand.SelectedIndex == i)
                {
                    THSettings.Instance.WeaponOffHand = i;
                }
            }
        }

        private void StoneBulwarkTotem_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.StoneBulwarkTotem = StoneBulwarkTotem.Checked;
        }

        private void StoneBulwarkTotemHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.StoneBulwarkTotemHP = (int) StoneBulwarkTotemHP.Value;
        }

        private void AstralShift_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AstralShift = AstralShift.Checked;
        }

        private void AstralShiftHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AstralShiftHP = (int) AstralShiftHP.Value;
        }

        private void PurifySpiritDebuff_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PurifySpiritDebuff = PurifySpiritDebuff.Checked;
        }

        private void PurifySpiritASAP_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PurifySpiritASAP = PurifySpiritASAP.Checked;
        }

        private void PurifySpiritDebuffNumber_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PurifySpiritDebuffNumber = (int) PurifySpiritDebuffNumber.Value;
        }

        private void Tremor_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Tremor = Tremor.Checked;
        }

        private void TremorDuration_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.TremorDuration = (int) TremorDuration.Value;
        }

        private void TremorMe_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.TremorMe = TremorMe.Checked;
        }

        private void TremorHealer_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.TremorHealer = TremorHealer.Checked;
        }

        private void TremorDPS_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.TremorDPS = TremorDPS.Checked;
        }

        private void SpiritLink_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SpiritLink = SpiritLink.Checked;
        }

        private void SpiritLinkUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SpiritLinkUnit = (int) SpiritLinkUnit.Value;
        }

        private void SpiritLinkHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SpiritLinkHP = (int) SpiritLinkHP.Value;
        }

        private void GroundingLow_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GroundingLow = GroundingLow.Checked;
        }

        private void GroundingLowHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GroundingLowHP = (int) GroundingLowHP.Value;
        }

        private void GroundingTrap_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GroundingTrap = GroundingTrap.Checked;
        }

        private void GroundingCast_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GroundingCast = GroundingCast.Checked;
        }

        private void GroundingCastMs_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.GroundingCastMs = (int) GroundingCastMs.Value;
        }

        private void AutoGhostWolf_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoGhostWolf = AutoGhostWolf.Checked;
        }

        private void PurgeASAP_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PurgeASAP = PurgeASAP.Checked;
        }

        private void PurgeASAPMana_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PurgeASAPMana = (int) PurgeASAPMana.Value;
        }

        private void PurgeNormal_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PurgeNormal = PurgeNormal.Checked;
        }

        private void PurgeNormalMana_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.PurgeNormalMana = (int) PurgeNormalMana.Value;
        }

        private void AutoGhostWolfCancel_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoGhostWolfCancel = AutoGhostWolfCancel.Checked;
        }

        private void FrostShockNearby_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockNearby = FrostShockNearby.Checked;
        }

        private void FrostShockNearbyDist_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockNearbyDist = (int) FrostShockNearbyDist.Value;
        }

        private void AttackResto_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackResto = AttackResto.Checked;
        }

        private void AttackRestoMana_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoMana = (int) AttackRestoMana.Value;
        }

        private void AttackRestoAny_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoAny = AttackRestoAny.Checked;
        }

        private void AttackRestoAnyHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoAnyHP = (int) AttackRestoAnyHP.Value;
        }

        private void AttackRestoEarthShock_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoEarthShock = AttackRestoEarthShock.Checked;
        }

        private void AttackRestoFlameShock_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoFlameShock = AttackRestoFlameShock.Checked;
        }

        private void AttackRestoChainLightning_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoChainLightning = AttackRestoChainLightning.Checked;
        }

        private void AttackRestoLavaBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoLavaBurst = AttackRestoLavaBurst.Checked;
        }

        private void AttackRestoLightningBolt_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoLightningBolt = AttackRestoLightningBolt.Checked;
        }

        private void FrostShockNearbyMana_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockNearbyMana = (int) FrostShockNearbyMana.Value;
        }

        private void WaterShieldAlways_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.WaterShieldAlways = WaterShieldAlways.Checked;
        }

        private void WaterShieldAlwaysMana_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.WaterShieldAlwaysMana = (int) WaterShieldAlwaysMana.Value;
        }

        private void AutoWaterWalking_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoWaterWalking = AutoWaterWalking.Checked;
        }

        private void EarthbindUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthbindUnit = (int) EarthbindUnit.Value;
        }

        private void Earthbind_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Earthbind = Earthbind.Checked;
        }

        private void EarthbindDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthbindDistance = (int) EarthbindDistance.Value;
        }

        private void SearingTotem_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SearingTotem = SearingTotem.Checked;
        }

        private void SearingTotemDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SearingTotemDistance = (int) SearingTotemDistance.Value;
        }

        private void CapacitorFriendLow_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorFriendLow = CapacitorFriendLow.Checked;
        }

        private void CapacitorFriendLowHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorFriendLowHP = (int) CapacitorFriendLowHP.Value;
        }

        private void CapacitorEnemyLow_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorEnemyLow = CapacitorEnemyLow.Checked;
        }

        private void CapacitorEnemyLowHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorEnemyLowHP = (int) CapacitorEnemyLowHP.Value;
        }

        private void CapacitorEnemyPack_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorEnemyPack = CapacitorEnemyPack.Checked;
        }

        private void CapacitorEnemyPackNumber_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorEnemyPackNumber = (int) CapacitorEnemyPackNumber.Value;
        }

        private void SpiritwalkersGrace_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SpiritwalkersGrace = SpiritwalkersGrace.Checked;
        }

        private void SpiritwalkersGraceHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.SpiritwalkersGraceHP = (int) SpiritwalkersGraceHP.Value;
        }

        private void AscendanceResto_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AscendanceResto = AscendanceResto.Checked;
        }

        private void AscendanceRestoUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AscendanceRestoUnit = (int) AscendanceRestoUnit.Value;
        }

        private void AscendanceRestoHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AscendanceRestoHP = (int) AscendanceRestoHP.Value;
        }

        private void Hex_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Hex = Hex.Checked;
        }

        private void HexHealer_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HexHealer = HexHealer.Checked;
        }

        private void HexDPS_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HexDPS = HexDPS.Checked;
        }

        private void CapacitorProjection_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorProjection = CapacitorProjection.Checked;
        }

        private void CapacitorProjectionMs_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CapacitorProjectionMs = (int) CapacitorProjectionMs.Value;
        }

        private void AttackRestoLightningBoltGlyph_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AttackRestoLightningBoltGlyph = AttackRestoLightningBoltGlyph.Checked;
        }

        private void EarthShieldAlways_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthShieldAlways = EarthShieldAlways.Checked;
        }

        private void EarthShieldAlwaysHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthShieldAlwaysHP = (int) EarthShieldAlwaysHP.Value;
        }

        private void Earthquake_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Earthquake = Earthquake.Checked;
        }

        private void EarthquakeUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthquakeUnit = (int) EarthquakeUnit.Value;
        }

        private void EarthShockElemental_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthShockElemental = EarthShockElemental.Checked;
        }

        private void EarthShockElementalCharge_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthShockElementalCharge = (int) EarthShockElementalCharge.Value;
        }

        private void MagmaTotemEle_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.MagmaTotemEle = MagmaTotemEle.Checked;
        }

        private void MagmaTotemEleUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.MagmaTotemEleUnit = (int) MagmaTotemEleUnit.Value;
        }

        private void Thunderstorm_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.Thunderstorm = Thunderstorm.Checked;
        }

        private void ThunderstormUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ThunderstormUnit = (int) ThunderstormUnit.Value;
        }


        private void ThunderstormCast_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ThunderstormCast = ThunderstormCast.Checked;
        }

        private void ThunderstormCastMs_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ThunderstormCastMs = (int) ThunderstormCastMs.Value;
        }

        private void FlameShockAoEEleMana_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EleAoEMana = (int) FlameShockAoEEleMana.Value;
        }

        private void ChainLightningEle_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ChainLightningEle = ChainLightningEle.Checked;
        }

        private void ChainLightningEleUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ChainLightningEleUnit = (int) ChainLightningEleUnit.Value;
        }

        private void ThunderstormMana_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ThunderstormMana = ThunderstormMana.Checked;
        }

        private void ThunderstormManaMN_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ThunderstormManaMN = (int) ThunderstormManaMN.Value;
        }

        private void AutoWriteLog_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoWriteLog = AutoWriteLog.Checked;
        }

        private void FrostShockNearbyDPS_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockNearbyRange = FrostShockNearbyRange.Checked;
        }

        private void FrostShockNearbyHealer_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockNearbyHealer = FrostShockNearbyHealer.Checked;
        }

        private void FrostShockNearbyMelee_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockNearbyMelee = FrostShockNearbyMelee.Checked;
        }

        private void FeralSpiritBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FeralSpiritBurst = FeralSpiritBurst.Checked;
        }

        private void AscendanceEnhCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AscendanceEnhCooldown = AscendanceEnhCooldown.Checked;
        }

        private void AscendanceEnhBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AscendanceEnhBurst = AscendanceEnhBurst.Checked;
        }

        private void ElementalBlastEnh_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ElementalBlastEnh = ElementalBlastEnh.Checked;
        }

        private void ElementalBlastEnhStack_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ElementalBlastEnhStack = (int) ElementalBlastEnhStack.Value;
        }

        private void HealingSurgeInCombatEnhFriend_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEnhFriend = HealingSurgeInCombatEnhFriend.Checked;
        }

        private void HealingSurgeInCombatEnhStackFriend_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEnhStackFriend = (int) HealingSurgeInCombatEnhStackFriend.Value;
        }

        private void HealingSurgeInCombatEnhHPFriend_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEnhHPFriend = (int) HealingSurgeInCombatEnhHPFriend.Value;
        }

        private void CleanseSpiritEnh_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CleanseSpiritEnh = CleanseSpiritEnh.Checked;
        }

        private void AscendanceEleCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AscendanceEleCooldown = AscendanceEleCooldown.Checked;
        }

        private void AscendanceEleBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AscendanceEleBurst = AscendanceEleBurst.Checked;
        }

        private void CleanseSpiritEle_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.CleanseSpiritEle = CleanseSpiritEle.Checked;
        }

        private void HealingSurgeOutCombatEle_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeOutCombatEle = HealingSurgeOutCombatEle.Checked;
        }

        private void HealingSurgeOutCombatEleHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeOutCombatEleHP = (int) HealingSurgeOutCombatEleHP.Value;
        }

        private void HealingSurgeInCombatEle_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEle = HealingSurgeInCombatEle.Checked;
        }

        private void HealingSurgeInCombatEleCC_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEleCC = HealingSurgeInCombatEleCC.Checked;
        }

        private void HealingSurgeInCombatEleHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEleHP = (int) HealingSurgeInCombatEleHP.Value;
        }

        private void HealingSurgeInCombatEleFriend_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEleFriend = HealingSurgeInCombatEleFriend.Checked;
        }

        private void HealingSurgeInCombatEleFriendCC_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEleFriendCC = HealingSurgeInCombatEleFriendCC.Checked;
        }

        private void HealingSurgeInCombatEleFriendHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.HealingSurgeInCombatEleFriendHP = (int) HealingSurgeInCombatEleFriendHP.Value;
        }

        private void ThunderstormMelee_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ThunderstormMelee = ThunderstormMelee.Checked;
        }

        private void ThunderstormMeleeUnit_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ThunderstormMeleeUnit = (int) ThunderstormMeleeUnit.Value;
        }

        private void FlameShockRogueDruid_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FlameShockRogueDruid = FlameShockRogueDruid.Checked;
        }

        private void ElementalMasteryCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ElementalMasteryCooldown = ElementalMasteryCooldown.Checked;
        }

        private void ElementalMasteryBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ElementalMasteryBurst = ElementalMasteryBurst.Checked;
        }

        private void AncestralGuidanceCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AncestralGuidanceCooldown = AncestralGuidanceCooldown.Checked;
        }

        private void AncestralGuidanceBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AncestralGuidanceBurst = AncestralGuidanceBurst.Checked;
        }

        private void WindwalkRootMe_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.WindwalkRootMe = WindwalkRootMe.Checked;
        }

        private void WindwalkRootFriend_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.WindwalkRootFriend = WindwalkRootFriend.Checked;
        }

        private void StormlashCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.StormlashCooldown = StormlashCooldown.Checked;
        }

        private void StormlashBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.StormlashBurst = StormlashBurst.Checked;
        }

        private void StormlashEnemy_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.StormlashEnemy = StormlashEnemy.Checked;
        }

        private void StormlashEnemyHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.StormlashEnemyHP = (int) StormlashEnemyHP.Value;
        }

        private void EarthElementalCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthElementalCooldown = EarthElementalCooldown.Checked;
        }

        private void EarthElementalBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.EarthElementalBurst = EarthElementalBurst.Checked;
        }

        private void FireElementalCooldown_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FireElementalCooldown = FireElementalCooldown.Checked;
        }

        private void FireElementalBurst_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FireElementalBurst = FireElementalBurst.Checked;
        }

        private void UnleashElementsCH_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnleashElementsCH = UnleashElementsCH.Checked;
        }

        private void UnleashElementsGHW_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnleashElementsGHW = UnleashElementsGHW.Checked;
        }

        private void UnleashElementsHR_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnleashElementsHR = UnleashElementsHR.Checked;
        }

        private void UnleashElementsHS_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.UnleashElementsHS = UnleashElementsHS.Checked;
        }

        private void ShamanisticCC_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShamanisticCC = ShamanisticCC.Checked;
        }

        private void ShamanisticCCDuration_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.ShamanisticCCDuration = (int) ShamanisticCCDuration.Value;
        }

        private void FrostShockEnh_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockEnh = FrostShockEnh.Checked;
        }

        private void FrostShockEnhMinDistance_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FrostShockEnhMinDistance = (int) FrostShockEnhMinDistance.Value;
        }

        private void LightningBoltEnh_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.LightningBoltEnh = LightningBoltEnh.Checked;
        }

        private void LightningBoltEnhMaelstromStack_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.LightningBoltEnhMaelstromStack = (int) LightningBoltEnhMaelstromStack.Value;
        }

        private void FeralSpiritLow_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FeralSpiritLow = FeralSpiritLow.Checked;
        }

        private void FeralSpiritLowHP_ValueChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FeralSpiritLowHP = (int) FeralSpiritLowHP.Value;
        }

        private void FlagReturnorPickup_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.FlagReturnorPickup = FlagReturnorPickup.Checked;
        }

        private void AutoSetFocus_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoSetFocus = AutoSetFocus.Checked;
        }

        private void AutoAttackOutCombat_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.AutoAttackOutCombat = AutoAttackOutCombat.Checked;
        }

        private void BindElemental_CheckedChanged(object sender, EventArgs e)
        {
            THSettings.Instance.BindElemental = BindElemental.Checked;
        }
    }
}