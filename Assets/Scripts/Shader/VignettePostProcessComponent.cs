using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable,VolumeComponentMenu("Post-processing/Custom/Vignette Post Process")]
public class VignettePostProcessComponent : VolumeComponent, IPostProcessComponent
{
    public bool IsActive()
    {
        return true;
    }

    public bool IsTileCompatible()
    {
        return false;
    }
    
    [Header("Vignette settings")]
    public FloatParameter Radius = new FloatParameter(0.5f,true);
    public FloatParameter Feathering = new FloatParameter(0.5f,true);
}
