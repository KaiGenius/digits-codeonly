using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlateView : MonoBehaviour
{
    private static readonly int h_color = Shader.PropertyToID("_Color");

    [SerializeField] private MeshRenderer[] renderers;
    
    public void SetColor(Color color)
    {
        foreach(var renderer in renderers)
            renderer.material.SetColor(h_color, color);
    }
}
