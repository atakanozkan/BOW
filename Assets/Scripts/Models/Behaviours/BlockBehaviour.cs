using System;
using System.Collections;
using System.Collections.Generic;
using BOW.Managers;
using BOW.Models.Objects;
using DG.Tweening;
using UnityEngine;

namespace BOW.Models
{
    public class BlockBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Block defaultBlock;

        public int positionX;
        public int positionY;
        private List<BlockBehaviour> matchNeighbors = new List<BlockBehaviour>();

        public bool isMerged = false;

        public void DropBlock(int dropAmount,int targetCol, float duration)
        {
            Cell targetCell  = GameManager.instance.GetGridManager().GetCellAtPosition(positionX+(dropAmount-1), targetCol);
            Vector3 endPosition = targetCell.transform.position;
            defaultBlock.isDropping = true;

            transform.DOMove(endPosition, duration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                GameManager.instance.GetGridManager().UpdateCellAndBlockPosition(this, positionX + (dropAmount - 1), targetCol);
                GameManager.instance.GetBlockManager().AddBlockOnList(defaultBlock);

                defaultBlock.isDropping = false;
                
                GameManager.instance.GetBlockManager().TriggerMergeControlOnStop(defaultBlock.GetBlockBehaviour(),
                    defaultBlock.isDropping);
            });
        }

        public void FallBlock(int dropAmount, int targetCol, float duration)
        {
            Cell targetCell = GameManager.instance.GetGridManager().GetCellAtPosition(positionX + (dropAmount), targetCol);
            Vector3 endPosition = targetCell.transform.position;
            defaultBlock.isFalling = true;

            transform.DOMove(endPosition, duration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                GameManager.instance.GetGridManager().UpdateCellAndBlockPosition(this, positionX + (dropAmount), targetCol);

                defaultBlock.isFalling = false;

                GameManager.instance.GetBlockManager().TriggerMergeControlOnStop(defaultBlock.GetBlockBehaviour(),
                    defaultBlock.isFalling);
            });
        }

        public bool FindMerge(List<Block> listBlocks, int borderX, int borderY)
        {
            matchNeighbors = SearchNeighborMatch(listBlocks, borderX, borderY);

            if (matchNeighbors != null && matchNeighbors.Count > 0)
            {
                BlockManager.instance.TriggerMerge(this, matchNeighbors);
                return true;
            }
            return false;
        }


        public List<BlockBehaviour> SearchNeighborMatch(List<Block> listBlocks, int borderX, int borderY)
        {
            int borderDownX = (positionX - 1) < 0 ? 0 : (positionX - 1);
            int borderUpX = (positionX + 1) >= borderX ? borderX-1 : (positionX + 1);
            int borderRightY = (positionY + 1) >= borderY ? borderY-1 : (positionY + 1);
            int borderLeftY = (positionY - 1) < 0 ? 0 : (positionY - 1);


            if (defaultBlock == null)
            {
                Debug.LogError("Default block not found!");
                return null;
            }
            matchNeighbors.Clear();

            for (int x = borderDownX; x <= borderUpX; x++)
            {
                for (int y = borderLeftY; y <= borderRightY; y++)
                {
                    if (x == positionX && y == positionY) continue;

                    Block neighborBlock = listBlocks.Find(block =>
                        block.GetBlockBehaviour().positionX == x &&
                        block.GetBlockBehaviour().positionY == y &&
                        !block.GetBlockBehaviour().isMerged);

                    if (neighborBlock != null && neighborBlock is NumberBlock)
                    {
                        NumberBlock numberBlockNeighbor = ((NumberBlock)neighborBlock);
                        bool doesMatch = numberBlockNeighbor.blockSize == ((NumberBlock)defaultBlock).blockSize;

                        if (doesMatch)
                        {

                            matchNeighbors.Add(numberBlockNeighbor.GetBlockBehaviour());
                        }
                    }
                }
            }

            return matchNeighbors;
        }

        public void ChangeBlockSize()
        {

        }

        public void SetPoisitonX(int x)
        {
            positionX = x;
        }
        public int GetPositionX()
        {
            return positionX;
        }

        public void SetPoisitonY(int y)
        {
            positionY = y;
        }
        public int GetPositionY()
        {
            return positionY;
        }

        public Block GetDefaultBlock()
        {
            return defaultBlock;
        }

    }


}

