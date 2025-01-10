using System;
using UnityEngine;
namespace FirstVillain.ScrollView
{
    public class InfiniteCell : MonoBehaviour
    {
        public event Action<GameObject> OnSelected;

        private RectTransform _rectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private InfiniteCellData cellData;
        public InfiniteCellData CellData
        {
            set
            {
                cellData = value;
                cellData.OnUpdate(this);
            }
            get
            {
                return cellData;
            }
        }

        public virtual void OnUpdate() { }

        public void InvokeSelected()
        {
            if (OnSelected != null)
                OnSelected.Invoke(gameObject);
        }
    }
}

