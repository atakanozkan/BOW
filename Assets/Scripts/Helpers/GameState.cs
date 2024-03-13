using System;

namespace BOW.Helpers
{
    [Flags]
    public enum GameState
    {
        Default = 0,
        Playing = 1,
        Pause = 2,
        Lose = 4,
        Win = 8,
        PowerUp = 16,
        PowerUpAnimation = 32,
        MergeAnimation = 64,
        BlockFall = 128,
        BlockDrop = 256,
    }
}