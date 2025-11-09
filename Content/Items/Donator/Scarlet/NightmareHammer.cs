using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.MainHammer;
using CalamityMod;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.Scarlet
{
    public class NightmareHammer: BaseHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<NightmareHammerProj>();
        public override void ExSSD()
        {
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<NightmareHammer>()] = ModContent.ItemType<GodsHammer>();
        }
        public override void ExSD()
        {
            Item.width = 88;
            Item.height = 94;
            Item.damage = 75;
            Item.useTime = 18;
            //这里的UseTime是有意改的很慢的
            Item.useAnimation = 18;
            Item.shootSpeed = 24f;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item103;
            Item.value = Item.buyPrice(gold: 12);
        }
        public override void ExModifyTooltips(List<TooltipLine> tooltips)
        {
            if (DownedBossSystem.downedDoG && !Main.LocalPlayer.Entropy().CanDisableGuideForGodsHammer)
                tooltips.QuickAddTooltip($"Mods.CalamityEntropy.Weapons.Rogue.{GetType().Name}.ShimmmerTooltip", Color.LightPink);
        }
        private static float UpdatePos
        {
            get
            {
                return ((float)(Math.Sin(Main.GlobalTimeWrappedHourly * 1f) * 1.2f + 1.4f)).ToClamp(1.0f,2.4f);
            }
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //草拟吗瑞德
            //没有击倒神长，正常绘制这把锤子
            if (!DownedBossSystem.downedDoG)
                return true;
            //第二个判定，如果已经获得了弑神锤，返回
            if (Main.LocalPlayer.Entropy().CanDisableGuideForGodsHammer)
                return true;

            //否则绘制这把锤子的其他效果。
            Texture2D tex = TextureAssets.Item[Type].Value;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * UpdatePos, null, Color.Pink with { A = 0 }, 0f, origin, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FallenHammer>().
                AddIngredient<AshesofCalamity>(15).
                AddIngredient<Necroplasm>(15).
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient(ItemID.LargeAmethyst).
                DisableDecraft().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
