using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class AbyssalVortex : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = Util.CUtil.rogueDC;
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.light = 2f;
            Projectile.timeLeft = 80;
            Projectile.penetrate = -1;
            Projectile.ArmorPenetration = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }
        public int ct = 0;
        float scale = 0;
        float scalej = 0.25f;
        public override void AI()
        {
            scale += scalej;
            scalej -= 0.03f;
            if (scale < 0)
            {
                Projectile.Kill();
            }
            Projectile.rotation += 0.2f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Projectile.Center.getRectCentered(188 * scale, 188 * scale).Intersects(targetHitbox);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t1 = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(t1, Projectile.Center - Main.screenPosition, null, Color.DarkBlue, Projectile.rotation, new Vector2(t1.Width, t1.Height) / 2, 188f / 408f * scale, SpriteEffects.None, 0);

            return false;
        }


    }


}