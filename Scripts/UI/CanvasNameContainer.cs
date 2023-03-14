using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasNameContainer : MonoBehaviour
{
    private static CanvasNameContainer instance;
    public static CanvasNameContainer ActiveInstance => instance;

    private void Awake()
    {
        instance = this;
    }
}
