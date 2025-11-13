using UnityEngine;
using System;

public class PlanetShotEvents
{
    public static Action<float, Vector3, Vector3> OnRelease;

    public static void InvokeOnRelease(float tension, Vector3 centerPos, Vector3 pullPos)
    {
        OnRelease?.Invoke(tension, centerPos, pullPos);
    }
}
