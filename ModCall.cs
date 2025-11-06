using System;
using System.Collections.Generic;
using System.Linq;

namespace CalamityEntropy
{
    /// <summary>
    /// ModCall 系统，提供类型安全的跨模组通信接口
    /// 但是我他妈的得说，傻逼TML为什么不支持模组互相引用
    /// 我操你妈 - HoCha113
    /// </summary>
    internal static class ModCall
    {
        //存储所有已注册的调用处理器
        private static readonly Dictionary<string, CallHandler> Handlers = new Dictionary<string, CallHandler>(StringComparer.OrdinalIgnoreCase);

        //是否已初始化
        private static bool _initialized = false;

        /// <summary>
        /// 初始化所有 Call 处理器
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            //===== 基础功能 =====
            RegisterHandler("GetVersion", GetVersion);
            RegisterHandler("GetModVersion", GetModVersion);

            //===== Boss 相关 =====
            RegisterHandler("GetBossDown", GetBossDown);
            RegisterHandler("SetBossDown", SetBossDown);
            RegisterHandler("GetBossList", GetBossList);

            //===== 书签系统 =====
            RegisterHandler("RegisterBookMark", RegisterBookMark);
            RegisterHandler("RegisterBookMarkEffect", RegisterBookMarkEffect);
            RegisterHandler("IsBookMark", IsBookMark);

            //===== UI 相关 =====
            RegisterHandler("SetBarColor", SetBarColor);
            RegisterHandler("OpenUI", OpenUI);

            //===== 游戏系统 =====
            RegisterHandler("SetTTHoldoutCheck", SetTTHoldoutCheck);
            RegisterHandler("GetTTHoldoutCheck", GetTTHoldoutCheck);
            RegisterHandler("CopyProjForTTwin", CopyProjForTTwin);

            //===== 玩家数据 =====
            RegisterHandler("GetPlayerData", GetPlayerData);
            RegisterHandler("SetPlayerData", SetPlayerData);

            //===== 物品系统 =====
            RegisterHandler("GetItemData", GetItemData);
            RegisterHandler("RegisterCustomItem", RegisterCustomItem);

            _initialized = true;
            CalamityEntropy.Instance?.Logger?.Info("[ModCall] System initialized with " + Handlers.Count + " handlers");
        }

        /// <summary>
        /// 主调用入口点
        /// </summary>
        public static object Call(params object[] args)
        {
            //确保已初始化
            if (!_initialized)
                Initialize();

            try
            {
                //参数验证
                if (args == null || args.Length == 0)
                    return ErrorResponse("No arguments provided");

                //获取调用名称
                if (!(args[0] is string callName))
                    return ErrorResponse("First argument must be a string (call name)");

                //查找处理器
                if (!Handlers.TryGetValue(callName, out CallHandler handler))
                    return ErrorResponse($"Unknown call: '{callName}'. Use 'GetAvailableCalls' to see all available calls.");

                //提取参数（去除第一个调用名）
                object[] callArgs = args.Skip(1).ToArray();

                //执行处理器
                object result = handler.Execute(callArgs);

                //记录成功调用
                LogCall(callName, true);

                return result;
            }
            catch (Exception ex)
            {
                string errorMsg = $"ModCall error: {ex.Message}";
                CalamityEntropy.Instance?.Logger?.Error(errorMsg);
                CalamityEntropy.Instance?.Logger?.Error(ex.StackTrace);
                return ErrorResponse(errorMsg);
            }
        }

        #region Call Handlers - 基础功能

        private static object GetVersion(object[] args)
        {
            return new
            {
                ModName = "CalamityEntropy",
                Version = CalamityEntropy.Instance?.Version?.ToString() ?? "Unknown",
                ModCallVersion = "2.0"
            };
        }

        private static object GetModVersion(object[] args)
        {
            return CalamityEntropy.Instance?.Version?.ToString() ?? "Unknown";
        }

        #endregion

        #region Call Handlers Boss相关

        private static object GetBossDown(object[] args)
        {
            if (args.Length < 1 || !(args[0] is string bossName))
                throw new ArgumentException("GetBossDown requires boss name (string)");

            //这里可以扩展为更复杂的 boss 击败检测逻辑
            switch (bossName.ToLower())
            {
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

        private static object SetBossDown(object[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("SetBossDown requires boss name (string) and value (bool)");

            if (!(args[0] is string bossName) || !(args[1] is bool value))
                throw new ArgumentException("SetBossDown: Invalid argument types");

            switch (bossName.ToLower())
            {
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

        private static object GetBossList(object[] args)
        {
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

        private static object RegisterBookMark(object[] args)
        {
            if (args.Length < 1 || !(args[0] is Dictionary<string, object> data))
                throw new ArgumentException("RegisterBookMark requires Dictionary<string, object>");

            //调用原有的书签注册逻辑
            //这里保持与原有系统的兼容性
            return CalamityEntropy.Instance.Call("RegisterBookMark", data);
        }

        private static object RegisterBookMarkEffect(object[] args)
        {
            if (args.Length < 1 || !(args[0] is Dictionary<string, object> data))
                throw new ArgumentException("RegisterBookMarkEffect requires Dictionary<string, object>");

            return CalamityEntropy.Instance.Call("RegisterBookMarkEffect", data);
        }

        private static object IsBookMark(object[] args)
        {
            if (args.Length < 1)
                throw new ArgumentException("IsBookMark requires an Item");

            return CalamityEntropy.Instance.Call("IsBookMark", args[0]);
        }

        #endregion

        #region Call Handlers UI相关

        private static object SetBarColor(object[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("SetBarColor requires NPC type (int) and Color");

            if (!(args[0] is int npcType))
                throw new ArgumentException("First argument must be int (NPC type)");

            if (!(args[1] is Microsoft.Xna.Framework.Color color))
                throw new ArgumentException("Second argument must be Color");

            Common.EntropyBossbar.bossbarColor[npcType] = color;
            return SuccessResponse($"Set bar color for NPC type {npcType}");
        }

        private static object OpenUI(object[] args)
        {
            if (args.Length < 1 || !(args[0] is string uiName))
                throw new ArgumentException("OpenUI requires UI name (string)");

            //可扩展的 UI 系统
            switch (uiName.ToLower())
            {
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

        private static object SetTTHoldoutCheck(object[] args)
        {
            if (args.Length < 1 || !(args[0] is bool value))
                throw new ArgumentException("SetTTHoldoutCheck requires bool value");

            Common.EGlobalProjectile.checkHoldOut = value;
            return SuccessResponse($"Set TT Holdout Check to {value}");
        }

        private static object GetTTHoldoutCheck(object[] args)
        {
            return Common.EGlobalProjectile.checkHoldOut;
        }

        private static object CopyProjForTTwin(object[] args)
        {
            if (args.Length < 1 || !(args[0] is int projID))
                throw new ArgumentException("CopyProjForTTwin requires projectile ID (int)");

            return CalamityEntropy.Instance.Call("CopyProjForTTwin", projID);
        }

        #endregion

        #region Call Handlers 数据访问

        private static object GetPlayerData(object[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("GetPlayerData requires player (Player) and key (string)");

            if (!(args[0] is Terraria.Player player))
                throw new ArgumentException("First argument must be Player");

            if (!(args[1] is string key))
                throw new ArgumentException("Second argument must be string (key)");

            var modPlayer = player.GetModPlayer<Common.EModPlayer>();

            //根据 key 返回不同的数据
            switch (key.ToLower())
            {
                //基础数值
                case "voidcharge":
                    return modPlayer.VoidCharge;
                case "brilliancecard":
                    return modPlayer.brillianceCard;
                case "shadowpact":
                    return modPlayer.shadowPact;

                //装备效果
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

                //状态数据
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

                //伤害相关
                case "thorn":
                    return modPlayer.Thorn;
                case "attackvoidtouch":
                    return modPlayer.AttackVoidTouch;
                case "summonercrit":
                case "summoncrit":
                    return modPlayer.summonCrit;
                case "meleedamagereduce":
                    return modPlayer.meleeDamageReduce;

                //潜行相关
                case "roguestealthregen":
                    return modPlayer.RogueStealthRegen;
                case "roguestealthregenmult":
                    return modPlayer.RogueStealthRegenMult;
                case "nostealthregen":
                    return modPlayer.NoNaturalStealthRegen;
                case "extrastealthbar":
                    return modPlayer.ExtraStealthBar;
                case "extrastealth":
                    return modPlayer.ExtraStealth;
                case "shadowstealth":
                    return modPlayer.shadowStealth;

                //武器状态
                case "weaponboost":
                    return modPlayer.WeaponBoost;
                case "shootspeed":
                    return modPlayer.shootSpeed;
                case "manacost":
                    return modPlayer.ManaCost;

                //Boss 相关
                case "cruiserlorebon­us":
                    return modPlayer.CruiserLoreBonus;
                case "nihilitytwinlorebon­us":
                    return modPlayer.NihilityTwinLoreBonus;
                case "prophetlorebon­us":
                    return modPlayer.ProphetLoreBonus;

                //冷却时间
                case "laststandcd":
                    return modPlayer.lastStandCd;
                case "mantlecd":
                    return modPlayer.mantleCd;
                case "magishieldcd":
                    return modPlayer.magiShieldCd;
                case "healingcd":
                    return modPlayer.HealingCd;

                //特殊状态
                case "godhead":
                    return modPlayer.Godhead;
                case "awarraith":
                    return modPlayer.AWraith;
                case "voidinspire":
                    return modPlayer.VoidInspire;
                case "mariviniumset":
                    return modPlayer.MariviniumSet;
                case "mariviniumshieldcount":
                    return modPlayer.MariviniumShieldCount;

                default:
                    throw new ArgumentException($"Unknown player data key: {key}");
            }
        }

        private static object SetPlayerData(object[] args)
        {
            if (args.Length < 3)
                throw new ArgumentException("SetPlayerData requires player (Player), key (string), and value");

            if (!(args[0] is Terraria.Player player))
                throw new ArgumentException("First argument must be Player");

            if (!(args[1] is string key))
                throw new ArgumentException("Second argument must be string (key)");

            var modPlayer = player.GetModPlayer<Common.EModPlayer>();

            //根据 key 设置不同的数据
            switch (key.ToLower())
            {
                //基础数值
                case "voidcharge":
                    if (args[2] is float floatVal)
                        modPlayer.VoidCharge = floatVal;
                    else if (args[2] is double doubleVal)
                        modPlayer.VoidCharge = (float)doubleVal;
                    else
                        throw new ArgumentException("VoidCharge requires float or double value");
                    break;

                case "brilliancecard":
                    if (args[2] is int intVal)
                        modPlayer.brillianceCard = intVal;
                    else
                        throw new ArgumentException("brillianceCard requires int value");
                    break;

                //装备效果 (布尔值)
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

                //状态数据
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

                case "voidinspire":
                    if (args[2] is int intVal5)
                        modPlayer.VoidInspire = intVal5;
                    else
                        throw new ArgumentException("VoidInspire requires int value");
                    break;

                case "extrastealth":
                    if (args[2] is float floatVal7)
                        modPlayer.ExtraStealth = floatVal7;
                    else if (args[2] is double doubleVal7)
                        modPlayer.ExtraStealth = (float)doubleVal7;
                    else
                        throw new ArgumentException("ExtraStealth requires float or double value");
                    break;

                case "shadowstealth":
                    if (args[2] is float floatVal8)
                        modPlayer.shadowStealth = floatVal8;
                    else if (args[2] is double doubleVal8)
                        modPlayer.shadowStealth = (float)doubleVal8;
                    else
                        throw new ArgumentException("shadowStealth requires float or double value");
                    break;

                default:
                    throw new ArgumentException($"Unknown or read-only player data key: {key}");
            }

            return SuccessResponse($"Set player data '{key}'");
        }

        private static object GetItemData(object[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("GetItemData requires item (Item) and key (string)");

            if (!(args[0] is Terraria.Item item))
                throw new ArgumentException("First argument must be Item");

            if (!(args[1] is string key))
                throw new ArgumentException("Second argument must be string (key)");

            var globalItem = item.GetGlobalItem<Common.EGlobalItem>();

            switch (key.ToLower())
            {
                case "dyetype":
                    return globalItem.DyeType;
                default:
                    throw new ArgumentException($"Unknown item data key: {key}");
            }
        }

        private static object RegisterCustomItem(object[] args)
        {
            if (args.Length < 1 || !(args[0] is Dictionary<string, object> data))
                throw new ArgumentException("RegisterCustomItem requires Dictionary<string, object>");

            //这里可以实现自定义物品注册逻辑
            return SuccessResponse("Custom item registration not yet implemented");
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 注册一个调用处理器
        /// </summary>
        private static void RegisterHandler(string name, Func<object[], object> handler, string description = null, string[] paramDescriptions = null)
        {
            Handlers[name] = new CallHandler(name, handler, description, paramDescriptions);
        }

        /// <summary>
        /// 成功响应
        /// </summary>
        private static object SuccessResponse(string message = "Success")
        {
            return new { Success = true, Message = message };
        }

        /// <summary>
        /// 错误响应
        /// </summary>
        private static object ErrorResponse(string error)
        {
            return new { Success = false, Error = error };
        }

        /// <summary>
        /// 记录调用日志（可选）
        /// </summary>
        private static void LogCall(string callName, bool success)
        {
            //可以在这里添加详细的调用日志
            //CalamityEntropy.Instance?.Logger?.Debug($"[ModCall] {callName}: {(success ? "Success" : "Failed")}");
        }

        #endregion

        #region 内部类

        /// <summary>
        /// 调用处理器包装类
        /// </summary>
        private class CallHandler
        {
            public string Name { get; }
            public Func<object[], object> Handler { get; }
            public string Description { get; }
            public string[] ParameterDescriptions { get; }

            public CallHandler(string name, Func<object[], object> handler, string description = null, string[] paramDescriptions = null)
            {
                Name = name;
                Handler = handler;
                Description = description ?? "No description available";
                ParameterDescriptions = paramDescriptions ?? new string[0];
            }

            public object Execute(object[] args)
            {
                return Handler(args);
            }

            public override string ToString()
            {
                return $"{Name}: {Description}";
            }
        }

        #endregion
    }
}
