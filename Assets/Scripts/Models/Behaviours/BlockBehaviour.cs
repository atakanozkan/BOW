using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public bool hasRecentlyMoved = false;
        public int positionX;
        public int positionY;
        private List<BlockBehaviour> matchNeighbors = new List<BlockBehaviour>();

        public bool isMerged = false;

        public void DropBlock(int dropAmount, int targetCol, float duration)
        {
            Cell targetCell = GameManager.instance.GetGridManager().GetCellAtPosition(positionX + (dropAmount - 1), targetCol);
            Vector3 endPosition = targetCell.transform.position;
            defaultBlock.isDropping = true;

            float calculatedDuration = ((float)dropAmount / (float)GameManager.instance.GetGridManager().GetGridHeight()) * duration;

            transform.DOMove(endPosition, calculatedDuration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                GameManager.instance.GetGridManager().UpdateCellAndBlockPosition(this, positionX + (dropAmount - 1), targetCol);
                GameManager.instance.GetBlockManager().AddBlockOnList(defaultBlock);

                defaultBlock.isDropping = false;
            });
        }

        public Tween FallBlock(int dropAmount, int targetCol, float duration)
        {
            Cell targetCell = GameManager.instance.GetGridManager().GetCellAtPosition(positionX + (dropAmount), targetCol);
            Vector3 endPosition = targetCell.transform.position;
            defaultBlock.isFalling = true;

            float calculatedDuration = ((float)dropAmount / (float)GameManager.instance.GetGridManager().GetGridHeight()) * duration;

            return transform.DOMove(endPosition, calculatedDuration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                GameManager.instance.GetGridManager().UpdateCellAndBlockPosition(this, positionX + (dropAmount), targetCol);

                defaultBlock.isFalling = false;
            });
        }

        public List<BlockBehaviour> FindMerge(List<Block> listBlocks, int borderX, int borderY)
        {
            matchNeighbors = SearchNeighborMatch(listBlocks, borderX, borderY);
            if (matchNeighbors != null && matchNeighbors.Count > 0)
            {
                return matchNeighbors;
            }
            return null;
        }


        public List<BlockBehaviour> SearchNeighborMatch(List<Block> listBlocks, int borderX, int borderY)
        {
            matchNeighbors = new List<BlockBehaviour>();

            int[][] neighborPositions = new int[][]
            {
                new int[] {positionX - 1, positionY}, // Up
                new int[] {positionX + 1, positionY}, // Down
                new int[] {positionX, positionY - 1}, // Left
                new int[] {positionX, positionY + 1}  // Right
            };

            foreach (var pos in neighborPositions)
            {
                if (pos[0] >= 0 && pos[0] < borderX && pos[1] >= 0 && pos[1] < borderY)
                {
                    Block neighborBlock = listBlocks.Find(block =>
                        block.GetBlockBehaviour().positionX == pos[0] &&
                        block.GetBlockBehaviour().positionY == pos[1] &&
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

        public void ResetBlockOnCell()
        {
            if (GameManager.instance.GetGridManager() != null &&
                GameManager.instance.GetGridManager().GetCellGridList() != null &&
                GameManager.instance.GetGridManager().GetCellGridList().Count > 0
                )
            {

                Cell cellAtPosition = GameManager.instance.GetGridManager().GetCellAtPosition(positionX, positionY);
                Block blockOnCell = cellAtPosition.GetBlockOnCell();
                if (defaultBlock == blockOnCell)
                {
                    GameManager.instance.GetGridManager().UpdateCellAndBlockPosition(null, positionX, positionY);
                }
                SetPoisitonX(0);
                SetPoisitonY(0);
                isMerged = false;
                defaultBlock.isDropping = true;
                defaultBlock.isFalling = false;
                hasRecentlyMoved = false;
            }
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

