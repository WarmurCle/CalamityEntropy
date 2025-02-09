using System;
using System.Collections;
using System.Collections.Generic;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
	public class VitalfeatherProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			// This makes the projectile use whip collision detection and allows flasks to be applied to it.
			ProjectileID.Sets.IsAWhip[Type] = true;
		}

		public override void SetDefaults() {
			// This method quickly sets the whip's properties.
			Projectile.DefaultToWhip();
			Projectile.MaxUpdates = 8;
			Projectile.WhipSettings.Segments = 19;
			Projectile.WhipSettings.RangeMultiplier = 3.1f;
		}
		public Vector2 lastTop = Vector2.Zero;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			if (new Rectangle(((int)lastTop.X - 36), ((int)lastTop.Y - 36), 72, 72).Intersects(target.Hitbox))
			{
				modifiers.SourceDamage *= 1.25f;
			}
        }
		
        public override bool PreAI()
        {
            var owner = Projectile.owner.ToPlayer();
            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            List<Vector2> points_ = Projectile.WhipPointsForCollision;
            points_.Clear();
            Projectile.FillWhipControlPoints(Projectile, points_);
            List<Vector2> points = points_;
			
            float swingProgress = Timer / swingTime;
            if (swingProgress > 0.5f && swingProgress < 0.85f)
            {
				Lighting.AddLight(lastTop, 1, 0.8f, 0.8f);
				EParticle.spawnNew(new Smoke(),  points[points.Count - 1] + Util.Util.randomVec(2), Util.Util.randomVec(1), Color.OrangeRed * 0.5f, 0.2f, 1, true, BlendState.Additive);
				
				Vector2 top = points[points.Count - 1];
                Vector2 sparkVelocity2 = (lastTop - top) * -0.04f;
                int sparkLifetime2 = Main.rand.Next(9, 14);
                float sparkScale2 = Main.rand.NextFloat(1.2f, 2.2f);
                Color sparkColor2 = Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat(0, 1));
                LineParticle spark = new LineParticle(top, sparkVelocity2, false, (int)(sparkLifetime2), sparkScale2, sparkColor2);
                GeneralParticleHandler.SpawnParticle(spark);

                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                int dustType = DustID.Smoke;
                if (Main.rand.NextBool(2))
                    dustType = DustID.FlameBurst;

				// After choosing a randomized dust and a whip segment to spawn from, dust is spawned.
				Dust dust; Vector2 spinningPoint;


                if (!Main.rand.NextBool(3) && Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f)
				{
					dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);
					dust.position = points[pointIndex];
					dust.fadeIn = 0.3f;
					spinningPoint = points[pointIndex] - points[pointIndex - 1];
					dust.noGravity = true;
					dust.velocity *= 0.5f;
					// This math causes these dust to spawn with a velocity perpendicular to the direction of the whip segments, giving the impression of the dust flying off like sparks.
					dust.velocity += spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
					dust.velocity *= 0.5f;
				}

                spawnArea = Utils.CenteredRectangle(points[points.Count - 1], new Vector2(10f, 10));
                dustType = DustID.FlameBurst;
                dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 100, Color.White);
                dust.position = points[pointIndex];
                dust.fadeIn = 0.1f;
                spinningPoint = points[pointIndex] - points[pointIndex - 1];
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                // This math causes these dust to spawn with a velocity perpendicular to the direction of the whip segments, giving the impression of the dust flying off like sparks.
                dust.velocity += spinningPoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;
            }
			lastTop = points[points.Count - 1];
            return true;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(ModContent.BuffType<DragonWhipDebuff>(), 240);
			Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			Projectile.damage = (int)(Projectile.damage * 0.9f);
			target.AddBuff(ModContent.BuffType<Dragonfire>(), 180);
			SoundEngine.PlaySound(in SoundID.Item14, target.Center);
			for (int i = 0; i < 40; i++)
			{
				int num = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 200, default(Color), 2);
				Dust obj = Main.dust[num];
				obj.position = target.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * target.width / 2f;
				obj.noGravity = true;
				obj.velocity = Util.Util.randomVec(8);
				num = Dust.NewDust(new Vector2(target.position.X, target.position.Y), target.width, target.height, 174, 0f, 0f, 100, default(Color), 2);
				obj.position = target.Center;
				obj.velocity.Y -= 6f;
				obj.velocity *= 2f;
				obj.noGravity = true;
				obj.fadeIn = 1f;
				obj.color = Color.Crimson * 0.5f;
			}


		}

		// This method draws a line between all points of the whip, in case there's empty space between the sprites.
		private void DrawLine(List<Vector2> list) {
			Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = new Vector2(0, 0.5f);

			Vector2 pos = list[0];
			for (int i = 0; i < list.Count - 1; i++) {
				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation();
				Color color = Color.OrangeRed;
				Vector2 scale = new Vector2(diff.Length() + 2, 2);
				if (i == list.Count - 2)
				{
					scale.X -= 8;
				}

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}
		}
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override bool PreDraw(ref Color lightColor) {
			List<Vector2> list_ = new List<Vector2>();
			Projectile.FillWhipControlPoints(Projectile, list_);
			List<Vector2> list = list_;
			DrawLine(list);

			//Main.DrawWhip_WhipBland(Projectile, list);
			// The code below is for custom drawing.
			// If you don't want that, you can remove it all and instead call one of vanilla's DrawWhip methods, like above.
			// However, you must adhere to how they draw if you do.

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++) {
				// These two values are set to suit this projectile's sprite, but won't necessarily work for your own.
				// You can change them if they don't!
				Rectangle frame = new Rectangle(0, 0, 22, 30); // The size of the Handle (measured in pixels)
				Vector2 origin = new Vector2(11, 24); // Offset for where the player's hand will start measured from the top left of the image.
				float scale = 1.5f;

				// These statements determine what part of the spritesheet to draw for the current segment.
				// They can also be changed to suit your sprite.
				if (i == list.Count - 2) {
					// This is the head of the whip. You need to measure the sprite to figure out these values.
					frame.Y = 54; // Distance from the top of the sprite to the start of the frame.
					frame.Height = 20; // Height of the frame.

					// For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true)) * 1.5f;
                    origin = new Vector2(11, 0);
					// Offset for where the player's hand will start measured from the top left of the image.

                }
                else if(i > 0)
				{
                    if (i % 2 == 0)
                    {
                        // Third segment
                        frame.Y = 30;
                        frame.Height = 12;
                        origin = new Vector2(11, 0);
                    }
                    else
                    {
                        // Second Segment
                        frame.Y = 42;
                        frame.Height = 12;
                        origin = new Vector2(11, 0);
                    }
                }

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Color.White;

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D light = Util.Util.getExtraTex("lightball");
            Main.spriteBatch.Draw(light, lastTop - Main.screenPosition, null, Color.Gold * 0.6f, 0, light.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
		}
	}
}
