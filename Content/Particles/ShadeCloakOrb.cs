using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Particles
{
    public class ShadeCloakOrb : EParticle
    {
        public List<Vector2> odp = new List<Vector2>();
        public override Texture2D Texture => ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/a_circle").Value;
        public override void OnSpawn()
        {
            this.Lifetime = 160;
        }
        public int maxLength = 12;
        public int PlayerIndex = 0;
        public override void AI()
        {
            Vector2 lp = Position;
            base.AI();
            if(Lifetime < 8)
            {
                Velocity *= 0;
                Position = Vector2.Lerp(Position, Vector2.Zero, (1 - (Lifetime / 8f)) * 0.8f);
            }
            else
            {
                Velocity *= 0.98f;
            }
            for(float i = 0.1f; i <= 1; i += 0.1f)
            {
                AddPoint(Vector2.Lerp(lp, Position, i));
            }
            
        }
        public void AddPoint(Vector2 pos)
        {
            odp.Insert(0, pos);
            if (odp.Count > maxLength)
            {
                odp.RemoveAt(odp.Count - 1);
            }
        }

        public override void Draw()
        {
            Vector2 worldPos = PlayerIndex.ToPlayer().Center;
            for(int i = 0; i < odp.Count; i++)
            {
                float sz = (i + 1) / (float)odp.Count;
                if(Lifetime > 60)
                {
                    sz = 1 - (i / (float)odp.Count);
                }
                Main.spriteBatch.Draw(Texture, odp[i] + worldPos - Main.screenPosition, null, this.Color, Rotation, getOrigin(),this.Scale * 0.16f * sz, SpriteEffects.None, 0);
            }
        }
    }
}
