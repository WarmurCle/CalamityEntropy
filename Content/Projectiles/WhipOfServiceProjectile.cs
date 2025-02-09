using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
	public class WhipOfServiceProjectile : ModProjectile
	{
		public int[] hitCd = new int[Main.player.Length];
		public override void SetStaticDefaults() {
			// This makes the projectile use whip collision detection and allows flasks to be applied to it.
			ProjectileID.Sets.IsAWhip[Type] = true;
		}
        public override void SetDefaults() {
			// This method quickly sets the whip's properties.
			Projectile.DefaultToWhip();
			Projectile.MaxUpdates = 8;
			Projectile.WhipSettings.Segments = 48;
			Projectile.WhipSettings.RangeMultiplier = 2.2f;
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
			target.Entropy().serviceWhipDamageBonus += 0.07f;
        }
        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
        public override bool PreAI()
        {
			Projectile.hostile = true;
			for(int i = 0; i < hitCd.Length; i++)
			{
				if (hitCd[i] > 0)
				{
					hitCd[i]--;
				}
			}
            var owner = Projectile.owner.ToPlayer();
            return true;
        }

		private void DrawLine(List<Vector2> list) {
			/*Texture2D texture = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
			Rectangle frame = texture.Frame();
			Vector2 origin = new Vector2(0, 0.5f);

			Vector2 pos = list[0];
			for (int i = 0; i < list.Count - 1; i++) {
				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation();
				Color color = new Color(138, 76, 57);
				Vector2 scale = new Vector2(diff.Length() + 2, 2);
				if (i == list.Count - 2)
				{
					scale.X -= 8;
				}

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

				pos += diff;
			}*/
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
				Vector2 origin = new Vector2(7, 4); // Offset for where the player's hand will start measured from the top left of the image.
				float scale = 1f;

				// These statements determine what part of the spritesheet to draw for the current segment.
				// They can also be changed to suit your sprite.
				if (i == list.Count - 2) {
					// This is the head of the whip. You need to measure the sprite to figure out these values.
					frame.Y = 36; // Distance from the top of the sprite to the start of the frame.
					frame.Height = 20; // Height of the frame.

					// For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = 1f;
                    origin = new Vector2(7, 0);
					// Offset for where the player's hand will start measured from the top left of the image.

                }
				else if(i > 0){
					// Second Segment
					frame.Y = 14;
					frame.Height = 14;
                    origin = new Vector2(7, 0);
                }

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;

				float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
				Color color = Lighting.GetColor((int)(pos.X / 16), (int)(pos.Y / 16));

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
            return false;
		}
	}
}
