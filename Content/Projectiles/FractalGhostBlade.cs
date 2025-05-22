using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class FractalGhostBlade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.light = 1f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6 * 60 * 4;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 4;
            Projectile.tileCollide = false;
        }
        bool init = true;
        public float counter = 0;
        public float pg = 0;
        public bool hited = false;
        public bool launch = true;
        public override bool? CanHitNPC(NPC target)
        {
            if(launch) return false;
            return base.CanHitNPC(target);
        }
        public bool playSound = true;
        public override void AI()
        {
            if (playSound)
            {
                playSound = false;
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
            }
            if(Projectile.timeLeft < 240)
            {
                Projectile.Opacity = Projectile.timeLeft / 240f;
            }
            Player player = Projectile.getOwner();
            player.Calamity().mouseWorldListener = true;
            if(Main.myPlayer == Projectile.owner)
            {
                Main.LocalPlayer.Calamity().mouseWorld = Main.MouseWorld;
            }
            if (init)
            {
                Projectile.rotation = Util.randomRot();
                init = false;
            }
            Projectile.rotation += 0.08f;
            if (counter < 30 * Projectile.MaxUpdates)
            {
                Projectile.velocity *= 0.96f;
            }
            else
            {
                NPC target = Util.FindTarget_HomingProj(Projectile, Projectile.Center, 4000);
                if (target != null)
                {
                    if (launch)
                    {
                        EParticle.spawnNew(new Particles.ImpactParticle(), Projectile.Center, Vector2.Zero, Color.IndianRed, 0.18f, 1, true, BlendState.Additive, (target.Center - Projectile.Center).ToRotation());

                        if (!Main.dedServ)
                        {
                            SoundStyle sound = SoundID.NPCDeath39;
                            sound.MaxInstances = 6;
                            sound.Pitch = -0.6f;
                            sound.Volume = 0.6f;
                            SoundEngine.PlaySound(sound, Projectile.Center);
                        }
                        Projectile.velocity = (target.Center - Projectile.Center).RotatedByRandom(0.6f).normalize() * 16;
                        launch = false;
                    }
                    Projectile.SmoothHomingBehavior(target.Center, 1, 0.01f);
                }
            }
            counter++;
        }
        public float rotSpeed = 0.001f;
        public override bool PreDraw(ref Color lightColor)
        {
            for(int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float prog = ((float)i / ProjectileID.Sets.TrailCacheLength[Type]);
                Color clr = Color.Lerp(new Color(242, 201, 190), new Color(48, 52, 79), prog) * 0.2f;
                Draw(Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) * 0.5f, clr, Projectile.oldRot[i], (int)Projectile.ai[1]);
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hited)
            {
                Projectile.timeLeft = 4 * 60;
            }
            hited = true;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/Fractal/SpiritFractalGlow";
        public void Draw(Vector2 pos, Color lightColor, float rotation, int dir)
        {
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? rotation + MathHelper.PiOver4 : rotation + MathHelper.Pi * 0.75f;
            var tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, pos - Main.screenPosition, null, lightColor * Projectile.Opacity, rot, tex.Size() * 0.5f, Projectile.scale, effect);
        }
    }


}