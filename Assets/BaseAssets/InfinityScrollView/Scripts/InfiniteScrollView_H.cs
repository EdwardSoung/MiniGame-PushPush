using System.Linq;
using UnityEngine;

public class InfiniteScrollView_H : InfiniteScrollView
{
    [HideInInspector] public bool isAtLeft = true;
    [HideInInspector] public bool isAtRight = true;
    public int rowCount = 1;

    public bool useWidthChangeAnimation;
    protected float _fixeWidth;

    protected virtual float startPadding => paddingLT.x;

    protected override void Awake()
    {
        base.Awake();

        _fixeWidth = Mathf.Abs(scrollRect.viewport.rect.width);
    }


    public override void SetPivotAndAnchor(Cell_Base cell)
    {
       cell.RectTransform.anchorMin = new Vector2(0, 1);
       cell.RectTransform.anchorMax = new Vector2(0, 1);
       cell.RectTransform.pivot = new Vector2(0, 1);
    }

    protected override void OnValueChanged(Vector2 normalizedPosition)
    {
        if (rowCount <= 0)
            rowCount = 1;

        float viewportInterval = useWidthChangeAnimation ? _fixeWidth : Mathf.Abs(scrollRect.viewport.rect.width);
        float minViewport = -scrollRect.content.anchoredPosition.x;
        Vector2 viewportRange = new(minViewport - extendVisibleRange, minViewport + viewportInterval + extendVisibleRange);
        float contentWidth = startPadding;

        for (int i = 0; i < dataList.Count; i += rowCount)
        {
            for (int j = 0; j < rowCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                Vector2 visibleRange = new(contentWidth, contentWidth + dataList[index].cellSize.x);

                if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                    RecycleCell(index);
            }

            contentWidth += dataList[i].cellSize.x + spacing;
        }

        contentWidth = startPadding;

        for (int i = 0; i < dataList.Count; i += rowCount)
        {
            for (int j = 0; j < rowCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                Vector2 visibleRange = new(contentWidth, contentWidth + dataList[index].cellSize.x);

                if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                {
                    SetupCell(index, new Vector2(contentWidth, (dataList[index].cellSize.y + spacing) * -j - paddingLT.y));

                    if (visibleRange.y >= viewportRange.x)
                        _dicIndexToCell[index].transform.SetAsLastSibling();
                    else
                        _dicIndexToCell[index].transform.SetAsFirstSibling();
                }
            }

            contentWidth += dataList[i].cellSize.x + spacing;
        }

        if (scrollRect.content.sizeDelta.x > viewportInterval && dataList.Count > 0)
        {
            isAtLeft = viewportRange.x + extendVisibleRange < dataList[0].cellSize.x;
            isAtRight = scrollRect.content.sizeDelta.x - viewportRange.y + extendVisibleRange < dataList[^1].cellSize.x;
        }
        else
        {
            isAtLeft = true;
            isAtRight = true;
        }

        TryOffOpenFx();
    }

    public sealed override void Refresh()
    {
        if (IsInitialized == false)
            Initialize();

        DoRefresh();
    }

    private void DoRefresh()
    {
        float width = paddingLT.x;
         
        for (int i = 0; i < dataList.Count; i += rowCount)
            width += dataList[i].cellSize.x + spacing;

        var keys = _dicIndexToCell.Keys.ToArray();
        foreach (var key in keys)
            RecycleCell(key);

        width += paddingRB.x;
        scrollRect.content.sizeDelta = new Vector2(width, scrollRect.content.sizeDelta.y);
        OnValueChanged(scrollRect.normalizedPosition);
    }

    public override bool IsVisbibleCell(int index)
    {
        Vector2 cellSize = GetPrefabBoundSize;
        RectTransform viewPort = scrollRect.viewport;
        RectTransform content = scrollRect.content;

        float viewPortRight = content.transform.localPosition.x + viewPort.rect.width;
        float viewPortLeft = content.transform.localPosition.x;
        float cellRight = (index + 1) * cellSize.x;
        float cellLeft = index * cellSize.x;

        bool isVisibleSelectCell = viewPortRight >= cellRight && viewPortLeft <= cellLeft;

        return isVisibleSelectCell;
    }
    
    protected override void DoSnap(int index, float duration, bool center, float offset, bool isReverse = false)
    {
        var columeNumber = index / rowCount;
        var width = columeNumber > 0 ? startPadding : 0;

        for (int i = 0; i < columeNumber; i++)
            width += dataList[i * rowCount].cellSize.x + spacing;
        
        if (center)
            width += (GetPrefabBoundSize.x - scrollRect.viewport.rect.width) * 0.5f;

        width += offset;
        
        if (isReverse == false)
            width = Mathf.Min(Mathf.Max(0, scrollRect.content.rect.width - scrollRect.viewport.rect.width), width);
        else
        {
            width = Mathf.Min(Mathf.Max(0, scrollRect.content.rect.width - scrollRect.viewport.rect.width), width);
            width = scrollRect.content.rect.width - width - scrollRect.viewport.rect.width;
        }

        DoSnap(width, duration);
    }
    
    protected override void DoSnap(float value, float duration)
    {
        if (scrollRect.content.anchoredPosition.x == value)
            return;

        DoSnapping(new Vector2(-value, 0), duration);
    }

    public override void SetPaddingLT(Vector2 padding)
    {
        paddingLT.x = padding.x;
    }

    public override void SetPaddingRB(Vector2 padding)
    {
        paddingRB.x = padding.x;
    }
}

