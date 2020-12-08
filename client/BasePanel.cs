using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    void Awake()
    {
        this.gameObject.SetActive(false);
    }

    virtual public void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }

    virtual public void HidePanel()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void DestroyPanel()
    {
        MonoBehaviour.DestroyImmediate(this.gameObject);
    }
    // Start is called before the first frame update

    public virtual void TogglePanel()
    {
        this.gameObject.SetActive(this.gameObject.activeSelf);
    }
}
