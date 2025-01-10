using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FirstVillain.ScrollView
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class InfiniteScrollView : UIBehaviour
    {
        [SerializeField] private int _cellPoolSize = 20;
        [SerializeField] protected float _spacing = 0f;
        [SerializeField] protected Vector2 _padding;
        [SerializeField] protected float _extendVisibleRange;

        [SerializeField] private InfiniteCell _cellPrefab;
        [SerializeField] protected ScrollRect _scrollRect;
        
        protected List<InfiniteCellData> _dataList = new List<InfiniteCellData>();
        protected List<InfiniteCell> _cellList = new List<InfiniteCell>();
        protected Queue<InfiniteCell> _cellPool = new Queue<InfiniteCell>();
        protected YieldInstruction _waitEndOfFrame = new WaitForEndOfFrame();
        private Coroutine _snappingProcesser;

        public event Action OnRectTransformUpdate;
        public event Action<GameObject> OnCellSelected;
        public Action OnRefresh;

        public bool IsInitialized
        {
            get;
            private set;
        }

        public virtual void Initialize()
        {
            if (IsInitialized)
                return;
            _scrollRect = GetComponent<ScrollRect>();
            _scrollRect.onValueChanged.AddListener(OnValueChanged);
            for (int i = 0; i < _cellPoolSize; i++)
            {
                var newCell = Instantiate(_cellPrefab, _scrollRect.content);
                newCell.gameObject.SetActive(false);
                _cellPool.Enqueue(newCell);
            }
            IsInitialized = true;
        }

        protected abstract void OnValueChanged(Vector2 normalizedPosition);

        public abstract void Refresh();

        public virtual void Add(InfiniteCellData data)
        {
            if (!IsInitialized)
            {
                Initialize();
            }
            data.Index = _dataList.Count;
            _dataList.Add(data);
            _cellList.Add(null);
        }

        public virtual void Remove(int index)
        {
            if (!IsInitialized)
            {
                Initialize();
            }
            if (_dataList.Count == 0)
                return;
            _dataList.RemoveAt(index);
            Refresh();
        }

        public abstract void Snap(int index, float duration);

        public void SnapLast(float duration)
        {
            Snap(_dataList.Count - 1, duration);
        }

        protected void DoSnapping(Vector2 target, float duration)
        {
            if (!gameObject.activeInHierarchy)
                return;
            StopSnapping();
            _snappingProcesser = StartCoroutine(ProcessSnapping(target, duration));
        }

        public void StopSnapping()
        {
            if (_snappingProcesser != null)
            {
                StopCoroutine(_snappingProcesser);
                _snappingProcesser = null;
            }
        }

        private IEnumerator ProcessSnapping(Vector2 target, float duration)
        {
            _scrollRect.velocity = Vector2.zero;
            Vector2 startPos = _scrollRect.content.anchoredPosition;
            float t = 0;
            while (t < 1f)
            {
                if (duration <= 0)
                    t = 1;
                else
                    t += Time.deltaTime / duration;
                _scrollRect.content.anchoredPosition = Vector2.Lerp(startPos, target, t);
                var normalizedPos = _scrollRect.normalizedPosition;
                if (normalizedPos.y < 0 || normalizedPos.x > 1)
                {
                    break;
                }
                yield return null;
            }
            if (duration <= 0)
                OnValueChanged(_scrollRect.normalizedPosition);
            _snappingProcesser = null;
        }

        protected void SetupCell(int index, Vector2 pos)
        {
            if (_cellList[index] == null)
            {
                var cell = _cellPool.Dequeue();
                cell.gameObject.SetActive(true);
                cell.CellData = _dataList[index];
                cell.RectTransform.anchoredPosition = pos;
                _cellList[index] = cell;
                cell.OnSelected += OnCellObjSelected;
            }
        }

        protected void RecycleCell(int index)
        {
            if (_cellList[index] != null)
            {
                var cell = _cellList[index];
                _cellList[index] = null;
                _cellPool.Enqueue(cell);
                cell.gameObject.SetActive(false);
                cell.OnSelected -= OnCellObjSelected;
            }
        }

        private void OnCellObjSelected(GameObject selectedCell)
        {
            OnCellSelected?.Invoke(selectedCell);
        }

        public virtual void Clear()
        {
            if (IsInitialized == false)
                Initialize();
            _scrollRect.velocity = Vector2.zero;
            _scrollRect.content.anchoredPosition = Vector2.zero;
            _dataList.Clear();
            for (int i = 0; i < _cellList.Count; i++)
            {
                RecycleCell(i);
            }
            _cellList.Clear();
            Refresh();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if (_scrollRect)
            {
                OnRectTransformUpdate?.Invoke();
            }
        }
    }
}