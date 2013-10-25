using System;
using Styx.Common;
using Styx.TreeSharp;
using Action = Styx.TreeSharp.Action;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region EnhancementRotation

        private static DateTime LastAoESearch;

        private static Composite EnhancementRotation()
        {
            return new PrioritySelector(
                new Action(delegate
                    {
                        if (LastAoESearch <= DateTime.Now)
                        {
                            if (THSettings.Instance.AutoAoE && (CountEnemyNear(Me, 10f) >= THSettings.Instance.UnittoStartAoE))
                            {
                                AoEModeOn = true;
                            }
                            else
                            {
                                AoEModeOn = false;
                            }
                            LastAoESearch = DateTime.Now + TimeSpan.FromMilliseconds(3000.0);
                        }
                        return RunStatus.Failure;
                    }),
                AutoTargetMelee(),//.
                MovementMoveToMelee(ret => Me.CurrentTarget),
                MovementMoveStop(ret => Me.CurrentTarget, 3),
                TargetMyPetTarget(),
                //DPS Rotation Here
                ///如果在变狼状态,且自动取消变狼没有被激活,那么后面的循环停止
                //////done
                GhostWolfHoldComp(),
                //Single Target Rotation
                new Decorator(
                    ret => !AoEModeOn && !Me.Mounted,
                    new PrioritySelector(
                        ///如果设置了自动攻击,则目标在5码内,自动取消变狼状态,如果10码内,每两秒自动激活自动攻击一次
                        //.AutoAttack(),
                        ///恢复SM用来回蓝的
                        AutoAttackOffTarget(),
                        ///血少,或者蓝少,且40码内有敌人以我为目标, -30伤害,15秒不耗蓝
                        ShamanisticRage(),
                        ///升腾
                        AscendanceEnh(),
                        ///元素掌握,+30%急速?
                        ElementalMastery(),
                        ///先祖指引，爆发或者按CD使用，有治疗50%一下用，无治疗，70%一下用
                        //////done
                        AncestralGuidance(),
                        ///不同天赋对自己或者队友使用幽灵步免疫减速
                        ///恢复血少，切有物理dps以自己为目标时使用
                        ///非恢复，敌人在40码内，自己被减速时使用
                        ///非恢复，队友被root，且队友目标为敌人
                        //可加入治疗职业被控时才用，需要调查一下是否可以用投掷
                        Windwalk(),
                        ///升腾时，30码使用Stormstrike
                        ///非升腾时，5码内使用
                        //////done
                        Stormblast(),
                        ///变青蛙
                        //需要改善
                        Hex(),
                        ///电鞭图腾,CD,burst,或者敌人血少
                        //////done
                        Stormlash(),
                        ///石壁图腾，血少且35码内有敌人
                        //改善为无治疗时使用
                        StoneBulwarkTotem(),
                        ///星界转换，血少且有敌人以我为目标
                        //////done
                        AstralShift(),
                        ///30码内有队友血少时,用根基
                        GroundingLow(),
                        ///队友血少,敌人血少,可同时电多个人时,使用电能图腾
                        //这个完全变了，而且配置界面也改变了，需要看看最新的界面
                        Capacitor(),
                        ///解青蛙
                        CleanseSpiritFriendlyASAPEnh(),
                        ///图元素图腾, CD或者爆发用, 判断是否有worthy的目标
                        EarthElemental(),
                        ///火元素图腾, CD或者爆发用, 判断是否有worthy的目标
                        FireElemental(),
                        ///需要治疗者在30码内，血少于门限，连接图腾
                        HealingTideTotem(),
                        ///治疗图腾，需要增加没有治疗时使用,或者治疗被晕时给治疗使用
                        HealingStreamTotem(),
                        //////done
                        ///4层漩涡或20码外或5码外且自己被定身或20码内，对方血不少或比我高，蓝高于40%，用治疗波,需要增加治疗被晕时给治疗加血
                        ///无治疗时，5层漩涡给自己回血（门限+20）
                        ///治疗被控时，5层漩涡给治疗回血（门限+10）
                        ///任何人被两个人攻击时，5层漩涡给血少的人回血（门限+20）
                        HealingSurgeInCombatEnh(),
                        ///自动补电盾,血少改水盾
                        WaterShieldEnh(),
                        LightningShield(),
                        ///武器付魔
                        //////TemporaryEnchantmentEnhancement(),
                        ///召唤狼, 取消了JJC内worthy target的判断
                        FeralSpirit(),
                        ///火元素图腾, CD或者爆发用, 判断是否有worthy的目标
                        FireElemental(),
                        ///图元素图腾, CD或者爆发用, 判断是否有worthy的目标
                        EarthElemental(),
                        ///对于会隐身职业用烈焰震击上debuff
                        FlameShockRogueDruid(),
                        ///没有被定身时用冰震
                        FrostShockEnhRoot(),
                        //////debuff<2秒时补debuff
                        //////done
                        FlameShockEnhNoFlametongue(),
                        //////done
                        UnleashElementsEnhSnare(),
                        ///没有被减速时使用冰震
                        FrostShockEnh(),
                        //////done
                        SearingTotem(),
                        //////done
                        ElementalBlastEnh(), //////
                        //////done
                        UnleashElementsEnhUnleashedFury(),
                        ///根据设定胡层数使用闪电
                        //////done
                        LightningBoltEnh(),
                        ///先祖迅捷+闪电
                        //////done
                        LightningBoltAncestralSwiftness(),
                        ///近距离使用
                        //////done
                        Stormstrike(),
                        //////done
                        FlameShockUnleashFlame(),
                        //////done
                        BindElemental(), //////
                        //////done
                        LavaLash(),
                        //////done
                        UnleashElementsEnh(),
                        //////done
                        PurgeASAPEleEnh(),
                        //////done
                        EarthShock(),
                        //////done
                        Earthbind(),
                        //////ElementalBlastEnh(),
                        //////done
                        LightningBoltFillerEnh(),
                        //////done
                        LightningBoltFillerEnhRange(),
                        //////done
                        FireNovaLoS(),
                        //////done
                        CleanseSpiritFriendlyEnh(),
                        HealingSurgeOutCombatEnh(),
                        WaterWalking(),
                        ///改动过，30码内没人，或者没有升腾时10码内没人，或者7码外有人在我背后
                        //////done
                        GhostWolfEnh(),
                        TemporaryEnchantmentEnhancement(),
                        AutoRez()
                        )),
                //AoE Target Rotation
                new Decorator(
                    ret => AoEModeOn && !Me.Mounted,
                    new PrioritySelector(
                        //AutoAttack(),
                        ShamanisticRage(),
                        AscendanceEnh(),
                        ElementalMastery(),
                        AncestralGuidance(),
                        Windwalk(),
                        Stormblast(),
                        Stormlash(),
                        StoneBulwarkTotem(),
                        AstralShift(),
                        GroundingLow(),
                        Capacitor(),
                        CleanseSpiritFriendlyASAPEnh(),
                        HealingTideTotem(),
                        HealingStreamTotem(),
                        HealingSurgeInCombatEnh(),
                        FeralSpirit(),
                        FireElemental(),
                        EarthElemental(),
                        MagmaTotemEnh(),
                        FireNovaAoE(),
                        FlameShockEnh(),
                        LavaLash(),
                        UnleashElementsEnhUnleashedFury(),
                        ChainLightning5MW(),
                        Stormstrike(),
                        ElementalBlastEnh(),
                        Hex(),
                        Earthbind(),
                        CleanseSpiritFriendlyEnh(),
                        HealingSurgeOutCombatEnh()
                        )),
                WriteDebug("")
                );
        }

        #endregion
    }
}