using MapEnum;

public static class Constants
{
    /// <summary>
    ///  Path to the folder in Resources
    /// </summary>
    public const string POOL_PATH = "Pool";
    public const string UI_PATH = "UI/";

    /// <summary>
    ///  Control input axis
    /// </summary>
    public const string HORIZONTAL = "Horizontal";
    public const string VERTICAL = "Vertical";

    /// <summary>
    ///  UI Animation Trigger
    /// </summary>
    public const string OPEN = "Open";
    public const string CLOSE = "Close";
    
    public const float TREE_HEIGHT = 1f;

    public const float MOVING_TIME = 0.25f;
    public const float MOVING_LOG_TIME = 0.01f;
}


namespace _Game
{
    public interface IInit
    {
        public void OnInit();
    }
}
