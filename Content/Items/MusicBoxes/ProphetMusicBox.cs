using Terraria.ModLoader;
using Terraria.ID;
using CalamityEntropy.Content.Tiles;

namespace CalamityEntropy.Content.Items.MusicBoxes
{
    public class ProphetMusicBox : MusicBox
    {
        public override string MusicFile => "Assets/Sounds/Music/SpectralForesight";
        public override int MusicBoxTile => ModContent.TileType<ProphetMusicBoxTile>();
    }
    public class ProphetMusicBoxTile: MusicBoxTile
    {
    }
}
