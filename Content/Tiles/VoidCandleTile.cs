using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityEntropy.Content.Tiles
{
    public class VoidCandleTile : ModTile
    {
        public static readonly SoundStyle ActivationSound = new("CalamityMod/Sounds/Item/LouderPhantomPhoenix2");

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(180, 40, 255), CalamityUtils.GetItemName<VoidCandle>());
            TileID.Sets.HasOutlines[Type] = false;
            AnimationFrameHeight = 18;
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool RightClick(int i, int j)
        {
            Player p = Main.LocalPlayer;

            p.AddBuff(ModContent.BuffType<VoidCandleBuff>(), 108000);

            SoundEngine.PlaySound(ActivationSound, new Vector2(i * 16, j * 16));

            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<VoidCandle>();
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter >= 6)
            {
                frame = (frame + 1) % 4;
                frameCounter = 0;
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.55f;
            g = 0.1f;
            b = 0.8f;
        }
    }
}
