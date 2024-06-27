using System;

public class GameStateManager
{
    public enum GameState
    {
        Running,
        Paused,
        GameOver
    }

    public static event Action<GameState> GameStateChanged;

    public static GameState CurrentGameState { private set; get; }

    private static GameStateManager _instance;
    public static GameStateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameStateManager();
            }

            return _instance;
        }
    }
    
    private GameStateManager()
    {
        CurrentGameState = GameState.Running;
    }
    
    public void SetGameState(GameState state)
    {
        CurrentGameState = state;
        GameStateChanged?.Invoke(CurrentGameState);
    }

    public bool IsPaused()
    {
        return CurrentGameState is GameState.Paused or GameState.GameOver;
    }

    public bool IsRunning()
    {
        return CurrentGameState == GameState.Running;
    }

}
