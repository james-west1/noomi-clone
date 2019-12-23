using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchButton : MonoBehaviour {
    public bool archButton;

    public void onPress()
    {
        archButton = true;
        Debug.Log("true!");
    }
    public void onRelease()
    {
        archButton = false;
        Debug.Log("false!");
    }
}
