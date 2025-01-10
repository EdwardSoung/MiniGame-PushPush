using System.Collections;
using UnityEngine;

namespace FirstVillain.ScrollView
{
    public class VerticalGridInfiniteScrollView : InfiniteScrollView
    {
        [SerializeField] private bool _isAtTop = true;
        [SerializeField] private bool _isAtBottom = true;
        [SerializeField] private int _columeCount = 1;

        protected override void OnValueChanged(Vector2 normalizedPosition)
        {
            if (_columeCount <= 0)
            {
                _columeCount = 1;
            }
            float viewportInterval = _scrollRect.viewport.rect.height;
            float minViewport = _scrollRect.content.anchoredPosition.y;
            Vector2 viewportRange = new Vector2(minViewport, minViewport + viewportInterval);
            float contentHeight = _padding.x;
            for (int i = 0; i < _dataList.Count; i += _columeCount)
            {
                for (int j = 0; j < _columeCount; j++)
                {
                    int index = i + j;
                    if (index >= _dataList.Count)
                        break;
                    var visibleRange = new Vector2(contentHeight, contentHeight + _dataList[index].CellSize.y);
                    if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                    {
                        RecycleCell(index);
                    }
                }
                contentHeight += _dataList[i].CellSize.y + _spacing;
            }
            contentHeight = _padding.x;
            for (int i = 0; i < _dataList.Count; i += _columeCount)
            {
                for (int j = 0; j < _columeCount; j++)
                {
                    int index = i + j;
                    if (index >= _dataList.Count)
                        break;
                    var visibleRange = new Vector2(contentHeight, contentHeight + _dataList[index].CellSize.y);
                    if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                    {
                        SetupCell(index, new Vector2((_dataList[index].CellSize.x + _spacing) * j, -contentHeight));
                        if (visibleRange.y >= viewportRange.x)
                            _cellList[index].transform.SetAsLastSibling();
                        else
                            _cellList[index].transform.SetAsFirstSibling();
                    }
                }
                contentHeight += _dataList[i].CellSize.y + _spacing;
            }
            if (_scrollRect.content.sizeDelta.y > viewportInterval)
            {
                _isAtTop = viewportRange.x + _extendVisibleRange <= _dataList[0].CellSize.y;
                _isAtBottom = _scrollRect.content.sizeDelta.y - viewportRange.y + _extendVisibleRange <= _dataList[_dataList.Count - 1].CellSize.y;
            }
            else
            {
                _isAtTop = true;
                _isAtBottom = true;
            }
        }

        public sealed override void Refresh()
        {
            if (!IsInitialized)
            {
                Initialize();
            }
            if (_scrollRect.viewport.rect.height == 0)
                StartCoroutine(DelayToRefresh());
            else
                DoRefresh();
        }

        private void DoRefresh()
        {
            float height = _padding.x;
            for (int i = 0; i < _dataList.Count; i += _columeCount)
            {
                height += _dataList[i].CellSize.y + _spacing;
            }
            for (int i = 0; i < _cellList.Count; i++)
            {
                RecycleCell(i);
            }
            height += _padding.y;
            _scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, height);
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
            var rowNumber = index / _columeCount;
            var height = _padding.x;
            for (int i = 0; i < rowNumber; i++)
            {
                height += _dataList[i * _columeCount].CellSize.y + _spacing;
            }
            height = Mathf.Min(_scrollRect.content.rect.height - _scrollRect.viewport.rect.height, height);
            if (_scrollRect.content.anchoredPosition.y != height)
            {
                DoSnapping(new Vector2(0, height), duration);
            }
        }
    }
}