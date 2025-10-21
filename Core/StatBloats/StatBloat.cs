using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Content.NPCs.NihilityTwin;
using CalamityMod;
using CalamityMod.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.StatBloats
{
    internal class CrossModStatBloats : ModSystem
    {
        internal static List<int> WeaponsVoid = [];
        internal static List<int> WeaponsAbyssalWyrm = [];
        internal static List<int> WeaponsAuric = [];
        internal static List<int> WeaponsRuinousSoul = [];
        internal static List<int> WeaponsPostProvi = [];
        internal static List<int> WeaponsLunarBar = [];
        internal static List<int> WeaponsCosmicBar = [];
        #region Boss表单
        internal static List<int> WeaponsVoidTwin = [];
        internal static List<int> WeaponsCruiser = [];
        #endregion
        internal static List<int> GiantList = [];
        #region 需要单独打表的武器
        internal static Dictionary<int,ItemDamageData> DirectTweaks = [];
        #endregion
        public static bool ActiveStatBloats = false;
        public override void PostAddRecipes()
        {
            GetActive();
            if (!ActiveStatBloats)
                return;

            //全局遍历所有配方
            foreach (Recipe recipe in Main.recipe)
            {
                Item item = recipe.createItem;
                int itemID = item.type;
                //可能没什么必要但还是留着了
                if (item.ModItem is null)
                    continue;

                if (HandleWeaponsVoid(recipe, item))
                    WeaponsVoid.Add(itemID);

                if (HandleWeaponsCosmicBar(recipe, item))
                    WeaponsCosmicBar.Add(itemID);

                if (HandleWeaponsRuinousSoul(recipe, item))
                    WeaponsRuinousSoul.Add(itemID);

                if (HandleWeaponsPostProvi(recipe, item))
                    WeaponsPostProvi.Add(itemID);

                if (HandleWeaponsLunarBar(recipe, item))
                    WeaponsLunarBar.Add(itemID);

                if (HandleWeaponsAbyssalWyrm(recipe, item))
                    WeaponsAbyssalWyrm.Add(itemID);
            }
            
            //这些也太少了，单独打表了
            Add<SolarStorm>(WeaponsAuric);
            Add<PrisonOfPermafrost>(WeaponsAuric);
            Add<ScorchingShoot>(WeaponsAuric);
            Add<Vitalfeather>(WeaponsAuric);

            //遍历所有掉落物，除了材料
            var purpleWormItem = CEUtils.FindLoots<CruiserHead>(false);
            WeaponsCruiser.AddRange(purpleWormItem.Where(id => !WeaponsCruiser.Contains(id)).Distinct());
            var voidTwinItem = CEUtils.FindLoots<NihilityActeriophage>(false);
            WeaponsVoidTwin.AddRange(voidTwinItem.Where(id => !WeaponsVoidTwin.Contains(id)).Distinct());
            

            //最后处理：部分物品如果跟随全局数值膨胀过于爆破，或者你就是想一些武器直接精确修改膨胀后的数值，则按如下格式进行修改：
            //1 首先将跟随了全局膨胀的武器从对应的表单中移除。
            //如果知道这个武器具体在哪个表单则优先直接用Remove的形式移除他
            //否则你就采用UnloadWeapons方法。具体使用方法见私有集合方法内
            Remove<Xytheron>(WeaponsAbyssalWyrm);
            Remove<HadopelagicEchoII>(WeaponsAbyssalWyrm);
            Remove<Zyphros>(WeaponsAbyssalWyrm);
            //2 以DictionaryAdd<武器名>(DirectTweaks, 伤害值) 将你需要的调整写入字典，完成。
            //妖龙大剑
            DictionaryAdd<Xytheron>(DirectTweaks, 50000);
            //回音2
            DictionaryAdd<HadopelagicEchoII>(DirectTweaks, 70000);
            //什么什么晶辉
            DictionaryAdd<Zyphros>(DirectTweaks, 4537);
            //至尊灾厄就只掉一把武器吗，哈基熵，你这家伙……
            DictionaryAdd<TheFilthyContractWithMammon>(DirectTweaks, 4050);
            DictionaryAdd<Mercy>(DirectTweaks, 720);

            //仅仅用于调试目的
            var giantList = WeaponsVoid.Concat(WeaponsCosmicBar)
                                        .Concat(WeaponsCruiser)
                                        .Concat(WeaponsRuinousSoul)
                                        .Concat(WeaponsPostProvi)
                                        .Concat(WeaponsLunarBar)
                                        .Concat(WeaponsVoid)
                                        .Concat(WeaponsVoidTwin)
                                        .Concat(WeaponsAuric)
                                        .Concat(WeaponsAbyssalWyrm);
            GiantList = [.. giantList];
        }
        
        #region 私有方法集合
        private static void Add<T>(List<int> list) where T : ModItem => list.Add(ModContent.ItemType<T>());
        private static void DictionaryAdd<T>(Dictionary<int, ItemDamageData> list, int modifyValue) where T : ModItem
        {
            var data = new ItemDamageData(ModContent.GetInstance<T>().Item.damage, modifyValue);
            list.Add(ModContent.ItemType<T>(), data);
        }
        private void GetActive()
        {
            bool loadedMod = ModLoader.TryGetMod("CalamityInheritance", out Mod inheritance);
            if (!loadedMod)
                return;
            try
            {
                //羁绊的谁家好人ModCall的名字这么难拼啊
                object result = inheritance.Call("GetConfigs", "CalStatInflationBACK");
                if (result is bool active)
                {
                    ActiveStatBloats = active;
                    Mod.Logger.Debug($"已尝试启用数值膨胀，配置：CalStatInflationBack: {active}");
                }
                else
                {
                    ActiveStatBloats = false;
                    Mod.Logger.Debug($"已尝试关闭数值膨胀，实际类型：{result?.GetType().Name}");
                }
            }
            catch (ArgumentException ex)
            {
                Mod.Logger.Warn($"调用ModCall出错！{ex.Message}");
                ActiveStatBloats = false;
            }
        }
        private static void Remove<T>(List<int> list) where T : ModItem => list.Remove(ModContent.ItemType<T>());
        /// <summary>
        /// 卸载对应表单内你输入的元素，这里是通过遍历所有表单实现的
        /// 除非你完全不知道这个武器归属哪个表单，或者你就是想看看灾熵加载速度的下限
        /// 否则不建议调用这个方法
        /// </summary>
        /// <typeparam name="T">模组物品名</typeparam>
        private static void UnloadWeapons<T>() where T : ModItem => UnloadWeapons(ModContent.ItemType<T>());
        /// <summary>
        /// 卸载对应表单内你输入的元素，这里是通过遍历所有表单实现的
        /// 除非你完全不知道这个武器归属哪个表单，或者你就是想看看灾熵加载速度的下限
        /// 否则不建议调用这个方法
        /// </summary>
        /// <param name="itemID">模组物品ID</param>
        private static void UnloadWeapons(int itemID)
        {
            //第一步，将上述使用的所有列表全部组合起来
            List<int>[] arrays =
            [
                WeaponsCruiser,
                WeaponsAbyssalWyrm,
                WeaponsAuric,
                WeaponsCosmicBar,
                WeaponsLunarBar,
                WeaponsPostProvi,
                WeaponsRuinousSoul,
                WeaponsVoid,
                WeaponsVoidTwin
            ];
            //第二步，直接遍历集合所有列表，如果符合条件则移除。
            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Contains(itemID))
                {
                    arrays[i].Remove(itemID);
                    break;
                }
            }
        }
        #endregion
        #region 遍历合成表处理
        private static bool HandleWeaponsAbyssalWyrm(Recipe recipe, Item item)
        {
            //必须有龙牙
            //没了。
            return item.damage > 0
                && recipe.HasIngredient<WyrmTooth>()
                && recipe.createItem.ModItem.Mod == CalamityEntropy.Instance;

        }
        //必须有伤害 材料必须有虚空锭，必须是灾熵实例，没了
        private static bool HandleWeaponsVoid(Recipe recipe, Item item)
        {
            return item.damage > 0
                    && recipe.HasIngredient<VoidBar>()
                    && recipe.createItem.ModItem.Mod == CalamityEntropy.Instance;
        }
        //必须有伤害，必须有宇宙锭/化魂神晶等，必须不能有金源/魔影/虚空锭，必须是灾熵实例
        private static bool HandleWeaponsCosmicBar(Recipe recipe, Item item)
        {
            return item.damage > 0
                    && recipe.createItem.ModItem.Mod == CalamityEntropy.Instance
                    && (recipe.HasIngredient<CosmiliteBar>()
                    || recipe.HasIngredient<AscendantSpiritEssence>()
                    || recipe.HasIngredient<NightmareFuel>()
                    || recipe.HasIngredient<EndothermicEnergy>()
                    || recipe.HasIngredient<DarksunFragment>())
                    && !recipe.HasIngredient<AuricBar>()
                    && !recipe.HasIngredient<VoidBar>()
                    && !recipe.HasIngredient<ShadowspecBar>();
        }
        //必须有伤害，必须有毁灭之灵，必须不能存在神后材料，必须是灾熵实例
        private static bool HandleWeaponsRuinousSoul(Recipe recipe, Item item)
        {
            return item.damage > 0
                && recipe.createItem.ModItem.Mod == CalamityEntropy.Instance
                && recipe.HasIngredient<RuinousSoul>()
                && !recipe.HasIngredient<AscendantSpiritEssence>()
                && !recipe.HasIngredient<CosmiliteBar>()
                && !recipe.HasIngredient<NightmareFuel>()
                && !recipe.HasIngredient<DarksunFragment>()
                && !recipe.HasIngredient<EndothermicEnergy>()
                && !recipe.HasIngredient<AuricBar>()
                && !recipe.HasIngredient<VoidBar>()
                && !recipe.HasIngredient<YharonSoulFragment>();
        }
        //必须有伤害，必须有亵渎后任意一种材料，必须不能存在任何魂花后材料，必须是灾熵实例
        private static bool HandleWeaponsPostProvi(Recipe recipe, Item item)
        {
            return item.damage > 0
                && recipe.createItem.ModItem.Mod == CalamityEntropy.Instance
                && (recipe.HasIngredient<UelibloomBar>()
                || recipe.HasIngredient<BloodstoneCore>()
                || recipe.HasIngredient<ArmoredShell>()
                || recipe.HasIngredient<DarkPlasma>()
                || recipe.HasIngredient<DivineGeode>()
                || recipe.HasIngredient<TwistingNether>())
                && !recipe.HasIngredient<AscendantSpiritEssence>()
                && !recipe.HasIngredient<RuinousSoul>()
                && !recipe.HasIngredient<CosmiliteBar>()
                && !recipe.HasIngredient<NightmareFuel>()
                && !recipe.HasIngredient<DarksunFragment>()
                && !recipe.HasIngredient<AuricBar>()
                && !recipe.HasIngredient<VoidBar>()
                && !recipe.HasIngredient<YharonSoulFragment>();
        }
        private static bool HandleWeaponsLunarBar(Recipe recipe, Item item)
        {
            //必须有伤害，必须合成表有星系异石 必须没有宇宙锭/神圣晶石，必须是灾熵实例
            return item.damage > 0
                    && (recipe.HasIngredient<GalacticaSingularity>() || recipe.HasIngredient(ItemID.LunarBar))
                    && (recipe.createItem.ModItem.Mod == CalamityEntropy.Instance)
                    && !recipe.HasIngredient<CosmiliteBar>()
                    && !recipe.HasIngredient<UelibloomBar>()
                    && !recipe.HasIngredient<RuinousSoul>()
                    && !recipe.HasIngredient<DivineGeode>();
        }
        #endregion
    }
    //原本用的字典，但我不知道过度创建字典会发生啥，这里改成了自定义的一个结构体了
    internal struct ItemDamageData(int itemDamage, int itemModifyDamage)
    {
        public int ItemDamage { get; set; } = itemDamage;
        public int ItemModifyDamage { get; set; } = itemModifyDamage;
    }
    public class StatBloatToNPC : GlobalNPC
    {
        //TODO：写在这里会有个多人同步的问题
        public override void SetDefaults(NPC npc)
        {
            if (!CrossModStatBloats.ActiveStatBloats)
                return;
            //傻逼紫色虫子：五倍血量与2.5倍防御
            if(npc.type == ModContent.NPCType<CruiserHead>() || npc.type == ModContent.NPCType<CruiserBody>() || npc.type == ModContent.NPCType<CruiserTail>())
            {
                npc.lifeMax = (int)(npc.lifeMax * 5f);
                npc.life = (int)(npc.life * 5f);
                npc.defense = (int)(npc.defense * 2.5f);
            }
            if(npc.type == ModContent.NPCType<NihilityActeriophage>() || npc.type == ModContent.NPCType<ChaoticCell>())
            {
                npc.lifeMax = (int)(npc.lifeMax * 2.5f);
                npc.life = (int)(npc.life * 2.5f);
                npc.defense = (int)(npc.defense * 1.5f);
            }
        }
    }
    internal class StatBloatToWeapons : GlobalItem
    {
        public override bool InstancePerEntity => true;
        #region 伤害增幅
        //巡游武器、虚空武器五倍伤害
        private const float CruiserWeaponsBoost = 5f;
        private const float VoidWeaponsBoost = 5f;
        private const float AuricWeaponsBoost = 5f;
        //妖龙系武器跟随魔影梯队，十倍
        //几把的10倍带出一个一千万dps的兵，改成7.5倍了
        private const float AbyssalWyrmWeaponsBoost = 7.5f;
        private const float LunarBarWeaponsBoost = 1.3f;
        private const float PostProviWeaponsBoost = 2.2f;
        private const float RuinousSoulWeaponsBoost = 2.4f;
        private const float CosmicBarWeaponsBoost = 3f;
        #endregion
        //直接修改SD属性
        public override void SetDefaults(Item entity)
        {
            if (!CrossModStatBloats.ActiveStatBloats)
                return;

            if (CrossModStatBloats.WeaponsCruiser.Contains(entity.type))
                entity.damage = (int)(entity.damage * CruiserWeaponsBoost);

            if (CrossModStatBloats.WeaponsVoid.Contains(entity.type))
                entity.damage = (int)(entity.damage * VoidWeaponsBoost);

            if (CrossModStatBloats.WeaponsAbyssalWyrm.Contains(entity.type))
                entity.damage = (int)(entity.damage * AbyssalWyrmWeaponsBoost);

            if (CrossModStatBloats.WeaponsCosmicBar.Contains(entity.type))
                entity.damage = (int)(entity.damage * CosmicBarWeaponsBoost);

            if (CrossModStatBloats.WeaponsLunarBar.Contains(entity.type))
                entity.damage = (int)(entity.damage * LunarBarWeaponsBoost);

            if (CrossModStatBloats.WeaponsPostProvi.Contains(entity.type))
                entity.damage = (int)(entity.damage * PostProviWeaponsBoost);

            if (CrossModStatBloats.WeaponsRuinousSoul.Contains(entity.type))
                entity.damage = (int)(entity.damage * RuinousSoulWeaponsBoost);

            if (CrossModStatBloats.WeaponsAuric.Contains(entity.type))
                entity.damage = (int)(entity.damage * AuricWeaponsBoost);

            if (CrossModStatBloats.WeaponsVoidTwin.Contains(entity.type))
                entity.damage = (int)(entity.damage * PostProviWeaponsBoost);
            
            //遍历这个字典实现批量调整。
            foreach(var (itemID, ItemDamageData) in CrossModStatBloats.DirectTweaks)
            {
                //而后修改这个伤害
                if (entity.type == itemID)
                    entity.damage = ItemDamageData.ItemModifyDamage;
            }

        }
        private enum BoostType
        {
            Percent,
            Direct
        };
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!CrossModStatBloats.ActiveStatBloats)
                return;

            //为所有被修改了数值的物品添加额外的Tooltip
            HandleBoostText(item, tooltips, AbyssalWyrmWeaponsBoost, CrossModStatBloats.WeaponsAbyssalWyrm);
            HandleBoostText(item, tooltips, VoidWeaponsBoost, CrossModStatBloats.WeaponsCruiser, CrossModStatBloats.WeaponsVoid, CrossModStatBloats.WeaponsAuric);
            HandleBoostText(item, tooltips, CosmicBarWeaponsBoost, CrossModStatBloats.WeaponsCosmicBar);
            HandleBoostText(item, tooltips, RuinousSoulWeaponsBoost, CrossModStatBloats.WeaponsRuinousSoul);
            HandleBoostText(item, tooltips, PostProviWeaponsBoost, CrossModStatBloats.WeaponsPostProvi, CrossModStatBloats.WeaponsVoidTwin);
            HandleBoostText(item, tooltips, LunarBarWeaponsBoost, CrossModStatBloats.WeaponsLunarBar);

            //单独打表的情况
            foreach (var (itemID, damage) in CrossModStatBloats.DirectTweaks)
                HandleBoostTextDirect(item, tooltips, itemID, damage.ItemModifyDamage, damage.ItemDamage);
        }
        private void HandleAdd(List<TooltipLine> tooltips, BoostType boostType, float boostValue, int preModifyDamage)
        {
            string path = $"{CEUtils.LocalPrefix}.Weapons.StatBloats";
            //依据修改模式填写
            string text = boostType switch
            {
                BoostType.Direct => $"{path}.Direct".ToLangValue().ToFormatValue(preModifyDamage, (int)boostValue),
                _ => $"{path}.Percent".ToLangValue().ToFormatValue(boostValue),
            };
            //也是用插值的
            var newLine = new TooltipLine(Mod, "Boost", text)
            {
                OverrideColor = new(248, 240, 166, 255)
            };
            var titleLine = new TooltipLine(Mod, "BoostTitle", $"{path}.Title".ToLangValue())
            {
                OverrideColor = new(231, 221, 133, 255)
            };
            int insertIndex = -1;
            //遍历所有的Tooltip行数寻找原版的Tooltip提示 
            for (int i = 0; i < tooltips.Count; i++)
            {
                TooltipLine tooltipLine = tooltips[i];
                //必须是原版实例
                if (tooltipLine.Name is "Tooltip0" && tooltipLine.Mod == "Terraria")
                {
                    insertIndex = i;
                    break;
                }
            }
            //如果可能，就往Tooltip的前一行进行添加
            if(insertIndex != -1)
            {
                tooltips.Insert(insertIndex, newLine);
                tooltips.Insert(insertIndex, titleLine);
            }
        }
        private void HandleBoostTextDirect(Item item, List<TooltipLine> tooltips, int itemID, int boostValue, int damageTweaks)
        {
            //如果不是对应的物品，不做任何事情
            if (item.type != itemID)
                return;
            //直接引用方法。
            HandleAdd(tooltips, BoostType.Direct, boostValue, damageTweaks);
        }
        private void HandleBoostText(Item item, List<TooltipLine> tooltips, float boostValue, params List<int>[] list)
        {
            IEnumerable<int> intList = [];
            foreach (var singleList in list)
            intList = intList.Concat(singleList);
            List<int> toList = [.. intList];

            if (toList.Contains(item.type))
            {
                HandleAdd(tooltips, BoostType.Percent, boostValue, -1);
            }
        }
    }
}
