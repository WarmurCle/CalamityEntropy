using CalamityEntropy.Util;
using SubworldLibrary;
using Terraria.ModLoader;

namespace CalamityEntropy.DimDungeon;

public class DimDungeonPlayer: ModPlayer
{
    public override void PreUpdate()
    {
        if (SubworldSystem.IsActive<DimDungeon.DimDungeonSubworld>())
        {
            Player.Entropy().crSky = 5;
        }
    }
}