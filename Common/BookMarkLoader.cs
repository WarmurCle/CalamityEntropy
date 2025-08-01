using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Items.Books.BookMarks;
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
                Action<Projectile, bool> bookUpdate = null)
        {
            CustomBMEffectsByName[name] = new BookmarkEffectFunctionGroups(onShoot, onActive, onProjectileSpawn, updateProjectile, onHitNPC, modifyHitNPC, bookUpdate);
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
      Func<int, int> modifyStat_LifeSteal = null,
       Func<int, int> modifyProjectileType = null,
       Func<int> modifyBaseProjectileType = null,
       Func<int, int> modifyShootCooldown = null,
       Func<Item, Item, bool> canBeEq = null)
        {
            CustomBMByID[ItemType] = new BookMarkTag(tex.Value, effectName, canBeEq == null ? default : canBeEq)
            {
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

        public static bool HasEmptyBookMarkSlot(Item item, Player player)
        {
            bool f = false;
            int c = player.GetMyMaxActiveBookMarks(item);
            if (c > 0)
            {
                for (int i = 0; i < c; i++)
                {
                    Item it = player.Entropy().EBookStackItems[i];
                    if (it.IsAir)
                    {
                        return true;
                    }
                }
            }
            return f;
        }
        public static void OnShoot(string Name, ModProjectile mp)
        {
            if (CustomBMEffectsByName.ContainsKey(Name))
            {
                CustomBMEffectsByName[Name].OnShoot?.Invoke(mp);
            }
        }
        public static void OnActive(string Name, ModProjectile mp)
        {
            if (CustomBMEffectsByName.ContainsKey(Name))
            {
                CustomBMEffectsByName[Name].OnActive?.Invoke(mp);
            }
        }
        public static void OnProjectileSpawn(string Name, Projectile p, bool ownerClient)
        {
            if (CustomBMEffectsByName.ContainsKey(Name))
            {
                CustomBMEffectsByName[Name].OnProjectileSpawn?.Invoke(p, ownerClient);
            }
        }
        public static void UpdateProjectile(string Name, Projectile p, bool ownerClient)
        {
            if (CustomBMEffectsByName.ContainsKey(Name))
            {
                CustomBMEffectsByName[Name].UpdateProjectile?.Invoke(p, ownerClient);
            }
        }
        public static void OnHitNPC(string Name, Projectile proj, NPC npc, int damageDone)
        {
            if (CustomBMEffectsByName.ContainsKey(Name))
            {
                CustomBMEffectsByName[Name].OnHitNPC?.Invoke(proj, npc, damageDone);
            }
        }
        public static void ModifyHitNPC(string Name, Projectile proj, NPC npc, NPC.HitModifiers modifiers)
        {
            if (CustomBMEffectsByName.ContainsKey(Name))
            {
                CustomBMEffectsByName[Name].ModifyHitNPC?.Invoke(proj, npc, modifiers);
            }
        }
        public static void BookUpdate(string Name, Projectile projectile, bool ownerClient)
        {
            if (CustomBMEffectsByName.ContainsKey(Name))
            {
                CustomBMEffectsByName[Name].BookUpdate?.Invoke(projectile, ownerClient);
            }
        }


        public static int GetMyMaxActiveBookMarks(this Player player, Item book)
        {
            return Math.Min(EBookUI.getMaxSlots(player, book), player.Entropy().EBookStackItems.Count);
        }
        public static Texture2D GetUITexture(Item item)
        {
            if (item.ModItem is BookMark bm)
            {
                return bm.UITexture;
            }
            if (CustomBMByID.ContainsKey(item.type))
            {
                return CustomBMByID[item.type].UITexture;
            }
            return null;
        }
        public static EBookProjectileEffect GetEffect(Item item)
        {
            if (item.ModItem is BookMark bm)
            {
                return bm.getEffect();
            }
            if (CustomBMByID.ContainsKey(item.type))
            {
                return CustomBMByID[item.type].getEffect();
            }
            return null;
        }
        public static void ModifyStat(Item item, EBookStatModifer modifer)
        {
            if (item.ModItem is BookMark bm)
            {
                bm.ModifyStat(modifer);
            }
            if (CustomBMByID.ContainsKey(item.type))
            {
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
        public static int ModifyProjectile(Item item, int origProj)
        {
            if (item.ModItem is BookMark bm)
            {
                return bm.modifyProjectile(origProj);
            }
            if (CustomBMByID.ContainsKey(item.type))
            {
                var tag = CustomBMByID[item.type];
                if (tag.ModifyProjectileType != null)
                {
                    return tag.ModifyProjectileType(origProj);
                }
            }
            return -1;
        }
        public static int ModifyBaseProjectile(Item item)
        {
            if (item.ModItem is BookMark bm)
            {
                return bm.modifyBaseProjectile();
            }
            if (CustomBMByID.ContainsKey(item.type))
            {
                var tag = CustomBMByID[item.type];
                if (tag.ModifyBaseProjectileType != null)
                {
                    return tag.ModifyBaseProjectileType();
                }
            }
            return -1;
        }
        public static void modifyShootCooldown(Item item, ref int shootCd)
        {
            if (item.ModItem is BookMark bm)
            {
                bm.modifyShootCooldown(ref shootCd);
            }
            if (CustomBMByID.ContainsKey(item.type))
            {
                var tag = CustomBMByID[item.type];
                if (tag.ModifyShootCooldown != null)
                {
                    shootCd = tag.ModifyShootCooldown(shootCd);
                }
            }
        }
        public static bool IsABookMark(Item item)
        {
            return item.ModItem is BookMark || CustomBMByID.ContainsKey(item.type);
        }
        private static bool CanBeEquipWith_Base(Item a, Item b)
        {
            return a.type != b.type;
        }
        public static bool CanBeEquipWith(Item a, Item b)
        {
            if (a.ModItem is BookMark bm)
            {
                return bm.CanBeEquipWith(b);
            }
            if (IsABookMark(a))
            {
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

            public BookmarkEffectFunctionGroups(
                Action<ModProjectile> onShoot = null,
                Action<ModProjectile> onActive = null,
                Action<Projectile, bool> onProjectileSpawn = null,
                Action<Projectile, bool> updateProjectile = null,
                Action<Projectile, NPC, int> onHitNPC = null,
                Action<Projectile, NPC, NPC.HitModifiers> modifyHitNPC = null,
                Action<Projectile, bool> bookUpdate = null
                )
            {
                OnShoot = onShoot;
                OnActive = onActive;
                OnProjectileSpawn = onProjectileSpawn;
                UpdateProjectile = updateProjectile;
                OnHitNPC = onHitNPC;
                ModifyHitNPC = modifyHitNPC;
                BookUpdate = bookUpdate;
            }
        }
        public class BookmarkEffect_OtherMod : EBookProjectileEffect
        {
            public override void OnShoot(EntropyBookHeldProjectile book)
            {
                BookMarkLoader.OnShoot(this.BMOtherMod_Name, book);
            }
            public override void OnActive(EntropyBookHeldProjectile book)
            {
                BookMarkLoader.OnActive(this.BMOtherMod_Name, book);
            }
            public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
            {
                BookMarkLoader.OnProjectileSpawn(this.BMOtherMod_Name, projectile, ownerClient);
            }
            public override void UpdateProjectile(Projectile projectile, bool ownerClient)
            {
                BookMarkLoader.UpdateProjectile(this.BMOtherMod_Name, projectile, ownerClient);
            }

            public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
            {
                BookMarkLoader.OnHitNPC(this.BMOtherMod_Name, projectile, target, damageDone);
            }

            public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
            {
                BookMarkLoader.ModifyHitNPC(this.BMOtherMod_Name, projectile, target, modifiers);
            }

            public override void BookUpdate(Projectile projectile, bool ownerClient)
            {
                BookMarkLoader.BookUpdate(this.BMOtherMod_Name, projectile, ownerClient);
            }
        }
        public class BookMarkTag
        {
            public string CustomBMEffectName;
            public Texture2D uiTex;
            public Func<Item, Item, bool> CanBeEquipWith = CanBeEquipWith_Base;
            public BookMarkTag(Texture2D uITexture, string customBMEffectName = null, Func<Item, Item, bool> canBeEquipWith = default)
            {
                uiTex = uITexture;
                this.CustomBMEffectName = customBMEffectName;
                if (canBeEquipWith != default)
                {
                    CanBeEquipWith = canBeEquipWith;
                }
            }
            public Texture2D UITexture => uiTex;
            public EBookProjectileEffect getEffect()
            {
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
            public Func<int, int> ModifyStat_LifeSteal;
            public Func<int, int> ModifyProjectileType;
            public Func<int> ModifyBaseProjectileType;
            public Func<int, int> ModifyShootCooldown;
        }
    }
}
