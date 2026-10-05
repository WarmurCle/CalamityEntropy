using CalamityEntropy.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    /// <summary>
    /// ModCall 系统，提供类型安全的跨模组通信接口
    /// 但是我他妈的得说，傻逼TML为什么不支持模组互相引用
    /// 我操你妈 - HoCha113
    /// </summary>
    internal static class ModCall
    {
        //这个字典存储所有已注册的调用处理器
        private static readonly Dictionary<string, CallHandler> Handlers = new Dictionary<string, CallHandler>(StringComparer.OrdinalIgnoreCase);

        //这个字段标记处理器是否已经初始化
        private static bool _initialized = false;

        /// <summary>
        /// Initialize 登记所有 Call 处理器
        /// </summary>
        public static void Initialize() {
            if (_initialized) {
                return;
            }

            //这里注册基础功能
            RegisterHandler("GetVersion", GetVersion);
            RegisterHandler("GetModVersion", GetModVersion);

            //这里注册 Boss 相关调用
            RegisterHandler("GetBossDown", GetBossDown);
            RegisterHandler("SetBossDown", SetBossDown);
            RegisterHandler("GetBossList", GetBossList);

            //这里注册书签系统调用
            //RegisterBookMark和RegisterBookMarkEffect由CalamityEntropy.Call旧入口处理，此处不注册以避免递归
            RegisterHandler("IsBookMark", IsBookMark);
            RegisterHandler("PerformBookmarkAttack", PerformBookmarkAttack);
            RegisterHandler("GetBookmarkInfo", GetBookmarkInfo_Call);
            RegisterHandler("GetBookmarkAttackCooldown", GetBookmarkAttackCooldown);
            RegisterHandler("GetBookMarkSlots", GetBookMarkSlots);
            RegisterHandler("AddBookMarkSlot", AddBookMarkSlot);
            RegisterHandler("GetPlayerBookmarks", GetPlayerBookmarks);
            RegisterHandler("CanEquipBookmarkWith", CanEquipBookmarkWith);
            RegisterHandler("GetBookmarkUITexture", GetBookmarkUITexture);

            //这里注册界面相关调用
            RegisterHandler("SetBarColor", SetBarColor);
            RegisterHandler("OpenUI", OpenUI);

            //这里注册游戏系统调用
            RegisterHandler("SetTTHoldoutCheck", SetTTHoldoutCheck);
            RegisterHandler("GetTTHoldoutCheck", GetTTHoldoutCheck);
            RegisterHandler("CopyProjForTTwin", CopyProjForTTwin);

            //这里注册玩家数据调用
            RegisterHandler("GetPlayerData", GetPlayerData);
            RegisterHandler("SetPlayerData", SetPlayerData);

            //这里注册物品系统调用
            RegisterHandler("GetItemData", GetItemData);
            RegisterHandler("RegisterCustomItem", RegisterCustomItem);

            _initialized = true;
            CalamityEntropy.Instance?.Logger?.Info("[ModCall] System initialized with " + Handlers.Count + " handlers");
        }

        /// <summary>
        /// Call 是对外的主入口
        /// </summary>
        public static object Call(params object[] args) {
            //确保已初始化
            if (!_initialized)
                Initialize();

            try {
                //这里先验证传入参数
                if (args == null || args.Length == 0)
                    return ErrorResponse("No arguments provided");

                //这里取出调用名称
                if (!(args[0] is string callName))
                    return ErrorResponse("First argument must be a string (call name)");

                //这里查找处理器,没注册的 Call 返回 null,交给旧系统处理
                if (!Handlers.TryGetValue(callName, out CallHandler handler))
                    return null;

                //这里去掉第一个调用名,留下其余参数
                object[] callArgs = args.Skip(1).ToArray();

                //这里执行找到的处理器
                object result = handler.Execute(callArgs);

                //这里记下这次调用成功
                LogCall(callName, true);

                return result;
            } catch (Exception ex) {
                string errorMsg = $"ModCall error: {ex.Message}";
                CalamityEntropy.Instance?.Logger?.Error(errorMsg);
                CalamityEntropy.Instance?.Logger?.Error(ex.StackTrace);
                return ErrorResponse(errorMsg);
            }
        }

        #region Call Handlers - 基础功能

        private static object GetVersion(object[] args) {
            return new {
                ModName = "CalamityEntropy",
                Version = CalamityEntropy.Instance?.Version?.ToString() ?? "Unknown",
                ModCallVersion = "2.0"
            };
        }

        private static object GetModVersion(object[] args) {
            return CalamityEntropy.Instance?.Version?.ToString() ?? "Unknown";
        }

        #endregion

        #region Call Handlers Boss相关

        private static object GetBossDown(object[] args) {
            if (args.Length < 1 || !(args[0] is string bossName))
                throw new ArgumentException("GetBossDown requires boss name (string)");

            //这里可以扩展为更复杂的 boss 击败检测逻辑
            switch (bossName.ToLower()) {
                case "cruiser":
                    return Common.EDownedBosses.downedCruiser;
                case "nihilitytwin":
                case "nihility_twin":
                    return Common.EDownedBosses.downedNihilityTwin;
                case "prophet":
                    return Common.EDownedBosses.downedProphet;
                case "luminaris":
                    return Common.EDownedBosses.downedLuminaris;
                case "acropolis":
                    return Common.EDownedBosses.downedAcropolis;
                default:
                    throw new ArgumentException($"Unknown boss: {bossName}");
            }
        }

        private static object SetBossDown(object[] args) {
            if (args.Length < 2)
                throw new ArgumentException("SetBossDown requires boss name (string) and value (bool)");

            if (!(args[0] is string bossName) || !(args[1] is bool value))
                throw new ArgumentException("SetBossDown: Invalid argument types");

            switch (bossName.ToLower()) {
                case "cruiser":
                    Common.EDownedBosses.downedCruiser = value;
                    break;
                case "nihilitytwin":
                case "nihility_twin":
                    Common.EDownedBosses.downedNihilityTwin = value;
                    break;
                case "prophet":
                    Common.EDownedBosses.downedProphet = value;
                    break;
                case "luminaris":
                    Common.EDownedBosses.downedLuminaris = value;
                    break;
                case "acropolis":
                    Common.EDownedBosses.downedAcropolis = value;
                    break;
                default:
                    throw new ArgumentException($"Unknown boss: {bossName}");
            }

            return SuccessResponse($"Set {bossName} down status to {value}");
        }

        private static object GetBossList(object[] args) {
            return new string[]
            {
                "Cruiser",
                "NihilityTwin",
                "Prophet",
                "Luminaris",
                "Acropolis"
            };
        }

        #endregion

        #region Call Handlers 书签系统

        private static object IsBookMark(object[] args) {
            if (args.Length < 1)
                throw new ArgumentException("IsBookMark需要1个参数: Item或int");

            if (args[0] is Item item)
                return BookMarkLoader.IsABookMark(item);

            if (args[0] is int typeId) {
                //IsBookMark 先查自定义注册表,再检查 ModContent 里的原生书签
                if (BookMarkLoader.CustomBMByID.ContainsKey(typeId))
                    return true;
                Item sample = Terraria.ID.ContentSamples.ItemsByType.TryGetValue(typeId, out var s) ? s : null;
                return sample != null && sample.ModItem is Content.Items.Books.BookMarks.BookMark;
            }

            throw new ArgumentException("第1个参数必须是Item或int(物品类型ID)");
        }

        /// <summary>
        /// 从第 5 个参数起可以省略,依次是伤害,击退,弹幕类型,射速,冷却,伤害类型名
        /// 返回值带 success, cooldownTicks, projectileIndex, projectileType
        /// </summary>
        private static object PerformBookmarkAttack(object[] args) {
            if (args.Length < 4)
                throw new ArgumentException("PerformBookmarkAttack至少需要4个参数: Item, Player, Vector2 position, Vector2 direction");

            if (!(args[0] is Item bookmarkItem))
                throw new ArgumentException("第1个参数必须是Item(书签物品)");
            if (!(args[1] is Player player))
                throw new ArgumentException("第2个参数必须是Player");
            if (!(args[2] is Vector2 position))
                throw new ArgumentException("第3个参数必须是Vector2(发射位置)");
            if (!(args[3] is Vector2 direction))
                throw new ArgumentException("第4个参数必须是Vector2(方向)");

            //下面读可以省略的参数
            int baseDamage = args.Length > 4 && args[4] is int d ? d : 50;
            float baseKnockback = args.Length > 5 && args[5] is float kb ? kb : 2f;
            int baseProjectileType = args.Length > 6 && args[6] is int pt ? pt : -1;
            float baseShootSpeed = args.Length > 7 && args[7] is float ss ? ss : 12f;
            int baseCooldown = args.Length > 8 && args[8] is int cd ? cd : 20;

            //伤害类型可以传字符串名
            DamageClass damageClass = DamageClass.Magic;
            if (args.Length > 9) {
                if (args[9] is DamageClass dc)
                    damageClass = dc;
                else if (args[9] is string dcName)
                    damageClass = ResolveDamageClass(dcName);
            }

            var result = BookMarkLoader.PerformBookmarkAttack(
                bookmarkItem, player, position, direction,
                baseDamage, baseKnockback, baseProjectileType, baseShootSpeed, baseCooldown, damageClass);

            //这里返回 Dictionary,方便别的模组弱引用调用
            return new Dictionary<string, object> {
                ["success"] = result.Success,
                ["cooldownTicks"] = result.CooldownTicks,
                ["projectileIndex"] = result.ProjectileIndex,
                ["projectileType"] = result.ProjectileType
            };
        }

        /// <summary>GetBookmarkInfo 只查询书签,不触发攻击</summary>
        private static object GetBookmarkInfo_Call(object[] args) {
            if (args.Length < 1)
                throw new ArgumentException("GetBookmarkInfo需要1个参数: Item");

            if (!(args[0] is Item bookmarkItem))
                throw new ArgumentException("第1个参数必须是Item");

            var info = BookMarkLoader.GetBookmarkInfo(bookmarkItem);
            if (info == null)
                return new Dictionary<string, object> { ["isBookmark"] = false };

            var dict = new Dictionary<string, object> {
                ["isBookmark"] = info.IsBookmark,
                ["hasEffect"] = info.HasEffect,
                ["hasStatModifiers"] = info.HasStatModifiers,
                ["replacesBaseProjectile"] = info.ReplacesBaseProjectile,
                ["replacesProjectile"] = info.ReplacesProjectile,
                ["modifiesCooldown"] = info.ModifiesCooldown
            };

            //有属性快照时,这里把各项数值写进返回字典
            if (info.StatSnapshot != null) {
                dict["statDamage"] = info.StatSnapshot.Damage;
                dict["statKnockback"] = info.StatSnapshot.Knockback;
                dict["statShotSpeed"] = info.StatSnapshot.shotSpeed;
                dict["statHoming"] = info.StatSnapshot.Homing;
                dict["statSize"] = info.StatSnapshot.Size;
                dict["statCrit"] = info.StatSnapshot.Crit;
                dict["statHomingRange"] = info.StatSnapshot.HomingRange;
                dict["statPenetrateAddition"] = info.StatSnapshot.PenetrateAddition;
                dict["statAttackSpeed"] = info.StatSnapshot.attackSpeed;
                dict["statArmorPenetration"] = info.StatSnapshot.armorPenetration;
                dict["statLifeSteal"] = info.StatSnapshot.lifeSteal;
            }

            return dict;
        }

        /// <summary>这个调用不发射弹幕,baseCooldown 默认 20,再按攻速缩短</summary>
        private static object GetBookmarkAttackCooldown(object[] args) {
            if (args.Length < 1)
                throw new ArgumentException("GetBookmarkAttackCooldown需要至少1个参数: Item");

            if (!(args[0] is Item bookmarkItem))
                throw new ArgumentException("第1个参数必须是Item");

            if (!BookMarkLoader.IsABookMark(bookmarkItem))
                return 0;

            int baseCooldown = args.Length > 1 && args[1] is int cd ? cd : 20;
            BookMarkLoader.modifyShootCooldown(bookmarkItem, ref baseCooldown);

            //这里按攻速缩短冷却
            var modifer = new Content.Items.Books.EBookStatModifer();
            BookMarkLoader.ModifyStat(bookmarkItem, modifer);
            if (modifer.attackSpeed > 0)
                baseCooldown = (int)(baseCooldown / modifer.attackSpeed);

            return Math.Max(1, baseCooldown);
        }

        /// <summary>调用方不传书时,改用玩家手持物品</summary>
        private static object GetBookMarkSlots(object[] args) {
            if (args.Length < 1 || !(args[0] is Player player))
                throw new ArgumentException("GetBookMarkSlots需要Player参数");

            Item book = args.Length > 1 && args[1] is Item b ? b : player.HeldItem;
            return player.GetMyMaxActiveBookMarks(book);
        }

        /// <summary>这个加成每帧会被重置,要在 UpdateEquips 里持续加上</summary>
        private static object AddBookMarkSlot(object[] args) {
            if (args.Length < 2)
                throw new ArgumentException("AddBookMarkSlot需要2个参数: Player, int");

            if (!(args[0] is Player player))
                throw new ArgumentException("第1个参数必须是Player");
            if (!(args[1] is int count))
                throw new ArgumentException("第2个参数必须是int");

            player.Entropy().AdditionalBookmarkSlot += count;
            return true;
        }

        /// <summary>返回数组按当前栏位数截断,空位放空 Item</summary>
        private static object GetPlayerBookmarks(object[] args) {
            if (args.Length < 1 || !(args[0] is Player player))
                throw new ArgumentException("GetPlayerBookmarks需要Player参数");

            var items = player.Entropy().EBookStackItems;
            if (items == null)
                return Array.Empty<Item>();

            int max = player.GetMyMaxActiveBookMarks(player.HeldItem);
            var result = new Item[max];
            for (int i = 0; i < max; i++) {
                result[i] = i < items.Count ? items[i] : new Item();
            }
            return result;
        }

        private static object CanEquipBookmarkWith(object[] args) {
            if (args.Length < 2)
                throw new ArgumentException("CanEquipBookmarkWith需要2个参数: Item, Item");

            if (!(args[0] is Item a) || !(args[1] is Item b))
                throw new ArgumentException("参数必须是Item");

            if (!BookMarkLoader.IsABookMark(a))
                return false;

            return BookMarkLoader.CanBeEquipWith(a, b);
        }

        private static object GetBookmarkUITexture(object[] args) {
            if (args.Length < 1 || !(args[0] is Item item))
                throw new ArgumentException("GetBookmarkUITexture需要1个参数: Item");

            return BookMarkLoader.GetUITexture(item);
        }

        private static DamageClass ResolveDamageClass(string name) {
            switch (name.ToLower()) {
                case "melee": return DamageClass.Melee;
                case "ranged": return DamageClass.Ranged;
                case "magic": return DamageClass.Magic;
                case "summon": return DamageClass.Summon;
                case "generic": return DamageClass.Generic;
                default: return DamageClass.Magic;
            }
        }

        #endregion

        #region Call Handlers UI相关

        private static object SetBarColor(object[] args) {
            if (args.Length < 2)
                throw new ArgumentException("SetBarColor requires NPC type (int) and Color");

            if (!(args[0] is int npcType))
                throw new ArgumentException("First argument must be int (NPC type)");

            if (!(args[1] is Microsoft.Xna.Framework.Color color))
                throw new ArgumentException("Second argument must be Color");

            Common.EntropyBossbar.bossbarColor[npcType] = color;
            return SuccessResponse($"Set bar color for NPC type {npcType}");
        }

        private static object OpenUI(object[] args) {
            if (args.Length < 1 || !(args[0] is string uiName))
                throw new ArgumentException("OpenUI requires UI name (string)");

            //OpenUI 按名字打开界面,以后可以在这里加分支
            switch (uiName.ToLower()) {
                case "armorforging":
                case "armor_forging":
                    Content.UI.ArmorForgingStationUI.Visible = true;
                    return SuccessResponse("Opened Armor Forging UI");
                default:
                    throw new ArgumentException($"Unknown UI: {uiName}");
            }
        }

        #endregion

        #region Call Handlers 游戏系统

        private static object SetTTHoldoutCheck(object[] args) {
            if (args.Length < 1 || !(args[0] is bool value))
                throw new ArgumentException("SetTTHoldoutCheck requires bool value");

            Common.EGlobalProjectile.checkHoldOut = value;
            return SuccessResponse($"Set TT Holdout Check to {value}");
        }

        private static object GetTTHoldoutCheck(object[] args) {
            return Common.EGlobalProjectile.checkHoldOut;
        }

        private static object CopyProjForTTwin(object[] args) {
            if (args.Length < 1 || !(args[0] is int projID))
                throw new ArgumentException("CopyProjForTTwin requires projectile ID (int)");

            //不能回调 CalamityEntropy.Instance.Call:Call 的第一句就是 ModCall 分发,
            //会再次命中本处理器,一次调用直接爆栈,而 StackOverflowException 抓不住
            Core.Integrations.CELegacyCallApi.CopyProjectileForTwistedTwin(projID);
            return SuccessResponse($"Copied projectile {projID} for Twisted Twins");
        }

        #endregion

        #region Call Handlers 数据访问

        private static object GetPlayerData(object[] args) {
            if (args.Length < 2)
                throw new ArgumentException("GetPlayerData requires player (Player) and key (string)");

            if (!(args[0] is Terraria.Player player))
                throw new ArgumentException("First argument must be Player");

            if (!(args[1] is string key))
                throw new ArgumentException("Second argument must be string (key)");

            var modPlayer = player.GetModPlayer<Common.EModPlayer>();

            //GetPlayerData 按 key 返回不同的数据
            switch (key.ToLower()) {
                //下面返回基础数值
                case "brilliancecard":
                    return modPlayer.brillianceCard;
                //脱离灾厄:shadowpact键随潜行退役删除

                //下面返回装备效果
                case "heartofstorm":
                    return modPlayer.heartOfStorm;
                case "deuscore":
                    return modPlayer.deusCore;
                case "mawofvoid":
                    return modPlayer.mawOfVoid;
                case "revelation":
                    return modPlayer.revelation;
                case "wyrmphantom":
                    return modPlayer.wyrmPhantom;
                case "vetrasylseye":
                    return modPlayer.vetrasylsEye;
                case "holymantle":
                    return modPlayer.holyMantle;
                case "holyshield":
                    return modPlayer.HolyShield;
                case "magishield":
                    return modPlayer.MagiShield;
                case "nihilityshell":
                    return modPlayer.nihShell;

                //下面返回状态数据
                case "liferegenpersec":
                    return modPlayer.lifeRegenPerSec;
                case "dodgechance":
                    return modPlayer.dodgeChance;
                case "voidresistance":
                    return modPlayer.voidResistance;
                case "temporaryarmor":
                    return modPlayer.temporaryArmor;
                case "enhancedmana":
                    return modPlayer.enhancedMana;
                case "movespeed":
                    return modPlayer.moveSpeed;
                case "cooldowntimemult":
                    return modPlayer.CooldownTimeMult;
                case "wingspeed":
                    return modPlayer.WingSpeed;
                case "wingtimemult":
                    return modPlayer.WingTimeMult;

                //下面返回伤害相关数据
                case "thorn":
                    return modPlayer.Thorn;
                case "attackvoidtouch":
                    return modPlayer.AttackVoidTouch;
                case "summonercrit":
                case "summoncrit":
                    return modPlayer.summonCrit;
                case "meleedamagereduce":
                    return modPlayer.meleeDamageReduce;

                //脱离灾厄:潜行相关键(roguestealthregen/roguestealthregenmult/nostealthregen/extrastealthbar/extrastealth/shadowstealth)随盗贼系统退役删除

                //下面返回武器状态
                case "weaponboost":
                    return modPlayer.WeaponBoost;
                case "shootspeed":
                    return modPlayer.shootSpeed;
                case "manacost":
                    return modPlayer.ManaCost;

                //下面返回 Boss 相关数据
                case "cruiserlorebon­us":
                    return modPlayer.CruiserLoreBonus;
                case "nihilitytwinlorebon­us":
                    return modPlayer.NihilityTwinLoreBonus;
                case "prophetlorebon­us":
                    return modPlayer.ProphetLoreBonus;

                //下面返回冷却时间
                case "laststandcd":
                    return modPlayer.lastStandCd;
                case "mantlecd":
                    return modPlayer.mantleCd;
                case "magishieldcd":
                    return modPlayer.magiShieldCd;
                case "healingcd":
                    return modPlayer.HealingCd;

                //下面返回特殊状态
                case "godhead":
                    return modPlayer.Godhead;
                case "mariviniumset":
                    return modPlayer.MariviniumSet;
                case "mariviniumshieldcount":
                    return modPlayer.MariviniumShieldCount;

                default:
                    throw new ArgumentException($"Unknown player data key: {key}");
            }
        }

        private static object SetPlayerData(object[] args) {
            if (args.Length < 3)
                throw new ArgumentException("SetPlayerData requires player (Player), key (string), and value");

            if (!(args[0] is Terraria.Player player))
                throw new ArgumentException("First argument must be Player");

            if (!(args[1] is string key))
                throw new ArgumentException("Second argument must be string (key)");

            var modPlayer = player.GetModPlayer<Common.EModPlayer>();

            //SetPlayerData 按 key 写入不同的数据
            switch (key.ToLower()) {
                case "brilliancecard":
                    if (args[2] is int intVal)
                        modPlayer.brillianceCard = intVal;
                    else
                        throw new ArgumentException("brillianceCard requires int value");
                    break;

                //下面写入装备效果,值是布尔
                case "heartofstorm":
                    if (args[2] is bool boolVal)
                        modPlayer.heartOfStorm = boolVal;
                    else
                        throw new ArgumentException("heartOfStorm requires bool value");
                    break;

                case "deuscore":
                    if (args[2] is bool boolVal2)
                        modPlayer.deusCore = boolVal2;
                    else
                        throw new ArgumentException("deusCore requires bool value");
                    break;

                case "holymantle":
                    if (args[2] is bool boolVal3)
                        modPlayer.holyMantle = boolVal3;
                    else
                        throw new ArgumentException("holyMantle requires bool value");
                    break;

                case "holyshield":
                    if (args[2] is bool boolVal4)
                        modPlayer.HolyShield = boolVal4;
                    else
                        throw new ArgumentException("HolyShield requires bool value");
                    break;

                case "magishield":
                    if (args[2] is int intVal2)
                        modPlayer.MagiShield = intVal2;
                    else
                        throw new ArgumentException("MagiShield requires int value");
                    break;

                //下面写入状态数据
                case "liferegenpersec":
                    if (args[2] is int intVal3)
                        modPlayer.lifeRegenPerSec = intVal3;
                    else
                        throw new ArgumentException("lifeRegenPerSec requires int value");
                    break;

                case "dodgechance":
                    if (args[2] is float floatVal2)
                        modPlayer.dodgeChance = floatVal2;
                    else if (args[2] is double doubleVal2)
                        modPlayer.dodgeChance = (float)doubleVal2;
                    else
                        throw new ArgumentException("dodgeChance requires float or double value");
                    break;

                case "voidresistance":
                    if (args[2] is float floatVal3)
                        modPlayer.voidResistance = floatVal3;
                    else if (args[2] is double doubleVal3)
                        modPlayer.voidResistance = (float)doubleVal3;
                    else
                        throw new ArgumentException("voidResistance requires float or double value");
                    break;

                case "temporaryarmor":
                    if (args[2] is float floatVal4)
                        modPlayer.temporaryArmor = floatVal4;
                    else if (args[2] is double doubleVal4)
                        modPlayer.temporaryArmor = (float)doubleVal4;
                    else
                        throw new ArgumentException("temporaryArmor requires float or double value");
                    break;

                case "movespeed":
                    if (args[2] is float floatVal5)
                        modPlayer.moveSpeed = floatVal5;
                    else if (args[2] is double doubleVal5)
                        modPlayer.moveSpeed = (float)doubleVal5;
                    else
                        throw new ArgumentException("moveSpeed requires float or double value");
                    break;

                case "cooldowntimemult":
                    if (args[2] is float floatVal6)
                        modPlayer.CooldownTimeMult = floatVal6;
                    else if (args[2] is double doubleVal6)
                        modPlayer.CooldownTimeMult = (float)doubleVal6;
                    else
                        throw new ArgumentException("CooldownTimeMult requires float or double value");
                    break;

                case "weaponboost":
                    if (args[2] is int intVal4)
                        modPlayer.WeaponBoost = intVal4;
                    else
                        throw new ArgumentException("WeaponBoost requires int value");
                    break;

                //脱离灾厄:extrastealth/shadowstealth写入键随盗贼系统退役删除

                default:
                    throw new ArgumentException($"Unknown or read-only player data key: {key}");
            }

            return SuccessResponse($"Set player data '{key}'");
        }

        private static object GetItemData(object[] args) {
            if (args.Length < 2)
                throw new ArgumentException("GetItemData requires item (Item) and key (string)");

            if (!(args[0] is Terraria.Item item))
                throw new ArgumentException("First argument must be Item");

            if (!(args[1] is string key))
                throw new ArgumentException("Second argument must be string (key)");

            var globalItem = item.GetGlobalItem<Common.EGlobalItem>();

            switch (key.ToLower()) {
                case "dyetype":
                    return globalItem.DyeType;
                default:
                    throw new ArgumentException($"Unknown item data key: {key}");
            }
        }

        private static object RegisterCustomItem(object[] args) {
            if (args.Length < 1 || !(args[0] is Dictionary<string, object> data))
                throw new ArgumentException("RegisterCustomItem requires Dictionary<string, object>");

            //这里可以实现自定义物品注册逻辑
            return SuccessResponse("Custom item registration not yet implemented");
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// RegisterHandler 登记一个调用处理器
        /// </summary>
        private static void RegisterHandler(string name, Func<object[], object> handler, string description = null, string[] paramDescriptions = null) {
            Handlers[name] = new CallHandler(name, handler, description, paramDescriptions);
        }

        /// <summary>
        /// SuccessResponse 返回成功结果
        /// </summary>
        private static object SuccessResponse(string message = "Success") {
            return new { Success = true, Message = message };
        }

        /// <summary>
        /// ErrorResponse 返回错误结果
        /// </summary>
        private static object ErrorResponse(string error) {
            return new { Success = false, Error = error };
        }

        /// <summary>
        /// LogCall 记录调用日志,目前留空
        /// </summary>
        private static void LogCall(string callName, bool success) {
            //可以在这里添加详细的调用日志
            //CalamityEntropy.Instance?.Logger?.Debug($"[ModCall] {callName}: {(success ? "Success" : "Failed")}");
        }

        #endregion

        #region 内部类

        /// <summary>
        /// CallHandler 包住一个调用处理器
        /// </summary>
        private class CallHandler
        {
            public string Name { get; }
            public Func<object[], object> Handler { get; }
            public string Description { get; }
            public string[] ParameterDescriptions { get; }

            public CallHandler(string name, Func<object[], object> handler, string description = null, string[] paramDescriptions = null) {
                Name = name;
                Handler = handler;
                Description = description ?? "No description available";
                ParameterDescriptions = paramDescriptions ?? new string[0];
            }

            public object Execute(object[] args) {
                return Handler(args);
            }

            public override string ToString() {
                return $"{Name}: {Description}";
            }
        }

        #endregion
    }
}
