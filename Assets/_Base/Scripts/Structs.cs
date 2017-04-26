public static class Structs
{
    public enum GameMode
    {
        Normal,
        MaxValues
    }

    public enum GameDifficulty
    {
        Normal,
        MaxValues
    }

    public enum GameView
    {
        Fixed = 0,
        FollowEntity,
        MaxValues
    }

    public enum GameScene
    {
        Splash = 0,
        Initialization,
        LoadingGame,
        Menu,
        Ingame,
        GameEnd,
        Score,
        Credits,
        Exit,
        MaxValues
    }
}
