using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Rarities;
using System.CommandLine.Parsing;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class TheDeadCut : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 98;
            Item.height = 88;
            Item.damage = 336;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.shoot = ModContent.ProjectileType<TheDeadCutProjectile>();
            Item.shootSpeed = 16f;
            Item.DamageType = CUtil.rogueDC;
            Item.ArmorPenetration = 56;
            Item.Entropy().tooltipStyle = 3;
            Item.Entropy().NameColor = new Color(110, 0, 140);
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(200, 0, 255);
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
        }
        public int dir = 1;
        public override float StealthDamageMultiplier => 4f;
        public override float StealthVelocityMultiplier => 1f;
        public override float StealthKnockbackMultiplier => 4f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.Calamity().StealthStrikeAvailable();
            int p = Projectile.NewProjectile(source, position, velocity, type, (int)(damage * (stealth ? 1.6f : 1)), knockback, player.whoAmI, dir);
            if (stealth)
            {
                p.ToProj().Calamity().stealthStrike = true;
                float sCost = 1;
                player.itemTimeMax *= 2;
                player.itemAnimationMax *= 2;
                
                if (player.Calamity().stealthStrike85Cost)
                {
                    sCost = 0.85f;
                }
                if (player.Calamity().stealthStrike75Cost)
                {
                    sCost = 0.75f;
                }
                if (player.Calamity().stealthStrikeHalfCost)
                {
                    sCost = 0.5f;
                }
                player.Calamity().rogueStealth -= player.Calamity().rogueStealthMax * sCost;
            }
            dir *= -1;
            return false;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Revelation>(), 5);
            recipe.AddIngredient<TwistingNether>(5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
