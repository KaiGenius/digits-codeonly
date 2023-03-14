using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScaleFormulaSO : ScriptableObject
{
    public float baseScale = 2;
    public float scalePerNumber => (limitScale - baseScale) / limitScaleAtNumber;
    public int limitScaleAtNumber = 1000;
    public float limitScale = 12;

    public float CalculateScale(int number)
    {
        var output = baseScale + (number - 1) * scalePerNumber;
        output = Mathf.Clamp(output, baseScale, limitScale);
        return output;
    }

    public float SpeedEvaulate(float currentScale)
    {
        currentScale = Mathf.Clamp(currentScale, baseScale, limitScale);
        return currentScale / limitScale;
    }

    public float ClampedScale(int number)
    {
        var output = baseScale + (number - 1) * scalePerNumber;
        output = Mathf.Clamp(output, baseScale, limitScale);
        return (output - baseScale) / (limitScale - baseScale);
    }
}
