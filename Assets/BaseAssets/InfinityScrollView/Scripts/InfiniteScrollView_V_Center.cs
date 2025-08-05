using UnityEngine;

public class InfiniteScrollView_V_Center : InfiniteScrollView_V
{
    protected override void OnValueChanged(Vector2 normalizedPosition)
    {
        if (columeCount <= 0)
            columeCount = 1;

        float viewportInterval = Mathf.Abs(scrollRect.viewport.rect.height);
        float minViewport = scrollRect.content.anchoredPosition.y;
        Vector2 viewportRange = new Vector2(minViewport, minViewport + viewportInterval);
        float contentHeight = paddingLT.y;

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

        contentHeight = paddingLT.y;

        Vector3 scrollRectHalfSize = scrollRect.viewport.rect.max / 2;

        for (int i = 0; i < dataList.Count; i += columeCount)
        {
            for (int j = 0; j < columeCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                var visibleRange = new Vector2(contentHeight, contentHeight + dataList[index].cellSize.y);

                var pos = new Vector2(0, -contentHeight);

                if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                {
                    if (index % columeCount == 0)
                    {
                        var sumSize = 0f;

                        for (int dataIndex = 0; dataIndex < dataList.Count; dataIndex++)
                        {
                            if (index + 1 <= dataIndex && dataIndex < index + columeCount)
                                sumSize += (dataList[dataIndex].cellSize.x / 2) + (spacing / 2);
                        }
                        pos.x = scrollRectHalfSize.x - (dataList[i].cellSize.x / 2) - sumSize;
                    }
                    else
                        pos.x = _dicIndexToCell[index - 1].RectTransform.anchoredPosition.x + dataList[index].cellSize.x + spacing;

                    SetupCell(index, pos);

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
            isAtTop = viewportRange.x + extendVisibleRange < dataList[0].cellSize.y;
            isAtBottom = scrollRect.content.sizeDelta.y - viewportRange.y + extendVisibleRange < dataList[^1].cellSize.y;
        }
        else
        {
            isAtTop = true;
            isAtBottom = true;
        }
    }
}