using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreUIView : MonoBehaviour
{
    [SerializeField] private DigitView digitView;
    [SerializeField] private Camera renderCamera;

    public void UpdateView(int scoreNumber)
    {
        digitView.SetNumber(scoreNumber.ToString());
        var bounds = digitView.Bounds;
        var pos = transform.position - bounds.center;
        renderCamera.transform.position = pos + new Vector3(0, -4.25f, -5);
    }

    public void UpdateMaterial(Color color) => digitView.UpdateMaterial(color);
}
