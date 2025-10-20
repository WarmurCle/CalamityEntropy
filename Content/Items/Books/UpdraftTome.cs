using CalamityEntropy.Content.Projectiles;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class UpdraftTome : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 64;
            Item.useAnimation = Item.useTime = 40;
            Item.crit = 5;
            Item.mana = 6;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark2").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<UpdraftTomeHeld>();
        public override int SlotCount => 2;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AerialiteBar>(6)
                .AddIngredient<AncientScriptures>()
                .AddTile(TileID.SkyMill)
                .Register();
        }
    }

    public class UpdraftTomeHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/UpdraftTome/UpdraftTomeOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/UpdraftTome/UpdraftTomePage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/UpdraftTome/UpdraftTomeUI";

        public override void playPageSound()
        {
            CEUtils.PlaySound("windpage", 1, Projectile.Center, 6, 0.52f);
        }

        public override float randomShootRotMax => 0.14f;
        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.Knockback *= 2;
            return m;
        }
        public override int baseProjectileType => ModContent.ProjectileType<UpdraftBullet>();
    }

}
