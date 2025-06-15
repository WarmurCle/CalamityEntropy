using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class NovaSlimerProj : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.pushByOther(1.4f);
            NPC target = Projectile.FindTargetWithinRange(3000);
            if (target != null)
            {
                Projectile.velocity *= 0.98f;
                Projectile.velocity += (target.Center - Projectile.Center).normalize() * 1f * (CEUtils.getDistance(Projectile.Center, target.Center) > 360 ? 1 : -1.4f);
                if (CEUtils.getDistance(Projectile.Center, target.Center) < 400 && Projectile.ai[0] > 40)
                {
                    if (ModLoader.TryGetMod("CatalystMod", out Mod caly))
                    {
                        if (caly.TryFind<ModProjectile>("NebulaSpear", out var nsp))
                        {
                            int type = nsp.Type;
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (target.Center - Projectile.Center).normalize() * 14, type, Projectile.damage, Projectile.knockBack, Projectile.owner, 1, -1, 0);
                        }
                    }
                    Projectile.Kill();
                }
            }
            else
            {
                if (Projectile.GetOwner().Distance(Projectile.Center) > 160)
                {
                    Projectile.velocity *= 0.99f;
                    Projectile.velocity += (Projectile.GetOwner().Center - Projectile.Center).normalize() * 0.8f;
                }


            }
            Projectile.rotation += 0.2f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (ModLoader.HasMod("CatalystMod"))
            {
                Texture2D tex = ModContent.Request<Texture2D>("CatalystMod/NPCs/Boss/Astrageldon/NovaSlimer").Value;
                Texture2D texGlow = ModContent.Request<Texture2D>("CatalystMod/NPCs/Boss/Astrageldon/NovaSlimer_Glow").Value;
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texGlow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
    }


}