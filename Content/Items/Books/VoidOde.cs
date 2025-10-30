﻿using CalamityEntropy.Common;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class VoidOde : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 125;
            Item.useAnimation = Item.useTime = 7;
            Item.crit = 10;
            Item.mana = 6;
            Item.shootSpeed = 44;
            Item.rare = ModContent.RarityType<VoidPurple>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BookMark6").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<VoidOdeHeld>();
        public override int SlotCount => 5;

        public override void AddRecipes()
        {

            CreateRecipe().AddIngredient<AshTranscript>()
                .AddIngredient<VoidBar>(5)
                .AddTile<VoidWellTile>()
                .Register();
        }
    }

    public class VoidOdeHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/VoidOde/VoidOdeOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/VoidOde/VoidOdePage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/VoidOde/VoidOdeUI";

        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.Homing += 1.6f;
            m.HomingRange += 1f;
            return m;
        }
        public override float randomShootRotMax => 0.4f;
        public override bool Shoot()
        {
            Vector2 ovel = Projectile.velocity;
            Vector2 opos = Projectile.Center;
            Projectile.Center = new Vector2(Main.MouseWorld.X + Main.rand.NextFloat(-120, 120), Projectile.GetOwner().Center.Y + 640);
            Projectile.velocity = (Main.MouseWorld - Projectile.Center).normalize() * 6;
            bool s = base.Shoot();
            Projectile.Center = opos;
            Projectile.velocity = ovel;
            return s;
        }
        public override int frameChange => 2;
        public override int baseProjectileType => ModContent.ProjectileType<SighterPinFriendly>();
        public override EBookProjectileEffect getEffect()
        {
            return new VoidOdeBookBaseEffect();
        }

    }
    public class VoidOdeBookBaseEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            base.OnProjectileSpawn(projectile, ownerClient);
            projectile.tileCollide = false;
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 120, 2, 600, 14);
        }
    }
}
