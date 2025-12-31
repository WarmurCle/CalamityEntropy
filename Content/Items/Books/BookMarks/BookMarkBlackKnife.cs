using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkBlackKnife : BookMark, IGetFromStarterBag
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("blackknife");
        public override void ModifyStat(EBookStatModifer modifer)
        {
            modifer.armorPenetration += 8;
        }
        public override Color tooltipColor => Color.Gray;
        public override EBookProjectileEffect getEffect()
        {
            return new BlackKnifeBMEffect();
        }
        public bool OwnAble(Player player, ref int count)
        {
            return ShadowCrystalDeltarune.Ch3Crystal;
        }
    }
    public class BlackKnifeBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 5 : 7) && CECooldowns.CheckCD("BlackKnifeBMProj", 3))
            {
                Projectile.NewProjectile(projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<BlackKnife>(), damageDone, projectile.knockBack, projectile.owner, target.whoAmI, CEUtils.randomRot());
            }
        }
    }
}
