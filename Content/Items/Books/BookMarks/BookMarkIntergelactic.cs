using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkIntergelactic : BookMark
    {
        public override Texture2D UITexture => BookMark.GetUITexture("Intergelactic");
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            if (ModLoader.TryGetMod("CatalystMod", out Mod caly))
            {
                if (caly.TryFind<ModRarity>("SuperbossRarity", out ModRarity rare))
                {
                    Item.rare = rare.Type;
                }
            }
        }
        public override Color tooltipColor => Color.LightSkyBlue;
        public override EBookProjectileEffect getEffect()
        {
            return new IGBMEffect();
        }
    }
    public class IGBMEffect : EBookProjectileEffect
    {
        public override void BookUpdate(Projectile projectile, bool s)
        {
            if (s && projectile.ModProjectile is EntropyBookHeldProjectile book)
            {
                if (Main.GameUpdateCount % 30 == 0)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, projectile.velocity * 0.6f, ModContent.ProjectileType<NovaSlimerProj>(), book.CauculateProjectileDamage(), projectile.knockBack, projectile.owner);
                }
            }
        }
    }
}