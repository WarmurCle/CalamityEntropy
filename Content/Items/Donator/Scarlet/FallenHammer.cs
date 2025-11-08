using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.FallenHammer;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.Scarlet
{
    public class FallenHammer: BaseHammerItem
    {
        //右键没刷新时间
        public override int ShootProjID => ModContent.ProjectileType<FallenHammerProj>();
        public override void ExSD()
        {
            Item.width = Item.height = 66;
            Item.damage = 30;
            //这里的ut有意为之
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 12);
        }

        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            string path = $"{CEUtils.LocalPrefix}.Weapons.Rogue.HammerHalfStealth";
            tooltips.QuickAddTooltip(path, Color.Yellow);
        }
        //实际合成材料可随意，我个人推荐为花后
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PunishmentHammer>().
                AddIngredient(ItemID.PaladinsHammer).
                AddIngredient<UnholyCore>(10).
                AddIngredient<LifeAlloy>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
