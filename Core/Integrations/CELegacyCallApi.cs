using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using static CalamityEntropy.Common.EGlobalNPC;

namespace CalamityEntropy.Core.Integrations
{
    /// <summary>
    /// ModCall 字典先跑,非 null 就停;没登记或返回 null 才到这里
    /// IsBookMark、SetBarColor、GetBookMarkSlots、AddBookMarkSlot、SetTTHoldoutCheck、GetTTHoldoutCheck、CopyProjForTTwin 两边都有,ModCall 恒非 null,这里的分支到不了,删了会改对外面
    /// </summary>
    internal static class CELegacyCallApi
    {
        public static object Handle(object[] args) {
            try {
                if (args.Length > 0) {
                    if (args[0] is string str) {
                        //Usage: bool flag = (bool)Mod.Call("CheckFlag", "cruiser(or any name below)");
                        if (str.ToLower().Equals("checkflag")) {
                            if (args.Length == 2 && args[1] is string name) {
                                name = name.ToLower();
                                if (name == "acropolis")
                                    return EDownedBosses.downedAcropolis;
                                if (name == "apsychos")
                                    return EDownedBosses.downedApsychos;
                                if (name == "luminaris")
                                    return EDownedBosses.downedLuminaris;
                                if (name == "prophet")
                                    return EDownedBosses.downedProphet;
                                if (name == "nihility_twins")
                                    return EDownedBosses.downedNihilityTwin;
                                if (name == "cruiser")
                                    return EDownedBosses.downedCruiser;
                            }
                            return false;
                        }
                        if (str.ToLower().Equals("RegisterBookMarkEffect".ToLower())) {
                            return RegisterBookmarkEffect(args);
                        }
                        if (str.ToLower().Equals("RegisterBookMark".ToLower())) {
                            return RegisterBookmark(args);
                        }
                        if (str.Equals("IsBookMark")) {
                            Item item = (Item)args[1];
                            return BookMarkLoader.IsABookMark(item);
                        }
                        #region TwistedTwinsStuff
                        if (str.Equals("SetTTHoldoutCheck")) {
                            EGlobalProjectile.checkHoldOut = (bool)args[1];
                        }
                        if (str.Equals("GetTTHoldoutCheck")) {
                            return EGlobalProjectile.checkHoldOut;
                        }
                        if (str.Equals("CopyProjForTTwin")) {
                            CopyProjectileForTwistedTwin((int)args[1]);
                        }
                        #endregion
                        //Usage: Mod.Call("SetBarColor", ModContent.NPCType<T>(), color);
                        if (str.Equals("SetBarColor")) {
                            int type = (int)args[1];
                            Color color = (Color)args[2];
                            EntropyBossbar.bossbarColor[type] = color;
                        }
                        if (str.Equals("GetBookMarkSlots")) {
                            return ((Player)args[1]).GetMyMaxActiveBookMarks(((Player)args[1]).HeldItem);
                        }
                        if (str.Equals("AddBookMarkSlot")) //每帧重置,要在 UpdateEquips 里持续加
                        {
                            ((Player)args[1]).Entropy().AdditionalBookmarkSlot += (int)args[2];
                        }
                        if (str.Equals("AddBookMarkSlotSpecialTexture")) //每帧清空,仅客户端
                        {
                            ((Player)args[1]).Entropy().BookmarkHolderSpecialTextures.Add((Texture2D)args[2]);
                        }
                        if (str.Equals("RegisterDebuff")) {
                            ExternalDebuffs.Add(
                                new DebuffDisplayEntry(
                                    (Func<NPC, bool>)args[1],
                                    (Func<Texture2D>)args[2]
                                )
                            );
                            return null;
                        }
                    }
                }
            } catch {
                string e = (args[0] is string str) ? $"({str})" : "";
                CalamityEntropy.Instance.Logger.Warn($"CalamityEntropy: ModCall's parameter is Error!{e}");
            }
            return null;
        }

        /// <summary>
        /// ModCall 同名也走这里,别再 Instance.Call 回来,Call 开头就是 ModCall,自己调自己会爆栈,StackOverflowException 抓不住
        /// </summary>
        public static void CopyProjectileForTwistedTwin(int projectileIndex) {
            Projectile projectile = projectileIndex.ToProj();
            int twinType = ModContent.ProjectileType<TwistedTwinMinion>();
            EGlobalProjectile.checkHoldOut = false;
            foreach (Projectile p in Main.ActiveProjectiles) {
                if (p.type != twinType || p.owner != Main.myPlayer) {
                    continue;
                }
                int phd = Projectile.NewProjectile(Main.LocalPlayer.GetSource_ItemUse(Main.LocalPlayer.HeldItem), p.Center, Vector2.Zero,
                    projectile.type, projectile.damage, projectile.knockBack, projectile.owner);
                Projectile ph = phd.ToProj();
                ph.scale *= 0.8f;
                ph.Entropy().IndexOfTwistedTwinShootedThisProj = p.identity;
                ph.netUpdate = true;
                ph.damage = (int)(ph.damage * TwistedTwinMinion.damageMul);
                if (!ph.usesLocalNPCImmunity) {
                    ph.usesLocalNPCImmunity = true;
                    ph.localNPCHitCooldown = 12;
                }
            }
            EGlobalProjectile.checkHoldOut = true;
        }

        private static object RegisterBookmarkEffect(object[] args) {
            if (!(args[1] is Dictionary<string, object> objects)) {
                CalamityEntropy.Instance.Logger.Warn("Args[1] Must be a Dictionary<string, object>");
                return null;
            }
            if (!objects.TryGetValue("Name", out object nameObj) || !(nameObj is string)) {
                CalamityEntropy.Instance.Logger.Warn("Name is required and must be a string");
                return null;
            }
            string name = (string)nameObj;

            BookMarkLoader.RegisterBookmarkEffect(
                name,
                GetAction<ModProjectile>(objects, "OnShoot"),
                GetAction<ModProjectile>(objects, "OnActive"),
                GetAction<Projectile, bool>(objects, "OnProjectileSpawn"),
                GetAction<Projectile, bool>(objects, "UpdateProjectile"),
                GetAction<Projectile, NPC, int>(objects, "OnHitNPC"),
                GetAction<Projectile, NPC, NPC.HitModifiers>(objects, "ModifyHitNPC"),
                GetAction<Projectile, bool>(objects, "BookUpdate")
            );
            return null;
        }

        private static object RegisterBookmark(object[] args) {
            if (!(args[1] is Dictionary<string, object> objects)) {
                CalamityEntropy.Instance.Logger.Warn("Args[1] Must be a Dictionary<string, object>");
                return null;
            }
            if (!objects.TryGetValue("ItemType", out object itemTypeObj) || !(itemTypeObj is int)) {
                CalamityEntropy.Instance.Logger.Warn("ItemType is required and must be an integer");
                return null;
            }
            int itemType = (int)itemTypeObj;

            if (!objects.TryGetValue("Texture", out object textureObj) || !(textureObj is Asset<Texture2D> texture)) {
                CalamityEntropy.Instance.Logger.Warn("Texture is required and must be an Asset<Texture2D>");
                return null;
            }
            Func<Item, Item, bool> canBeEquipWith = null;
            if (objects.TryGetValue("CanBeEquipWithFunc", out var cbew_func) && cbew_func is Func<Item, Item, bool> fc) {
                canBeEquipWith = fc;
            }

            string effectName = objects.TryGetValue("EffectName", out object effectNameObj) && effectNameObj is string
                ? (string)effectNameObj : "";

            Func<int> modifyBaseProjectileType = objects.TryGetValue("ModifyBaseProjectileType", out object mbptObj) && mbptObj is Func<int> mbpt
                ? mbpt : null;

            BookMarkLoader.RegisterBookmark(
                itemType,
                texture,
                effectName,
                GetFunc<float, float>(objects, "ModifyStat_Damage"),
                GetFunc<float, float>(objects, "ModifyStat_Knockback"),
                GetFunc<float, float>(objects, "ModifyStat_ShootSpeed"),
                GetFunc<float, float>(objects, "ModifyStat_Homing"),
                GetFunc<float, float>(objects, "ModifyStat_Size"),
                GetFunc<float, float>(objects, "ModifyStat_Crit"),
                GetFunc<float, float>(objects, "ModifyStat_HomingRange"),
                GetFunc<int, int>(objects, "ModifyStat_PenetrateAddition"),
                GetFunc<float, float>(objects, "ModifyStat_AttackSpeed"),
                GetFunc<int, int>(objects, "ModifyStat_ArmorPenetration"),
                GetFunc<float, float>(objects, "ModifyStat_LifeSteal"),
                GetFunc<int, int>(objects, "ModifyProjectileType"),
                modifyBaseProjectileType,
                GetFunc<int, int>(objects, "ModifyShootCooldown"),
                canBeEquipWith
            );
            return null;
        }

        //跨模组传进来的委托类型对不上就当没给,不抛
        private static Action<T1> GetAction<T1>(Dictionary<string, object> objects, string key) {
            return objects.TryGetValue(key, out object actionObj) && actionObj is Action<T1> a ? a : null;
        }

        private static Action<T1, T2> GetAction<T1, T2>(Dictionary<string, object> objects, string key) {
            return objects.TryGetValue(key, out object actionObj) && actionObj is Action<T1, T2> a ? a : null;
        }

        private static Action<T1, T2, T3> GetAction<T1, T2, T3>(Dictionary<string, object> objects, string key) {
            return objects.TryGetValue(key, out object actionObj) && actionObj is Action<T1, T2, T3> a ? a : null;
        }

        private static Func<TInput, TOutput> GetFunc<TInput, TOutput>(Dictionary<string, object> objects, string key) {
            return objects.TryGetValue(key, out object funcObj) && funcObj is Func<TInput, TOutput> f ? f : null;
        }
    }
}
