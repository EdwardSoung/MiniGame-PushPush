using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelLoadingIntro : UIBase
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _loadingText;

    private void Start()
    {
        _loadingSlider.value = 0;
        _loadingText.text = "0%";

        EventBus.Instance.Subscribe<EventUpdateTableLadingProgress>(OnUpdateLoadingGauge);
    }
    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<EventUpdateTableLadingProgress>(OnUpdateLoadingGauge);
    }

    //게이지에 넣을게 미확실..
    private void OnUpdateLoadingGauge(EventUpdateTableLadingProgress e)
    {
        _loadingSlider.value = e.Progress;
        _loadingText.text = $"{(int)(e.Progress * 100)}%";
    }
}
