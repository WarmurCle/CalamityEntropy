
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public abstract class EntropyBook : ModItem
    {
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;
            Item.damage = 64;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 20;
        }
        public virtual int HeldProjectileType => -1;
        public override void UpdateInventory(Player player)
        {
            if(player.HeldItem == Item)
            {
                if (player.ownedProjectileCounts[HeldProjectileType] <= 0)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, HeldProjectileType, 0, 0, player.whoAmI, Item.type);
                }
            }
        }
    }
    public class EBookStatModifer
    {
        public float Damage = 1;
        public float Knockback = 1;
        public float shotSpeed = 1;
        public float Homing = 0;
        public float Size = 1;
        public float Crit = 0;
        public float HomingRange = 1;
        public int PenetrateAddition = 0;
    }
    public abstract class EntropyBookHeldProjectile : ModProjectile
    {
        public int ItemType => (int)Projectile.ai[0];
        public int openAnim = 0;
        public bool UIOpen = false;
        public int UIOpenAnm = 0;
        public int shotCooldown = 0;
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = Projectile.width = 16;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public virtual EBookStatModifer getBaseModifer()
        {
            EBookStatModifer modifer = new EBookStatModifer() { Damage = Projectile.getOwner().GetTotalDamage(Projectile.DamageType).Multiplicative, Knockback = Projectile.getOwner().GetTotalKnockback(Projectile.DamageType).Multiplicative, Crit = Projectile.getOwner().GetTotalCritChance(Projectile.DamageType)};
            return modifer;
        }

        public virtual string OpenAnimationPath => "";
        public virtual Texture2D[] OpenAnimations()
        {
            Texture2D[] texs = new Texture2D[3];
            for (int i = 0; i < 3; i++)
            {
                texs[i] = ModContent.Request<Texture2D>(OpenAnimationPath + i.ToString(), AssetRequestMode.ImmediateLoad).Value;
            }
            return texs;
        }
        public virtual string PageAnimationPath => "";
        public virtual Texture2D[] PageAnimations()
        {
            Texture2D[] texs = new Texture2D[5];
            for (int i = 0; i < 5; i++)
            {
                texs[i] = ModContent.Request<Texture2D>(PageAnimationPath + i.ToString(), AssetRequestMode.ImmediateLoad).Value;
            }
            return texs;
        }
        public virtual string UIOpenAnimationPath => "";
        public virtual Texture2D[] UIOpenAnimations()
        {
            Texture2D[] texs = new Texture2D[4];
            for (int i = 0; i < 4; i++)
            {
                texs[i] = ModContent.Request<Texture2D>(UIOpenAnimationPath + i.ToString(), AssetRequestMode.ImmediateLoad).Value;
            }
            return texs;
        }
        public int pageTurnAnm = 0;
        public virtual void playTurnPageAnimation()
        {
            pageTurnAnm = 4;
            Projectile.frameCounter = 0;
        }
        public bool active = false;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(active);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            active = reader.ReadBoolean();
        }

        public virtual Texture2D getTexture()
        {
            if (UIOpen)
            {
                return UIOpenAnimations()[UIOpenAnm];
            }
            else
            {
                if (openAnim < 2)
                {
                    return OpenAnimations()[UIOpenAnm];
                }
                else
                {
                    return PageAnimations()[pageTurnAnm];
                }
            }
        }
        public virtual void setVel()
        {
            if (Main.myPlayer != Projectile.owner)
                return;
            Vector2 newVel = (Main.MouseWorld - Projectile.getOwner().Center).SafeNormalize(Vector2.UnitX);
            if(Projectile.velocity != newVel)
            {
                Projectile.velocity = newVel;
                Projectile.netUpdate = true;
            }
        }
        public virtual Vector2 heldOffset => new Vector2(14, 6);
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public virtual int baseProjectileType => ProjectileID.EnchantedBeam;
        public virtual int getShootProjectileType()
        {
            return baseProjectileType;
        }
        public virtual bool Shoot()
        {
            int type = getShootProjectileType();
            ShootSingleProjectile(type, Projectile.Center, Projectile.velocity);
            return true;
        }
        public virtual void ShootSingleProjectile(int type, Vector2 pos, Vector2 velocity)
        {
            EBookStatModifer modifer = getBaseModifer();
            Projectile proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, velocity.normalize() * Projectile.getOwner().HeldItem.shootSpeed * modifer.shotSpeed, type, (int)(Projectile.getOwner().HeldItem.damage * modifer.Damage * (Projectile.Entropy().ttindex < 0 ? 1 : TwistedTwinMinion.damageMul)), Projectile.getOwner().HeldItem.knockBack * modifer.Knockback, Projectile.owner).ToProj();
            proj.penetrate += modifer.PenetrateAddition;
            proj.OriginalCritChance += (int)modifer.Crit;
            proj.Size *= modifer.Size;
            if(proj.ModProjectile is EBookBaseProjectile bp)
            {
                bp.homing += modifer.Homing;
                bp.homingRange *= modifer.HomingRange;
            }
        }
        public override void AI()
        {
            var player = Projectile.getOwner();
            if(player.HeldItem.type != ItemType)
            {
                Projectile.Kill();
                return;
            }
            setVel();
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Projectile.getOwner().Center + (UIOpen ? Vector2.Zero : new Vector2(heldOffset.X, heldOffset.Y * (Projectile.velocity.X > 0 ? 1 : -1))).RotatedBy(Projectile.rotation);
            if(Main.myPlayer == Projectile.owner)
            {
                bool flag = Main.mouseLeft && !Main.LocalPlayer.mouseInterface && !UIOpen;
                if(flag != active)
                {
                    active = flag;
                    Projectile.netUpdate = true;
                }
                if (active)
                {
                    player.heldProj = Projectile.whoAmI;
                    player.itemTime = 3;
                    player.itemAnimation = 3;
                }
                if (Projectile.velocity.X > 0)
                {
                    player.direction = 1;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
                }
                else
                {
                    player.direction = -1;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
                }
                if (active && openAnim >= 2)
                {
                    if(shotCooldown <= 0)
                    {
                        if (Shoot())
                        {
                            if (active && pageTurnAnm == 0)
                            {
                                playTurnPageAnimation();
                            }
                            shotCooldown = Projectile.getOwner().HeldItem.useTime;
                        }
                    }
                    else
                    {
                        shotCooldown--;
                    }
                }
            }
            Projectile.frameCounter++;
            if(!active && !UIOpen && openAnim == 0)
            {
                Projectile.frameCounter = 0;
            }
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (pageTurnAnm > 0)
                {
                    pageTurnAnm--;
                }
                if (UIOpen)
                {
                    if (UIOpenAnm < 3)
                    {
                        UIOpenAnm++;
                    }
                    if (openAnim > 0)
                    {
                        openAnim--;
                    }
                }
                else
                {
                    if (UIOpenAnm > 0)
                    {
                        UIOpenAnm--;
                    }
                    if (active)
                    {
                        if (openAnim < 2)
                        {
                            openAnim++;
                        }
                    }
                    else {
                        if (openAnim > 0)
                        {
                            openAnim--;
                        }
                    }
                }
            }
            Projectile.getOwner().heldProj = Projectile.whoAmI;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = getTexture();
            Main.EntitySpriteDraw(texture, Projectile.Center + Projectile.gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, (Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically), 0);
            return false;
        }
    }
    public abstract class EBookBaseProjectile : ModProjectile
    {
        public float homing = 0;
        public float homingRange = 460;
        public bool init = true;
        public bool sync = false;
        public bool EffectInit = true;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(homing);
            writer.Write(homingRange);
            writer.Write(Projectile.penetrate);
            writer.Write(Projectile.scale);

            writer.Write(ProjectileEffects.Count);
            foreach (var effect in ProjectileEffects)
            {
                writer.Write(effect.RegisterName());
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            homing = reader.ReadSingle();
            homingRange = reader.ReadSingle();
            Projectile.penetrate = reader.ReadInt32();
            Projectile.scale = reader.ReadSingle();

            this.ProjectileEffects.Clear();
            for(int i = 0; i < reader.ReadInt32(); i++)
            {
                this.ProjectileEffects.Add(EBookProjectileEffect.findByName(reader.ReadString()));
            }
            sync = true;
        }
        public List<EBookProjectileEffect> ProjectileEffects = new List<EBookProjectileEffect>();
        public virtual void ApplyHoming()
        {
            if(homing <= 0)
            {
                return;
            }
            NPC homingTarget = Projectile.FindTargetWithinRange(this.homingRange, (Projectile.tileCollide ? true : false));
            if (homingTarget != null)
            {
                Projectile.velocity += (homingTarget.Center - Projectile.Center).normalize() * homing;
            }
        }
        public override void AI()
        {
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
                    Util.Util.SyncProj(Projectile.whoAmI);
                }
            }
            if (sync)
            {
                if (EffectInit)
                {
                    EffectInit = false;
                    foreach(var effect in  ProjectileEffects)
                    {
                        effect.OnProjectileSpawn(Projectile, Main.myPlayer == Projectile.owner);
                    }
                }
            }
            foreach (var effect in ProjectileEffects)
            {
                effect.UpdateProjectile(Projectile, Main.myPlayer == Projectile.owner);
            }
            this.ApplyHoming();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach(var effect in this.ProjectileEffects)
            {
                effect.onHitNPC(Projectile, target);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (var effect in this.ProjectileEffects)
            {
                effect.modifyHitNPC(Projectile, target, ref modifiers);
            }
        }
    }
    public abstract class EBookBaseLaser : EBookBaseProjectile
    {
        public int segLength = 15;
        public int segCounts = 180;
        public int penetrate = 1;
        public float width = 16;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void ApplyHoming() { }
        public virtual List<Vector2> getSamplePoints()
        {
            var points = new List<Vector2>();
            bool laserHoming = homing > 0;
            Vector2 startPos = Projectile.Center;
            List<NPC> hited = new List<NPC>();
            Vector2 nowPos = startPos;
            Vector2 addVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * segLength;
            Vector2 lastPos = startPos;
            var activenpcs = Main.ActiveNPCs;
            for (int i = 0; i < segCounts; i++)
            {
                foreach(NPC npc in Main.ActiveNPCs)
                {
                    if (hited.Contains(npc) || !npc.CanBeChasedBy(Projectile))
                    {
                        continue;
                    }
                    if (Util.Util.LineThroughRect(lastPos, nowPos, npc.getRect(), (int)width))
                    {
                       hited.Add(npc);
                    }
                }
                if(hited.Count >= penetrate)
                {
                    return points;
                }
                if (laserHoming)
                {
                    Vector2 oldPos = Projectile.Center;
                    Projectile.Center = nowPos;
                    NPC homingTarget = null;
                    float dist = homingRange;
                    foreach(NPC npc in activenpcs)
                    {
                        if (!hited.Contains(npc) && !npc.friendly && npc.CanBeChasedBy(Projectile))
                        {
                            float r = Util.Util.getDistance(nowPos, npc.Center);
                            if (r < this.homingRange)
                            {
                                dist = r;
                                homingTarget = npc;
                            }
                        }
                    }
                    if (homingTarget != null)
                    {
                        addVel += (homingTarget.Center - Projectile.Center).normalize() * homing;
                        addVel *= 0.96f;
                    }
                    Projectile.Center = oldPos;
                }
                lastPos = nowPos;
                points.Add(nowPos);
                nowPos += addVel.SafeNormalize(Vector2.UnitX) * segLength;
            }
            return points;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            var points = this.getSamplePoints();
            for(int i = 1; i < points.Count; i++)
            {
                if (Util.Util.LineThroughRect(points[i - 1], points[i], targetHitbox, (int)width))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool PreAI()
        {
            if(this.penetrate < Projectile.penetrate)
            {
                this.penetrate = Projectile.penetrate;
            }
            return base.PreAI();
        }
        public override void AI()
        {
            base.AI();
        }
    }
    public abstract class EBookProjectileEffect : ModType
    {
        public static List<EBookProjectileEffect> instances;
        protected sealed override void Register()
        {
            if (instances == null)
            {
                instances = new List<EBookProjectileEffect>();
            }
            instances.Add(this);
        }
        public override void Unload()
        {
            instances = null;
        }
        public static EBookProjectileEffect findByName(string name)
        {
            if(instances == null)
            {
                return null;
            }
            foreach (EBookProjectileEffect eff in instances)
            {
                if (eff.RegisterName() == name)
                {
                    return eff;
                }
            }
            return null;
        }
        public virtual string RegisterName()
        {
            return this.Name;
        }
        public virtual void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {

        }
        public virtual void UpdateProjectile(Projectile projectile, bool ownerClient)
        {

        }

        public virtual void onHitNPC(Projectile projectile, NPC target)
        {

        }

        public virtual void modifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {

        }
    }
}