using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    [SerializeField] private GameObject newText;
    [SerializeField] private StarView[] stars;

    public void SetLevel(int value)
    {
        newText.SetActive(value is 0);

        value = Mathf.Clamp(value, 0, stars.Length);
        value -= 1;

        foreach (var s in stars)
            s.SetState(StarView.StarState.IsNotHas);

        for(int i = 0; i <= value; i++)
        {
            stars[i].SetState(StarView.StarState.IsHas);
        }

        //if(value < stars.Length-1)
        //    stars[value+1].SetState(StarView.StarState.IsNextLevel);
    }
}
