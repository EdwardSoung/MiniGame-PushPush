using FirstVillain.Entities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointItem : MonoBehaviour
{
    [SerializeField] private Transform _propRoot;
    [SerializeField] private TextMeshProUGUI _propPointText;
    [SerializeField] private TextMeshProUGUI _propAmountText;
    [SerializeField] private TextMeshProUGUI _propTotalPointText;

    [SerializeField] private Renderer _renderer;

    public void SetData(JPropInfoData data, int count)
    {
        gameObject.SetActive(false);
        var material = AddressableManager.Instance.LoadAssetAsync<Material>(data.MaterialName);
        var renderers = _renderer.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material = material;
        }
        gameObject.SetActive(true);
        _propAmountText.text = $"x {count}";
        _propPointText.text = $"[{data.Point}]";
        _propTotalPointText.text = string.Format("{0:#,###}", count * data.Point);
    }
}
