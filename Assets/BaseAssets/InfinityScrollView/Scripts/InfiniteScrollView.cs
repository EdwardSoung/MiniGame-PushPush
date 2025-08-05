using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public abstract class InfiniteScrollView : UIBehaviour
{
    private readonly CompositeDisposable _subscribers = new();

    public struct InfiniteCellData
    {
        public Vector2 cellSize;

        public InfiniteCellData(float x, float y)
        {
            this.cellSize = new Vector2(x, y);
        }

        public InfiniteCellData(Vector2 cellSize)
        {
            this.cellSize = cellSize;
        }
    }

    protected bool IsInitialized = false;

    [SerializeField] private int cellPoolSize = 20;

    [SerializeField] protected float spacing = 0f;
    [FormerlySerializedAs("padding")][SerializeField] protected Vector2 paddingLT;
    [SerializeField] protected Vector2 paddingRB;
    [SerializeField] protected float extendVisibleRange;

    private ObjectPool<Cell_Base> cellPooling;
    [SerializeField] private Cell_Base cellPrefab;

    [SerializeField] protected bool useFirstOpenCellFx;

    [Tooltip("Cell 전체연출시간")][Min(0.1f)][SerializeField] protected float OpenCellFxDurationSec;

    protected ScrollRect scrollRect;
    protected readonly List<InfiniteCellData> dataList = new();

    protected readonly Dictionary<int, Cell_Base> _dicIndexToCell = new();
    protected readonly Dictionary<Cell_Base, int> _dicCellToIndex = new();

    private readonly Subject<(int, Cell_Base)> _eventCellUpdateSubject = new();
    public IObservable<(int, Cell_Base)> EventCellUpdateTrigger => _eventCellUpdateSubject;

    private readonly Subject<(int, Cell_Base)> _eventCellClickSubject = new();
    public IObservable<(int, Cell_Base)> EventCellClickTrigger => _eventCellClickSubject;

    private readonly Subject<(int, Cell_Base)> _eventCellClickDownSubject = new();
    public IObservable<(int, Cell_Base)> EventCellClickDownTrigger => _eventCellClickDownSubject;

    private readonly Subject<(int, Cell_Base)> _eventCellClickUpSubject = new();
    public IObservable<(int, Cell_Base)> EventCellClickUpTrigger => _eventCellClickUpSubject;

    private CancellationTokenSource snappingCts = new();

    public Vector2 GetPrefabBoundSize => cellPrefab.RectTransform.rect.size;
    public Cell_Base GetPrefab => cellPrefab;
    public float Spacing => spacing;
    public ScrollRect ScrollRect => scrollRect;
    public int GetDataCount => dataList.Count;
    public Vector2 PaddingLT => paddingLT;
    public Vector2 PaddingRB => paddingRB;

    protected bool _isFirstOpen;
    protected CompositeDisposable _subscribeCellFx = new();

    protected override void Awake()
    {
        base.Awake();

        if (IsInitialized == false)
            Initialize();

        else
            OnOpenFx();
    }

    protected virtual void Initialize()
    {
        scrollRect = GetComponent<ScrollRect>();
        scrollRect.OnValueChangedAsObservable()
                  .Subscribe(OnValueChanged)
                  .AddTo(_subscribers);

        scrollRect.SetLayoutHorizontal();
        scrollRect.SetLayoutVertical();

        cellPooling = new ObjectPool<Cell_Base>(
        () =>
        {
            var cellBase = Instantiate(cellPrefab, scrollRect.content);

            SetPivotAndAnchor(cellBase);

            cellBase.OnEventClickTrigger.Subscribe(cellBase =>
            {
                _eventCellClickSubject.OnNext((_dicCellToIndex[cellBase], cellBase));

            }).AddTo(_subscribers);

            cellBase.OnEventPointerDownTrigger.Subscribe(cellBase =>
            {
                _eventCellClickDownSubject.OnNext((_dicCellToIndex[cellBase], cellBase));

            }).AddTo(_subscribers);

            cellBase.OnEventPointerUpTrigger.Subscribe(cellBase =>
            {
                _eventCellClickUpSubject.OnNext((_dicCellToIndex[cellBase], cellBase));

            }).AddTo(_subscribers);


            return cellBase;
        },
        go =>
        {
            go.gameObject.SetActive(true);
        },
        go =>
        {
            go.gameObject.SetActive(false);
        },
        go =>
        {
            Destroy(go.gameObject);
        }
        , false, 10, cellPoolSize);

        IsInitialized = true;
        _isFirstOpen = true;
    }

    protected override void OnDestroy()
    {
        _subscribers.Dispose();

        dataList.Clear();

        _dicIndexToCell.Clear();
        _dicCellToIndex.Clear();

        cellPooling?.Dispose();
        cellPooling = null;

        _eventCellClickSubject.Dispose();

        _eventCellUpdateSubject.Dispose();

        _eventCellClickDownSubject.Dispose();

        _eventCellClickUpSubject.Dispose();

        snappingCts.Cancel();
        snappingCts.Dispose();

        _subscribeCellFx.Dispose();

        base.OnDestroy();
    }

    protected abstract void OnValueChanged(Vector2 normalizedPosition);

    public abstract void Refresh();

    public abstract void SetPivotAndAnchor(Cell_Base cell);

    protected abstract void DoSnap(int index, float duration, bool center, float offset, bool isReverse = false);

    protected abstract void DoSnap(float width, float duration);

    public void CellRefreshUpdate()
    {
        if (IsInitialized == false)
            Initialize();

        foreach (var item in _dicIndexToCell)
            _eventCellUpdateSubject.OnNext((item.Key, item.Value));
    }

    public IEnumerable<(int, Cell_Base)> GetCells()
    {
        foreach (var cell in _dicIndexToCell)
            yield return (cell.Key, cell.Value);
    }

    public void CellRefreshUpdateAtIndex(int index)
    {
        if (IsInitialized == false)
            Initialize();

        if (index < 0)
            return;

        if (_dicIndexToCell.TryGetValue(index, out var cell))
        {
            _eventCellUpdateSubject.OnNext((index, cell));
        }
    }

    public void CellRefrshSelectAtIndex(int index)
    {
        if (IsInitialized == false)
            Initialize();

        if (_dicIndexToCell.TryGetValue(index, out var cell))
        {
            _eventCellClickSubject.OnNext((index, cell));
        }
    }

    public void Add(Vector2 cellSize)
    {
        Add(new InfiniteCellData(cellSize));
    }

    public void Add(InfiniteCellData data)
    {
        if (IsInitialized == false)
            Initialize();

        dataList.Add(data);
    }

    public void AddAt(int index, Vector2 cellSize)
    {
        AddAt(index, new InfiniteCellData(cellSize));
    }

    public void AddAt(int index, InfiniteCellData data)
    {
        if (IsInitialized == false)
            Initialize();

        dataList.Insert(index, data);
    }

    public void Remove(int index, int count)
    {
        if (IsInitialized == false)
            Initialize();

        if (dataList.Count <= index)
            return;

        var maxIndex = index + count;
        if (maxIndex > dataList.Count)
        {
            count -= maxIndex - dataList.Count;
        }

        for (int i = 0; i < count; ++i)
        {
            dataList.RemoveAt(index);
            RecycleCell(index);
        }
    }

    public virtual void Remove(int index)
    {
        if (IsInitialized == false)
            Initialize();

        if (dataList.Count == 0)
            return;

        dataList.RemoveAt(index);
        RecycleCell(index);

        Refresh();
    }

    public abstract bool IsVisbibleCell(int index);

    public int GetVisibleCellCount()
    {
        Vector2 cellSize = cellPrefab.RectTransform.rect.size;
        RectTransform viewPort = scrollRect.viewport;
        int visibleColumnDataCount = (int)(viewPort.rect.width / (cellSize.x + spacing));
        int visibleRowDataCount = (int)(viewPort.rect.height / (cellSize.y + spacing));

        return visibleColumnDataCount * visibleRowDataCount;
    }

    public void SnapLast(float duration)
    {
        Snap(dataList.Count - 1, duration);
    }

    protected virtual void DoSnapping(Vector2 target, float duration)
    {
        if (gameObject.activeInHierarchy == false)
            return;

        ProcessSnapping(target, duration).Forget();
    }

    private async UniTask ProcessSnapping(Vector2 target, float duration)
    {
        if (duration == 0f)
        {
            scrollRect.content.anchoredPosition = target;
            OnValueChanged(scrollRect.normalizedPosition);
            return;
        }

        snappingCts.Cancel();
        snappingCts.Dispose();
        snappingCts = new CancellationTokenSource();

        scrollRect.velocity = Vector2.zero;
        Vector2 startPos = scrollRect.content.anchoredPosition;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            scrollRect.content.anchoredPosition = Vector2.Lerp(startPos, target, t);

            var normalizedPos = scrollRect.normalizedPosition;

            if (normalizedPos.y < 0 || normalizedPos.x > 1)
                break;

            if (t >= 1f)
                break;

            await UniTask.Yield(PlayerLoopTiming.Update, snappingCts.Token);
        }
    }

    protected void TryOffOpenFx()
    {
        if (useFirstOpenCellFx && _subscribeCellFx.Count > 0)
        {
            _subscribeCellFx.Dispose();
            _isFirstOpen = false;
            foreach (var cellItem in _dicIndexToCell)
            {
                cellItem.Value.gameObject.SetActive(true);
                cellItem.Value.PlayOpenFx(true);
                _eventCellUpdateSubject.OnNext((cellItem.Key, cellItem.Value));
            }
        }
    }

    protected void OnOpenFx()
    {
        if (useFirstOpenCellFx && _isFirstOpen)
        {
            foreach (var item in _dicIndexToCell)
                PlayOpenCellFx(item.Key, item.Value, _dicIndexToCell.Count);

            _isFirstOpen = false;
        }
        else
        {
            OnValueChanged(new Vector2(0, 0));
        }
    }

    protected void PlayOpenCellFx(int index, Cell_Base cell, int cellCount)
    {
        cell.gameObject.SetActive(false);
        var openDelayTime = OpenCellFxDurationSec / cellCount * index;
        Observable.Timer(TimeSpan.FromSeconds(openDelayTime)).Subscribe(_ =>
        {
            _eventCellUpdateSubject.OnNext((index, cell));
            cell.gameObject.SetActive(true);
            cell.PlayOpenFx(true);
        }).AddTo(_subscribeCellFx);
    }

    protected void SetupCell(int index, Vector2 pos)
    {
        if (_dicIndexToCell.TryGetValue(index, out var cell) == false)
        {
            cell = cellPooling.Get();
            cell.RectTransform.anchoredPosition = pos;

            _dicIndexToCell.Add(index, cell);
            _dicCellToIndex.Add(cell, index);

            if (useFirstOpenCellFx && _isFirstOpen)
                cell.gameObject.SetActive(false);
            else
                _eventCellUpdateSubject.OnNext((index, cell));
        }
    }

    protected void RecycleCell(int index)
    {
        if (_dicIndexToCell.ContainsKey(index) == true)
        {
            var cell = _dicIndexToCell[index];

            _dicIndexToCell.Remove(index);
            _dicCellToIndex.Remove(cell);

            cellPooling.Release(cell);
        }
    }

    public virtual void ClearData()
    {
        if (IsInitialized == false)
            Initialize();

        dataList.Clear();

        var keys = _dicIndexToCell.Keys.ToArray();
        foreach (var key in keys)
            RecycleCell(key);

        _dicIndexToCell.Clear();
        _dicCellToIndex.Clear();
    }

    public virtual void Clear()
    {
        if (IsInitialized == false)
            Initialize();

        scrollRect.velocity = Vector2.zero;
        scrollRect.content.anchoredPosition = Vector2.zero;

        dataList.Clear();

        var keys = _dicIndexToCell.Keys.ToArray();
        foreach (var key in keys)
            RecycleCell(key);

        _dicIndexToCell.Clear();
        _dicCellToIndex.Clear();

        Refresh();
    }

    public Vector2 GetCellSize(int index)
    {
        return dataList[index].cellSize;
    }

    public Vector2 GetPostion()
    {
        return scrollRect.content.anchoredPosition;
    }

    public void Snap(int index, float duration, bool center = false, float offset = 0, bool isReverse = false)
    {
        if (IsInitialized == false)
            Initialize();

        if (index >= dataList.Count)
            return;

        DoSnap(index, duration, center, offset, isReverse);
    }

    public void Snap(float width, float duration)
    {
        if (IsInitialized == false)
            Initialize();

        DoSnap(width, duration);
    }

    public virtual void SetPaddingLT(Vector2 padding)
    {
        paddingLT = padding;
    }

    public virtual void SetPaddingRB(Vector2 padding)
    {
        paddingRB = padding;
    }
}
