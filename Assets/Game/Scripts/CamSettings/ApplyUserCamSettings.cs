using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyUserCamSettings : MonoBehaviour
{
    AudioSource music;


    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }

    private void Update()
    {
        music.volume = GameData.UserSettings.volume;
    }
}
