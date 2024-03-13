using System;
using UnityEngine;
using BOW.Patterns;
using BOW.Helpers;

namespace BOW.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        #region Serialized Field
        [SerializeField]
        private GridManager gridManager;
        [SerializeField]
        private BlockManager blockManager;
        #endregion
        public GameState currentGameState;


        #region Seed
        #endregion

        #region Action
        public Action<GameState> OnGameStateChanged;
        #endregion


        public void ChangeGameState(GameState state)
        {
            if (currentGameState != state)
            {
                currentGameState = state;
                OnGameStateChanged?.Invoke(state);
            }
           
        }
        
        public GridManager GetGridManager()
        {
            return gridManager;
        }

        public BlockManager GetBlockManager()
        {
            return blockManager;
        }
    }
}




