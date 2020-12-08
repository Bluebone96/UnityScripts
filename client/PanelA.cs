using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelA : MonoBehaviour
{
    public void OnButtonClick()
    {
        UIManager.Instance.ClosePanel(this.name);
    }
}
