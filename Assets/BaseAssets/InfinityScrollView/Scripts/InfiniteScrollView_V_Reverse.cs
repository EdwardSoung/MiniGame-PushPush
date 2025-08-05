using UnityEngine;

public class InfiniteScrollView_V_Reverse : InfiniteScrollView_V
{
    protected override float startPadding => paddingRB.y;
    
    public override void SetPivotAndAnchor(Cell_Base cell)
    {
        cell.RectTransform.anchorMin = Vector2.zero;
        cell.RectTransform.anchorMax = Vector2.zero;
        cell.RectTransform.pivot = Vector2.zero;
    }

    protected override void OnValueChanged(Vector2 normalizedPosition)
    {
        if (columeCount <= 0)
            columeCount = 1;

        float viewportInterval = useHeightChangeAnimation ? _fixedHeight : Mathf.Abs(scrollRect.viewport.rect.height);
        float minViewport = -scrollRect.content.anchoredPosition.y;
        Vector2 viewportRange = new Vector2(minViewport, minViewport + viewportInterval);
        float contentHeight = paddingRB.y;

        for (int i = 0; i < dataList.Count; i += columeCount)
        {
            for (int j = 0; j < columeCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                var visibleRange = new Vector2(contentHeight, contentHeight + dataList[index].cellSize.y);

                if (visibleRange.y < viewportRange.x || visibleRange.x > viewportRange.y)
                    RecycleCell(index);
            }

            contentHeight += dataList[i].cellSize.y + spacing;
        }

        contentHeight = paddingRB.y;

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
                    SetupCell(index, new Vector2((dataList[index].cellSize.x + spacing) * j + paddingLT.x, contentHeight));

                    if (visibleRange.y >= viewportRange.x)
                        _dicIndexToCell[index].transform.SetAsLastSibling();
                    else
                        _dicIndexToCell[index].transform.SetAsFirstSibling();
                }
            }

            contentHeight += dataList[i].cellSize.y + spacing;
        }

        if (scrollRect.content.sizeDelta.y > viewportInterval && dataList.Count > 0)
        {
            isAtTop = scrollRect.content.sizeDelta.y - viewportRange.y + extendVisibleRange < dataList[^1].cellSize.y;
            isAtBottom = viewportRange.x + extendVisibleRange < dataList[0].cellSize.y;
        }
        else
        {
            isAtTop = true;
            isAtBottom = true;
        }
    }
    
    //버티컬, 리버스 스크롤은 테스트 못해봄
    protected override void DoSnap(float value, float duration)
    {
        if (scrollRect.content.anchoredPosition.y == value)
            return;
        
        DoSnapping(new Vector2(0, value), duration);
    }
}