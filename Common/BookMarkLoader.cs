using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.UI.EntropyBookUI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public static class BookMarkLoader
    {
        public static Dictionary<string, BookmarkEffectFunctionGroups> CustomBMEffectsByName;
        public static Dictionary<int, BookMarkTag> CustomBMByID;
        public static void RegisterBookmarkEffect(string name, Action<ModProjectile> onShoot = null,
                Action<ModProjectile> onActive = null,
                Action<Projectile, bool> onProjectileSpawn = null,
                Action<Projectile, bool> updateProjectile = null,
                Action<Projectile, NPC, int> onHitNPC = null,
                Action<Projectile, NPC, NPC.HitModifiers> modifyHitNPC = null,
                Action<Projectile, bool> bookUpdate = null,
                Action<Player, Vector2, Vector2, int, float> onStandaloneAttack = null) {
            CustomBMEffectsByName[name] = new BookmarkEffectFunctionGroups(onShoot, onActive, onProjectileSpawn, updateProjectile, onHitNPC, modifyHitNPC, bookUpdate, onStandaloneAttack);
        }

        public static void RegisterBookmark(int ItemType, Asset<Texture2D> tex, string effectName = "",
            Func<float, float> modifyStat_Damage = null,
       Func<float, float> modifyStat_Knockback = null,
       Func<float, float> modifyStat_ShootSpeed = null,
       Func<float, float> modifyStat_Homing = null,
       Func<float, float> modifyStat_Size = null,
       Func<float, float> modifyStat_Crit = null,
       Func<float, float> modifyStat_HomingRange = null,
       Func<int, int> modifyStat_PenetrateAddition = null,
       Func<float, float> modifyStat_AttackSpeed = null,
      Func<int, int> modifyStat_ArmorPenetration = null,
      Func<float, float> modifyStat_LifeSteal = null,
       Func<int, int> modifyProjectileType = null,
       Func<int> modifyBaseProjectileType = null,
       Func<int, int> modifyShootCooldown = null,
       Func<Item, Item, bool> canBeEq = null) {
            CustomBMByID[ItemType] = new BookMarkTag(tex == null ? null : tex.Value, effectName, canBeEq == null ? default : canBeEq) {
                ModifyStat_Damage = modifyStat_Damage,
                ModifyStat_Knockback = modifyStat_Knockback,
                ModifyStat_ShootSpeed = modifyStat_ShootSpeed,
                ModifyStat_Homing = modifyStat_Homing,
                ModifyStat_Size = modifyStat_Size,
                ModifyStat_Crit = modifyStat_Crit,
                ModifyStat_HomingRange = modifyStat_HomingRange,
                ModifyStat_PenetrateAddition = modifyStat_PenetrateAddition,
                ModifyStat_AttackSpeed = modifyStat_AttackSpeed,
                ModifyStat_ArmorPenetration = modifyStat_ArmorPenetration,
                ModifyStat_LifeSteal = modifyStat_LifeSteal,
                ModifyProjectileType = modifyProjectileType,
                ModifyBaseProjectileType = modifyBaseProjectileType,
                ModifyShootCooldown = modifyShootCooldown
            };
        }
        public static bool GetPlayerHeldEntropyBook(Player player, out EntropyBookHeldProjectile eb) {
            eb = null;
            foreach (Projectile p in Main.ActiveProjectiles) {
                if (p.owner == player.whoAmI && p.ModProjectile != null && p.ModProjectile is EntropyBookHeldProjectile ebh) {
                    eb = ebh;
                    return true;
                }
            }
            return false;
        }
        public static bool HeldingBookAndHasBookmarkEffect<T>(Player player) where T : EBookProjectileEffect {
            if (player.HeldItem.ModItem != null && player.HeldItem.ModItem is EntropyBook ebi && GetPlayerHeldEntropyBook(player, out var eb)) {
                if (eb.UIOpen)
                    return false;
                int c = player.GetMyMaxActiveBookMarks(player.HeldItem);
                if (c > 0) {
                    for (int i = 0; i < c; i++) {
                        Item it = player.Entropy().EBookStackItems[i];
                        if (IsABookMark(it) && GetEffect(it) is T) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool HasEmptyBookMarkSlot(Item item, Player player) {
            bool f = false;
            int c = player.GetMyMaxActiveBookMarks(item);
            if (c > 0) {
                for (int i = 0; i < c; i++) {
                    Item it = player.Entropy().EBookStackItems[i];
                    if (it.IsAir) {
                        return true;
                    }
                }
            }
            return f;
        }
        public static void OnShoot(string Name, ModProjectile mp) {
            if (CustomBMEffectsByName.ContainsKey(Name)) {
                CustomBMEffectsByName[Name].OnShoot?.Invoke(mp);
            }
        }
        public static void OnActive(string Name, ModProjectile mp) {
            if (CustomBMEffectsByName.ContainsKey(Name)) {
                CustomBMEffectsByName[Name].OnActive?.Invoke(mp);
            }
        }
        public static void OnProjectileSpawn(string Name, Projectile p, bool ownerClient) {
            if (CustomBMEffectsByName.ContainsKey(Name)) {
                CustomBMEffectsByName[Name].OnProjectileSpawn?.Invoke(p, ownerClient);
            }
        }
        public static void UpdateProjectile(string Name, Projectile p, bool ownerClient) {
            if (CustomBMEffectsByName.ContainsKey(Name)) {
                CustomBMEffectsByName[Name].UpdateProjectile?.Invoke(p, ownerClient);
            }
        }
        public static void OnHitNPC(string Name, Projectile proj, NPC npc, int damageDone) {
            if (CustomBMEffectsByName.ContainsKey(Name)) {
                CustomBMEffectsByName[Name].OnHitNPC?.Invoke(proj, npc, damageDone);
            }
        }
        public static void ModifyHitNPC(string Name, Projectile proj, NPC npc, NPC.HitModifiers modifiers) {
            if (CustomBMEffectsByName.ContainsKey(Name)) {
                CustomBMEffectsByName[Name].ModifyHitNPC?.Invoke(proj, npc, modifiers);
            }
        }
        public static void BookUpdate(string Name, Projectile projectile, bool ownerClient) {
            if (CustomBMEffectsByName.ContainsKey(Name)) {
                CustomBMEffectsByName[Name].BookUpdate?.Invoke(projectile, ownerClient);
            }
        }


        public static int GetMyMaxActiveBookMarks(this Player player, Item book) {
            if (player.Entropy().EBookStackItems == null)
                return 0;
            return Math.Min(EBookUI.getMaxSlots(player, book), player.Entropy().EBookStackItems.Count);
        }
        public static Texture2D GetUITexture(Item item) {
            if (item.ModItem is BookMark bm) {
                return bm.UITexture;
            }
            if (CustomBMByID.ContainsKey(item.type)) {
                return CustomBMByID[item.type].UITexture;
            }
            return null;
        }
        public static EBookProjectileEffect GetEffect(Item item) {
            if (item.ModItem is BookMark bm) {
                return bm.getEffect();
            }
            if (CustomBMByID.ContainsKey(item.type)) {
                return CustomBMByID[item.type].getEffect();
            }
            return null;
        }
        public static void ModifyStat(Item item, EBookStatModifer modifer) {
            if (item.ModItem is BookMark bm) {
                bm.ModifyStat(modifer);
            }
            if (CustomBMByID.ContainsKey(item.type)) {
                var tag = CustomBMByID[item.type];
                if (tag.ModifyStat_Damage != null)
                    modifer.Damage = tag.ModifyStat_Damage(modifer.Damage);
                if (tag.ModifyStat_Knockback != null)
                    modifer.Knockback = tag.ModifyStat_Knockback(modifer.Knockback);
                if (tag.ModifyStat_ShootSpeed != null)
                    modifer.shotSpeed = tag.ModifyStat_ShootSpeed(modifer.shotSpeed);
                if (tag.ModifyStat_Homing != null)
                    modifer.Homing = tag.ModifyStat_Homing(modifer.Homing);
                if (tag.ModifyStat_Size != null)
                    modifer.Size = tag.ModifyStat_Size(modifer.Size);
                if (tag.ModifyStat_Crit != null)
                    modifer.Crit = tag.ModifyStat_Crit(modifer.Crit);
                if (tag.ModifyStat_HomingRange != null)
                    modifer.HomingRange = tag.ModifyStat_HomingRange(modifer.HomingRange);
                if (tag.ModifyStat_PenetrateAddition != null)
                    modifer.PenetrateAddition = tag.ModifyStat_PenetrateAddition(modifer.PenetrateAddition);
                if (tag.ModifyStat_AttackSpeed != null)
                    modifer.attackSpeed = tag.ModifyStat_AttackSpeed(modifer.attackSpeed);
                if (tag.ModifyStat_ArmorPenetration != null)
                    modifer.armorPenetration = tag.ModifyStat_ArmorPenetration(modifer.armorPenetration);
                if (tag.ModifyStat_LifeSteal != null)
                    modifer.lifeSteal = tag.ModifyStat_LifeSteal(modifer.lifeSteal);
            }
        }
        public static int ModifyProjectile(Item item, int origProj) {
            if (item.ModItem is BookMark bm) {
                return bm.modifyProjectile(origProj);
            }
            if (CustomBMByID.ContainsKey(item.type)) {
                var tag = CustomBMByID[item.type];
                if (tag.ModifyProjectileType != null) {
                    return tag.ModifyProjectileType(origProj);
                }
            }
            return -1;
        }
        public static int ModifyBaseProjectile(Item item) {
            if (item.ModItem is BookMark bm) {
                return bm.modifyBaseProjectile();
            }
            if (CustomBMByID.ContainsKey(item.type)) {
                var tag = CustomBMByID[item.type];
                if (tag.ModifyBaseProjectileType != null) {
                    return tag.ModifyBaseProjectileType();
                }
            }
            return -1;
        }
        public static void modifyShootCooldown(Item item, ref int shootCd) {
            if (item.ModItem is BookMark bm) {
                bm.modifyShootCooldown(ref shootCd);
            }
            if (CustomBMByID.ContainsKey(item.type)) {
                var tag = CustomBMByID[item.type];
                if (tag.ModifyShootCooldown != null) {
                    shootCd = tag.ModifyShootCooldown(shootCd);
                }
            }
        }
        public static bool IsABookMark(Item item) {
            return item.ModItem is BookMark || CustomBMByID.ContainsKey(item.type);
        }
        private static bool CanBeEquipWith_Base(Item a, Item b) {
            return a.type != b.type;
        }
        public static bool CanBeEquipWith(Item a, Item b) {
            if (a.ModItem is BookMark bm) {
                return bm.CanBeEquipWith(b);
            }
            if (IsABookMark(a)) {
                return CustomBMByID[a.type].CanBeEquipWith.Invoke(a, b);
            }
            return false;
        }
        public class BookmarkEffectFunctionGroups
        {
            public Action<ModProjectile> OnShoot;
            public Action<ModProjectile> OnActive;
            public Action<Projectile, bool> OnProjectileSpawn;
            public Action<Projectile, bool> UpdateProjectile;
            public Action<Projectile, NPC, int> OnHitNPC;
            public Action<Projectile, NPC, NPC.HitModifiers> ModifyHitNPC;
            public Action<Projectile, bool> BookUpdate;
            public Action<Player, Vector2, Vector2, int, float> OnStandaloneAttack;

            public BookmarkEffectFunctionGroups(
                Action<ModProjectile> onShoot = null,
                Action<ModProjectile> onActive = null,
                Action<Projectile, bool> onProjectileSpawn = null,
                Action<Projectile, bool> updateProjectile = null,
                Action<Projectile, NPC, int> onHitNPC = null,
                Action<Projectile, NPC, NPC.HitModifiers> modifyHitNPC = null,
                Action<Projectile, bool> bookUpdate = null,
                Action<Player, Vector2, Vector2, int, float> onStandaloneAttack = null
                ) {
                OnShoot = onShoot;
                OnActive = onActive;
                OnProjectileSpawn = onProjectileSpawn;
                UpdateProjectile = updateProjectile;
                OnHitNPC = onHitNPC;
                ModifyHitNPC = modifyHitNPC;
                BookUpdate = bookUpdate;
                OnStandaloneAttack = onStandaloneAttack;
            }
        }
        public class BookmarkEffect_OtherMod : EBookProjectileEffect
        {
            public override void OnShoot(EntropyBookHeldProjectile book) {
                BookMarkLoader.OnShoot(this.BMOtherMod_Name, book);
            }
            public override void OnActive(EntropyBookHeldProjectile book) {
                BookMarkLoader.OnActive(this.BMOtherMod_Name, book);
            }
            public override void OnProjectileSpawn(Projectile projectile, bool ownerClient) {
                BookMarkLoader.OnProjectileSpawn(this.BMOtherMod_Name, projectile, ownerClient);
            }
            public override void UpdateProjectile(Projectile projectile, bool ownerClient) {
                BookMarkLoader.UpdateProjectile(this.BMOtherMod_Name, projectile, ownerClient);
            }

            public override void OnHitNPC(Projectile projectile, NPC target, int damageDone) {
                BookMarkLoader.OnHitNPC(this.BMOtherMod_Name, projectile, target, damageDone);
            }

            public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
                BookMarkLoader.ModifyHitNPC(this.BMOtherMod_Name, projectile, target, modifiers);
            }

            public override void BookUpdate(Projectile projectile, bool ownerClient) {
                BookMarkLoader.BookUpdate(this.BMOtherMod_Name, projectile, ownerClient);
            }

            public override void OnStandaloneAttack(Player player, Vector2 position, Vector2 direction, int damage, float knockback) {
                if (CustomBMEffectsByName.TryGetValue(this.BMOtherMod_Name, out var funcs)) {
                    funcs.OnStandaloneAttack?.Invoke(player, position, direction, damage, knockback);
                }
            }
        }
        public class BookMarkTag
        {
            public string CustomBMEffectName;
            public Texture2D uiTex;
            public Func<Item, Item, bool> CanBeEquipWith = CanBeEquipWith_Base;
            public BookMarkTag(Texture2D uiTexture, string customBMEffectName = null, Func<Item, Item, bool> canBeEquipWith = default) {
                uiTex = uiTexture;
                this.CustomBMEffectName = customBMEffectName;
                if (canBeEquipWith != default) {
                    CanBeEquipWith = canBeEquipWith;
                }
            }
            public Texture2D UITexture => uiTex;
            public EBookProjectileEffect getEffect() {
                return CustomBMEffectsByName.ContainsKey(this.CustomBMEffectName) ? new BookmarkEffect_OtherMod() { BMOtherMod_Name = this.CustomBMEffectName } : null;
            }
            public Func<float, float> ModifyStat_Damage;
            public Func<float, float> ModifyStat_Knockback;
            public Func<float, float> ModifyStat_ShootSpeed;
            public Func<float, float> ModifyStat_Homing;
            public Func<float, float> ModifyStat_Size;
            public Func<float, float> ModifyStat_Crit;
            public Func<float, float> ModifyStat_HomingRange;
            public Func<int, int> ModifyStat_PenetrateAddition;
            public Func<float, float> ModifyStat_AttackSpeed;
            public Func<int, int> ModifyStat_ArmorPenetration;
            public Func<float, float> ModifyStat_LifeSteal;
            public Func<int, int> ModifyProjectileType;
            public Func<int> ModifyBaseProjectileType;
            public Func<int, int> ModifyShootCooldown;
        }

        /// <summary>PerformBookmarkAttack 不经过 EntropyBookHeldProjectile,外部调用走 ModCall("PerformBookmarkAttack")</summary>
        public static BookmarkAttackResult PerformBookmarkAttack(
            Item bookmarkItem,
            Player player,
            Vector2 position,
            Vector2 direction,
            int baseDamage = 50,
            float baseKnockback = 2f,
            int baseProjectileType = -1,
            float baseShootSpeed = 12f,
            int baseCooldown = 20,
            DamageClass damageClass = null) {
            if (!IsABookMark(bookmarkItem))
                return new BookmarkAttackResult { Success = false, CooldownTicks = 0 };

            damageClass ??= DamageClass.Magic;

            //内部书签如果重写了 PerformAttack,这里优先用它
            if (bookmarkItem.ModItem is BookMark bm) {
                var context = new BookmarkAttackContext {
                    Player = player,
                    Position = position,
                    Direction = direction,
                    BaseDamage = baseDamage,
                    BaseKnockback = baseKnockback,
                    BaseProjectileType = baseProjectileType,
                    BaseShootSpeed = baseShootSpeed,
                    BaseCooldown = baseCooldown,
                    DamageType = damageClass
                };
                var customResult = bm.PerformAttack(context);
                if (customResult != null)
                    return customResult;
            }

            //下面走默认实现
            return PerformDefaultBookmarkAttack(bookmarkItem, player, position, direction,
                baseDamage, baseKnockback, baseProjectileType, baseShootSpeed, baseCooldown, damageClass);
        }

        private static BookmarkAttackResult PerformDefaultBookmarkAttack(
            Item bookmarkItem, Player player, Vector2 position, Vector2 direction,
            int baseDamage, float baseKnockback, int baseProjectileType, float baseShootSpeed,
            int baseCooldown, DamageClass damageClass) {
            var result = new BookmarkAttackResult();

            //这里收集属性修改
            EBookStatModifer modifer = new EBookStatModifer();
            modifer.Crit = player.GetTotalCritChance(damageClass);
            modifer.Knockback = player.GetTotalKnockback(damageClass).ApplyTo(baseKnockback);
            modifer.attackSpeed = player.GetTotalAttackSpeed(damageClass);
            ModifyStat(bookmarkItem, modifer);
            result.AppliedModifier = modifer;

            //这里确定弹幕类型
            int projType = baseProjectileType >= 0 ? baseProjectileType : ModContent.ProjectileType<RuneBullet>();
            int baseReplace = ModifyBaseProjectile(bookmarkItem);
            if (baseReplace >= 0) projType = baseReplace;
            int typeReplace = ModifyProjectile(bookmarkItem, projType);
            if (typeReplace >= 0) projType = typeReplace;

            //这里计算冷却
            int cooldown = baseCooldown;
            modifyShootCooldown(bookmarkItem, ref cooldown);
            cooldown = modifer.attackSpeed > 0 ? (int)(cooldown / modifer.attackSpeed) : cooldown;
            result.CooldownTicks = Math.Max(1, cooldown);

            //这里计算伤害和击退
            int dmg = (int)player.GetTotalDamage(damageClass).ApplyTo(baseDamage * modifer.Damage);
            float kb = modifer.Knockback;

            //这里生成弹幕
            Vector2 dir = direction.SafeNormalize(Vector2.UnitX);
            Vector2 vel = dir * baseShootSpeed * modifer.shotSpeed;

            int projIndex = Projectile.NewProjectile(
                player.GetSource_FromThis(), position, vel,
                projType, dmg, kb, player.whoAmI);

            Projectile proj = Main.projectile[projIndex];
            result.ProjectileIndex = projIndex;
            result.ProjectileType = projType;

            //这里把属性写到弹幕上
            if (proj.penetrate >= 0)
                proj.penetrate += modifer.PenetrateAddition;
            proj.CritChance = (int)modifer.Crit;
            proj.scale *= modifer.Size;
            proj.ArmorPenetration += (int)(player.GetTotalArmorPenetration(damageClass) + modifer.armorPenetration);
            proj.DamageType = damageClass;

            //这里取效果实例,只取一次,免得 CustomBM 重复创建
            EBookProjectileEffect effect = GetEffect(bookmarkItem);

            //弹幕如果是 EBookBaseProjectile,这里挂上书签效果
            if (proj.ModProjectile is EBookBaseProjectile bp) {
                bp.mainProj = true;
                bp.homing += modifer.Homing;
                bp.homingRange *= modifer.HomingRange;
                bp.attackSpeed = modifer.attackSpeed;
                bp.lifeSteal += modifer.lifeSteal;

                if (effect != null) {
                    bp.ProjectileEffects.Add(effect);
                }
            }

            //这里调用 OnStandaloneAttack,复现 OnShoot 和 OnActive 里的独立逻辑
            if (effect != null) {
                effect.OnStandaloneAttack(player, position, direction, dmg, kb);
            }

            result.Success = true;
            return result;
        }

        /// <summary>
        /// GetBookmarkInfo 查询书签能力,不触发任何攻击
        /// </summary>
        public static BookmarkInfo GetBookmarkInfo(Item bookmarkItem) {
            if (!IsABookMark(bookmarkItem))
                return null;

            var info = new BookmarkInfo();
            info.IsBookmark = true;

            //这里检查属性修改
            var testModifer = new EBookStatModifer();
            ModifyStat(bookmarkItem, testModifer);
            info.HasStatModifiers = testModifer.Damage != 1 || testModifer.Knockback != 1 ||
                testModifer.shotSpeed != 1 || testModifer.Homing != 0 || testModifer.Size != 1 ||
                testModifer.Crit != 0 || testModifer.HomingRange != 1 || testModifer.PenetrateAddition != 0 ||
                testModifer.attackSpeed != 1 || testModifer.armorPenetration != 0 || testModifer.lifeSteal != 0;
            info.StatSnapshot = testModifer;

            //这里检查弹幕有没有被替换
            info.ReplacesBaseProjectile = ModifyBaseProjectile(bookmarkItem) >= 0;
            info.ReplacesProjectile = ModifyProjectile(bookmarkItem, -1) >= 0;

            //这里检查冷却有没有被修改
            int testCd = 20;
            modifyShootCooldown(bookmarkItem, ref testCd);
            info.ModifiesCooldown = testCd != 20;

            //这里检查有没有主动效果
            info.HasEffect = GetEffect(bookmarkItem) != null;

            //这里读取 UI 纹理
            info.UITexture = GetUITexture(bookmarkItem);

            return info;
        }
    }

    /// <summary>
    /// BookmarkAttackContext 是外部模组传入的书签攻击参数
    /// </summary>
    public class BookmarkAttackContext
    {
        public Player Player;
        public Vector2 Position;
        public Vector2 Direction;
        public int BaseDamage = 50;
        public float BaseKnockback = 2f;
        /// <summary>BaseProjectileType 是基础弹幕类型,-1 表示用默认 RuneBullet,书签如果替换弹幕就会换掉它</summary>
        public int BaseProjectileType = -1;
        public float BaseShootSpeed = 12f;
        public int BaseCooldown = 20;
        public DamageClass DamageType = DamageClass.Magic;
    }

    /// <summary>
    /// BookmarkAttackResult 是书签攻击返回的结果
    /// </summary>
    public class BookmarkAttackResult
    {
        public bool Success;
        /// <summary>CooldownTicks 是建议的冷却 tick 数,外部模组应等这段时间后再触发</summary>
        public int CooldownTicks;
        /// <summary>AppliedModifier 是这次应用的属性修改快照</summary>
        public EBookStatModifer AppliedModifier;
        /// <summary>ProjectileIndex 是生成弹幕在 Main.projectile 里的索引,-1 表示没生成</summary>
        public int ProjectileIndex = -1;
        /// <summary>ProjectileType 是实际用的弹幕类型 ID</summary>
        public int ProjectileType = -1;
    }

    /// <summary>
    /// BookmarkInfo 只供查询书签能力,不触发攻击
    /// </summary>
    public class BookmarkInfo
    {
        public bool IsBookmark;
        public bool HasStatModifiers;
        public bool ReplacesBaseProjectile;
        public bool ReplacesProjectile;
        public bool ModifiesCooldown;
        public bool HasEffect;
        public EBookStatModifer StatSnapshot;
        public Texture2D UITexture;
    }
}
