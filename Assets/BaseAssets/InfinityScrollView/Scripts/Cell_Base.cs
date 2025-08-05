using System;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Cell_Base : MonoBehaviour
{
    private readonly CompositeDisposable _subscribers = new();

    protected readonly Subject<Cell_Base> _eventOnClickSubject = new();
    public IObservable<Cell_Base> OnEventClickTrigger => _eventOnClickSubject;

    protected readonly Subject<Cell_Base> _eventPointerDownSubject = new();
    public IObservable<Cell_Base> OnEventPointerDownTrigger => _eventPointerDownSubject;

    protected readonly Subject<Cell_Base> _eventOnPointerUpSubject = new();
    public IObservable<Cell_Base> OnEventPointerUpTrigger => _eventOnPointerUpSubject;

    protected readonly Subject<Cell_Base> _eventOnPointerEnterSubject = new();
    public IObservable<Cell_Base> OnEventPointerEnterTrigger => _eventOnPointerEnterSubject;

    protected readonly Subject<Cell_Base> _eventOnPointerExitSubject = new();
    public IObservable<Cell_Base> OnEventPointerExitTrigger => _eventOnPointerExitSubject;

    public RectTransform RectTransform => transform as RectTransform;

    [SerializeField] protected bool useOpenTween;
    [SerializeField] protected GameObject _openTween;

    private void Awake()
    {

        if (TryGetComponent<Button>(out var button))
        {
            button.OnClickAsObservable().Subscribe(_ =>
           {
               _eventOnClickSubject.OnNext(this);

           }).AddTo(_subscribers);

            button.OnPointerDownAsObservable().Subscribe(_ =>
            {
                _eventPointerDownSubject.OnNext(this);

            }).AddTo(_subscribers);

            button.OnPointerUpAsObservable().Subscribe(_ =>
            {
                _eventOnPointerUpSubject.OnNext(this);

            }).AddTo(_subscribers);

            button.OnPointerEnterAsObservable().Subscribe(_ =>
            {
                _eventOnPointerEnterSubject.OnNext(this);

            }).AddTo(_subscribers);

            button.OnPointerExitAsObservable().Subscribe(_ =>
            {
                _eventOnPointerExitSubject.OnNext(this);

            }).AddTo(_subscribers);
        }

        if (_openTween != null)
            _openTween.SetActive(false);
    }

    private void OnDestroy()
    {
        _subscribers.Dispose();

        _eventOnClickSubject.Dispose();

        _eventPointerDownSubject.Dispose();

        _eventOnPointerUpSubject.Dispose();

        _eventOnPointerEnterSubject.Dispose();

        _eventOnPointerExitSubject.Dispose();

    }

    public void PlayOpenFx(bool isPlay = false)
    {
        if (useOpenTween == false || _openTween == null)
            return;

        _openTween.SetActive(false);
        if (isPlay)
            _openTween.SetActive(true);
    }
}
