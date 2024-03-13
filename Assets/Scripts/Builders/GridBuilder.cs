using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BOW.Managers;
using BOW.Models.Objects;
using BOW.Models.Pool;

namespace BOW.Builders
{
    public class GridBuilder : MonoBehaviour
    {
        #region Serialized Field
        [SerializeField] private Cell cell;
        [SerializeField] private Block numberBlock;
        [SerializeField] private GameObject grid;
        [SerializeField] private SpriteRenderer gridRenderer;
        [SerializeField] private SpriteRenderer cellRenderer;
        [SerializeField] private SpriteRenderer numberBlockRenderer;
        [SerializeField] private Camera mainCamera;
        #endregion

        private float scaleRateWidth;
        private float scaleRateHeight;

        public Sprite[] sprite_list;
        private List<Cell> listGrid;

        public void ScaleAndBuild(int width, int height)
        {
            ScaleGrid();
            GenerateGrids(width,height);
            GenerateBlocksToPool(width, height);
        }

        public void ScaleGrid()
        {
            float newGridScale = (float)((float)mainCamera.pixelWidth / (float)mainCamera.pixelHeight);

            if (newGridScale < BOW.Constants.Constants.DEFAULTSCREENRATIO)
            {
                newGridScale /= BOW.Constants.Constants.DEFAULTSCREENRATIO;
                grid.gameObject.transform.localScale = new Vector3(newGridScale, newGridScale);
            }

        }

        public List<Cell> ArrangeCells(List<Cell> tempList, int width, int height)
        {
            var list = new List<Cell>(tempList);

            float gridWidth = gridRenderer.bounds.size.x;
            float gridHeight = gridRenderer.bounds.size.y;

            float originalCellWidth = cellRenderer.bounds.size.x;
            float originalCellHeight = cellRenderer.bounds.size.y;

            float newCellWidth = gridWidth / width;
            float newCellHeight = gridHeight / height;

            scaleRateWidth = newCellWidth / originalCellWidth / grid.transform.localScale.x * cell.transform.localScale.x;
            scaleRateHeight = newCellHeight / originalCellHeight / grid.transform.localScale.y * cell.transform.localScale.y;

            for (int row = 0; row < height; row++)
            {

                for (int col = 0; col < width; col++)
                {

                    Vector3 position = new Vector3(grid.transform.position.x - (gridWidth / 2) + (newCellWidth / 2) + (newCellWidth * col),
                    grid.transform.position.y + (gridHeight / 2) - (newCellHeight / 2) - (newCellHeight * row));

                    list[row * width + col].transform.position = position;
                    list[row * width + col].transform.localScale = new Vector2(scaleRateWidth, scaleRateHeight);
                }
            }

            return list;
        }

        public Block ScaleNumberBlock(Block block, int width, int height) {

            float gridWidth = gridRenderer.bounds.size.x;
            float gridHeight = gridRenderer.bounds.size.y;

            float originalBlockWidth = numberBlockRenderer.bounds.size.x;
            float originalBlockHeight = numberBlockRenderer.bounds.size.y;

            float newBlockWidth = gridWidth / width;
            float newBlockHeight = gridHeight / height;

            scaleRateWidth = newBlockWidth / originalBlockWidth / grid.transform.localScale.x * numberBlock.transform.localScale.x;
            scaleRateHeight = newBlockHeight / originalBlockHeight / grid.transform.localScale.y * numberBlock.transform.localScale.y;

            block.transform.localScale = new Vector2(scaleRateWidth, scaleRateHeight);

            return block;
        }

        public List<Cell> GenerateGrids(int width,int height)
        {
            int size = width * height;
            int filledBoxCount = (size);
            List<Cell> tempList = GenerateFilledList(filledBoxCount, size);

            listGrid = new List<Cell>(size * size);

            int count = 0;
            foreach (Cell cell in tempList)
            {
                cell.name = "cell " + (count / width) + "_" + (count % width);
                cell.SetRow((count / width));
                cell.SetColumn((count % width));
                listGrid.Add(cell);
                count++;
            }
            listGrid = ArrangeCells(listGrid, width, height);
            return listGrid;
        }



        public List<Cell> GenerateFilledList(int filledCount, int gridsize)
        {
            List<Cell> tempList = new List<Cell>(gridsize);
            Debug.Log(gridsize);
            int count = 0;
            while (count < gridsize)
            {
                PoolItem poolItem = PoolManager.instance.GetFromPool(PoolItemType.GhostCell, grid.transform);

                if (poolItem == null)
                {
                    poolItem = Instantiate(PoolManager.instance.prefabList[0]);
                    PoolManager.instance.AddToUsing(poolItem, grid.transform);
                }
                Cell pooledCell = poolItem.GetComponent<Cell>();
                if (count < filledCount)
                {
                    pooledCell.SetCellState(CellState.Open);
                }
                else
                {
                    pooledCell.SetCellState(CellState.Closed);
                }
                pooledCell.gameObject.SetActive(true);

                tempList.Add(pooledCell);
                count++;
            }
            return tempList;
        }


        public void GenerateBlocksToPool(int width,int height)
        {
            int gridsize = width * height;
            Debug.Log(gridsize);
            int count = 0;
            while (count < gridsize)
            {
                PoolItem poolItem = Instantiate(PoolManager.instance.prefabList[1]);

                
                Block pooledBlock = poolItem.GetComponent<Block>();


                pooledBlock = ScaleNumberBlock(pooledBlock, width, height);

                PoolManager.instance.AddToAvailable(poolItem);
                count++;
            }
        }

        public List<Cell> GetGridCellList()
        {
            return listGrid;
        }

        public GameObject GetGrid()
        {
            return grid;
        }
    }
}