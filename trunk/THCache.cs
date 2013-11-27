using System;
using System.Collections.Generic;
using System.Linq;
using Styx.Common;
using Styx.CommonBot;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;

namespace TuanHA_Combat_Routine
{
    public partial class Classname
    {
        #region AuraCache

        private class AuraCacheClass
        {
            internal DateTime AuraCacheExpireTime { get; set; }
            internal ulong AuraCacheUnit { get; set; }
            internal WoWAura AuraCacheAura { get; set; }
            internal string AuraCacheName { get; set; }
            internal int AuraCacheId { get; set; }
        }

        private static readonly List<AuraCacheClass> AuraCacheList = new List<AuraCacheClass>();
        private static TimeSpan AuraCacheExpire;

        //////done
        private static void AuraCacheUpdate(WoWUnit target, bool ForceUpdate = false)
        {
            var AuraCacheUnit = AuraCacheList.FirstOrDefault(unit => unit.AuraCacheUnit == target.Guid);

            if (ForceUpdate)
            {
                if (AuraCacheUnit != null)
                {
                    AuraCacheList.RemoveAll(unit => AuraCacheUnit.AuraCacheUnit == target.Guid);
                }

                var expireTime = DateTime.Now + AuraCacheExpire;
                try
                {
                    foreach (WoWAura aura in target.GetAllAuras())
                    {
                        ////Logging.Write("Add {0} from {1} ({2}) to AuraCacheUnit", aura.Name, target.Guid, target.SafeName);
                        //AuraCacheList.Add(new AuraCacheClass
                        //    {
                        //        AuraCacheExpireTime = expireTime,
                        //        AuraCacheUnit = target.Guid,
                        //        AuraCacheAura = aura,
                        //        AuraCacheName = aura.Name,
                        //        AuraCacheId = aura.SpellId,
                        //    });
                        AuraCacheClass item = new AuraCacheClass
                        {
                            AuraCacheExpireTime = expireTime,
                            AuraCacheUnit = target.Guid,
                            AuraCacheAura = aura,
                            AuraCacheName = aura.Name,
                            AuraCacheId = aura.SpellId
                        };
                        AuraCacheList.Add(item);
                    }
                }
                catch (Exception)
                {
                    Logging.Write("Error AuraCacheList.Add");
                    throw;
                }

            }

            if (AuraCacheUnit == null)
            {
                //Logging.Write("AuraCacheList Do Not Containt {0} Add All Aura to AuraCacheList", target.SafeName);
                var expireTime = DateTime.Now + AuraCacheExpire;
                try
                {
                    foreach (var aura in target.GetAllAuras())
                    {
                        //Logging.Write("Add {0} from {1} ({2}) to AuraCacheUnit", aura.Name, target.Guid, target.SafeName);
                        //AuraCacheList.Add(new AuraCacheClass
                        //    {
                        //        AuraCacheExpireTime = expireTime,
                        //        AuraCacheUnit = target.Guid,
                        //        AuraCacheAura = aura,
                        //        AuraCacheName = aura.Name,
                        //        AuraCacheId = aura.SpellId,
                        //    });
                        AuraCacheClass class3 = new AuraCacheClass
                        {
                            AuraCacheExpireTime = expireTime,
                            AuraCacheUnit = target.Guid,
                            AuraCacheAura = aura,
                            AuraCacheName = aura.Name,
                            AuraCacheId = aura.SpellId
                        };
                        AuraCacheList.Add(class3);
                    }
                }
                catch (Exception)
                {
                    Logging.Write("Error AuraCacheList.Add");
                    throw;
                }
                //Logging.Write("Total AuraCacheList for {0}", target.SafeName);
                //foreach (var aura in AuraCacheList)
                //{
                //    if (aura.AuraCacheUnit != target)
                //    {
                //        continue;
                //    }
                //    Logging.Write("{0} - {1}", aura.AuraCacheId, aura.AuraCacheAura.Name);
                //}
            }
            else if (AuraCacheUnit.AuraCacheExpireTime < DateTime.Now)
            {
                //Logging.Write("AuraCacheList Containt {0} But Cache Time Expired, Update All Aura to AuraCacheList",
                //              target.SafeName);

                AuraCacheList.RemoveAll(unit => AuraCacheUnit.AuraCacheUnit == target.Guid);

                var expireTime = DateTime.Now + AuraCacheExpire;
                foreach (WoWAura aura in target.GetAllAuras())
                {
                    //Logging.Write("Add {0} from {1} ({2}) to AuraCacheUnit", aura.Name, target.Guid, target.SafeName);
                    //AuraCacheList.Add(new AuraCacheClass
                    //    {
                    //        AuraCacheExpireTime = expireTime,
                    //        AuraCacheUnit = target.Guid,
                    //        AuraCacheAura = aura,
                    //        AuraCacheName = aura.Name,
                    //        AuraCacheId = aura.SpellId,
                    //    });
                    AuraCacheClass class4 = new AuraCacheClass
                    {
                        AuraCacheExpireTime = expireTime,
                        AuraCacheUnit = target.Guid,
                        AuraCacheAura = aura,
                        AuraCacheName = aura.Name,
                        AuraCacheId = aura.SpellId
                    };
                    AuraCacheList.Add(class4);
                }

                //Logging.Write("Total AuraCacheList for {0}", target.SafeName);
                //foreach (var aura in AuraCacheList)
                //{
                //    if (aura.AuraCacheUnit != target)
                //    {
                //        continue;
                //    }
                //    Logging.Write("{0} - {1}", aura.AuraCacheId, aura.AuraCacheAura.Name);
                //}
            }
            //else
            //{
            //    Logging.Write("AuraCacheList Containt {0} and Cache Time NOT Expired, Do Not update to AuraCacheList",
            //                  target.SafeName);

            //    Logging.Write("Total AuraCacheList for {0}", target.SafeName);
            //    foreach (var aura in AuraCacheList)
            //    {
            //        if (aura.AuraCacheUnit != target)
            //        {
            //            continue;
            //        }
            //        Logging.Write("{0} - {1}", aura.AuraCacheId, aura.AuraCacheAura.Name);
            //    }
            //}
        }

        #endregion

        #region SpellsCooldownHS

        private static readonly HashSet<string> SpellsCooldownHS = new HashSet<string>
            {
                //Racial
                "Arcane Torrent",
                "Berserking",
                "Blood Fury",
                "Cannibalize",
                "Darkflight",
                "Escape Artist",
                "Every Man for Himself",
                "Gift of the Naaru",
                "Rocket Jump",
                "Shadowmeld",
                "Stoneform",
                "War Stomp",
                "Will of the Forsaken",

                //Class Spells
                "Ascendance",
                "Astral Shift",
                "Ancestral Swiftness",
                "Ancestral Guidance",
                "Call of the Elements",
                "Capacitor Totem",
                "Earth Elemental Totem",
                "Earthquake",
                "Earth Shock",
                "Earthbind Totem",
                "Earthgrab Totem",
                "Elemental Blast",
                "Elemental Mastery",
                "Grounding Totem",
                "Healing Rain",
                "Healing Stream Totem",
                "Healing Tide Totem",
                "Hex",
                "Feral Spirit",
                "Fire Elemental Totem",
                "Healing Rain",
                "Lava Lash",
                "Mana Tide Totem",
                "Primal Strike",
                "Shamanistic Rage",
                "Riptide",
                "Spiritwalker's Grace",
                "Spirit Link Totem",
                "Stone Bulwark Totem",
                "Stormlash Totem",
                "Stormstrike",
                "Stormblast",
                "Thunderstorm",
                "Tremor Totem",
                "Unleash Elements",
                "Windwalk Totem",
            };

        #endregion

        #region ClearEnemyandFriendListCache

        private static void ClearEnemyandFriendListCacheEvent(object sender, LuaEventArgs args)
        {
            //Logging.Write("=============================");
            //Logging.Write("ClearEnemyandFriendListCacheEvent");
            //Logging.Write("=============================");

            GlobalCheckTime = DateTime.Now - TimeSpan.FromSeconds(30);

            EnemyListCache.Clear();
            FriendListCache.Clear();
        }

        #endregion

        #region EnemyListCache

        private static readonly Dictionary<ulong, DateTime> EnemyListCache = new Dictionary<ulong, DateTime>();

        private static void EnemyListCacheClear()
        {
            //var indexToRemove = new List<ulong>();

            //foreach (var unit in EnemyList)
            //{
            //    if (unit.Value + TimeSpan.FromSeconds(ExpireSeconds) < DateTime.Now)
            //    {
            //        indexToRemove.Add(unit.Key);
            //    }
            //}

            var indexToRemove = EnemyListCache.Where(
                unit =>
                unit.Value < DateTime.Now).Select(unit => unit.Key).ToList();

            //if (indexToRemove.Count > 0)
            //{
            //    Logging.Write("EnemyList To Be Removed");
            //    foreach (var unit in EnemyList)
            //    {
            //        Logging.Write("Key {0} Value {1}", unit.Key, unit.Value);
            //    }
            //    Logging.Write("_______________________");
            //}

            foreach (var index in indexToRemove)
            {
                //Logging.Write("Remove {0} from EnemyList", EnemyList[index]);
                EnemyListCache.Remove(index);
            }

            //if (indexToRemove.Count <= 0) return;
            //Logging.Write("EnemyList After Removed");
            //foreach (var unit in EnemyList)
            //{
            //    Logging.Write("Key {0} Value {1}", unit.Key, unit.Value);
            //}
            //Logging.Write("_______________________");
        }

        private static void EnemyListCacheAdd(WoWUnit unit, int expireSeconds = 10)
        {
            if (HasAuraArenaPreparation || HasAuraPreparation)
            {
                return;
            }
            if (EnemyListCache.ContainsKey(unit.Guid))
            {
                return;
            }
            //Logging.Write("Add {0} ({1}) to EnemyListCache", unit.Guid, unit.Name);
            EnemyListCache.Add(unit.Guid, DateTime.Now + TimeSpan.FromSeconds(expireSeconds));
        }

        #endregion

        #region FriendListCache

        private static readonly Dictionary<ulong, DateTime> FriendListCache = new Dictionary<ulong, DateTime>();

        private static void FriendListCacheClear()
        {
            //var indexToRemove = new List<ulong>();

            //foreach (var unit in FriendList)
            //{
            //    if (unit.Value + TimeSpan.FromSeconds(ExpireSeconds) < DateTime.Now)
            //    {
            //        indexToRemove.Add(unit.Key);
            //    }
            //}

            var indexToRemove = FriendListCache.Where(
                unit =>
                unit.Value < DateTime.Now).Select(unit => unit.Key).ToList();

            //if (indexToRemove.Count > 0)
            //{
            //    Logging.Write("FriendList To Be Removed");
            //    foreach (var unit in FriendList)
            //    {
            //        Logging.Write("Key {0} Value {1}", unit.Key, unit.Value);
            //    }
            //    Logging.Write("_______________________");
            //}

            foreach (var index in indexToRemove)
            {
                //Logging.Write("Remove {0} from FriendList", FriendList[index]);
                FriendListCache.Remove(index);
            }

            //if (indexToRemove.Count <= 0) return;
            //Logging.Write("FriendList After Removed");
            //foreach (var unit in FriendList)
            //{
            //    Logging.Write("Key {0} Value {1}", unit.Key, unit.Value);
            //}
            //Logging.Write("_______________________");
        }

        private static void FriendListCacheAdd(WoWUnit unit, int expireSeconds = 60)
        {
            if (FriendListCache.ContainsKey(unit.Guid)) return;
            //Logging.Write("Add {0} ({1}) to FriendListCache", unit.Guid, unit.Name);
            FriendListCache.Add(unit.Guid, DateTime.Now + TimeSpan.FromSeconds(expireSeconds));
        }

        #endregion

        #region InLineOfSpellSightCache@

        private static readonly Dictionary<ulong, DateTime> InLineOfSpellSightCache = new Dictionary<ulong, DateTime>();

        private static void InLineOfSpellSightCacheClear()
        {
            foreach (ulong num in (from unit in InLineOfSpellSightCache
                                   where unit.Value < DateTime.Now
                                   select unit.Key).ToList<ulong>())
            {
                InLineOfSpellSightCache.Remove(num);
            }
        }

        private static void InLineOfSpellSightCacheAdd(WoWUnit unit, int expireMilliseconds = 100)
        {
            if (!InLineOfSpellSightCache.ContainsKey(unit.Guid))
            {
                InLineOfSpellSightCache.Add(unit.Guid, DateTime.Now + TimeSpan.FromMilliseconds((double)expireMilliseconds));
            }
        }

        private static bool InLineOfSpellSightCheck(WoWUnit unit)
        {
            if ((unit == null) || !unit.IsValid)
            {
                return false;
            }
            InLineOfSpellSightCacheClear();
            if ((unit != Me) && !InLineOfSpellSightCache.ContainsKey(unit.Guid))
            {
                if (!unit.InLineOfSpellSight)
                {
                    return false;
                }
                if (InRaid || InDungeon)
                {
                    InLineOfSpellSightCacheAdd(unit, 300);
                    return true;
                }
                InLineOfSpellSightCacheAdd(unit, 100);
            }
            return true;
        }

        #endregion

        #region CanCastCache

        private static readonly Dictionary<string, DateTime> SpellsCooldownCache = new Dictionary<string, DateTime>();
        //private static DateTime SkipOnGcdSpellCheck;
        private static DateTime spellCooldownBack;

        private static void SpellsCooldownCacheAdd(string spellName, DateTime spellCooldownBack)
        {
            //if (!SpellsCooldownHS.Contains(spellName))
            //{
            //    //Logging.Write("There's no {0} in SpellsCooldownCache, skip SpellsCooldownCacheAdd", spellName);
            //    return;
            //}
            if (UseSpecialization == 1 && spellName == "Lava Burst")
            {
                return;
            }

            if (spellName.Contains("Shock"))
            {
                SpellsCooldownCache.Add("Earth Shock", spellCooldownBack);
                SpellsCooldownCache.Add("Flame Shock", spellCooldownBack);
                SpellsCooldownCache.Add("Frost Shock", spellCooldownBack);
            }
            else if (spellName == "Primal Strike" || spellName == "Storm Strike" || spellName == "Stormblast")
            {
                SpellsCooldownCache.Add("Primal Strike", spellCooldownBack);
                SpellsCooldownCache.Add("Storm Strike", spellCooldownBack);
                SpellsCooldownCache.Add("Stormblast", spellCooldownBack);
            }
            else
            {
                SpellsCooldownCache.Add(spellName, spellCooldownBack);
            }
        }

        private static void SpellsCooldownCacheClear()
        {
            if (HasAuraArenaPreparation)
            {
                SpellsCooldownCache.Clear();
                return;
            }

            var indexToRemove = SpellsCooldownCache.Where(
                unit =>
                unit.Value < DateTime.Now).Select(unit => unit.Key).ToList();


            foreach (var index in indexToRemove)
            {
                try
                {
                    SpellsCooldownCache.Remove(index);
                }
                catch (Exception)
                {
                    Logging.Write("Fail to SpellsCooldownCache.Remove({0})", index);
                    throw;
                }
            }
        }

        //////done
        //private static bool CanCastCheck(string spellName, bool IsOffGCDSpell = false)
        //{
        //    SpellsCooldownCacheClear();

        //    if (SpellsCooldownCache.ContainsKey(spellName))
        //    {
        //        //Logging.Write("SpellsCooldownCache contains {0}, skip check", spellName);
        //        return false;
        //    }

        //    //if (!IsOffGCDSpell && SkipOnGcdSpellCheck > DateTime.Now)
        //    //{
        //    //    //Logging.Write("CanCastCheck: Global Cooldown back time {0} - Now is {1}, SkipOffGCDSpellCheck check",
        //    //    //              SkipOffGCDSpellCheck.ToString("ss:fff"), DateTime.Now.ToString("ss:fff"));
        //    //    return false;
        //    //}

        //    if (!SpellManager.HasSpell(spellName))
        //    {
        //        return false;
        //    }

        //    if ((InArena || InBattleground) &&
        //        spellName != "Thunderstorm" &&
        //        spellName != "Shamanistic Rage" &&
        //        spellName != "Tremor Totem" &&
        //        DebuffCC(Me))
        //    {
        //        return false;
        //    }

        //    double spellPowerCost;
        //    if (UseSpecialization == 2 &&
        //        SpellManager.Spells[spellName].School.ToString().Contains("Nature"))
        //        //SpellManager.Spells[spellName].School == WoWSpellSchool.Nature)
        //    {
        //        spellPowerCost = (GetSpellPowerCost(spellName) / 100) * (100 - (20 * MyAuraStackCount(53817, Me)));
        //        //Maelstrom Weapon 
        //    }
        //    else
        //    {
        //        spellPowerCost = GetSpellPowerCost(spellName);
        //    }

        //    if (Me.CurrentMana < spellPowerCost)
        //    {
        //        return false;
        //    }

        //    if (spellName == "Elemental Blast")
        //    {
        //        Logging.Write("Elemental Blash:" + MyAuraStackCount(53817, Me) + ":" + SpellManager.Spells[spellName].School);
        //    }

        //    double spellCastTime;
        //    //if (SpellManager.Spells[spellName].School == WoWSpellSchool.Nature &&

        //    if (!IsMoving(Me))
        //    {
        //        spellCastTime = 0;
        //    }
        //    else if (spellName == "Earthgrab Totem" ||
        //        (SpellManager.Spells[spellName].School.ToString().Contains("Nature") &&
        //        (MeHasAura(16188) || UseSpecialization == 2 && MyAuraStackCount(53817, Me) > 4)) ||
        //        (MeHasAura(77762) && spellName == "Lava Burst"))
        //        //Nature Swiftness and Maelstrom Weapon
        //    {
        //        spellCastTime = 0;
        //    }
        //    else
        //    {
        //        spellCastTime = GetSpellCastTime(spellName);
        //    }

        //    if (spellName != "Lightning Bolt" && spellCastTime > 0 && IsMoving(Me) && !MeHasAura("Spiritwalker's Grace"))
        //    {
        //        return false;
        //    }

        //    if (IsOffGCDSpell && GetSpellCooldown(spellName).TotalMilliseconds <= 0)
        //    {
        //        return true;
        //    }


        //    if ((!Me.IsCasting || Me.IsCasting && Me.CurrentCastTimeLeft.TotalMilliseconds <= MyLatency))
        //    {
        //        if ((UseSpecialization == 3) && (((spellName == "Healing Raing") || (spellName == "Cleanse")) || (spellName == "Riptide")))
        //        {
        //            spellCooldownBack = (DateTime.Now + GetSpellCooldown(spellName)) - (TimeSpan.FromMilliseconds(MyLatency) + TimeSpan.FromMilliseconds(300.0));
        //        }
        //        else if (((spellName == "Lightning Bolt") || (spellName == "Healing Wave")) && (MyLatency > 0.0))
        //        {
        //            spellCooldownBack = (DateTime.Now + GetSpellCooldown(spellName)) - TimeSpan.FromMilliseconds(MyLatency * 0.5);
        //        }

        //        else
        //        {
        //            spellCooldownBack = (DateTime.Now + GetSpellCooldown(spellName)) - TimeSpan.FromMilliseconds(MyLatency);
        //        }
        //        //////spellCooldownBack = DateTime.Now + GetSpellCooldown(spellName) - TimeSpan.FromMilliseconds(MyLatency);

        //        if (spellCooldownBack <= DateTime.Now)
        //        {
        //            return true;
        //        }
        //    }

        //    //if (spellCooldownBack > DateTime.Now)
        //    //{
        //    SpellsCooldownCacheAdd(spellName, spellCooldownBack);
        //    //}

        //    //if (SkipOnGcdSpellCheck < DateTime.Now)
        //    //{
        //    //    if (UseLightningShieldGCDCheck)
        //    //    {
        //    //        SkipOnGcdSpellCheck = DateTime.Now + SpellManager.Spells["Lightning Shield"].CooldownTimeLeft -
        //    //                              TimeSpan.FromMilliseconds(MyLatency);
        //    //    }
        //    //    else
        //    //    {
        //    //        SkipOnGcdSpellCheck = DateTime.Now + SpellManager.GlobalCooldownLeft -
        //    //                              TimeSpan.FromMilliseconds(MyLatency);
        //    //    }
        //    //}

        //    return false;
        //}

        private static bool CanCastCheck(string spellName, bool IsOffGCDSpell = false)
        {
            SpellsCooldownCacheClear();
            if (!SpellsCooldownCache.ContainsKey(spellName))
            {
                double spellPowerCost;
                double spellCastTime;
                if (!SpellManager.HasSpell(spellName))
                {
                    return false;
                }
                if ((InArena || InBattleground) && (((spellName != "Thunderstorm") && (spellName != "Shamanistic Rage")) && ((spellName != "Tremor Totem") && DebuffCC(Me))))
                {
                    return false;
                }
                if (spellName == "Elemental Blast")
                {
                    Logging.Write("Elemental Blash:" + MyAuraStackCount(53817, Me) + ":" + SpellManager.Spells[spellName].School);
                }
                if (UseSpecialization == 2 && SpellManager.Spells[spellName].School == WoWSpellSchool.Nature)
                {
                    spellPowerCost = (GetSpellPowerCost(spellName) / 100.0) * (100.0 - (20.0 * MyAuraStackCount(0xd239, Me)));
                }
                else
                {
                    spellPowerCost = GetSpellPowerCost(spellName);
                }
                if (Me.CurrentMana < spellPowerCost)
                {
                    return false;
                }
                if (!IsMoving(Me))
                {
                    spellCastTime = 0.0;
                }
                else if ((((spellName == "Lightning Bolt") || MeHasAura("Spiritwalker's Grace")) || ((spellName == "Lava Burst") && MeHasAura(0x12fc2))) || (SpellManager.Spells[spellName].School.ToString().Contains("Nature") && (MeHasAura(0x3f3c) || ((UseSpecialization == 2) && (MyAuraStackCount(0xd239, Me) > 4.0)))))
                //else if ((((spellName == "Lightning Bolt") || MeHasAura("Spiritwalker's Grace")) || ((spellName == "Lava Burst") && MeHasAura(0x12fc2))) || (SpellManager.Spells[spellName].School == WoWSpellSchool.Nature && (MeHasAura(0x3f3c) || ((UseSpecialization == 2) && (MyAuraStackCount(0xd239, Me) > 4.0)))))
                {
                    spellCastTime = 0.0;
                }
                else
                {
                    spellCastTime = GetSpellCastTime(spellName);
                }
                if ((spellCastTime <= 0.0) || !IsMoving(Me))
                {
                    if (IsOffGCDSpell && (GetSpellCooldown(spellName).TotalMilliseconds <= 0.0))
                    {
                        return true;
                    }
                    if (!Me.IsCasting || ((Me.IsCasting && (Me.CurrentCastTimeLeft.TotalMilliseconds <= MyLatency)) && (Me.CurrentChannelTimeLeft.TotalMilliseconds <= MyLatency)))
                    {
                        spellCooldownBack = (DateTime.Now + GetSpellCooldown(spellName)) - TimeSpan.FromMilliseconds(MyLatency);
                        if (spellCooldownBack <= DateTime.Now)
                        {
                            return true;
                        }
                    }
                    SpellsCooldownCacheAdd(spellName, spellCooldownBack);
                }
            }
            return false;
        }

        #endregion

        #region CanUseCache

        private static readonly Dictionary<WoWItem, DateTime> ItemsCooldownCache = new Dictionary<WoWItem, DateTime>();
        private static DateTime ItemCooldownBack;

        private static TimeSpan GetItemCooldown(WoWItem item)
        {
            // Check for engineering tinkers!
            var itemSpell = Lua.GetReturnVal<string>("return GetItemSpell(" + item.Entry + ")", 0);
            if (string.IsNullOrEmpty(itemSpell) || !item.Usable)
            {
                return TimeSpan.FromSeconds(30);
            }


            return item.CooldownTimeLeft;
        }

        private static void ItemsCooldownCacheAdd(WoWItem itemName, DateTime ItemCooldownBack)
        {
            //Logging.Write("{0} CooldownBack in {1} ms", ItemName, ItemCooldownBack);

            if (ItemCooldownBack > DateTime.Now)
            {
                //Logging.Write("Add {0} to ItemsCooldownCache, cooldown is back in {1} ms.", ItemName,
                //              ItemCooldownBack);
                ItemsCooldownCache.Add(itemName, ItemCooldownBack);
            }
        }

        private static void ItemsCooldownCacheClear()
        {
            var indexToRemove = ItemsCooldownCache.Where(
                unit =>
                unit.Value <= DateTime.Now).Select(unit => unit.Key).ToList();


            foreach (var index in indexToRemove)
            {
                ItemsCooldownCache.Remove(index);
            }
        }

        private static bool CanUseCheck(WoWItem ItemName)
        {
            ItemsCooldownCacheClear();


            if (ItemsCooldownCache.ContainsKey(ItemName))
            {
                //Logging.Write("ItemsCooldownCache contains {0}, skip check", ItemName);
                return false;
            }

            ItemCooldownBack = DateTime.Now + GetItemCooldown(ItemName);

            if (ItemCooldownBack <= DateTime.Now)
            {
                return true;
            }

            ItemsCooldownCacheAdd(ItemName, ItemCooldownBack);
            return false;
        }

        #endregion
    }
}