using System.Linq;
using UnityEngine;

public class InfiniteScrollView_V : InfiniteScrollView
{ 
    [HideInInspector] public bool isAtTop = true;
    [HideInInspector] public bool isAtBottom = true;

    public int columeCount = 1;

    public bool useHeightChangeAnimation;
    protected float _fixedHeight;

    protected virtual float startPadding => paddingLT.y;
    public bool IsAtContentInViewprot => scrollRect.viewport.rect.height >= scrollRect.content.rect.height;

    protected override void Awake()
    {
        base.Awake();

        _fixedHeight = Mathf.Abs(scrollRect.viewport.rect.height);
    }

    public override void SetPivotAndAnchor(Cell_Base cell)
    {
        cell.RectTransform.anchorMin = new Vector2(0, 1);
        cell.RectTransform.anchorMax = new Vector2(0, 1);
        cell.RectTransform.pivot = new Vector2(0, 1);
    }

    protected override void OnValueChanged(Vector2 normalizedPosition)
    {
        if (columeCount <= 0)
            columeCount = 1;

        float viewportInterval = useHeightChangeAnimation ? _fixedHeight : Mathf.Abs(scrollRect.viewport.rect.height);
        float minViewport = scrollRect.content.anchoredPosition.y;
        Vector2 viewportRange = new(minViewport, minViewport + viewportInterval);
        float contentHeight = paddingLT.y;

        for (int i = 0; i < dataList.Count; i += columeCount)
        {
            for (int j = 0; j < columeCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                Vector2 visibleRange = new(contentHeight, contentHeight + dataList[index].cellSize.y);

                if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                    RecycleCell(index);
            }

            contentHeight += dataList[i].cellSize.y + spacing;
        }

        contentHeight = paddingLT.y;

        for (int i = 0; i < dataList.Count; i += columeCount)
        {
            for (int j = 0; j < columeCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                var visibleRange = new Vector2(contentHeight, contentHeight + dataList[index].cellSize.y);

                if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                {
                    SetupCell(index, new Vector2((dataList[index].cellSize.x + spacing) * j + paddingLT.x, -contentHeight));

                    if (visibleRange.y >= viewportRange.x)
                        _dicIndexToCell[index].transform.SetAsLastSibling();
                    else
                        _dicIndexToCell[index].transform.SetAsFirstSibling();
                }
            }

            contentHeight += dataList[i].cellSize.y + spacing;
        }

        contentHeight += PaddingRB.y;

        if (contentHeight > viewportInterval && dataList.Count > 0)
        {
            isAtTop = viewportRange.x + extendVisibleRange < dataList[0].cellSize.y;
            isAtBottom = contentHeight - viewportRange.y + extendVisibleRange < dataList[^1].cellSize.y;
        }
        else
        {
            isAtTop = true;
            isAtBottom = true;
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
        float height = paddingLT.y;

        for (int i = 0; i < dataList.Count; i += columeCount)
            height += dataList[i].cellSize.y + spacing;

        var keys = _dicIndexToCell.Keys.ToArray();
        foreach (var key in keys)
            RecycleCell(key);

        height += paddingRB.y;

        scrollRect.content.sizeDelta = new(scrollRect.content.sizeDelta.x, height);

        RefreshChanged();
    }

    public void RefreshChanged()
    {
        OnValueChanged(scrollRect.normalizedPosition);
    }

    public override bool IsVisbibleCell(int index)
    {
        Vector2 cellSize = GetPrefabBoundSize;
        RectTransform viewPort = scrollRect.viewport;
        RectTransform content = scrollRect.content;

        float viewPortTop = content.transform.localPosition.y + viewPort.rect.height;
        float viewPortBottom = content.transform.localPosition.y;
        float cellTop = (index + 1) * cellSize.y;
        float cellBottom = index * cellSize.y;

        bool isVisibleSelectCell = viewPortTop >= cellTop && viewPortBottom <= cellBottom;

        return isVisibleSelectCell;
    }

    protected override void DoSnap(int index, float duration, bool center, float offset, bool isReverse = false)
    {
        var rowNumber = index / columeCount;
        var height = rowNumber > 0 ? startPadding : 0;

        for (int i = 0; i < rowNumber; i++)
            height += dataList[i * columeCount].cellSize.y + spacing;
        
        if (center)
            height += (dataList[rowNumber].cellSize.y - scrollRect.viewport.rect.height) * 0.5f;

        height += offset;

        height = Mathf.Clamp(height, 0, Mathf.Max(0, scrollRect.content.rect.height - scrollRect.viewport.rect.height));

        DoSnap(height, duration);
    }
    
    protected override void DoSnap(float value, float duration)
    {
        if (scrollRect.content.anchoredPosition.y == value)
            return;
        
        DoSnapping(new Vector2(0, value), duration);
    }

    public override void SetPaddingLT(Vector2 padding)
    {
        paddingLT.x = padding.x;
        paddingLT.y = padding.y;
    }

    public override void SetPaddingRB(Vector2 padding)
    {
        paddingRB.x = padding.x;
        paddingRB.y = padding.y;
    }
}