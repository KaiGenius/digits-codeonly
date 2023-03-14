using Core;
using System.Collections.Generic;
using UnityEngine;

public class TowerDispatcher : UnitySingleton<TowerDispatcher>
{
    private List<Tower> allTowers = new List<Tower>();
    private float lastUpdateTime = 0;

    public void Register(Tower tower)
    {
        allTowers.Add(tower);
    }

    private void Update()
    {
        if(Time.time - lastUpdateTime >= 1f)
        {
            lastUpdateTime = Time.time;
            for(int i =0;i<allTowers.Count;i++)
            {
                var current = allTowers[i];
                if (current.IsCaptured)
                    current.score += 1;
            }
        }
    }
}
