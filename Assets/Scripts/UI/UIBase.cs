using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public E_UI_ROOT_TYPE UI_ROOT_TYPE { get; private set; }

    public void SetUIBaseType(E_UI_ROOT_TYPE type)
    {
        UI_ROOT_TYPE = type;
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        UIManager.Instance.CloseUI();
    }

    public virtual void CloseAction()
    {

    }
}
