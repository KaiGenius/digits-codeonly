using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuffCardBackgroundSO : ScriptableObject
{
    [SerializeField] private List<KeyPair> data = new List<KeyPair>();

    public KeyPair Select(Characters.StatName stat)
    {
        foreach(var d in data)
        {
            if (d.stat == stat)
                return d;
        }
        throw new System.ArgumentOutOfRangeException();
    }

    [System.Serializable]
    public struct KeyPair
    {
        public Characters.StatName stat;
        public Sprite background, icon;
    }
}
