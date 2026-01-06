/*using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static CalamityEntropy.Common.BookMarkLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class EBookProjGlobal : GlobalProjectile
    {
        public static void ApplyBookmarkEffectToProjectile(Player player, Projectile proj, Item bookmark)
        {
            if (proj.penetrate >= 0)
                proj.penetrate += modifer.PenetrateAddition;
            proj.CritChance = bookItem.crit + (int)modifer.Crit;
            proj.scale *= modifer.Size * scaleMul;
            proj.ArmorPenetration += (int)(Projectile.GetOwner().GetTotalArmorPenetration(Projectile.DamageType) + modifer.armorPenetration + bookItem.ArmorPenetration);

            if (proj.TryGetGlobalProjectile<EBookProjGlobal>(out var bp))
            {
                bp.shooter = proj.identity;
                bp.mainProj = true;
                bp.ShooterModProjectile = this;
                bp.homing += modifer.Homing;
                bp.homingRange *= modifer.HomingRange;
                bp.attackSpeed = modifer.attackSpeed;
                bp.lifeSteal += modifer.lifeSteal;
                for (int i = 0; i < Math.Min(Main.LocalPlayer.GetMyMaxActiveBookMarks(bookItem), Projectile.GetOwner().Entropy().EBookStackItems.Count); i++)
                {
                    Item it = Projectile.GetOwner().Entropy().EBookStackItems[i];
                    if (BookMarkLoader.IsABookMark(it))
                    {
                        if (BookMarkLoader.GetEffect(it) != null)
                        {
                            bp.ProjectileEffects.Add(BookMarkLoader.GetEffect(it));
                        }
                    }
                }
                if (this.getEffect() != null)
                {
                    bp.ProjectileEffects.Add(this.getEffect());
                }
                if (colorMult != default)
                {
                    bp.color = bp.baseColor.MultiplyRGBA(colorMult);
                    bp.initColor = false;
                }
            }
        }
        public bool Active = false;
        public int hitCount = 0;
        public float homing = 0;
        public float homingRange = 460;
        public bool init = true;
        public bool sync = false;
        public bool EffectInit = true;
        public int lifeSteal = 0;
        public float gravity = 0;
        public bool mainProj = false;
        public virtual Color baseColor => Color.White;
        public Color color;
        public bool initColor = true;
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (!Active)
                return;
            hitbox = Projectile.Center.getRectCentered(hitbox.Width * Projectile.scale, hitbox.Height * Projectile.scale);
        }
        public override bool PreAI()
        {
            if (!Active)
                return true;
            if (ShooterModProjectile == null)
            {
                Projectile p = shooter.ToProj_Identity();
                if (p != null && p != default)
                {
                    if (p.ModProjectile != null)
                    {
                        ShooterModProjectile = p.ModProjectile;
                    }
                }
            }

            if (initColor)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                initColor = false;
                color = baseColor;
                Projectile.CritChance /= 2;
            }
            return true;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if (!Active)
                return;
            writer.Write(shooter);
            writer.Write(homing);
            writer.Write(homingRange);
            writer.Write(Projectile.penetrate);
            writer.Write(Projectile.scale);
            writer.Write(Projectile.CritChance);
            writer.Write(lifeSteal);
            writer.Write(gravity);
            writer.Write(mainProj);

            writer.Write(ProjectileEffects.Count);
            foreach (var effect in ProjectileEffects)
            {
                writer.Write(effect.RegisterName());
                writer.Write(effect.BMOtherMod_Name);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if (!Active)
                return;
            shooter = reader.ReadInt32();
            homing = reader.ReadSingle();
            homingRange = reader.ReadSingle();
            Projectile.penetrate = reader.ReadInt32();
            Projectile.scale = reader.ReadSingle();
            Projectile.CritChance = reader.ReadInt32();
            lifeSteal = reader.ReadInt32();
            gravity = reader.ReadSingle();
            mainProj = reader.ReadBoolean();

            this.ProjectileEffects.Clear();
            int r = reader.ReadInt32();
            for (int i = 0; i < r; i++)
            {
                var bef = EBookProjectileEffect.findByName(reader.ReadString());
                string omN = reader.ReadString();
                if (omN != string.Empty)
                {
                    bef = new BookmarkEffect_OtherMod() { BMOtherMod_Name = omN };
                }
                this.ProjectileEffects.Add(bef);
            }
            sync = true;
        }
        public List<EBookProjectileEffect> ProjectileEffects = new List<EBookProjectileEffect>();
        public float attackSpeed = 1;
        public ModProjectile ShooterModProjectile = null;
        public int shooter = -1;

        public virtual void ApplyHoming()
        {
            if (homing <= 0)
            {
                return;
            }
            NPC homingTarget = Projectile.FindTargetWithinRange(this.homingRange, (Projectile.tileCollide ? true : false));
            if (homingTarget != null)
            {
                Projectile.velocity *= 1f - (homing * 0.075f);
                Projectile.velocity += (homingTarget.Center - Projectile.Center).normalize() * homing * 4.2f;
            }
        }
        public override void AI()
        {
            if (!Active)
                return;
            if (init)
            {
                init = false;
                bool ownerClient = Main.myPlayer == Projectile.owner;
                if (ownerClient)
                {
                    sync = true;
                }
                if (ownerClient)
                {
                    CEUtils.SyncProj(Projectile.whoAmI);
                }
            }
            if (sync)
            {
                if (EffectInit)
                {
                    EffectInit = false;
                    foreach (var effect in ProjectileEffects)
                    {
                        effect.OnProjectileSpawn(Projectile, Main.myPlayer == Projectile.owner);
                    }
                }
            }
            Projectile.velocity.Y += this.gravity;
            foreach (var effect in ProjectileEffects)
            {
                effect.UpdateProjectile(Projectile, Main.myPlayer == Projectile.owner);
            }
            this.ApplyHoming();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Active)
                return;
            hitCount++;
            foreach (var effect in this.ProjectileEffects)
            {
                effect.OnHitNPC(Projectile, target, damageDone);
            }
            if (lifeSteal > 0)
            {
                Projectile.GetOwner()?.Entropy().TryHealMeWithCd(lifeSteal, 4);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!Active)
                return;
            foreach (var effect in this.ProjectileEffects)
            {
                effect.ModifyHitNPC(Projectile, target, ref modifiers);
            }
        }
    }
}*/