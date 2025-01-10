using System.Collections;
using UnityEngine;
namespace FirstVillain.ScrollView
{
    public class HorizontalGridInfiniteScrollView : InfiniteScrollView
    {
        [SerializeField] private int _rowCount = 1;
        [SerializeField] private bool _isAtLeft = true;
        [SerializeField] private bool _isAtRight = true;
        protected override void OnValueChanged(Vector2 normalizedPosition)
        {
            if (_rowCount <= 0)
            {
                _rowCount = 1;
            }
            float viewportInterval = _scrollRect.viewport.rect.width;
            float minViewport = -_scrollRect.content.anchoredPosition.x;
            Vector2 viewportRange = new Vector2(minViewport - _extendVisibleRange, minViewport + viewportInterval + _extendVisibleRange);
            float contentWidth = _padding.x;
            for (int i = 0; i < _dataList.Count; i += _rowCount)
            {
                for (int j = 0; j < _rowCount; j++)
                {
                    int index = i + j;
                    if (index >= _dataList.Count)
                        break;
                    var visibleRange = new Vector2(contentWidth, contentWidth + _dataList[index].CellSize.x);
                    if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                    {
                        RecycleCell(index);
                    }
                }
                contentWidth += _dataList[i].CellSize.x + _spacing;
            }
            contentWidth = _padding.x;
            for (int i = 0; i < _dataList.Count; i += _rowCount)
            {
                for (int j = 0; j < _rowCount; j++)
                {
                    int index = i + j;
                    if (index >= _dataList.Count)
                        break;
                    var visibleRange = new Vector2(contentWidth, contentWidth + _dataList[index].CellSize.x);
                    if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                    {
                        SetupCell(index, new Vector2(contentWidth, (_dataList[index].CellSize.y + _spacing) * -j));
                        if (visibleRange.y >= viewportRange.x)
                            _cellList[index].transform.SetAsLastSibling();
                        else
                            _cellList[index].transform.SetAsFirstSibling();
                    }
                }
                contentWidth += _dataList[i].CellSize.x + _spacing;
            }
            if (_scrollRect.content.sizeDelta.x > viewportInterval)
            {
                _isAtLeft = viewportRange.x + _extendVisibleRange <= _dataList[0].CellSize.x;
                _isAtRight = _scrollRect.content.sizeDelta.x - viewportRange.y + _extendVisibleRange <= _dataList[_dataList.Count - 1].CellSize.x;
            }
            else
            {
                _isAtLeft = true;
                _isAtRight = true;
            }
        }

        public sealed override void Refresh()
        {
            if (!IsInitialized)
            {
                Initialize();
            }
            if (_scrollRect.viewport.rect.width == 0)
                StartCoroutine(DelayToRefresh());
            else
                DoRefresh();
        }

        private void DoRefresh()
        {
            float width = _padding.x;
            for (int i = 0; i < _dataList.Count; i += _rowCount)
            {
                width += _dataList[i].CellSize.x + _spacing;
            }
            for (int i = 0; i < _cellList.Count; i++)
            {
                RecycleCell(i);
            }
            width += _padding.y;
            _scrollRect.content.sizeDelta = new Vector2(width, _scrollRect.content.sizeDelta.y);
            OnValueChanged(_scrollRect.normalizedPosition);
            OnRefresh?.Invoke();
        }

        private IEnumerator DelayToRefresh()
        {
            yield return _waitEndOfFrame;
            DoRefresh();
        }

        public override void Snap(int index, float duration)
        {
            if (!IsInitialized)
                return;
            if (index >= _dataList.Count)
                return;
            var columeNumber = index / _rowCount;
            var width = _padding.x;
            for (int i = 0; i < columeNumber; i++)
            {
                width += _dataList[i * _rowCount].CellSize.x + _spacing;
            }
            width = Mathf.Min(_scrollRect.content.rect.width - _scrollRect.viewport.rect.width, width);
            if (_scrollRect.content.anchoredPosition.x != width)
            {
                DoSnapping(new Vector2(-width, 0), duration);
            }
        }
    }
}

