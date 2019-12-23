using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetGoButton : MonoBehaviour
{
    public bool letGoButton;

    public void onPress()
    {
        letGoButton = true;
    }
}
