using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDetails : MonoBehaviour
{

    public string detailText;
    public GameObject tipPrefab;
    GameObject tip;
    bool showingTip = false;
    public Vector3 tipOffset = new Vector3(0, .2f, 0);


    private void OnMouseOver()
    {
        
        if (!showingTip)
        {
            tip = Instantiate(tipPrefab, transform);
            tip.GetComponent<TextMeshProUGUI>().text = detailText;
            tip.transform.position = transform.position + tipOffset;
            showingTip = true;
        }
    }

    private void OnMouseExit()
    {
        showingTip = false;
        Destroy(tip);
    }



}
