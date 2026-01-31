using CalamityMod;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Tools
{
    public class MottledSpear : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.width = 54;
            Item.height = 48;
            Item.shootSpeed = MottledSpearHook.LaunchSpeed;
            Item.shoot = ModContent.ProjectileType<MottledSpearHook>();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[1]", MottledSpearHook.GrappleRangInTiles);
            tooltips.Replace("[2]", MottledSpearHook.LaunchSpeed);
            tooltips.Replace("[3]", MottledSpearHook.ReelbackSpeed);
            tooltips.Replace("[4]", MottledSpearHook.PullSpeed);
        }
    }
    public class MottledSpearHook : ModProjectile
    {
        public const float PullSpeed = 24f;

        public const float ReelbackSpeed = 28f;

        public const float LaunchSpeed = 28f;

        public const float GrappleRangInTiles = 32f;

        public override void SetDefaults()
        {
            base.Projectile.CloneDefaults(230);
        }

        public override bool? CanUseGrapple(Player player)
        {
            int num = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == base.Projectile.type)
                {
                    num++;
                }
            }
            if (num > 0)
            {
                return false;
            }
            return true;
        }

        public override float GrappleRange()
        {
            return GrappleRangInTiles * 16;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = ReelbackSpeed;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = PullSpeed;
            if (Projectile.Distance(player.MountedCenter) < 32)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D hook = Projectile.GetTexture();
            Projectile.DrawHook(this.getTextureAlt("Chain")); //Draw the chain
            Vector2 origin = new Vector2(40, hook.Height / 2);
            Main.EntitySpriteDraw(hook, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.Center.X > Projectile.GetOwner().Center.X ? SpriteEffects.None : SpriteEffects.FlipVertically);
            return false;
        }

        public override void AI()
        {
            base.Projectile.spriteDirection = -base.Projectile.direction;
            if (base.Projectile.ai[0] == 2f)
            {
                base.Projectile.extraUpdates = 1;
            }
            else
            {
                base.Projectile.extraUpdates = 0;
            }
            Projectile.rotation = (Projectile.Center - Projectile.GetOwner().Center).ToRotation();
        }
    }
}
