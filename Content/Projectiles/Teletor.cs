using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class Teletor : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {   
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 32;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.light = 0.5f;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.ArmorPenetration = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(weaponVel);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            weaponVel = reader.ReadVector2();
        }
        public Vector2 weaponVel = Vector2.Zero;
        public Vector2 weaponPos { get { return new Vector2(Projectile.ai[1], Projectile.ai[2]); } set { Projectile.ai[1] = value.X; Projectile.ai[2] = value.Y; } }
        public override void AI()
        {
            
            if (Util.Util.getDistance(weaponPos, Projectile.Center) > 3000)
            {
                weaponPos = Projectile.Center;
            }
            Player player = Main.player[Projectile.owner];
            if (Util.Util.getDistance(Projectile.Center, player.Center) > 3600)
            {
                Projectile.Center = player.Center;
            }
                if (player.HasBuff(ModContent.BuffType<TeletorBuff>()))
            {
                Projectile.timeLeft = 3;
            }
            NPC target = null;
            if (player.HasMinionAttackTargetNPC)
            {
                target = Main.npc[player.MinionAttackTargetNPC];
                float betw = Vector2.Distance(target.Center, Projectile.Center);
                if (betw > 2000f)
                {
                    target = null;
                }

            }
            if (target == null || !target.active)
            {
                NPC t = Projectile.FindTargetWithinRange(1000, false);
                if (t != null)
                {
                    target = t;
                }
            }
            Projectile.ai[0]--;
            Vector2 targetPos = player.Center - new Vector2(0, 120);
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 60)
            {
                Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.8f;
                Projectile.velocity *= 0.98f;
            }
            if (target != null)
            {
                Projectile.direction = (target.Center.X > Projectile.Center.X ? 1 : -1);
                if (Util.Util.getDistance(Projectile.Center, weaponPos) < 120)
                {
                    Projectile.ai[0] = -1;
                }
                if (Projectile.ai[0] >= 0)
                {
                    weaponVel += (Projectile.Center + new Vector2(0, -60) - weaponPos).SafeNormalize(Vector2.Zero) * 5f;
                }
                else
                {
                    weaponVel += (target.Center - weaponPos).SafeNormalize(Vector2.Zero) * 5f;
                }
            }
            else
            {
                weaponVel += (Projectile.Center + new Vector2(0, -60) - weaponPos).SafeNormalize(Vector2.Zero) * 5f;
                Projectile.direction = (Projectile.velocity.X > 0 ? 1 : -1);
                
            }
            weaponVel *= 0.9f;
            
            Projectile.rotation = MathHelper.ToRadians(Projectile.velocity.X * 1.4f);
            
            weaponPos += weaponVel;
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.netUpdate = true;
            }


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] <= 0)
            {
                Projectile.ai[0] = 90;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return new Rectangle((int)(weaponPos.X - 8), (int)(weaponPos.Y - 8), 16, 16).Intersects(targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Vector2 L1 = Projectile.Center + new Vector2(-11 * Projectile.direction, -12).RotatedBy(Projectile.rotation);
            Vector2 L2 = Projectile.Center + new Vector2(11 * Projectile.direction, -14).RotatedBy(Projectile.rotation);

            List<Vector2> points1 = LightningGenerator.GenerateLightning(L1, weaponPos, 22, 6);
            List<Vector2> points2 = LightningGenerator.GenerateLightning(L2, weaponPos, 22, 6);
            Util.Util.DrawLines(points1, Color.Red * 0.6f, 2);
            Util.Util.DrawLines(points2, Color.Blue * 0.6f, 2);

            Texture2D weaponTex = TextureAssets.Item[4].Value;
            Main.spriteBatch.Draw(weaponTex, weaponPos - Main.screenPosition, null, Lighting.GetColor((int)weaponPos.X / 16, (int)weaponPos.Y / 16), MathHelper.ToRadians(-45) + MathHelper.ToRadians(weaponVel.X * 1.8f), weaponTex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            Texture2D spt = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(spt, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, spt.Size() / 2, Projectile.scale, (Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally), 0);
            return false;
        }

    }
}

