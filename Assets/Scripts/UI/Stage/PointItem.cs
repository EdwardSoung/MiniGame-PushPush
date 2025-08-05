using FirstVillain.Entities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointItem : Cell_Base
{
    [SerializeField] private TextMeshProUGUI _propPointText;
    [SerializeField] private TextMeshProUGUI _propAmountText;
    [SerializeField] private TextMeshProUGUI _propTotalPointText;

    public void SetData(JPropInfoData data, int count)
    {
        var uiProp = AddressableManager.Instance.Spawn(data.UIPrefabName, transform);
        uiProp.transform.Reset();
        var material = AddressableManager.Instance.LoadAssetAsync<Material>(data.MaterialName);
        var renderers = uiProp.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material = material;
        }
        _propAmountText.SetText($"x {count}");
        _propPointText.SetText($"[{data.Point}]");
        _propTotalPointText.text = string.Format("{0:#,###}", count * data.Point);
    }
}
