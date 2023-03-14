using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActorName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI printer;
    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    public void SetText(string text)
    {
        printer.text = text;
    }

    public void SetColor(Color color)
    {
        printer.color = color;
    }

    public void SetPosition(Vector3 worldPos, Vector3 screenOffset = default)
    {
        if (mainCam == null)
            mainCam = Camera.main;
        var screenPos = mainCam.WorldToScreenPoint(worldPos);
        transform.position = screenPos + screenOffset;
    }
}
