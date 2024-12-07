using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class AbyssalDragonHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 1;
            Projectile.height = 1; 
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        int maxTime;
        public override void AI(){
            Player owner = Projectile.owner.ToPlayer();
            maxTime = owner.HeldItem.useTime;
            Projectile.ai[0]+=1 * owner.GetAttackSpeed(DamageClass.Magic);
            if (Projectile.ai[0] > maxTime)
            {
                Projectile.Kill();
            }
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 nv = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.One);
                if (nv != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = nv;
                
            }
            if (Projectile.velocity.X > 0)
            {
                Projectile.direction = 1;
                owner.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
                owner.direction = -1;
            }
            Projectile.Center = owner.MountedCenter;
            owner.itemRotation = Projectile.rotation * Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.heldProj = Projectile.whoAmI;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(texture, Projectile.owner.ToPlayer().MountedCenter - Main.screenPosition, null, lightColor, Projectile.rotation - MathHelper.ToRadians(45) * Projectile.direction, new Vector2(0, texture.Height / 2), Projectile.scale * 0.8f, (Projectile.direction > 0 ? SpriteEffects.FlipVertically : SpriteEffects.None));
            return false;
        }
    }

}