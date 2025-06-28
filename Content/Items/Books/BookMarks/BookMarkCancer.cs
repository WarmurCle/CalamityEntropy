using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkCancer : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().stroke = true;
            Item.Entropy().NameColor = Color.LightBlue;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().tooltipStyle = 4;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Cancer");
        public override Color tooltipColor => Color.LightBlue;
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Size += 1.8f;
        }
        public override EBookProjectileEffect getEffect()
        {
            return new CancerBMEffect();
        }
    }

    public class CancerBMEffect : EBookProjectileEffect
    {
        public override void UpdateProjectile(Projectile projectile, bool ownerClient)
        {
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.hostile && projectile.Colliding(projectile.getRect(), p.getRect()))
                {
                    if (p.velocity.Length() * p.MaxUpdates > 4)
                    {
                        p.velocity *= 0.8f;
                    }
                    p.damage = (int)(p.damage * 0.95f);
                }
            }
        }
    }
}