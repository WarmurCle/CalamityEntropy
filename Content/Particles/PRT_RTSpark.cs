using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace CalamityEntropy.Content.Particles
{
    internal class PRT_RTSpark : BasePRT
    {
        public bool AffectedByGravity;
        public Entity entity;
        public override int InGame_World_MaxCount => 4000;
        public PRT_RTSpark(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Entity entity = null) {
            Position = relativePosition;
            Velocity = velocity;
            AffectedByGravity = affectedByGravity;
            Scale = scale;
            Lifetime = lifetime;
            this.entity = entity;
        }

        public override void AI() {
            Scale *= 0.95f;
            Color = Color.White;
            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && AffectedByGravity) {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }

            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;

            if (entity != null) {
                if (entity.active) {
                    Position += entity.velocity;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => false;

        public static bool HasSet(out List<BasePRT> prts) {
            int rtSparkID = PRTLoader.GetParticleID<PRT_RTSpark>();
            List<BasePRT> reset = new List<BasePRT>();
            foreach (var prt in PRTLoader.PRT_InGame_World_Inds) {
                if (!prt.active) {
                    continue;
                }
                if (prt.ID != rtSparkID) {
                    continue;
                }

                reset.Add(prt);
            }

            prts = reset;
            return reset.Count > 0;
        }

        public static void DrawAll(SpriteBatch spriteBatch, List<BasePRT> prts) {
            int rtSparkID = PRTLoader.GetParticleID<PRT_RTSpark>();
            foreach (var prt in prts) {
                Vector2 scale = new Vector2(0.5f, 1.6f) * prt.Scale;
                Texture2D texture = PRTLoader.PRT_IDToTexture[prt.ID];

                spriteBatch.Draw(texture, prt.Position - Main.screenPosition, null, Color.White, prt.Rotation, texture.Size() * 0.5f, scale, 0, 0f);
            }
        }
    }
}
