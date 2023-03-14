using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressionProvider : MonoBehaviour
{
    [SerializeField] private BaseBuffSO[] allAvalibleBuffs;
    private IActor actor;
    private BaseBuff[] buffs;

    private List<BaseBuff> toRemove = new List<BaseBuff>();

    private void Start()
    {
        actor = GetComponent<IActor>();
        buffs = new BaseBuff[allAvalibleBuffs.Length];
        for(int i =0;i<buffs.Length; i++)
        {
            buffs[i] = new BaseBuff(allAvalibleBuffs[i]);
        }
    }

    public int DecreaseLevel(int amount)
    {
        int baseAmount = amount;
        while(amount > 0)
        {
            int thisCycleAmount = 0;
            foreach(var b in buffs)
            {
                if(b.DecreaseLevel(actor.Stats))
                {
                    thisCycleAmount++;
                    amount--;
                }

                if (amount is 0)
                    break;
            }

            if (thisCycleAmount is 0 && amount > 0)
                break;
        }

        return baseAmount - amount;
    }

    public BaseBuff GetBuffBy(StatName statName)
    {
        foreach(var buff in buffs)
        {
            if (buff.statName == statName)
                return buff;
        }

        return null;
    }

    public void SelectValidRandomBuffs(List<BaseBuff> container, int count)
    {
        container.Clear();

        foreach(var b in buffs)
        {
            if (b.IsMax)
                continue;
            container.Add(b);
        }
    REPEAT:
        if (container.Count > count)
        {
       
            bool isRequireRepeat = false;
            toRemove.Clear();
            var req = container.Count - count;
            for(int i =0; i<req;i++)
            {
                var selected = container[Random.Range(0, container.Count)];
                if(toRemove.Contains(selected))
                {
                    isRequireRepeat = true;
                    break;
                }
                toRemove.Add(selected);
            }

            foreach (var t in toRemove)
                container.Remove(t);
            if (isRequireRepeat)
                goto REPEAT;
        }
    }
}

public class BaseBuff
{
    public readonly string name;
    public readonly string description;
    public readonly StatName statName;

    public readonly float baseValue_Level;
    public readonly float multValue_Level;
    public readonly int maxLevel = 5;

    private int currentLevel = 0;

    public int CurrentLevel => currentLevel;
    public bool IsMax => currentLevel == maxLevel;
    
    public bool IncreaseLevel(StatsContainer stats)
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
        }
        else
            return false;

        var stat = stats[statName];
        stat.BaseValue += baseValue_Level;
        stat.Multipiler += multValue_Level;
        return true;
    }

    public bool DecreaseLevel(StatsContainer stats)
    {
        if (currentLevel <= 0)
            return false;
        currentLevel--;
        var stat = stats[statName];
        stat.BaseValue -= baseValue_Level;
        stat.Multipiler -= multValue_Level;
        return true;
    }

    public BaseBuff(BaseBuffSO data)
    {
        name = data.Name;
        description = data.Description;
        statName = data.StatName;
        baseValue_Level = data.BaseValue_Level;
        multValue_Level = data.MultValue_Level;
        maxLevel = data.MaxLevel;
        currentLevel = 0;
    }
}
