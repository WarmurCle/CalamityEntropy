using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Utilities;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace CalamityEntropy.Content.UI.EntropyBookUI
{
    public static class EBookUI
    {
        public static bool active = false;
        public static float slotRot = 3;
        public static float slotDist = 0;
        public static Item bookItem = null;
        public static int getMaxSlots(Player player, Item item)
        {
            if (item == null)
            {
                return 0;
            }
            if (item.ModItem is EntropyBook eb)
            {
                return eb.SlotCount;
            }
            return 0;
        }
        public static void checkStackItemList()
        {
            if (Main.LocalPlayer.Entropy().EBookStackItems == null)
            {
                Main.LocalPlayer.Entropy().EBookStackItems = new List<Item>();
            }
            if (bookItem != null)
            {
                while (Main.LocalPlayer.Entropy().EBookStackItems.Count < getMaxSlots(Main.LocalPlayer, bookItem))
                {
                    Main.LocalPlayer.Entropy().EBookStackItems.Add(new Item(ItemID.None));
                }
                if (Main.LocalPlayer.Entropy().EBookStackItems.Count > getMaxSlots(Main.LocalPlayer, bookItem))
                {
                    for (int i = getMaxSlots(Main.LocalPlayer, bookItem) - 1; i >= 0; i--)
                    {
                        if (Main.LocalPlayer.Entropy().EBookStackItems[i].type == ItemID.None)
                        {
                            Main.LocalPlayer.Entropy().EBookStackItems.RemoveAt(i);
                        }
                    }
                }
            }
        }
        public static bool lastMouseLeft = false;
        public static bool lastMouseRight = false;
        public static int closeAnm = 0;
        public static void update()
        {
            checkStackItemList();
            if (!(Main.playerInventory) || Main.mouseItem.ModItem is EntropyBook)
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
                if (closeAnm > 0)
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
            if (Main.LocalPlayer.Entropy().EBookStackItems == null)
            {
                return;
            }
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

                    if (Main.LocalPlayer.Entropy().EBookStackItems.Count > i && Main.LocalPlayer.Entropy().EBookStackItems[i].type != ItemID.None)
                    {
                        if (BookMarkLoader.IsABookMark(Main.LocalPlayer.Entropy().EBookStackItems[i]) && BookMarkLoader.GetUITexture(Main.LocalPlayer.Entropy().EBookStackItems[i]) != null)
                        {
                            Main.spriteBatch.Draw(BookMarkLoader.GetUITexture(Main.LocalPlayer.Entropy().EBookStackItems[i]), pos, null, Color.White * (closeAnm / 11f), 0, BookMarkLoader.GetUITexture(Main.LocalPlayer.Entropy().EBookStackItems[i]).Size() / 2, 1, SpriteEffects.None, 0);
                        }
                        else
                        {
                            ItemSlot.DrawItemIcon(Main.LocalPlayer.Entropy().EBookStackItems[i], 1, Main.spriteBatch, pos, 0.6f, 256, Color.White * (closeAnm / 11f));
                        }

                    }
                    if (active && Main.MouseScreen.getRectCentered(2, 2).Intersects(pos.getRectCentered(36, 46)))
                    {
                        if (!Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir)
                        {
                            Utilities.Util.showItemTooltip(Main.LocalPlayer.Entropy().EBookStackItems[i]);
                        }
                        Main.LocalPlayer.mouseInterface = true;
                        if (Main.mouseLeft && !lastMouseLeft && (Main.mouseItem.IsAir || BookMarkLoader.IsABookMark(Main.mouseItem)) && !(Main.mouseItem.IsAir && Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir))
                        {
                            lastMouseLeft = true;
                            Item mouseItem = Main.mouseItem.Clone();
                            Main.mouseItem = Main.LocalPlayer.Entropy().EBookStackItems[i];
                            Main.LocalPlayer.Entropy().EBookStackItems[i] = mouseItem;
                            Utilities.Util.PlaySound("turnPage");
                        }
                        if (Main.mouseRight && !lastMouseRight && slotDist > 100)
                        {
                            if (!Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir)
                            {
                                for (int ii = 10; ii < 50; ii++)
                                {
                                    if (Main.LocalPlayer.inventory[ii].IsAir)
                                    {
                                        ItemIO.Load(Main.LocalPlayer.inventory[ii], ItemIO.Save(Main.LocalPlayer.Entropy().EBookStackItems[i]));
                                        Main.LocalPlayer.Entropy().EBookStackItems[i].TurnToAir();
                                        Utilities.Util.PlaySound("turnPage");
                                        break;
                                    }
                                }
                            }
                        }
                        if (Main.mouseItem.IsAir && Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir)
                        {
                            Main.instance.MouseText(CalamityEntropy.Instance.GetLocalization("SlotInfo").Value);
                        }
                    }


                }
                lastMouseLeft = Main.mouseLeft;
                lastMouseRight = Main.mouseRight;
            }
        }
    }
}