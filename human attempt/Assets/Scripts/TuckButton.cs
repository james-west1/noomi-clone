using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuckButton : MonoBehaviour
{
    public bool tuckButton;
    // Start is called before the first frame update
    public void onPress()
    {
        tuckButton = true;
    }
    public void onRelease()
    {
        tuckButton = false;
    }
}
