using CalamityEntropy.Content.Tiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.MusicBoxes
{
    public class ApsychosMusicBox : MusicBox
    {
        public override string MusicFile => "Assets/Sounds/Music/Apsychos";
        public override int MusicBoxTile => ModContent.TileType<ApsychosMusicBoxTile>();
    }
    public class ApsychosMusicBoxTile : MusicBoxTile
    {
    }
}
