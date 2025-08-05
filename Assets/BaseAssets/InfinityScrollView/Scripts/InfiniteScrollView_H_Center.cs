using UnityEngine;

public class InfiniteScrollView_H_Center : InfiniteScrollView_H
{
    protected override void OnValueChanged(Vector2 normalizedPosition)
    {
        if (rowCount <= 0)
            rowCount = 1;

        float viewportInterval = Mathf.Abs(scrollRect.viewport.rect.width);
        float minViewport = scrollRect.content.anchoredPosition.x;
        Vector2 viewportRange = new Vector2(minViewport - extendVisibleRange, minViewport + viewportInterval + extendVisibleRange);
        float contentWidth = paddingLT.x;

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

        contentWidth = paddingLT.x;

        Vector3 scrollRectHalfSize = scrollRect.viewport.rect.max / 2;

        for (int i = 0; i < dataList.Count; i += rowCount)
        {
            for (int j = 0; j < rowCount; j++)
            {
                int index = i + j;

                if (index >= dataList.Count)
                    break;

                var visibleRange = new Vector2(contentWidth, contentWidth + dataList[index].cellSize.x);

                var pos = new Vector2(contentWidth, 0);

                if (visibleRange.y >= viewportRange.x && visibleRange.x <= viewportRange.y)
                {
                    if (index % rowCount == 0)
                    {
                        var sumSize = 0f;
                        for (int dataIndex = 0; dataIndex < dataList.Count; dataIndex++)
                        {
                            if (index + 1 <= dataIndex && dataIndex < index + rowCount)
                                sumSize += (dataList[dataIndex].cellSize.y / 2) + (spacing / 2);
                        }

                        pos.y = scrollRectHalfSize.y - (dataList[i].cellSize.y / 2) - sumSize;
                    }
                    else
                        pos.y = _dicIndexToCell[index - 1].RectTransform.anchoredPosition.y + dataList[index].cellSize.y + spacing;

                    SetupCell(index, pos);

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
}
