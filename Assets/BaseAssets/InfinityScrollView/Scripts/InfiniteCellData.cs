using System;
using UnityEngine;
namespace FirstVillain.ScrollView
{
    public class InfiniteCellData
    {
        public int Index { get; set; }
        public Vector2 CellSize { get; private set; }

        private object _data;

        public Action<InfiniteCell> OnUpdated;

        public InfiniteCellData()
        {

        }

        public InfiniteCellData(Vector2 cellSize)
        {
            this.CellSize = cellSize;
        }

        public InfiniteCellData(Vector2 cellSize, object data)
        {
            this.CellSize = cellSize;
            this._data = data;
        }
        
        public void OnUpdate(InfiniteCell cell)
        {
            OnUpdated?.Invoke(cell);
        }
    }
}