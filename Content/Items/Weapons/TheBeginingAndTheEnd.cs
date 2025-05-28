using CalamityEntropy.Content.Projectiles.BNE;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class TheBeginingAndTheEnd : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 38;
            Item.damage = 3200;
            Item.ArmorPenetration = 80;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.ArmorPenetration = 86;
            Item.knockBack = 1f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ModContent.RarityType<AbyssalBlue>();
            Item.shoot = ModContent.ProjectileType<TheBeginning>();
            Item.shootSpeed = 9f;
            Item.DamageType = CEUtils.RogueDC;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int p1 = ModContent.ProjectileType<TheBeginning>();
            int p2 = ModContent.ProjectileType<TheEnd>();
            if (player.Calamity().StealthStrikeAvailable())
            {
                int r = (Main.rand.NextBool() ? -1 : 1);
                int p = Projectile.NewProjectile(source, position, velocity.RotatedBy(0.2f * r), p1, (int)(damage), knockback, player.whoAmI);
                Main.projectile[p].Calamity().stealthStrike = true;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
                p = Projectile.NewProjectile(source, position, velocity.RotatedBy(-0.2f * r), p2, (int)(damage), knockback, player.whoAmI);
                Main.projectile[p].Calamity().stealthStrike = true;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                }
                return false;
            }
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, p2, damage, knockback, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, p1, damage, knockback, player.whoAmI);
            }
            return false;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public static void playShootSound(Vector2 c)
        {
            CEUtils.PlaySound("bne" + Main.rand.Next(0, 3).ToString(), 1, c);
        }
        public override float StealthDamageMultiplier => 1f;
        public override float StealthVelocityMultiplier => 0.8f;
        public override float StealthKnockbackMultiplier => 3f;

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<JawsOfOblivion>())
                .AddIngredient(ModContent.ItemType<WyrmTooth>(), 12)
                .AddTile(ModContent.TileType<AbyssalAltarTile>())
                .Register();
        }
    }
}
