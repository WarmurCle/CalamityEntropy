using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Prominence : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 6));
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 80;
            Item.damage = 190;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = Util.Util.GetSound("ProminenceShoot");
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 22f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-28, 0);
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            p.ToProj().Entropy().ProminenceArrow = true;
            Util.Util.SyncProj(p);
            for(int i = 0; i < 24; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.6f) * 0.3f * Main.rand.NextFloat(0.4f, 1f), ModContent.ProjectileType<ProminenceSplitShot>(), damage / 12, knockback * 2, player.whoAmI);
                Vector2 vel = velocity.RotatedByRandom(0.84f) * 1.4f * Main.rand.NextFloat(0.4f, 1f);
                EParticle.spawnNew(new StrikeParticle(), position, vel, Color.Lerp(Color.OrangeRed, new Color(255, 231, 66), Main.rand.NextFloat()), 0.54f, 1, true, BlendState.Additive, vel.ToRotation());
            }

            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.FragmentSolar, 16)
                .AddIngredient<TheBallista>()
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
        public override bool RangedPrefix()
        {
            return true;
        }
    }
    public class ProminenceSplitShot : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0.6f;
            Projectile.timeLeft = 160;
            Projectile.MaxUpdates = 8;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ArmorPenetration = 64;
        }
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void AI()
        {
            Projectile.velocity *= 0.97f;
            odp.Add(Projectile.Center);
            if(odp.Count > 12)
            {
                odp.RemoveAt(0);
            }
        }
        public List<Vector2> odp = new List<Vector2>();
        public Color clr = Color.Lerp(Color.OrangeRed, new Color(255, 231, 66), Main.rand.NextFloat());
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Texture2D circle = Util.Util.getExtraTex("BasicCircle");
            for(int i = 0; i < odp.Count; i++)
            {
                float s = (i + 1f) / (float)odp.Count * ((float)Projectile.timeLeft / 160f);
                Main.spriteBatch.Draw(circle, odp[i] - Main.screenPosition, null, Color.Lerp(clr, Color.White, (i + 1f) / (float)odp.Count), 0, circle.Size() * 0.5f, 0.18f * s, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
