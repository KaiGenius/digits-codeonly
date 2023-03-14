using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SizeLevelList : ScriptableObject
{
    public List<SizeLevel> spawnSizeList = new List<SizeLevel>();
    public SizeLevel GetRandomSize()
    {
        return spawnSizeList[UnityEngine.Random.Range(0, spawnSizeList.Count)];
    }

    public float GetSizeForLevel(int level)
    {
        foreach(var item in spawnSizeList)
        {
            if (item.level == level)
                return item.size;
        }

        return -1;
    }
}
