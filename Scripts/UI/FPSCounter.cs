using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI printer;
    private float[] arrayOfElements = new float[60];
    private int iterator = 0;

    private void Update()
    {
        if(iterator >= arrayOfElements.Length)
        {
            var fps = GetAverageFPS();
            printer.text = $"{fps:0} FPS";
            iterator = 0;
        }
        else
        {
            arrayOfElements[iterator++] = Time.deltaTime;
        }

    }

    private float GetAverageFPS()
    {
        float sum = 0;
        for(int i =0;i<arrayOfElements.Length;i++)
        {
            sum += arrayOfElements[i];
        }

        return 1f / (sum / arrayOfElements.Length);
    }
}
