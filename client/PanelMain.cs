using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelMain : MonoBehaviour
{
    public void OnClickSetting()
    {
        UIManager.Instance.ShowPanel("SettingPanel");
        GameManager.Instance.GamePause();
    }

    public void OnClickBag()
    {
        UIManager.Instance.ShowPanel("BagPanel");
    }
}
