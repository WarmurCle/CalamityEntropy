using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityEntropy.Common.EGlobalItem;

namespace CalamityEntropy.Content.Items
{
    public class DragEntity : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.width = 2;
            Item.height = 2;
            Item.rare = ItemRarityID.Master;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.channel = true;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.EverythingElse;
        }

        public static Entity dragging = null;
        public static Vector2 dragOffset = Vector2.Zero;
        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (!player.channel)
                {
                    dragging = null;
                }
                else
                {
                    if (dragging != null)
                    {
                        dragging.Center = Main.MouseWorld + dragOffset;
                    }
                }
            }
        }
        public override bool? UseItem(Player player)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.Hitbox.Intersects(Main.MouseWorld.getRectCentered(2, 2)))
                {
                    dragging = npc;
                    dragOffset = npc.Center - Main.MouseWorld;
                    return true;
                } 
            }
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.Hitbox.Intersects(Main.MouseWorld.getRectCentered(2, 2)))
                {
                    dragging = proj;
                    dragOffset = proj.Center - Main.MouseWorld;
                    return true;
                }
            }
            return false;
        }
    }
}
