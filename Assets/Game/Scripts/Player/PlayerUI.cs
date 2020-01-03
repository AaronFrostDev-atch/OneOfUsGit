using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    RawImage pointer;
    Color pointerColor = Color.white;
    
    void Start()
    {
        pointer = GameObject.Find("Pointer").GetComponent<RawImage>();
        pointerColor.a = 0;
    }


    public void ShowPointer(bool show)
    {


        if (show)
        {
            pointerColor.a = Mathf.Lerp(pointerColor.a, 1, .1f);
        }
        else
        {
            pointerColor.a = Mathf.Lerp(pointerColor.a, 0, .1f);
        }

        pointer.color = pointerColor;
    }
}
