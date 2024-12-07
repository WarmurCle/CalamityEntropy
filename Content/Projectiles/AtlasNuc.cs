using System.Collections.Generic;
using CalamityMod;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.Boss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class AtlasNuc : ModProjectile
    {

        public float TileCollisionYThreshold => Projectile.ai[0];

        public bool HasCollidedWithGround
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }

        public ref float SquishFactor => ref Projectile.localAI[0];

        public const float Gravity = 1.1f;

        public const float MaxFallSpeed = 24f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 14;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 86;
            Projectile.height = 130;
            Projectile.netImportant = true;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 400;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
        }

        public override void AI()
        {
            if (SquishFactor <= 0f)
                SquishFactor = 1f;

            if (Projectile.velocity.Y == 0f && !HasCollidedWithGround)
            {
                PerformGroundCollisionEffects();
                HasCollidedWithGround = true;
                Projectile.netUpdate = true;
            }

            // Undo squish effects.
            SquishFactor = MathHelper.Lerp(SquishFactor, 1f, 0.08f);

            // Determine whether to collide with tiles.
            Projectile.tileCollide = Projectile.Bottom.Y >= TileCollisionYThreshold;

            // Calculate frames.
            Projectile.frameCounter++;
            if (!HasCollidedWithGround)
                Projectile.frame = Projectile.frameCounter / 6 % 5;
            else
            {
                Projectile.velocity.X = 0f;
                if (Projectile.frame < 5)
                    Projectile.frame = 5;
                if (Projectile.frameCounter % 8 == 7)
                {
                    Projectile.frame++;

                    // Release the upper part of the pod into the air and spawn the cannon.
                    if (Projectile.frame == 8)
                    {
                        SoundEngine.PlaySound(ThanatosHead.VentSound, Projectile.Top);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            int type = ModContent.ProjectileType<AresGaussNukeProjectile>();
                            Vector2 gaussNukeVelocity = Vector2.Normalize(Main.LocalPlayer.Center - Projectile.Center) * 16;
                            int damage = 99999;
                            float offset = 40f;
                            Projectile.NewProjectile(Main.npc[Projectile.owner].GetSource_FromAI(), Projectile.Center + Vector2.Normalize(gaussNukeVelocity) * offset, gaussNukeVelocity, type, damage, 0f, Main.myPlayer, 0f, Main.LocalPlayer.Center.Y);
                        }
                    }
                }
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = Main.projFrames[Projectile.type] - 1;
            }

            // Fall downward.
            Projectile.velocity.Y += Gravity;
            if (Projectile.velocity.Y > MaxFallSpeed)
                Projectile.velocity.Y = MaxFallSpeed;
        }

        public void PerformGroundCollisionEffects()
        {
            // Become squished.
            SquishFactor = 1.4f;

            // Mechanical Cart laser dust. Looks epic.
            int dustID = 182;
            int dustCount = 54;
            for (int i = 0; i < dustCount; i += 2)
            {
                float pairSpeed = Main.rand.NextFloat(0.5f, 16f);
                Dust d = Dust.NewDustDirect(Projectile.Bottom, 0, 0, dustID);
                d.velocity = Vector2.UnitX * pairSpeed;
                d.scale = 2.7f;
                d.noGravity = true;

                d = Dust.NewDustDirect(Projectile.BottomRight, 0, 0, dustID);
                d.velocity = Vector2.UnitX * -pairSpeed;
                d.scale = 2.7f;
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(Projectile.Center), 1800f, 1000f, 0f, 4.5f);
        }

        // As a means of obscuring contents when they spawn (such as ensuring that the minigun doesn't seem to pop into existence), this projectile draws above most other projectiles.
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool? CanDamage() => false;

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsDropPodGlow").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 scale = Projectile.scale * new Vector2(SquishFactor, 1f / SquishFactor);
            Vector2 origin = frame.Size() * new Vector2(0.5f, 0.5f / SquishFactor);
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, scale, 0, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Color.White, Projectile.rotation, origin, scale, 0, 0);
            return false;
        }
    }
}
