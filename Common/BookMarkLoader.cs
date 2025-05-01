//软依赖自定义书签
//未完成

/*using CalamityEntropy.Content.Items.Books;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MonoMod.RuntimeDetour;

namespace CalamityEntropy.Common
{
    public static class BookMarkLoader
    {
        public static Dictionary<string, BookmarkEffectFunctionGroups> CustomBMEffectsByName;
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
        public class BookmarkEffectFunctionGroups
        {
            public Action<ModProjectile> OnShoot;
            public Action<ModProjectile> OnActive;
            public Action<Projectile, bool> OnProjectileSpawn;
            public Action<Projectile, bool> UpdateProjectile;
            public Action<Projectile, NPC, int> OnHitNPC;
            public Action<Projectile, NPC, NPC.HitModifiers> ModifyHitNPC;
            public BookmarkEffectFunctionGroups(
                Action<ModProjectile> onShoot = null, 
                Action<ModProjectile> onActive = null,
                Action<Projectile, bool> onProjectileSpawn = null, 
                Action<Projectile, bool> updateProjectile = null,
                Action<Projectile, NPC, int> onHitNPC = null,
                Action<Projectile, NPC, NPC.HitModifiers> modifyHitNPC = null
                )
            {
                OnShoot = onShoot;
                OnActive = onActive;
                OnProjectileSpawn = onProjectileSpawn;
                UpdateProjectile = updateProjectile;
                OnHitNPC = onHitNPC;
                ModifyHitNPC = modifyHitNPC;
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
        }
        public class BookMarkTag
        {
            public string CustomBMEffectName;
            public Texture2D uiTex;
            public BookMarkTag(Texture2D uITexture, string customBMEffectName)
            {
                uiTex = uITexture;
                this.CustomBMEffectName = customBMEffectName;
            }
            public Texture2D UITexture => uiTex;
            public EBookProjectileEffect getEffect()
            {
                return CustomBMEffectsByName.ContainsKey(this.CustomBMEffectName) ? new BookmarkEffect_OtherMod() { BMOtherMod_Name = this.CustomBMEffectName} : null;
            }
            public Texture2D GetUITexture(string name)
            {
                return uiTex;
            }
            public void ModifyStat(EBookStatModifer modifer) { }
            public  int modifyProjectile(int pNow)
            {
                return -1;
            }
            public int modifyBaseProjectile()
            {
                return -1;
            }
            public void modifyShootCooldown(ref int shootCooldown)
            {

            }
            public Color tooltipColor => Color.Green;
        }
    }
}
*/