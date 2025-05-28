using CalamityEntropy.Content.Tiles;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.MusicBoxes
{
    public class NihilityTwinsMusicBox : MusicBox
    {
        public override string MusicFile => "Assets/Sounds/Music/vtfight";
        public override int MusicBoxTile => ModContent.TileType<NihilityTwinsMusicBoxTile>();
    }
    public class NihilityTwinsMusicBoxTile : MusicBoxTile
    {
    }
}
