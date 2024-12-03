using System;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;

namespace CalamityEntropy.DimDungeon;

public static class DimDungeonExtension
{
    [Pure]
    public static bool IsHorizontal(this Direction direction)
    {
        return direction == Direction.Left || direction == Direction.Right;
    }

    [Pure]
    public static Direction Reverse(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
        }
        throw new ArgumentException("Invalid direction");
    }
}