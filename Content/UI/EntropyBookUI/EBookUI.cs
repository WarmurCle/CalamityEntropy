using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Books;
using Microsoft.Xna.Framework.Graphics;
using System;
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
        public static float outline = 0;
        public static int getMaxSlots(Player player, Item item)
        {
            int additional = player.Entropy().AdditionalBookmarkSlot;
            if (item == null || item.ModItem == null)
            {
                return 0;
            }
            if (item.ModItem is EntropyBook eb)
            {
                return eb.SlotCount + additional;
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
            if (!Main.dedServ && !BookmarkInsertCondition.Instance.IsCompleted)
            {
                if (active)
                {
                    foreach (var i in Main.LocalPlayer.Entropy().EBookStackItems)
                    {
                        if (BookMarkLoader.IsABookMark(i))
                        {
                            BookmarkInsertCondition.Instance.Complete();
                        }
                    }
                }
            }
        }
        public static int ChoosedBMSlot = 0;
        public static void draw()
        {
            bool sync = false;
            if (Main.LocalPlayer.Entropy().EBookStackItems == null)
            {
                return;
            }
            checkStackItemList();
            bool outlineFlag = false;
            if (active || closeAnm > 0)
            {
                int c = getMaxSlots(Main.LocalPlayer, bookItem);
                List<Texture2D> texSpecial = Main.LocalPlayer.Entropy().BookmarkHolderSpecialTextures;
                for (int i = 0; i < c; i++)
                {
                    Texture2D holderTexture = null;
                    Vector2 pos = Main.ScreenSize.ToVector2() / 2 + (MathHelper.ToRadians(i * (360f / c)) + slotRot).ToRotationVector2() * slotDist;
                    if (bookItem.ModItem is EntropyBook eb)
                    {
                        holderTexture = eb.BookMarkTexture;
                        if (i >= c - texSpecial.Count)
                        {
                            holderTexture = texSpecial[i - (c - texSpecial.Count)];
                        }
                        Main.spriteBatch.Draw(holderTexture, pos, null, Color.White * (closeAnm / 11f), 0, holderTexture.Size() / 2, 1, SpriteEffects.None, 0);
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
                        ChoosedBMSlot = i;
                        if (!Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir)
                        {
                            CEUtils.showItemTooltip(Main.LocalPlayer.Entropy().EBookStackItems[i]);
                        }
                        Main.LocalPlayer.mouseInterface = true;
                        if (Main.mouseLeft && !lastMouseLeft && (Main.mouseItem.IsAir || BookMarkLoader.IsABookMark(Main.mouseItem)) && !(Main.mouseItem.IsAir && Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir))
                        {
                            bool flag = true;
                            if (!Main.mouseItem.IsAir)
                            {
                                for (int h = 0; h < Math.Min(EBookUI.getMaxSlots(Main.LocalPlayer, bookItem), Main.LocalPlayer.Entropy().EBookStackItems.Count); h++)
                                {
                                    if (BookMarkLoader.IsABookMark(Main.LocalPlayer.Entropy().EBookStackItems[h]))
                                    {
                                        var bm = Main.LocalPlayer.Entropy().EBookStackItems[h];
                                        if (!BookMarkLoader.CanBeEquipWith(Main.mouseItem, bm))
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (flag)
                            {
                                lastMouseLeft = true;
                                Item mouseItem = Main.mouseItem.Clone();
                                Main.mouseItem = Main.LocalPlayer.Entropy().EBookStackItems[i];
                                Main.LocalPlayer.Entropy().EBookStackItems[i] = mouseItem;
                                CEUtils.PlaySound("turnPage");
                                sync = true;
                            }
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
                                        CEUtils.PlaySound("turnPage");
                                        sync = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (Main.mouseItem.IsAir && Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir)
                        {
                            Main.instance.MouseText(CalamityEntropy.Instance.GetLocalization("SlotInfo").Value);
                        }
                        if (Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir || (!Main.LocalPlayer.Entropy().EBookStackItems[i].IsAir && Main.mouseItem.IsAir))
                            outlineFlag = true;
                    }
                    if (outline >= 0.005f)
                    {
                        if (i == ChoosedBMSlot && shader != null)
                        {
                            Main.spriteBatch.End();
                            shader.CurrentTechnique.Passes[0].Apply();
                            shader.Parameters["texSize"].SetValue(holderTexture.Size());
                            shader.Parameters["color"].SetValue(Color.Yellow.ToVector4());
                            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, shader, Main.UIScaleMatrix);
                            Main.spriteBatch.Draw(holderTexture, pos, null, Color.White * (closeAnm / 11f) * outline, 0, holderTexture.Size() / 2, 1, SpriteEffects.None, 0);
                            Main.spriteBatch.UseBlendState_UI(BlendState.AlphaBlend);

                        }
                    }

                }
                outline = float.Lerp(outline, outlineFlag ? 1 : 0, 0.1f);
                
                lastMouseLeft = Main.mouseLeft;
                lastMouseRight = Main.mouseRight;
                if (sync && Main.netMode != NetmodeID.SinglePlayer)
                {
                    Main.LocalPlayer.Entropy().SyncBookmarks();
                }
            }
        }
        public static Effect shader = null;
    }
}