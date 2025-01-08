using FirstVillain.Entities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RewardItem : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GameObject _bonusIcon;

    public void SetData(JItemData item, int count, bool isBonus = false)
    {
        var icon = AddressableManager.Instance.LoadAssetAsync<Sprite>(item.ResourceName);
        _icon.sprite = icon;
        _icon.SetNativeSize();

        _countText.text = string.Format("{0:#,###}", count);

        _bonusIcon.SetActive(isBonus);
    }
}
