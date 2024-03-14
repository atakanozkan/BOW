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

        public delegate int BlockFallHandler(List<BlockBehaviour> blocks);
        public event BlockFallHandler OnBlocksFall;

        public delegate void BlockStopHandler(BlockBehaviour baseBlock, bool isFalling);
        public event BlockStopHandler OnBlockStopMove;


        public float dropBlockSpeed = 0.8f;
        public float fallBlockSpeed = 0.3f;
        public int waitForMergeControlEnd = 0;
        public List<Block> blockList = new List<Block>();

        public void TriggerMergeControlOnStop(BlockBehaviour baseBlock, bool isFalling)
        {
            OnBlockStopMove?.Invoke(baseBlock, isFalling);
        }

        public void TriggerMerge(BlockBehaviour baseBlock, List<BlockBehaviour> mergingBlocks)
        {
            OnBlocksMerged?.Invoke(baseBlock, mergingBlocks);
        }

        public int TriggerFall(List<BlockBehaviour> blocks)
        {
            return (int)(OnBlocksFall?.Invoke(blocks));
        }




        public void ControlBlockMergeOnStop(BlockBehaviour block, bool isFalling)
        {
            StartCoroutine(CheckTheMovementStop());

            bool foundMerge = block.FindMerge(blockList, gridManager.GetGridHeight(), gridManager.GetGridWidth());
            if (!foundMerge)
            {
                Debug.Log("found no merge");
                waitForMergeControlEnd--;
                if (waitForMergeControlEnd <= 0)
                {
                    Debug.Log("check state change");
                    StartCoroutine(CheckAndAdjustGameState());
                }
            }
            else
            {
                waitForMergeControlEnd--;
            }
        }

        public bool CheckAllBlocksStop()
        {
            bool check = true;

            foreach (Block block in blockList)
            {
                if (block.isFalling || block.isDropping)
                {
                    check = false;
                    break;
                }
            }
            return check;
        }

        private IEnumerator CheckTheMovementStop() {
            yield return new WaitUntil(()=>CheckAllBlocksStop());
        }

        public bool CheckAllBlocksIfMerge()
        {
            bool check = false;

            foreach (Block block in blockList)
            {
                if (block.GetBlockBehaviour().SearchNeighborMatch(blockList, gridManager.GetGridHeight(), gridManager.GetGridWidth()).Count > 0)
                {
                    check = true;
                    break;
                }
            }
            return check;
        }

        private IEnumerator CheckAndAdjustGameState()
        {
            yield return new WaitUntil(() => waitForMergeControlEnd <= 0);
            if (!CheckAllBlocksIfMerge())
            {
                Debug.Log("not merge at all");
                GameManager.instance.ChangeGameState(Helpers.GameState.Playing);
            }
        }

        private void HandleBlockMerge(BlockBehaviour baseBlock, List<BlockBehaviour> mergingBlocks)
        {
            GameManager.instance.ChangeGameState(Helpers.GameState.MergeAnimation);

            int mergeCount = mergingBlocks.Count();

            foreach (BlockBehaviour blockBehaviour in mergingBlocks)
            {
                blockBehaviour.isMerged = true;

                PoolItem poolItem = blockBehaviour.gameObject.GetComponent<PoolItem>();
                RemoveBlockFromList(blockBehaviour.GetDefaultBlock());
                blockBehaviour.ResetBlockOnCell();
                PoolManager.instance.ResetPoolItem(poolItem);
            }

            Block block = baseBlock.GetDefaultBlock();
            if (block is NumberBlock)
            {
                ((NumberBlock)block).blockSize = ((NumberBlock)block).blockSize * ((int)Math.Pow(2, mergeCount));
                block.SetBlockText(((NumberBlock)block).blockSize.ToString());
            }


            GameManager.instance.ChangeGameState(Helpers.GameState.BlockFall);
            var blockBehaviourList = blockList.Select(block => block.GetBlockBehaviour()).ToList();


            int fallBlockCount = TriggerFall(blockBehaviourList);

            if(fallBlockCount == 0)
            {
                //waitForMergeControlEnd++;
                ControlBlockMergeOnStop(baseBlock, false);
            }
        }



        public int FallBlocks(List<BlockBehaviour> blocks)
        {
            Sequence fallSequence = DOTween.Sequence();
            List<BlockBehaviour> fallList = new List<BlockBehaviour>();

            if (GameManager.instance.currentGameState == Helpers.GameState.BlockFall)
            {
                foreach (BlockBehaviour block in blocks)
                {
                    int dropUnit = FindAmountFall(gridManager.GetGridHeight(), block.GetPositionY(), block.GetPositionX() + 1);
                    if(dropUnit > 0)
                    {
                        fallSequence.Append(block.FallBlock(dropUnit, block.GetPositionY(), fallBlockSpeed));
                        fallList.Add(block);
                    }
                }
            }

            fallSequence.Play().OnComplete(
                () =>
                {
                    foreach(BlockBehaviour blockBehaviour in fallList)
                    {
                        Debug.Log("Check for merge after fall!");
                        waitForMergeControlEnd++;
                        GameManager.instance.GetBlockManager().TriggerMergeControlOnStop(blockBehaviour,false);
                    }
                }
                );

            return fallList.Count();
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


            Block block = CreateDropBlock(spawnPoint,targetCol);
            BlockBehaviour blockBehaviour = block.GetBlockBehaviour();

            int dropUnit = FindAmountFall(gridManager.GetGridHeight(), targetCol, blockBehaviour.GetPositionX());

            if (dropUnit == 0) { return; } // TO DO : Implement gameOver

            GameManager.instance.ChangeGameState(Helpers.GameState.BlockDrop);
            waitForMergeControlEnd++;
            blockBehaviour.DropBlock(dropUnit, targetCol, dropBlockSpeed);
        }

        public Block CreateDropBlock(Transform spawnPoint, int targetCol) //PLAYER
        {
            var poolItem = PoolManager.instance.GetFromPool(PoolItemType.NumberedBlock, gridManager.GetGridTransform());

            Block block = poolItem.GetComponent<Block>();
            Cell cellInColumn = gridManager.GetCellAtPosition(block.GetBlockBehaviour().GetPositionX(), targetCol);

            Vector3 newSpawnPoint = new Vector3(
                spawnPoint.transform.position.x,
                cellInColumn.transform.position.y,
                spawnPoint.transform.position.z
                );

            int index = UnityEngine.Random.Range(0, 4);

            int[] tuple = { 2, 4, 8, 16 };

            if(block is NumberBlock)
            {
                ((NumberBlock)block).blockSize = tuple[index];
                block.SetBlockText(((NumberBlock)block).blockSize.ToString());
            }


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

