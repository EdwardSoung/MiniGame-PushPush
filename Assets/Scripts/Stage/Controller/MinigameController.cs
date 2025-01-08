using FirstVillain.EventBus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameController : MonoBehaviour
{
    [SerializeField] private RectTransform _bgRect;
    [SerializeField] private Slider _gauge;

    private float _moveSpeed = 3;
    private bool _isStop = false;

    private Coroutine _moveCoroutine;
    private void Start()
    {
        EventBus.Instance.Subscribe<EventMinigameStop>(OnMinigameStop);
    }

    public void StartGame()
    {
        _gauge.gameObject.SetActive(true);
        _gauge.value = 0;
        _isStop = false;
        if(_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }
        _moveCoroutine = StartCoroutine(MoveHolder());
    }

    private void OnMinigameStop(EventMinigameStop e)
    {
        _isStop = true;
    }
    private IEnumerator MoveHolder()
    {
        float value = 0;
        int dir = 1;
        while(!_isStop)
        {
            value += Time.deltaTime * _moveSpeed * dir;
            if(value > 1f)
            {
                value = 1f;
                dir = -1;
            }
            if(value < 0)
            {
                value = 0;
                dir = 1;
            }
            _gauge.value = value;
            yield return null;
        }

        float point = _gauge.value;
        _gauge.gameObject.SetActive(false);
        EventBus.Instance.Publish(new EventSendMinigamePoint(point));
    }
}
