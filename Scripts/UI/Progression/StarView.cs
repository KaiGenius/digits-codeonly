using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarView : MonoBehaviour
{
    public enum StarState
    {
        IsHas,
        IsNextLevel,
        IsNotHas,
    }
    [SerializeField] private Image image;

    public void SetState(StarState starState)
    {
        Color color;
        switch (starState)
        {
            case StarState.IsHas:
                image.color = Color.white;
                break;
            case StarState.IsNextLevel:
                color = Color.white * 0.5f;
                color.a = 1;
                image.color = color;
                break;
            case StarState.IsNotHas:
                image.color = new Color(0, 0, 0, 0);
                break;
        }
    }
}
