using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [CreateAssetMenu]
    public class StatsContainerSO : ScriptableObject, IEnumerable<Stat>
    {
        [SerializeField] private List<Stat> stats = new List<Stat>();

        public IEnumerator<Stat> GetEnumerator()
        {
            return stats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void OnValidate()
        {
            foreach(var itm1 in stats)
                foreach(var itm2 in stats)
                {
                    if (itm1 == itm2)
                        continue;
                    if(itm1.Name == itm2.Name)
                    {
                        Debug.LogError($"Некоторые статы имеют одинаковое имя. Это не допустимо! Конфликт элементов {itm1.Name}: {stats.IndexOf(itm1)} vs {stats.IndexOf(itm2)}");
                    }
                }
        }
    }
}
