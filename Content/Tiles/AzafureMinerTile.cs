using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Particles;
using InnoVault;
using InnoVault.TileProcessors;
using InnoVault.UIHandles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileWaterDeath[Type] = false;

            AddMapEntry(new Color(190, 72, 81), VaultUtils.GetLocalizedItemName<AzafureMiner>());

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(2, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(
                AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide
                , TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.LavaDeath = false;

            TileObjectData.addTile(Type);
        }

        public override void MouseOver(int i, int j)
        {
            Main.LocalPlayer.SetMouseOverByTile<AzafureMiner>();
        }

        public override bool RightClick(int i, int j)
        {
            if (VaultUtils.SafeGetTopLeft(i, j, out var point))
            {
                TileProcessorLoader.ByPositionGetTP(point, out var tp);
                if (tp == null)
                    return false;
                AzMinerUI.tp = (AzMinerTP)tp;
                UIHandleLoader.GetUIHandleOfType<AzMinerUI>().Active = !UIHandleLoader.GetUIHandleOfType<AzMinerUI>().Active;
                Main.playerInventory = true;
                SoundEngine.PlaySound(SoundID.MenuOpen with { Pitch = 0.3f });
                return true;
            }
            return false;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!VaultUtils.SafeGetTopLeft(i, j, out var point))
            {
                return false;
            }
            if (!TileProcessorLoader.ByPositionGetTP(point, out AzMinerTP azMinerTP))
            {
                return false;
            }

            Tile t = Main.tile[i, j];
            int frameXPos = t.TileFrameX;
            int frameYPos = t.TileFrameY;
            Texture2D tex = TextureAssets.Tile[Type].Value;
            Vector2 offset = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange) + azMinerTP.OffsetPos;
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + offset;
            Color drawColor = Lighting.GetColor(i, j);
            if (!azMinerTP.IsWork)
            {
                drawColor.R /= 2;
                drawColor.G /= 2;
                drawColor.B /= 2;
                drawColor.A = 255;
            }

            if (!t.IsHalfBlock && t.Slope == 0)
            {
                spriteBatch.Draw(tex, drawOffset, new Rectangle(frameXPos, frameYPos, 16, 16)
                    , drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            else if (t.IsHalfBlock)
            {
                spriteBatch.Draw(tex, drawOffset + Vector2.UnitY * 8f, new Rectangle(frameXPos, frameYPos, 16, 16)
                    , drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
            }
            return false;
        }
    }

    public class AzMinerTP : TileProcessor
    {
        public override int TargetTileID => ModContent.TileType<AzafureMinerTile>();
        public List<Item> filters;
        public List<Item> items;
        public static int FiltersCount = 6;
        public static int ItemsCount = 35;
        public bool IsWork;
        public Vector2 OffsetPos;
        private Vector2 currentOffset = Vector2.Zero;
        private Vector2 targetOffset = Vector2.Zero;
        private Vector2 targetOffset2 = Vector2.Zero;
        public static readonly Dictionary<int, bool> ItemIsOre = [];
        public readonly static HashSet<int> gemIDs = [ItemID.Ruby, ItemID.Sapphire, ItemID.Diamond, ItemID.Emerald, ItemID.Topaz, ItemID.Amethyst];
        public override void SetStaticProperty()
        {
            try
            {
                HashSet<int> oreTileIDs = [];
                for (int i = 0; i < TileLoader.TileCount; i++)
                {
                    if (!TileID.Sets.Ore[i])
                    {
                        continue;
                    }
                    oreTileIDs.Add(i);
                }

                for (int i = 0; i < ItemLoader.ItemCount; i++)
                {
                    Item item = new Item(i);
                    if (item.type == ItemID.None)
                    {
                        continue;
                    }

                    ItemIsOre.Add(i, oreTileIDs.Contains(item.createTile) || gemIDs.Contains(i));
                }
            }
            catch(System.Exception ex)
            {
                CalamityEntropy.Instance.Logger.Error($"AzMinerTP.SetStaticProperty: An Error Has Occurred {ex.Message}");
            }
        }
        public override void SetProperty()
        {
            IsWork = true;
            filters = [];
            items = [];
            for (int i = 0; i < FiltersCount; i++)
            {
                filters.Add(new Item());
            }
            for (int i = 0; i < ItemsCount; i++)
            {
                items.Add(new Item());
            }
        }

        public override void LoadData(TagCompound tag)
        {
            filters = [];
            items = [];
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
            TagCompound itemSaves = [];
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

        public override void OnKill()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;
            foreach (var item in filters)
            {
                if (!item.IsAir)
                {
                    Item.NewItem(Item.GetSource_None(), this.CenterInWorld.getRectCentered(40, 40), item.Clone(), noGrabDelay: true);
                }
            }
            foreach (var item in items)
            {
                if (!item.IsAir)
                {
                    Item.NewItem(Item.GetSource_None(), this.CenterInWorld.getRectCentered(40, 40), item.Clone(), noGrabDelay: true);
                }
            }
        }

        public override void Update()
        {
            if (IsWork)
            {
                //随机目标抖动点，范围 ±1.5
                targetOffset = targetOffset2 + new Vector2(
                    Main.rand.NextFloat(-8f, 8f),
                    Main.rand.NextFloat(-4f, 10f)
                );

                //让 currentOffset 缓慢向 targetOffset 逼近，产生平滑抖动效果
                currentOffset = Vector2.Lerp(currentOffset, targetOffset, 0.1f);
            }
            else
            {
                targetOffset2 = Vector2.Zero;
                //非工作时平滑回到0
                currentOffset = Vector2.Lerp(currentOffset, Vector2.Zero, 0.1f);
            }

            targetOffset2 *= 0.6f;
            OffsetPos = new Vector2((int)currentOffset.X, (int)currentOffset.Y);

            List<int> types = [];
            IsWork = false;
            foreach (Item item in filters) {
                if (item.type == ItemID.None) {
                    continue;
                }
                types.Add(item.type);
                IsWork = true;
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            bool didMine = false;

            for (int i = 0; i < 3; i++)
            {
                Point16 p = new Point16(Main.rand.Next(Main.maxTilesX), Main.rand.Next(Main.maxTilesY));

                Tile t = Main.tile[p.X, p.Y];
                if (!t.HasTile)
                {
                    continue;
                }

                int itemtype = t.GetTileDrop(p.X, p.Y);

                //if (!ItemIsOre.ContainsKey(itemtype))
                //{
                //    continue;//这个判定目前是不需要的，因为types里面的肯定都是矿物物品
                //}
                if (!types.Contains(itemtype))
                {
                    continue;
                }

                for (int x = -14; x < 15; x++)
                {
                    for (int y = -14; y < 15; y++)
                    {
                        if (!CEUtils.inWorld(p.X + x, p.Y + y))
                        {
                            continue;
                        }

                        if (!ItemIsOre.ContainsKey(Main.tile[p.X + x, p.Y + y].GetTileDrop(p.X + x, p.Y + y)))
                        {
                            continue;
                        }

                        if (!MineOre(p.X + x, p.Y + y))
                        {
                            continue;
                        }
                        didMine = true;
                    }
                }
            }

            if (!didMine)
            {
                return;
            }

            SendData();
        }

        public bool MineOre(int x, int y)
        {
            Tile t = Main.tile[x, y];
            int itemType = t.GetTileDrop(x, y);
            if (itemType <= 0)
            {
                return false;
            }

            //先检查矿石是否在过滤器里
            bool matchesFilter = filters.Any(f => f.type == itemType);
            if (!matchesFilter)
            {
                return false;
            }

            //先尝试堆叠已有物品
            foreach (var slot in items)
            {
                if (slot.type != itemType || slot.stack >= slot.maxStack)
                {
                    continue;
                }
                slot.stack++;
                PerformMineEffects(x, y);
                return true;
            }

            //再尝试放到空格
            foreach (var slot in items)
            {
                if (!slot.IsAir)
                {
                    continue;
                }
                slot.SetDefaults(itemType);
                PerformMineEffects(x, y);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 执行采矿后通用的视觉/音效/网络同步
        /// </summary>
        private void PerformMineEffects(int x, int y)
        {
            Main.tile[x, y].ClearTile();
            if (Main.dedServ)
            {
                NetMessage.SendTileSquare(-1, x, y);
            }


            if (!Main.rand.NextBool(12))
            {
                return;
            }
            //粒子效果
            EParticle.NewParticle(
                new EMediumSmoke(),
                this.CenterInWorld + new Vector2(Main.rand.NextFloat(-24, 24), 0),
                new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-2, -6)),
                Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()),
                Main.rand.NextFloat(0.8f, 1.4f),
                1,
                true,
                BlendState.AlphaBlend,
                CEUtils.randomRot()
            );
            //淫叫
            SoundEngine.PlaySound(SoundID.Tink with { PitchRange = (-0.1f, 2f) }, CenterInWorld);

            if (targetOffset2.Length() > 1f) 
            {
                return;
            }
            //蹦跶一下
            targetOffset2 += new Vector2(0, -116);
        }
        public override void SendData(ModPacket data)
        {
            data.Write(IsWork);
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
            IsWork = reader.ReadBoolean();
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

    public class AzMinerUI : UIHandle
    {
        [VaultLoaden("CalamityEntropy/Assets/UI/Miner/AzafureMinerUI")]
        private static Asset<Texture2D> UIBarTex;
        [VaultLoaden("CalamityEntropy/Assets/UI/Miner/UISlot")]
        private static Asset<Texture2D> UISlotTex;
        [VaultLoaden("CalamityEntropy/Assets/UI/Miner/UISlot2")]
        private static Asset<Texture2D> UISlotFilterTex;
        private static bool IsActive;
        public override bool Active
        {
            get
            {
                return IsActive || sengs > 0;
            }
            set => IsActive = value;
        }
        public static AzMinerTP tp = null;
        public static List<UISlot> filters;
        public static List<UISlot> items;
        private bool onDrag;
        private bool onTopDarg;
        private Vector2 dragOffset;
        private static float sengs = 0;
        public override void Update()
        {
            if (!IsActive)
            {
                if (sengs > 0f)
                {
                    sengs -= 0.1f;
                }
                else
                {
                    return;
                }
            }
            else if (sengs < 1f)
            {
                sengs += 0.1f;
            }

            sengs = MathHelper.Clamp(sengs, 0f, 1f);

            if (tp == null)
            {
                return;
            }

            if (sengs < 1f)
            {
                DrawPosition = Vector2.Lerp(DrawPosition, new Vector2(UIBarTex.Width() * sengs, Main.screenHeight / 2), 0.2f);
            }

            UIHitBox = (DrawPosition - UIBarTex.Size() / 2).GetRectangle(UIBarTex.Size());
            hoverInMainPage = UIHitBox.Intersects(MouseHitBox);
            if (hoverInMainPage)
            {
                player.mouseInterface = true;
                if (keyLeftPressState == KeyPressState.Held && !onTopDarg)
                {
                    if (!onDrag)
                    {
                        dragOffset = DrawPosition - MousePosition;
                    }
                    onDrag = true;
                }
            }

            if (onDrag)
            {
                player.mouseInterface = true;
                DrawPosition = MousePosition + dragOffset;

                //限制 UI 位置不超出屏幕
                Vector2 halfSize = UIBarTex.Size() / 2f;
                DrawPosition.X = MathHelper.Clamp(DrawPosition.X, halfSize.X, Main.screenWidth - halfSize.X);
                DrawPosition.Y = MathHelper.Clamp(DrawPosition.Y, halfSize.Y, Main.screenHeight - halfSize.Y);

                if (keyLeftPressState == KeyPressState.Released || onTopDarg)
                {
                    onDrag = false;
                }
            }

            if (!Main.playerInventory || tp.CenterInWorld.Distance(Main.LocalPlayer.Center) > 20 * 10)
            {
                IsActive = false;
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
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            if (tp == null)
            {
                return;
            }

            Color drawColor = Color.White * sengs;

            Main.spriteBatch.Draw(UIBarTex.Value, DrawPosition, null, drawColor, 0, UIBarTex.Value.Size() / 2f, sengs, SpriteEffects.None, 0);
            foreach (var slot in filters)
            {
                Texture2D tex = UISlotFilterTex.Value;
                Main.spriteBatch.Draw(tex, DrawPosition + slot.pos * sengs, null, drawColor, 0, tex.Size() / 2f, 1, SpriteEffects.None, 0);
                if (slot.GetItem().IsAir)
                {
                    continue;
                }
                slot.CheckHover();
                ItemSlot.DrawItemIcon(slot.GetItem(), 1, Main.spriteBatch, slot.pos * sengs + DrawPosition, 1, 128, drawColor);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, slot.GetItem().stack.ToString(), slot.pos + DrawPosition + new Vector2(-15, 6), drawColor, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
            }
            foreach (var slot in items)
            {
                Texture2D tex = UISlotTex.Value;
                Main.spriteBatch.Draw(tex, DrawPosition + slot.pos, null, drawColor, 0, tex.Size() / 2f, 1, SpriteEffects.None, 0);
                if (slot.GetItem().IsAir)
                {
                    continue;
                }
                slot.CheckHover();
                ItemSlot.DrawItemIcon(slot.GetItem(), 1, Main.spriteBatch, slot.pos + DrawPosition, 1, 128, drawColor);
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, slot.GetItem().stack.ToString(), slot.pos + DrawPosition + new Vector2(-15, 6), drawColor, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
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
            public Rectangle GetRect() => (UIHandleLoader.GetUIHandleOfType<AzMinerUI>().DrawPosition + pos).getRectCentered(X * 2, Y * 2);
            public Item GetItem()
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
                    if (Main.mouseItem.IsAir && !GetItem().IsAir)
                    {
                        CEUtils.showItemTooltip(GetItem());
                    }
                }
            }
            private void DoUpdate()
            {
                if (!Main.MouseScreen.getRectCentered(1, 1).Intersects(this.GetRect()))
                {
                    return;
                }
                Main.LocalPlayer.mouseInterface = true;
                if (GetItem().IsAir && type == 0 && Main.mouseItem.IsAir)
                {
                    Main.instance.MouseText(CalamityEntropy.Instance.GetLocalization("SlotInfo2").Value);
                }
                if (Main.mouseItem.IsAir && GetItem().IsAir)
                {
                    return;
                }
                if (mlLast || !Main.mouseLeft)
                {
                    return;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    SoundEngine.PlaySound(SoundID.Grab);
                    Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Loot(), GetItem(), GetItem().stack);
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
                    bool setMouseAir = true;
                    if (Main.mouseItem.type == GetItem().type)
                    {
                        SoundEngine.PlaySound(SoundID.Grab);

                        Item targetSlot = (type == 0) ? tp.filters[itemIndex] : tp.items[itemIndex];

                        int total = targetSlot.stack + Main.mouseItem.stack;
                        if (total > targetSlot.maxStack)
                        {
                            // 计算多余的数量
                            int overflow = total - targetSlot.maxStack;

                            // 填满当前槽位
                            targetSlot.stack = targetSlot.maxStack;
                            setMouseAir = false;
                            Main.mouseItem.stack = overflow;
                        }
                        else
                        {
                            // 正常合并
                            targetSlot.stack = total;
                        }

                        if (setMouseAir)
                        {
                            // 清空鼠标
                            Main.mouseItem.TurnToAir();
                        }
                    }
                    else
                    {
                        // 判断鼠标物品是否为矿石
                        bool mouseIsOre = AzMinerTP.ItemIsOre.TryGetValue(Main.mouseItem.type, out bool isOre) && isOre;

                        if (!mouseIsOre && Main.mouseItem.type != ItemID.None)
                        {
                            SoundEngine.PlaySound(SoundID.MenuTick);
                            Main.NewText(CalamityEntropy.Instance.GetLocalization("AzMinerOreWarning").Value, Color.Red);
                            return;
                        }

                        // 如果直接替换，先判断目标槽位是否有物品并检查堆叠逻辑
                        SoundEngine.PlaySound(SoundID.Grab);
                        Item mouse = Main.mouseItem.Clone();

                        Item targetSlot;
                        if (type == 0)
                        {
                            targetSlot = tp.filters[itemIndex];
                            Main.mouseItem = targetSlot.Clone();
                            tp.filters[itemIndex] = mouse;
                        }
                        else
                        {
                            targetSlot = tp.items[itemIndex];
                            Main.mouseItem = targetSlot.Clone();
                            tp.items[itemIndex] = mouse;
                        }

                        // 检查替换物品是否超过堆叠上限（极端情况）
                        if (targetSlot.stack > targetSlot.maxStack)
                        {
                            int overflow = targetSlot.stack - targetSlot.maxStack;
                            targetSlot.stack = targetSlot.maxStack;

                            Main.mouseItem.stack = overflow;
                        }
                    }
                }
                tp.SendData();
            }
            public void Update()
            {
                DoUpdate();
                mlLast = Main.mouseLeft;
            }
        }
    }
}
