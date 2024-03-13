using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BOW.Models.Objects
{
    public class Cell : MonoBehaviour
    {
        public CellState cellState = CellState.Closed;

        [SerializeField]
        private Block currentBlock;
        private int row;
        private int column;


        public int GetColumnIndex()
        {
            return column;
        }

        public int GetRowIndex()
        {
            return this.row;
        }
        public void SetColumn(int col)
        {
            this.column = col;
        }

        public void SetRow(int row)
        {
            this.row = row;
        }

        public CellState GetState()
        {
            return cellState;
        }

        public void SetCellState(CellState state)
        {
            cellState = state;
        }

        public void SetBlockOnCell(Block block)
        {
            if(block != null)
            {
                currentBlock = block;
                SetCellState(CellState.Open);
            }
            else
            {
                currentBlock = null;
                SetCellState(CellState.Closed);
            }
        }

        public Block GetBlockOnCell()
        {
            return currentBlock;
        }

    }
}
