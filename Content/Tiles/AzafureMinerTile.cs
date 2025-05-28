using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Utilities;
using CalamityMod;
using InnoVault;
using InnoVault.TileProcessors;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;

namespace CalamityEntropy.Content.Tiles
{
    public class AzafureMinerTile : ModTile
    {
        public override void Load()
        {
            AzMinerUI.items = new List<AzMinerUI.UISlot>();
            AzMinerUI.filters = new List<AzMinerUI.UISlot>();
            for (int i = 0; i < AzMinerTP.FiltersCount; i++)
            {
                AzMinerUI.filters.Add(new AzMinerUI.UISlot() { pos = new Vector2(-168, -115 + (240f / AzMinerTP.FiltersCount) * i), X = 20, Y = 20, itemIndex = i });
            }
            int z = 0;
            Vector2 pos = new Vector2(-110, -100);
            for (int i = 0; i < AzMinerTP.ItemsCount; i++)
            {
                AzMinerUI.items.Add(new AzMinerUI.UISlot() { pos = new Vector2(pos.X, pos.Y), type = 1, itemIndex = i });
                z++;
                pos.X += 50;
                if (z >= 7)
                {
                    z = 0;
                    pos.Y += 50;
                    pos.X = -110;
                }
            }
        }
        public override void Unload()
        {
            AzMinerUI.filters = null;
            AzMinerUI.items = null;
        }
        public override void SetStaticDefaults()
        {
            RegisterItemDrop(ModContent.ItemType<AzafureMiner>());
            TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(2, 2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 4, 0);
            TileObjectData.addTile(Type);
            Main.tileFrameImportant[(int)base.Type] = true;
            AddMapEntry(Color.DarkGray, CalamityUtils.GetItemName<AzafureMiner>());

        }

        public override bool RightClick(int i, int j)
        {
            if (VaultUtils.SafeGetTopLeft(i, j, out var point))
            {
                TileProcessorLoader.ByPositionGetTP(point, out var tp);
                if (tp == null)
                    return false;
                AzMinerUI.tp = (AzMinerTP)tp;
                AzMinerUI.Active = true;
                Main.playerInventory = true;
                return true;
            }
            return false;
        }
    }

    public class AzMinerTP : TileProcessor
    {
        public List<Item> filters;
        public List<Item> items;
        public static int FiltersCount = 6;
        public static int ItemsCount = 35;
        public override void LoadData(TagCompound tag)
        {
            filters = new List<Item>();
            items = new List<Item>();
            if (tag.ContainsKey("items"))
            {
                TagCompound isave = tag.Get<TagCompound>("items");
                for (int i = 0; i < FiltersCount; i++)
                {
                    filters.Add(ItemIO.Load(isave.Get<TagCompound>("f" + i.ToString())));
                }
                for (int i = 0; i < ItemsCount; i++)
                {
                    items.Add(ItemIO.Load(isave.Get<TagCompound>("i" + i.ToString())));
                }
            }
            else
            {
                for (int i = 0; i < FiltersCount; i++)
                {
                    filters.Add(new Item());
                }
                for (int i = 0; i < ItemsCount; i++)
                {
                    items.Add(new Item());
                }
            }
        }
        public override void SaveData(TagCompound tag)
        {
            TagCompound itemSaves = new TagCompound();
            int c = 0;
            foreach (Item item in filters)
            {
                itemSaves.Add("f" + c.ToString(), ItemIO.Save(item));
                c++;
            }
            c = 0;
            foreach (Item item in items)
            {
                itemSaves.Add("i" + c.ToString(), ItemIO.Save(item));
                c++;
            }
            tag.Add("items", itemSaves);
        }
        public override int TargetTileID => ModContent.TileType<AzafureMinerTile>();
        public override void Update()
        {
            if (items == null || filters == null || filters.Count == 0 || items.Count == 0)
            {
                filters = new List<Item>();
                items = new List<Item>();
                for (int i = 0; i < FiltersCount; i++)
                {
                    filters.Add(new Item());
                }
                for (int i = 0; i < ItemsCount; i++)
                {
                    items.Add(new Item());
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                bool f = false;
                List<int> types = new List<int>();
                foreach (Item item in filters)
                {
                    types.Add(item.type);
                }
                for (int i = 0; i < 14; i++)
                {
                    Point p = new Point(Main.rand.Next(Main.maxTilesX), Main.rand.Next(Main.maxTilesY));
                    if (TileID.Sets.Ore[Main.tile[p.X, p.Y].TileType])
                    {
                        Tile t = Main.tile[p.X, p.Y];
                        if (method == null)
                        {
                            method = typeof(WorldGen).GetMethod("KillTile_GetItemDrops", BindingFlags.Static | BindingFlags.NonPublic,
                                null,
                                new System.Type[]
                                {
                    typeof(int), typeof(int), typeof(Tile), typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType(), typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType(), typeof(bool)
                                },
                                null);
                        }
                        int itemtype = t.GetTileDorp();
                        if (itemtype <= 0)
                        {
                            object[] parameters = new object[]
                            {
                p.X, p.Y, t, null, null, null, null, false
                            };
                            method.Invoke(null, parameters);
                            itemtype = (int)parameters[3];
                        }
                        if (!types.Contains(itemtype))
                        {
                            continue;
                        }
                        for (int x = -14; x < 15; x++)
                        {
                            for (int y = -14; y < 15; y++)
                            {
                                if (Util.inWorld(p.X + x, p.Y + y))
                                {
                                    if (TileID.Sets.Ore[Main.tile[p.X + x, p.Y + y].TileType])
                                        if (MineOre(p.X + x, p.Y + y))
                                        {
                                            f = true;
                                        }
                                }
                            }
                        }
                    }
                }
                if (f)
                {
                    this.SendData();
                }
            }
        }
        public static MethodBase method;
        public bool MineOre(int x, int y)
        {
            Tile t = Main.tile[x, y];
            if (method == null)
            {
                method = typeof(WorldGen).GetMethod("KillTile_GetItemDrops", BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new System.Type[]
                    {
                    typeof(int), typeof(int), typeof(Tile), typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType(), typeof(int).MakeByRefType(),
                    typeof(int).MakeByRefType(), typeof(bool)
                    },
                    null);
            }
            int itemtype = t.GetTileDorp();
            if (itemtype <= 0)
            {
                object[] parameters = new object[]
                {
                x, y, t, null, null, null, null, false
                };
                method.Invoke(null, parameters);
                itemtype = (int)parameters[3];
            }
            if (itemtype > 0)
            {
                foreach (var i in filters)
                {
                    if (i.type == itemtype)
                    {
                        foreach (var c in items)
                        {
                            if (c.type == itemtype && c.stack < c.maxStack)
                            {
                                c.stack++;
                                Main.tile[x, y].ClearTile();
                                if (Main.dedServ)
                                {
                                    NetMessage.SendTileSquare(-1, x, y);
                                }
                                if (Main.rand.NextBool(12))
                                {
                                    EParticle.NewParticle(new EMediumSmoke(), this.CenterInWorld + new Vector2(Main.rand.NextFloat(-24, 24), 0), new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-2, -6)), Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()), Main.rand.NextFloat(0.8f, 1.4f), 1, true, BlendState.AlphaBlend, Utilities.Util.randomRot());
                                }
                                return true;
                            }
                        }
                        foreach (var c in items)
                        {
                            if (c.IsAir)
                            {
                                c.SetDefaults(itemtype);
                                Main.tile[x, y].ClearTile();
                                if (Main.dedServ)
                                {
                                    NetMessage.SendTileSquare(-1, x, y);
                                }
                                if (Main.rand.NextBool(12))
                                {
                                    EParticle.NewParticle(new EMediumSmoke(), this.CenterInWorld + new Vector2(Main.rand.NextFloat(-24, 24), 0), new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-2, -6)), Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()), Main.rand.NextFloat(0.8f, 1.4f), 1, true, BlendState.AlphaBlend, Utilities.Util.randomRot());
                                }
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public override void SendData(ModPacket data)
        {
            for (int i = 0; i < FiltersCount; i++)
            {
                Item item = filters[i];
                data.Write(item.type);
                data.Write7BitEncodedInt(item.netID);
                data.Write7BitEncodedInt(item.stack);
            }
            for (int i = 0; i < ItemsCount; i++)
            {
                Item item = items[i];
                data.Write(item.type);
                data.Write7BitEncodedInt(item.netID);
                data.Write7BitEncodedInt(item.stack);
            }
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            for (int i = 0; i < FiltersCount; i++)
            {
                Item item = new Item(reader.ReadInt32());
                item.netDefaults(reader.Read7BitEncodedInt());
                item.stack = reader.Read7BitEncodedInt();
                filters[i] = item;
            }
            for (int i = 0; i < ItemsCount; i++)
            {
                Item item = new Item(reader.ReadInt32());
                item.netDefaults(reader.Read7BitEncodedInt());
                item.stack = reader.Read7BitEncodedInt();
                items[i] = item;
            }
        }
    }

    public static class AzMinerUI
    {
        [VaultLoaden("CalamityEntropy/Assets/UI/Miner/AzafureMinerUI")]
        private static Asset<Texture2D> UIBarTex;
        [VaultLoaden("CalamityEntropy/Assets/UI/Miner/UISlot")]
        private static Asset<Texture2D> UISlotTex;
        [VaultLoaden("CalamityEntropy/Assets/UI/Miner/UISlot2")]
        private static Asset<Texture2D> UISlotFilterTex;
        public static bool Active = false;
        public static AzMinerTP tp = null;
        public static List<UISlot> filters;
        public static List<UISlot> items;
        public static Vector2 baseCenter = Main.ScreenSize.ToVector2() / 2f;

        public static void Update()
        {
            if (!Active || tp == null)
            {
                return;
            }
            if (!Main.playerInventory || tp.CenterInWorld.Distance(Main.LocalPlayer.Center) > 20 * 10)
            {
                Active = false;
                return;
            }
            foreach (UISlot s in filters)
            {
                s.Update();
            }
            foreach (UISlot s in items)
            {
                s.Update();
            }

        }
        public static void Draw()
        {
            if (!Active || tp == null)
            {
                return;
            }
            Main.spriteBatch.Draw(UIBarTex.Value, baseCenter, null, Color.White, 0, UIBarTex.Value.Size() / 2f, 1, SpriteEffects.None, 0);
            foreach (var slot in filters)
            {
                Texture2D tex = UISlotFilterTex.Value;
                Main.spriteBatch.Draw(tex, baseCenter + slot.pos, null, Color.White, 0, tex.Size() / 2f, 1, SpriteEffects.None, 0);
                if (slot.getItem().IsAir)
                {
                    continue;
                }
                slot.CheckHover();
                ItemSlot.DrawItemIcon(slot.getItem(), 1, Main.spriteBatch, slot.pos + baseCenter, 1, 128, Color.White);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, slot.getItem().stack.ToString(), slot.pos + baseCenter + new Vector2(-15, 6), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            }
            foreach (var slot in items)
            {
                Texture2D tex = UISlotTex.Value;
                Main.spriteBatch.Draw(tex, baseCenter + slot.pos, null, Color.White, 0, tex.Size() / 2f, 1, SpriteEffects.None, 0);
                if (slot.getItem().IsAir)
                {
                    continue;
                }
                slot.CheckHover();
                ItemSlot.DrawItemIcon(slot.getItem(), 1, Main.spriteBatch, slot.pos + baseCenter, 1, 128, Color.White);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, slot.getItem().stack.ToString(), slot.pos + baseCenter + new Vector2(-15, 6), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            }
        }
        public class UISlot
        {
            public int itemIndex = 0;
            public int type = 0;
            public Vector2 pos = new Vector2();
            public int X = 20;
            public int Y = 20;
            public bool mlLast = false;
            public Rectangle GetRect()
            {
                return (baseCenter + pos).getRectCentered(X * 2, Y * 2);
            }
            public Item getItem()
            {
                if (type == 0)
                {
                    return tp.filters[itemIndex];
                }
                else
                {
                    return tp.items[itemIndex];
                }
            }
            public void CheckHover()
            {
                if (Main.MouseScreen.getRectCentered(1, 1).Intersects(this.GetRect()))
                {
                    if (Main.mouseItem.IsAir && !getItem().IsAir)
                    {
                        Util.showItemTooltip(getItem());
                    }
                }
            }
            public void Update()
            {
                if (Main.MouseScreen.getRectCentered(1, 1).Intersects(this.GetRect()))
                {
                    Main.LocalPlayer.mouseInterface = true;
                    if (getItem().IsAir && type == 0 && Main.mouseItem.IsAir)
                    {
                        Main.instance.MouseText(CalamityEntropy.Instance.GetLocalization("SlotInfo2").Value);
                    }
                    if (!(Main.mouseItem.IsAir && getItem().IsAir))
                    {
                        if (!mlLast && Main.mouseLeft)
                        {
                            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                            {
                                SoundEngine.PlaySound(SoundID.Grab);
                                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Loot(), getItem(), getItem().stack);
                                if (type == 0)
                                {
                                    tp.filters[itemIndex].TurnToAir();
                                }
                                else
                                {
                                    tp.items[itemIndex].TurnToAir();
                                }

                            }
                            else
                            {
                                if (Main.mouseItem.type == getItem().type)
                                {
                                    SoundEngine.PlaySound(SoundID.Grab);
                                    if (type == 0)
                                    {
                                        tp.filters[itemIndex].stack += Main.mouseItem.stack;
                                    }
                                    else
                                    {
                                        tp.items[itemIndex].stack += Main.mouseItem.stack;
                                    }
                                    Main.mouseItem.TurnToAir();
                                }
                                else
                                {

                                    SoundEngine.PlaySound(SoundID.Grab);
                                    Item mouse = Main.mouseItem.Clone();
                                    if (type == 0)
                                    {
                                        Main.mouseItem = tp.filters[itemIndex].Clone();
                                        tp.filters[itemIndex] = mouse;
                                    }
                                    else
                                    {
                                        Main.mouseItem = tp.items[itemIndex].Clone();
                                        tp.items[itemIndex] = mouse;
                                    }
                                }
                            }
                            tp.SendData();
                        }
                    }
                }
                mlLast = Main.mouseLeft;
            }
        }

    }
}
