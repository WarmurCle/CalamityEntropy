namespace CalamityEntropy.DimDungeon;

public static class DimDungeonExtension
{
    public static bool IsHorizontal(this Direction direction)
    {
        return direction == Direction.Left || direction == Direction.Right;
    }
}