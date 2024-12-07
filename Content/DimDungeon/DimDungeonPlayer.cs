using CalamityEntropy.Util;
using SubworldLibrary;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.DimDungeon;

public class DimDungeonPlayer: ModPlayer
{
    public override void PreUpdate()
    {
        if (SubworldSystem.IsActive<DimDungeonSubworld>())
        {
            Player.Entropy().crSky = 5;
        }
    }
}