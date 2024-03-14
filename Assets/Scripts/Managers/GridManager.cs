using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BOW.Builders;
using BOW.Models.Objects;
using static BOW.Managers.LevelManager;
using BOW.Models;
using System;

namespace BOW.Managers {

    public class GridManager : MonoBehaviour
    {
        #region Serialized Field
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private GridBuilder gridBuilder;
        [SerializeField] private int gridWidth;
        [SerializeField] private int gridHeight;
        #endregion

        private GameObject grid;
        private List<Cell> currentGridList;

        private void Start()
        {
            gridBuilder.ScaleAndBuild(gridWidth,gridHeight);
            currentGridList = gridBuilder.GetGridCellList();
            grid = gridBuilder.GetGrid();
        }

        public void UpdateCellAndBlockPosition(BlockBehaviour blockBehaviour, int newX, int newY)
        {
            if(blockBehaviour == null)
            {
                Cell popCell = GetCellAtPosition(newX, newY);
                popCell.SetBlockOnCell(null);
                return;
            }

            Cell oldCell = GetCellAtPosition(blockBehaviour.GetPositionX(), blockBehaviour.GetPositionY());
            Cell newCell = GetCellAtPosition(newX, newY);

            if (oldCell != null) oldCell.SetBlockOnCell(null);
            if (newCell != null) newCell.SetBlockOnCell(blockBehaviour.GetComponent<Block>());

            blockBehaviour.SetPoisitonX(newX);
            blockBehaviour.SetPoisitonY(newY);
        }

        public Cell GetCellAtPosition(int x, int y)
        {
            return currentGridList[x*gridWidth+y];
        }

        public List<Cell> GetCellGridList()
        {
            return currentGridList;
        }

        public int GetGridWidth()
        {
            return gridWidth;
        }

        public int GetGridHeight()
        {
            return gridHeight;
        }

        public Transform GetGridTransform()
        {
            return grid.transform;
        }
    }

}
