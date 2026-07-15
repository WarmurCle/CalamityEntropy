using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Thalassian
{
    public class ThalassianGleam : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 16;
            Item.useAnimation = Item.useTime = 7;
            Item.crit = 10;
            Item.mana = 10;
            Item.shootSpeed = 44;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D BookMarkTexture => CEUtils.pixelTex;
        public override int HeldProjectileType => ModContent.ProjectileType<ThalassianGleamHeld>();
        public override int SlotCount => GetSlotCount();
        public static int GetSlotCount()
        {
            return 4;
        }
        public override void AddRecipes()
        {

        }
        public override bool PreDrawBookmarkSlot(Item bookmark, Vector2 pos, float alpha, float scale, float outlineAlpha)
        {

            return true;
        }
    }

    public class ThalassianGleamHeld : EntropyBookDrawingAlt
    {
        public override string OpenAnimationPath => CEUtils.WhiteTexPath;
        public override string PageAnimationPath => CEUtils.WhiteTexPath;
        public override string UIOpenAnimationPath => CEUtils.WhiteTexPath;
        public override int OpenAnmCount => 1;
        public override int PageAnmCount => 1;
        public override int UIOpenAnmCount => 1;

        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();

            return m;
        }
        public override float randomShootRotMax => 0.05f;
        public override bool Shoot()
        {
            Vector2 ovel = Projectile.velocity;
            Vector2 opos = Projectile.Center;
            Projectile.Center = Projectile.Center + Projectile.rotation.ToRotationVector2() * 42 * Projectile.scale;
            Projectile.velocity = (Main.MouseWorld - Projectile.Center).normalize() * Projectile.velocity.Length();
            bool s = base.Shoot();
            Projectile.Center = opos;
            Projectile.velocity = ovel;
            return s;
        }
        public override void playTurnPageAnimation()
        {
        }
        public override Texture2D getTexture()
        {
            return Projectile.GetTexture();
        }
        public override Rectangle GetFrame()
        {
            Texture2D tex = getTexture();
            return new Rectangle(0, 0, tex.Width, tex.Height);
        }
        public override Vector2 GetOrigin()
        {
            return new Vector2(30, 80);
        }
        public override void AI()
        {
            base.AI();
            if (UIOpen)
            {
                if(UIOpenAnmSCount < 20)
                    UIOpenAnmSCount++;
                Projectile.Center += Vector2.UnitY * (18 - CEUtils.GetRepeatedCosFromZeroToOne(UIOpenAnmSCount / 20f, 2) * 40) * Projectile.scale;
                Projectile.rotation = -MathHelper.PiOver2;
            }
            else
            {
                UIOpenAnmSCount = 0;
                Projectile.rotation = CEUtils.RotateTowardsAngle(Projectile.rotation, (Projectile.velocity.X > 0 ? 0 : MathHelper.Pi), 0.5f, false);
                Projectile.rotation += (Projectile.velocity.X > 0 ? 1 : -1) * 1f;
            }
        }
        public int UIOpenAnmSCount = 0;
        public override int frameChange => 1;
        public override int baseProjectileType => ModContent.ProjectileType<SighterPinFriendly>();
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = getTexture();
            Vector2 origin = GetOrigin();
            float rot = 1.22f;
            int dir = Projectile.velocity.X > 0 ? 1 : -1;
            if(dir < 0)
            {
                rot *= -1;
                origin.Y = texture.Height - origin.Y;
            }
            
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + rot, origin, Projectile.scale, (Projectile.velocity.X > 0 || UIOpen ? SpriteEffects.None : SpriteEffects.FlipVertically), 0);
            return false;
        }
    }
}
