using System;
using System.Collections;
using System.Collections.Generic;
using BOW.Builders;
using BOW.Models;
using BOW.Models.Pool;
using BOW.Patterns;
using DG.Tweening;
using UnityEditor;
using System.Linq;
using UnityEngine;
using BOW.Models.Objects;
using static UnityEngine.GraphicsBuffer;
using System.Drawing;

namespace BOW.Managers
{

    public class BlockManager : Singleton<BlockManager>
    {
        #region Serialized Field
        [SerializeField] private GridManager gridManager;
        [SerializeField] private GameObject spawnPosition;
        #endregion

        public delegate void BlockMergeHandler(BlockBehaviour baseBlock, List<BlockBehaviour> mergingBlocks);
        public event BlockMergeHandler OnBlocksMerged;

        public delegate void BlockFallHandler(List<BlockBehaviour> blocks);
        public event BlockFallHandler OnBlocksFall;

        public delegate void BlockStopHandler(BlockBehaviour baseBlock, bool isFalling);
        public event BlockStopHandler OnBlockStopMove;


        public float dropBlockSpeed = 0.8f;
        public float fallBlockSpeed = 0.3f;
        public List<Block> blockList = new List<Block>();

        public void TriggerMergeControlOnStop(BlockBehaviour baseBlock, bool isFalling)
        {
            OnBlockStopMove?.Invoke(baseBlock, isFalling);
        }

        public void TriggerMerge(BlockBehaviour baseBlock, List<BlockBehaviour> mergingBlocks)
        {
            OnBlocksMerged?.Invoke(baseBlock, mergingBlocks);
        }

        public void TriggerFall(List<BlockBehaviour> blocks)
        {
            OnBlocksFall?.Invoke(blocks);
        }

        public void ControlBlockMergeOnStop(BlockBehaviour block, bool isFalling)
        {

            if (!block.FindMerge(blockList, gridManager.GetGridHeight(), gridManager.GetGridWidth()))
            {
                GameManager.instance.ChangeGameState(Helpers.GameState.Playing);
            }
        }

        private void HandleBlockMerge(BlockBehaviour baseBlock, List<BlockBehaviour> mergingBlocks)
        {
            GameManager.instance.ChangeGameState(Helpers.GameState.MergeAnimation);

            foreach (BlockBehaviour blockBehaviour in mergingBlocks)
            {
                blockBehaviour.isMerged = true;

                PoolItem poolItem = blockBehaviour.gameObject.GetComponent<PoolItem>();
                blockBehaviour.isMerged = false;

                gridManager.UpdateCellAndBlockPosition(null, blockBehaviour.GetPositionX(), blockBehaviour.GetPositionY());
                RemoveBlockFromList(blockBehaviour.GetDefaultBlock());
                PoolManager.instance.ResetPoolItem(poolItem);



            }
            GameManager.instance.ChangeGameState(Helpers.GameState.BlockFall);
            var blockBehaviourList = blockList.Select(block => block.GetBlockBehaviour()).ToList();

            TriggerFall(blockBehaviourList);
        }



        public void FallBlocks(List<BlockBehaviour> blocks)
        {
            if (GameManager.instance.currentGameState == Helpers.GameState.BlockFall)
            {
                foreach (BlockBehaviour block in blocks)
                {
                    int dropUnit = FindAmountFall(gridManager.GetGridHeight(), block.GetPositionY(), block.GetPositionX() + 1);
                    block.FallBlock(dropUnit, block.GetPositionY(), fallBlockSpeed);
                }
            }
        }
        private int FindAmountFall(int rowSize, int targetCol, int targetRow)
        {
            int counter = 0;

            for (int row = targetRow; row < rowSize; row++)
            {
                if (gridManager.GetCellAtPosition(row, targetCol).GetBlockOnCell() == null)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }
            return counter;
        }


        public void DropBlockMovement(Transform spawnPoint, int targetCol) // PLAYER
        {


            Block block = CreateDropBlock(spawnPoint);
            BlockBehaviour blockBehaviour = block.GetBlockBehaviour();

            int dropUnit = FindAmountFall(gridManager.GetGridHeight(), targetCol, blockBehaviour.GetPositionX());

            if (dropUnit == 0) { return; } // TO DO : Implement gameOver

            GameManager.instance.ChangeGameState(Helpers.GameState.BlockDrop);
            blockBehaviour.DropBlock(dropUnit, targetCol, dropBlockSpeed);
        }

        public Block CreateDropBlock(Transform spawnPoint) //PLAYER
        {
            var poolItem = PoolManager.instance.GetFromPool(PoolItemType.NumberedBlock, gridManager.GetGridTransform());

            Block block = poolItem.GetComponent<Block>();
            Cell cellInColumn = gridManager.GetCellAtPosition(block.GetBlockBehaviour().GetPositionX(), 0);

            Vector3 newSpawnPoint = new Vector3(

                spawnPoint.transform.position.x,
                cellInColumn.transform.position.y,
                spawnPoint.transform.position.z
                );

            block.transform.position = newSpawnPoint;

            return block;
        }

        public void AddBlockOnList(Block block)
        {
            blockList.Add(block);
        }

        public void RemoveBlockFromList(Block block)
        {
            blockList.Remove(block);
        }

        private void OnEnable()
        {
            OnBlocksMerged += HandleBlockMerge;
            OnBlocksFall += FallBlocks;
            OnBlockStopMove += ControlBlockMergeOnStop;
        }

        private void OnDisable()
        {
            OnBlocksMerged -= HandleBlockMerge;
            OnBlocksFall -= FallBlocks;
            OnBlockStopMove -= ControlBlockMergeOnStop;
        }


    }

}

