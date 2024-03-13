using System;
using BOW.Controllers;
using UnityEngine;

namespace BOW.Managers
{
    public class GameplayEventManager : MonoBehaviour
    {
        public static GameplayEventManager instance;

        public event Action OnBlockDropComplete;
        public event Action OnBlocksMerged;
        public event Action OnBlocksFallComplete;

        [SerializeField]
        private BlockManager blockManager;
        [SerializeField]
        private PlayerController playerController;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void ProcessNextPhase(Helpers.GameState currentState)
        {
            switch (currentState)
            {
                case Helpers.GameState.BlockDrop:
                    OnBlockDropComplete?.Invoke();
                    break;
                case Helpers.GameState.MergeAnimation:
                    OnBlocksMerged?.Invoke();
                    break;
                case Helpers.GameState.BlockFall:
                    OnBlocksFallComplete?.Invoke();
                    break;
            }
        }


        public void TriggerBlockDrop()
        {
            ProcessNextPhase(Helpers.GameState.BlockDrop);
        }

        public void TriggerBlocksMerged()
        {
            ProcessNextPhase(Helpers.GameState.MergeAnimation);
        }

        public void TriggerBlocksFallComplete()
        {
            ProcessNextPhase(Helpers.GameState.BlockFall);
        }

        private void OnEnable()
        {
            GameManager.instance.OnGameStateChanged += ProcessNextPhase;
        }

        private void OnDisable()
        {
            GameManager.instance.OnGameStateChanged -= ProcessNextPhase;
        }
    }
}
