using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    // Start is called before the first frame update
    public void ContinueGame()
    {
        GameManager.Instance.GameStart();
        UIManager.Instance.ClosePanel(this.name);
    }
    public void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }
}
