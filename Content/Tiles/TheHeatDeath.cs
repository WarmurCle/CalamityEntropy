using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.PrefixItem;
using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityEntropy.Content.Tiles
{
    public class TheHeatDeath : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileLighted[base.Type] = true;
            Main.tileSpelunker[base.Type] = true;
            base.MineResist = 6f;
            base.HitSound = AuricOre.MineSound;
            AddMapEntry(new Color(150, 0, 0));
            RegisterItemDrop(ModContent.ItemType<BlessingHeatDeath>());
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Lighting.AddLight(new Vector2(i, j) * 16, 0.2f, 0.05f, 0.05f);
        }
    }
}