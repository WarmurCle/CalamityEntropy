using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityEntropy.CalamityEntropy;

namespace CalamityEntropy.Content.Items
{
    public class EntropyModeToggle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.scale = 0.6f;
            Item.UseSound = Util.Util.GetSound("soul");
        }
        public override bool? UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                CalamityEntropy.EntropyMode = !CalamityEntropy.EntropyMode;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (CalamityEntropy.EntropyMode)
                    {
                        Main.NewText(Mod.GetLocalization("EntropyModeActive").Value, new Color(170, 18, 225));
                    }
                    else
                    {
                        Main.NewText(Mod.GetLocalization("EntropyModeDeactive").Value, new Color(170, 18, 225));
                    }
                }
                else
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)NetPackages.SyncEntropyMode);
                    packet.Write(CalamityEntropy.EntropyMode);
                    packet.Send();
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.FallenStar, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
