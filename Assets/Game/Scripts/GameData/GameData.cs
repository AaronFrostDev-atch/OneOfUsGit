using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{ 


    public static class ScenesInBuild
    {
        public static readonly string[] scenes = {
         "Main Menu",
         "Prototyping"
     };
    }


    public static class SpawnPositions 
    {
        public static readonly Vector3[] spawns =
        {
            new Vector3(0, 10, 0)
        };
    }


    public static class UserSettings
    {
        public static float mouseSensitivity = 3f;
        public static float mouseSmoothing = .7f;
        public static float volume = 10f;



    }


    public static class WorldLimits
    {
        public static float worldVoidDepth = -100f;
    }


    

}
