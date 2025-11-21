using CalamityMod;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.DustCarverBow
{
    public class DustCarver : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 150;
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = null;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<DustCarverHeld>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] < 1 && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, (Main.MouseWorld - player.Center).normalize() * Item.shootSpeed, Item.shoot, 0, 0, player.whoAmI);
            }
        }
    }
    public class DustCarverHeld : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.width = Projectile.height = 12;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void AI()
        {
            Player player = Projectile.GetOwner();
            Projectile.StickToPlayer();
            player.SetHandRot(Projectile.rotation);
            if(!(player.HeldItem.ModItem is DustCarver))
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 5;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Texture2D eye = this.getTextureGlow();
            SpriteEffects effect = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Vector2 origin = Projectile.velocity.X > 0 ? new Vector2(tex.Width / 2, 83) : new Vector2(tex.Width / 2, tex.Height - 83);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, effect, 0);
            Main.spriteBatch.SetBlendState(BlendState.NonPremultiplied);

            Color eyeColor = Color.White;
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition, null, eyeColor, Projectile.rotation, origin, Projectile.scale, effect, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}