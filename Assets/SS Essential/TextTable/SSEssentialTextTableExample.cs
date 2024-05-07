using SS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SSEssentialTextTableExample : MonoBehaviour
{
    public Canvas canvas;

    public void SetLanguage(int id)
    {
        switch(id)
        {
            case 0:
                TextTable.SetCurrentLanguage(SystemLanguage.ChineseTraditional);
                break;
            case 1:
                TextTable.SetCurrentLanguage(SystemLanguage.ChineseSimplified);
                break;
            case 2:
                TextTable.SetCurrentLanguage(SystemLanguage.English);
                break;
        }
    }
}
