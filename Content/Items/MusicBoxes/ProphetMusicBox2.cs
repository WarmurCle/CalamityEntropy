using CalamityEntropy.Content.Tiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.MusicBoxes
{
    public class ProphetMusicBox2 : MusicBox
    {
        public override string MusicFile => "Assets/Sounds/Music/Prophet2";
        public override int MusicBoxTile => ModContent.TileType<ProphetMusicBoxTile2>();
    }
    public class ProphetMusicBoxTile2 : MusicBoxTile
    {
    }
}
