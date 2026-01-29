using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkAries : BookMark
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
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.Damage += 0.06f;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Aries");
        public override Color tooltipColor => Color.LightBlue;
        public override EBookProjectileEffect getEffect()
        {
            return new AriesBMEffect();
        }
    }

    public class AriesBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 2 : 3))
                Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<AriesExplosion>(), (damageDone / 6).Softlimitation(240), 1, projectile.owner);
        }
    }
}