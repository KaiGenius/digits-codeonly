using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLocker : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    private void Awake()
    {
        Input.simulateMouseWithTouches = true;
        Application.targetFrameRate = targetFrameRate;
    }
}
