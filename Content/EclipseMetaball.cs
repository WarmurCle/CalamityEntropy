
using CalamityMod.Enums;
using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content
{
    public class EclipseMetaball : Metaball
    {
        public class EclipseParticle
        {
            public float Size;

            public Vector2 Velocity;

            public Vector2 Center;

            public EclipseParticle(Vector2 center, Vector2 velocity, float size)
            {
                Center = center;
                Velocity = velocity;
                Size = size;
            }

            public void Update()
            {
                Size *= 0.94f;
                Center += Velocity;
                Velocity *= 0.96f;
            }
        }
        public override GeneralDrawLayer DrawLayer => GeneralDrawLayer.BeforeProjectiles;

        private static List<Asset<Texture2D>> layerAssets;

        public static List<EclipseParticle> Particles
        {
            get;
            private set;
        } = new();

        public override bool AnythingToDraw => Particles.Count > 0;

        public override IEnumerable<Texture2D> Layers
        {
            get
            {
                for (int i = 0; i < layerAssets.Count; i++)
                    yield return layerAssets[i].Value;
            }
        }

        public override Color EdgeColor => new(255, 206, 60);

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            layerAssets = new() { CEUtils.getExtraTexAsset("black") };
        }

        public override void ClearInstances() => Particles.Clear();

        public override void Update()
        {
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 2.5f);
        }

        public static void SpawnParticle(Vector2 position, Vector2 velocity, float size) =>
            Particles.Add(new(position, velocity, size));

        public override Vector2 CalculateManualOffsetForLayer(int layerIndex)
        {
            return Vector2.Zero;
        }

        public override void DrawInstances()
        {
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle").Value;

            foreach (EclipseParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                Vector2 origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();
                Main.spriteBatch.Draw(tex, drawPosition, null, Color.White, 0f, origin, scale, 0, 0f);
            }
        }
    }
}