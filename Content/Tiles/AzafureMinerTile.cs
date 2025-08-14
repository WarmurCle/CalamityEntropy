using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Particles;
using InnoVault;
using InnoVault.TileProcessors;
using InnoVault.UIHandles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
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
            AzMinerUI.Items = new List<AzMinerUISlot>();
            AzMinerUI.Filters = new List<AzMinerUISlot>();
            for (int i = 0; i < AzMinerTP.FiltersCount; i++)
            {
                AzMinerUI.Filters.Add(new AzMinerUISlot() { OffsetPos = new Vector2(-168, -115 + (240f / AzMinerTP.FiltersCount) * i), X = 20, Y = 20, itemIndex = i });
            }
            int z = 0;
            Vector2 pos = new Vector2(-110, -100);
            for (int i = 0; i < AzMinerTP.ItemsCount; i++)
            {
                AzMinerUI.Items.Add(new AzMinerUISlot() { OffsetPos = new Vector2(pos.X, pos.Y), type = 1, itemIndex = i });
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
            AzMinerUI.Filters = null;
            AzMinerUI.Items = null;
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
            if (TileProcessorLoader.AutoPositionGetTP<AzMinerTP>(i, j, out var azMinerTP))
            {
                if (AzMinerUI.AzMinerTP == azMinerTP)
                {//相同，说明是点击同一个建筑，进行开关逻辑
                    AzMinerUI.Instance.Active = !AzMinerUI.Instance.Active;
                }
                else
                {//不相同，说明是交叉点击不同建筑，进行切换逻辑保持开启状态
                    AzMinerUI.Instance.Active = true;
                }

                AzMinerUI.AzMinerTP = azMinerTP;
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

                    ItemIsOre.Add(i, oreTileIDs.Contains(item.createTile) || Common.EGlobalItem.GemItemIDToTileIDMap.ContainsKey(i));
                }
            }
            catch (System.Exception ex)
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
                    VaultUtils.SpwanItem(this.FromObjectGetParent(), HitBox, item.Clone());
                }
            }
            foreach (var item in items)
            {
                if (!item.IsAir)
                {
                    VaultUtils.SpwanItem(this.FromObjectGetParent(), HitBox, item.Clone());
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
            foreach (Item item in filters)
            {
                if (item.type == ItemID.None)
                {
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
                //   continue;//这个判定目前是不需要的，因为types里面的肯定都是矿物物品
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
            Tile tile = Main.tile[x, y];
            int itemType = tile.GetTileDrop(x, y);
            if (itemType <= 0)
            {
                return false;
            }

            if (EGlobalItem.GemTileIDToItemIDMap.TryGetValue(tile.TileType, out int dropID))
            {
                itemType = dropID;
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

        ///<summary>
        ///执行采矿后通用的视觉/音效/网络同步
        ///</summary>
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
        internal static Asset<Texture2D> UIBarTex;
        [VaultLoaden("CalamityEntropy/Assets/UI/Miner/UISlot")]
        internal static Asset<Texture2D> UISlotTex;
        public static AzMinerUI Instance => UIHandleLoader.GetUIHandleOfType<AzMinerUI>();
        public static AzMinerTP AzMinerTP { get; set; } = null;
        public static List<AzMinerUISlot> Filters { get; set; }
        public static List<AzMinerUISlot> Items { get; set; }
        private float hoverSengs;
        internal int dontDragTime;
        private bool onDrag;
        private Vector2 dragOffset;
        internal static float sengs = 0;
        private static bool IsActive;
        public override bool Active {
            get {
                if (AzMinerTP == null || !AzMinerTP.Active) {
                    IsActive = false;
                }
                return IsActive || sengs > 0;
            }
            set => IsActive = value;
        }
        public override void Update()
        {
            if (dontDragTime > 0)
            {
                dontDragTime--;
            }

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

            if (!hoverInMainPage) {
                if (hoverSengs > 0f) {
                    hoverSengs -= 0.1f;
                }
            }
            else if (hoverSengs < 1f) {
                hoverSengs += 0.1f;
            }

            sengs = MathHelper.Clamp(sengs, 0f, 1f);
            hoverSengs = MathHelper.Clamp(hoverSengs, 0f, 1f);

            if (AzMinerTP == null)
            {
                return;
            }

            if (sengs < 1f)
            {
                DrawPosition = Vector2.Lerp(DrawPosition, new Vector2(UIBarTex.Width() * sengs, Main.screenHeight / 2), 0.2f);
            }

            UIHitBox = (DrawPosition - UIBarTex.Size() / 2).GetRectangle(UIBarTex.Size());
            hoverInMainPage = UIHitBox.Intersects(MouseHitBox);

            if (!Main.playerInventory || AzMinerTP.CenterInWorld.Distance(Main.LocalPlayer.Center) > 20 * 10)
            {
                IsActive = false;
                return;
            }

            if (onDrag) {
                player.mouseInterface = true;
                DrawPosition = MousePosition + dragOffset;

                if (keyLeftPressState == KeyPressState.Released) {
                    onDrag = false;
                }
            }

            //限制 UI 位置不超出屏幕
            Vector2 halfSize = UIBarTex.Size() / 2f;
            DrawPosition.X = MathHelper.Clamp(DrawPosition.X, halfSize.X, Main.screenWidth - halfSize.X);
            DrawPosition.Y = MathHelper.Clamp(DrawPosition.Y, halfSize.Y, Main.screenHeight - halfSize.Y);

            foreach (AzMinerUISlot s in Filters)
            {
                s.Update();
            }
            foreach (AzMinerUISlot s in Items)
            {
                s.Update();
            }

            if (hoverInMainPage) {
                player.mouseInterface = true;
                if (keyLeftPressState == KeyPressState.Pressed) {
                    if (!onDrag) {
                        dragOffset = DrawPosition - MousePosition;
                    }

                    onDrag = dontDragTime <= 0;
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            if (AzMinerTP == null)
            {
                return;
            }

            Color drawColor = Color.White * sengs;
            //看起来不怎么好看，所以取消掉
            //Main.spriteBatch.Draw(UIBarTex.Value, DrawPosition, null, Color.Gold with { A = 0 }, 0
            //   , UIBarTex.Value.Size() / 2f, sengs + 0.01f * hoverSengs, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(UIBarTex.Value, DrawPosition, null, drawColor, 0, UIBarTex.Value.Size() / 2f, sengs, SpriteEffects.None, 0);
            foreach (var slot in Filters)
            {
                slot.Draw(spriteBatch);
            }
            foreach (var slot in Items)
            {
                slot.Draw(spriteBatch);
            }
        }
    }

    public class AzMinerUISlot : UIHandle
    {
        public override LayersModeEnum LayersMode => LayersModeEnum.None;
        public int itemIndex = 0;
        public int type = 0;
        public Vector2 OffsetPos = new Vector2();
        public int X = 20;
        public int Y = 20;
        public bool mlLast = false;
        private float hoverSengs;
        private bool hoverRightHeld;
        private int lastHoverSlot = -1;
        public Item Item {
            get {
                if (type == 0) {
                    return AzMinerUI.AzMinerTP.filters[itemIndex];
                }
                else {
                    return AzMinerUI.AzMinerTP.items[itemIndex];
                }
            }
            set {
                if (type == 0) {
                    AzMinerUI.AzMinerTP.filters[itemIndex] = value;
                }
                else {
                    AzMinerUI.AzMinerTP.items[itemIndex] = value;
                }
            }
        }

        public override void Update() {
            //更新位置和鼠标交互区域
            DrawPosition = AzMinerUI.Instance.DrawPosition + OffsetPos;
            UIHitBox = (DrawPosition - AzMinerUI.UISlotTex.Size() / 2)
                .GetRectangle(AzMinerUI.UISlotTex.Size());

            hoverInMainPage = UIHitBox.Intersects(MouseHitBox);

            if (!hoverInMainPage) {
                hoverSengs = Math.Max(0f, hoverSengs - 0.1f);
                hoverRightHeld = false;
                return;
            }
            hoverSengs = Math.Min(1f, hoverSengs + 0.1f);

            Main.LocalPlayer.mouseInterface = true;

            //显示物品信息
            if (Main.mouseItem.IsAir && !Item.IsAir) {
                CEUtils.showItemTooltip(Item);
            }

            //空槽提示
            if (Item.IsAir && type == 0 && Main.mouseItem.IsAir) {
                Main.instance.MouseText(CalamityEntropy.Instance.GetLocalization("SlotInfo2").Value);
            }

            //Shift合并
            if (!Item.IsAir && Main.keyState.PressingShift()) {
                TryMergeStacks();
            }

            //空物品格直接退出
            if (Main.mouseItem.IsAir && Item.IsAir) {
                return;
            }

            //鼠标右键操作（滑动加堆叠 + 单击）
            HandleRightClick();

            //鼠标左键操作
            HandleLeftClick();

            AzMinerUI.AzMinerTP.SendData();
        }

        private void HandleRightClick() {
            int currentSlot = itemIndex; //当前格子索引

            //如果滑动到了新的格子，重置去抖状态
            if (currentSlot != lastHoverSlot) {
                hoverRightHeld = false;
                lastHoverSlot = currentSlot;
            }

            if (Main.mouseItem.IsAir) {
                return;
            }

            bool canAdd = Item.IsAir || (Item.type == Main.mouseItem.type && Item.stack < Item.maxStack && Main.mouseItem.stack > 0);

            if (!canAdd) {
                return;
            }

            if (keyRightPressState == KeyPressState.Held && hoverInMainPage) {
                if (!hoverRightHeld) {
                    AddOneFromMouse();
                    hoverRightHeld = true;
                }
            }
            else if (keyRightPressState == KeyPressState.Pressed) {
                AddOneFromMouse();
                hoverRightHeld = true;
            }
        }

        private void HandleLeftClick() {
            if (keyLeftPressState != KeyPressState.Pressed)
                return;

            AzMinerUI.Instance.dontDragTime = 2;

            //Shift 丢出物品
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) {
                SoundEngine.PlaySound(SoundID.Grab);
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Loot(), Item, Item.stack);
                ClearSlot();
                return;
            }

            //左键正常堆叠/交换
            if (Main.mouseItem.type == Item.type) {
                MergeWithMouse();
            }
            else {
                //只能放矿石
                if (!IsOre(Main.mouseItem) && Main.mouseItem.type != ItemID.None) {
                    SoundEngine.PlaySound(SoundID.MenuTick);
                    Main.NewText(CalamityEntropy.Instance.GetLocalization("AzMinerOreWarning").Value, Color.Red);
                    return;
                }
                SwapWithMouse();
            }
        }

        private void AddOneFromMouse() {
            SoundEngine.PlaySound(SoundID.Grab);
            if (Item.IsAir) {
                Item = Main.mouseItem.Clone();
                Item.stack = 1;
            }
            else {
                Item.stack++;
            }

            Main.mouseItem.stack--;
            if (Main.mouseItem.stack <= 0)
                Main.mouseItem.TurnToAir();
        }

        private void MergeWithMouse() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item targetSlot = GetSlotRef();
            int total = targetSlot.stack + Main.mouseItem.stack;

            if (total > targetSlot.maxStack) {
                int overflow = total - targetSlot.maxStack;
                targetSlot.stack = targetSlot.maxStack;
                Main.mouseItem.stack = overflow;
            }
            else {
                targetSlot.stack = total;
                Main.mouseItem.TurnToAir();
            }
        }

        private void SwapWithMouse() {
            SoundEngine.PlaySound(SoundID.Grab);
            Item targetSlot = GetSlotRef();

            Item temp = targetSlot.Clone();
            targetSlot.SetDefaults(Main.mouseItem.type);
            targetSlot.stack = Main.mouseItem.stack;

            Main.mouseItem = temp;
        }

        private void TryMergeStacks() {
            bool merged = false;
            foreach (var slot in AzMinerUI.Items) {
                if (slot.itemIndex == itemIndex || Item.type != slot.Item.type)
                    continue;

                if (Item.stack >= Item.maxStack)
                    continue;

                int total = Item.stack + slot.Item.stack;
                if (total > Item.maxStack) {
                    Item.stack = Item.maxStack;
                    slot.Item.stack = total - Item.maxStack;
                }
                else {
                    Item.stack = total;
                    slot.Item.TurnToAir();
                }
                merged = true;
            }

            if (merged)
                SoundEngine.PlaySound(SoundID.Grab);
        }

        private void ClearSlot() {
            if (type == 0) {
                AzMinerUI.AzMinerTP.filters[itemIndex].TurnToAir();
            } 
            else {
                AzMinerUI.AzMinerTP.items[itemIndex].TurnToAir();
            }
                
        }

        private Item GetSlotRef() {
            return type == 0
                ? AzMinerUI.AzMinerTP.filters[itemIndex]
                : AzMinerUI.AzMinerTP.items[itemIndex];
        }

        private bool IsOre(Item item) {
            return AzMinerTP.ItemIsOre.TryGetValue(item.type, out bool isOre) && isOre;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D tex = AzMinerUI.UISlotTex.Value;
            if (AzMinerUI.sengs >= 1f) {
                Main.spriteBatch.Draw(tex, DrawPosition, null, Color.Gold with { A = 0 }, 0, tex.Size() / 2f, 1 + 0.1f * hoverSengs, SpriteEffects.None, 0);
            }
            
            Main.spriteBatch.Draw(tex, DrawPosition, null, Color.White * AzMinerUI.sengs, 0, tex.Size() / 2f, AzMinerUI.sengs, SpriteEffects.None, 0);
            if (Item.IsAir) {
                return;
            }

            ItemSlot.DrawItemIcon(Item, 1, Main.spriteBatch, DrawPosition, 1 + 0.2f * hoverSengs, 128, Color.White * AzMinerUI.sengs);
            Main.spriteBatch.DrawString(FontAssets.MouseText.Value, Item.stack.ToString(), DrawPosition + new Vector2(-15, 6), Color.White * AzMinerUI.sengs, 0, Vector2.Zero, 0.7f + 0.2f * hoverSengs, SpriteEffects.None, 0);
        }
    }
}
