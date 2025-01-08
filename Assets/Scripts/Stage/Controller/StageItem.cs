using FirstVillain.Entities;
using FirstVillain.EventBus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageItem : MonoBehaviour
{
    //아이템 세팅
    [SerializeField] private Transform _iconRoot;

    private float _rotateSpeed = 7;

    private int _effectId = 0;

    private GameObject _itemObj;
    public void SetData(JStageItemData table)
    {
        _effectId = table.EffectId;
        var obj = AddressableManager.Instance.Spawn(table.PrefabName, _iconRoot);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        _itemObj = obj;
        StartCoroutine(AutoRotate());
    }
    private IEnumerator AutoRotate()
    {
        while(gameObject.activeInHierarchy)
        {
            _iconRoot.Rotate(Vector3.up, Time.deltaTime * _rotateSpeed, Space.Self);
            yield return null;
        }
    }

    public void UseItem()
    {
        EventBus.Instance.Publish(new EventUseItem(_effectId));
        AddressableManager.Instance.Release(_itemObj);
    }
}
