using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using CalamityEntropy.Content.Items.Books.BookMarks;

namespace CalamityEntropy.Content.UI.EntropyBookUI
{
    public static class EBookUI
    {
        public static List<Item> stackItems = null;
        public static bool active = false;
        public static float slotRot = 3;
        public static float slotDist = 0;
        public static Item bookItem = null;
        public static int getMaxSlots(Player player, Item item)
        {
            if(item == null)
            {
                return 1;
            }
            if(item.ModItem is EntropyBook eb)
            {
                return eb.SlotCount;
            }
            return 1;
        }
        public static void checkStackItemList()
        {
            if (stackItems == null)
            {
                stackItems = new List<Item>();
            } 
            while (stackItems.Count < getMaxSlots(Main.LocalPlayer, Main.LocalPlayer.HeldItem))
            {
                stackItems.Add(new Item(ItemID.None));
            }
        }
        public static bool lastMouseLeft = false;
        public static int closeAnm = 0;
        public static void update()
        {
            checkStackItemList();
            if(!(Main.playerInventory) || Main.mouseItem.ModItem is EntropyBook)
            {
                active = false;
            }
            if (active)
            {
                closeAnm = 11;
                slotRot *= 0.88f;
                slotDist += (128 - slotDist) * 0.12f;
            }
            else
            {
                if(closeAnm > 0)
                {
                    closeAnm--;
                    slotDist *= 0.88f;
                    slotRot += (3 - slotRot) * 0.12f;
                }
                else
                {
                    slotRot = 3;
                    slotDist = 0;
                }
            }
        }

        public static void draw()
        {
            checkStackItemList();
            if (active || closeAnm > 0)
            {
                int c = getMaxSlots(Main.LocalPlayer, bookItem);
                for (int i = 0; i < c; i++)
                {
                    Vector2 pos = Main.ScreenSize.ToVector2() / 2 + (MathHelper.ToRadians(i * (360f / c)) + slotRot).ToRotationVector2() * slotDist;
                    if (bookItem.ModItem is EntropyBook eb)
                    {
                        Main.spriteBatch.Draw(eb.BookMarkTexture, pos, null, Color.White * (closeAnm / 11f), 0, eb.BookMarkTexture.Size() / 2, 1, SpriteEffects.None, 0);
                    }
                    
                    if (stackItems.Count > i && stackItems[i].type != ItemID.None)
                    {
                        if (stackItems[i].ModItem is BookMark bm && bm.UITexture != null)
                        {
                            Main.spriteBatch.Draw(bm.UITexture, pos, null, Color.White * (closeAnm / 11f), 0, bm.UITexture.Size() / 2, 1, SpriteEffects.None, 0);
                        }
                        else
                        {
                            ItemSlot.DrawItemIcon(stackItems[i], 1, Main.spriteBatch, pos, 0.6f, 256, Color.White * (closeAnm / 11f));
                        }
                        
                    }
                    if (active && Main.MouseScreen.getRectCentered(2, 2).Intersects(pos.getRectCentered(36, 46)))
                    {
                        if (!stackItems[i].IsAir)
                        {
                            Util.Util.showItemTooltip(stackItems[i]);
                        }
                        Main.LocalPlayer.mouseInterface = true;
                        if (Main.mouseLeft && !lastMouseLeft && (Main.mouseItem.IsAir || Main.mouseItem.ModItem is BookMark) && !(Main.mouseItem.IsAir && stackItems[i].IsAir))
                        {
                            lastMouseLeft = true;
                            Item mouseItem = Main.mouseItem.Clone();
                            Main.mouseItem = stackItems[i];
                            stackItems[i] = mouseItem;
                            Util.Util.PlaySound("turnPage");
                        }
                        if(Main.mouseItem.IsAir && stackItems[i].IsAir)
                        {
                            Main.instance.MouseText(CalamityEntropy.Instance.GetLocalization("SlotInfo").Value);
                        }
                    }
                    

                }
                lastMouseLeft = Main.mouseLeft;
            }
        }
    }
}