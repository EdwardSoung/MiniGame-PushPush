using UnityEngine;

public class InfiniteScrollView_H_Reverse : InfiniteScrollView_H
{
    protected override float startPadding => paddingRB.x;
    
    public override void SetPivotAndAnchor(Cell_Base cell)
    {
        cell.RectTransform.anchorMin = Vector2.one;
        cell.RectTransform.anchorMax = Vector2.one;
        cell.RectTransform.pivot = Vector2.one;
    }

    protected override void OnValueChanged(Vector2 normalizedPosition)
    {
        if (rowCount <= 0)
            rowCount = 1;

        float viewportInterval = useWidthChangeAnimation ? _fixeWidth : Mathf.Abs(scrollRect.viewport.rect.width);
        float minViewport = scrollRect.content.anchoredPosition.x;
        Vector2 viewportRange = new Vector2(minViewport - extendVisibleRange, minViewport + viewportInterval + extendVisibleRange);
        float contentWidth = startPadding;

        for (int i = 0; i < dataList.Count; i += rowCount)
        {
            for (int j = 0; j < rowCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                var visibleRange = new Vector2(contentWidth, contentWidth + dataList[index].cellSize.x);

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

                var visibleRange = new Vector2(contentWidth, contentWidth + dataList[index].cellSize.x);

                if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                {
                    SetupCell(index, new Vector2(-contentWidth, (dataList[index].cellSize.y + spacing) * -j - paddingLT.y));

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
            isAtRight = viewportRange.x + extendVisibleRange < dataList[0].cellSize.x;
            isAtLeft = scrollRect.content.sizeDelta.x - viewportRange.y + extendVisibleRange < dataList[^1].cellSize.x;
        }
        else
        {
            isAtRight = true;
            isAtLeft = true;
        }
    }
    
    protected override void DoSnap(float value, float duration)
    {
        if (scrollRect.content.anchoredPosition.x == value)
            return;
        
        DoSnapping(new Vector2(value, 0), duration);
    }
}