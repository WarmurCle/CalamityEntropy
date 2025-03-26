using CalamityEntropy.Content.Projectiles.Uhrwerk;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Uhrwerk;

public class UhrwerkHammer : ModItem
{
    public override void SetDefaults()
    {
        Item.damage = 2;
        Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
        Item.width = 42;
        Item.height = 42;
        Item.noUseGraphic = true;
        Item.useTime = 16;
        Item.useAnimation = 0;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 1;
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.Item23;
        Item.channel = true;
        Item.noMelee = true;
        Item.shoot = ModContent.ProjectileType<UhrwerkHammerCallout>();
        Item.shootSpeed = 1f;
    }
    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }
    public override bool IsLoadingEnabled(Mod mod)
    {
        return false;
    }
}