using System;
using System.Collections;
using System.Collections.Generic;
using BOW.Managers;
using BOW.Models;
using BOW.Models.Objects;
using UnityEngine;

namespace BOW.Controllers {

    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Cannon cannon;
        [SerializeField]
        private GameObject spawnPoint;

        [SerializeField]
        private BlockManager blockManager;

        public event Action<int> OnCannonMoved;

        public void Update()
        {
            if(GameManager.instance.currentGameState  == Helpers.GameState.WaitingForDrop)
            {
                DropBlock();
            }
        }

        public void TriggerCannonMovement(int colIndex)
        {
            OnCannonMoved?.Invoke(colIndex);
        }

        public void DropBlock()
        {
            blockManager.DropBlockMovement(spawnPoint.transform,cannon.colIndex);
        }

        public void MoveCannonOnX(int colIndex) {
            Vector3 cellPositionOnCol = GameManager.instance.GetGridManager().GetCellAtPosition(0, colIndex).transform.position;
            Vector3 targetPosition = new Vector3(
                cellPositionOnCol.x,
                cannon.transform.position.y,
                cannon.transform.position.z
                );
            cannon.colIndex = colIndex;
            cannon.Move(targetPosition);
        }

        private void OnEnable()
        {
            OnCannonMoved += MoveCannonOnX;
        }

        private void OnDisable()
        {
            OnCannonMoved -= MoveCannonOnX;
        }

    }

}

