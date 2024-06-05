using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class ResourceView : MonoBehaviour
{

    public TMP_Text text;

    [Client]
    public void UpdateText(int newValue)
    {
        text.text = newValue.ToString();
    }
}
