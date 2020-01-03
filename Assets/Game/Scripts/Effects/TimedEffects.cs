using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

public class TimedEffects : MonoBehaviour
{
    public bool useEffects = true;
    Volume volume;
    bool hasWarped = false;
    bool fadeInWarp = true;
    LensDistortion tempLens;
    public float maxLensIntensity = .2f;
    public bool spawnedIn = false;


    void Start()
    {
        volume = GetComponent<Volume>();

        // Get volume override to alter
        if (volume.profile.TryGet<LensDistortion>(out tempLens))
        {
            tempLens.intensity.value = 0;
        }
    }

    private void Update()
    {
        if (useEffects)
        {
            WarpCamera();

            // Call more effects here
        }
        
    }


    void WarpCamera()
    {
        if (spawnedIn)
        {

            if (!hasWarped)
            {
                if (fadeInWarp & tempLens.intensity.value > -maxLensIntensity)
                {
                    tempLens.intensity.Interp(tempLens.intensity.value, -maxLensIntensity, .05f);

                    if (tempLens.intensity.value + maxLensIntensity < .1f)
                    {
                        fadeInWarp = false;
                    }

                }


                if (!fadeInWarp)
                {
                    tempLens.intensity.Interp(tempLens.intensity.value, 0f, .01f);
                    if (tempLens.intensity.value > .01 & tempLens.intensity.value < .01)
                    {
                        hasWarped = true;
                        tempLens.intensity.value = 0;
                    }
                }
            }

        }
    }




}
