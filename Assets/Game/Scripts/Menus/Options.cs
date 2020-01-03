using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Options : MonoBehaviour
{
    public void ChangeSensitivity(float newSensitivity)
    {
        // Change 
        GameData.UserSettings.mouseSensitivity = newSensitivity * 2;

        // Display 
        GameObject.Find("Sensitivity Text").GetComponent<TextMeshProUGUI>().text = "Sensitivity: " + Math.Round(newSensitivity, 1);
    }

    public void ChangeSmoothing(float newSmoothing)
    {
        // Change 
        GameData.UserSettings.mouseSmoothing = newSmoothing;

        // Display 
        GameObject.Find("Smoothing Text").GetComponent<TextMeshProUGUI>().text = "Mouse Smoothing: " + Math.Round(newSmoothing, 1);
    }


    public void ChangeVolume(float newVolume)
    {
        // Change 
        GameData.UserSettings.volume = newVolume;

        // Display 
        GameObject.Find("Volume Text").GetComponent<TextMeshProUGUI>().text = "Volume: " + Math.Round(newVolume, 1);
    }
}
