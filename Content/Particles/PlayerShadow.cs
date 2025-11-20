using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Particles
{
    public class PlayerShadow : EParticle
    {
        public override Texture2D Texture => CEUtils.pixelTex;
        public Player plr;
        public override void OnSpawn()
        {
            this.Lifetime = 20;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / (float)this.TimeLeftMax;
        }

        public override void Draw()
        {
            if (plr != null)
            {
                Main.PlayerRenderer.DrawPlayer(Main.Camera, plr, this.Position, this.Rotation, Vector2.Zero, (1 - Opacity * 0.35f), 1);
            }
        }
    }
    public class PlayerShadowBlack : EParticle
    {
        public override Texture2D Texture => CEUtils.pixelTex;
        public Player plr;
        public override void OnSpawn()
        {
            this.Lifetime = 20;
        }
        public override void AI()
        {
            base.AI();
            this.Opacity = this.Lifetime / (float)this.TimeLeftMax;
            Opacity *= alpha;
        }
        public float alpha = 1;
        public Player clone = null;
        public override void Draw()
        {
            if (clone == null)
            {
                clone = new Player();

                clone.CopyVisuals(plr);
                clone.skinColor = Color.Black;
                clone.shirtColor = Color.Black;
                clone.underShirtColor = Color.Black;
                clone.pantsColor = Color.Black;
                clone.shoeColor = Color.Black;
                clone.hairColor = Color.Black;
                clone.eyeColor = Color.Red;
                for (int i = 0; i < clone.dye.Length; i++)
                {
                    if (clone.dye[i].type != ItemID.ShadowDye)
                    {
                        clone.dye[i].SetDefaults(ItemID.ShadowDye);
                    }
                }
                clone.ResetEffects();
                clone.ResetVisibleAccessories();
                clone.DisplayDollUpdate();
                clone.UpdateSocialShadow();
                clone.UpdateDyes();
                clone.PlayerFrame();
                if (plr.ItemAnimationActive && plr.altFunctionUse != 2)
                    clone.bodyFrame = plr.bodyFrame;
                else
                    clone.bodyFrame.Y = 0;
                clone.legFrame.Y = 0;
                clone.direction = plr.direction;
            }
            Main.PlayerRenderer.DrawPlayer(Main.Camera, clone, Position, 0f, clone.fullRotationOrigin, 1 - Opacity, 1f);

        }
    }

}
