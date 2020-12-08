using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proto.Unity;
using Google.Protobuf;
using Utility;
public class LoginPanel : MonoBehaviour
{
    public InputField m_name;
    public InputField m_pass;


    public void OnButtonClick()
    {
        GameManager.Instance.LoginVerify(m_name.text, m_pass.text);
    }
}
