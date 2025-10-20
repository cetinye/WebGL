using System;

namespace Name_It_Or_Run_It
{
    public static class GameStateManager
    {
        private static GameState gameState = GameState.WAITING_SELECTION;
        public static event Action OnGameStateChanged;

        public static void SetGameState(GameState newGameState)
        {
            gameState = newGameState;
            OnGameStateChanged?.Invoke();
        }

        public static GameState GetGameState()
        {
            return gameState;
        }

        public enum GameState
        {
            WAITING_SELECTION,
            ASK_QUESTION,
        }
    }
}