using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Characters
{
    /*
1) ХП
2) Скорость движения
3) Скорость деша
4) Дальность деша
5) Скорость перезарядки деша
6) Кол-во зарядов деша
7) Дальность атаки
8) Задержка атаки
9) Инвентированность
10) Множитель опыта
11) Возможность открывать сундуки
12) Сопротивление дальним атакам
13) Сопротивление ближним атакам
14) Сила атаки
15) Конус атаки
16) Неуязвимость 
     */

    public enum StatName
    {
        Speed,
        Passive_ScoreUp,
        Magnet_Range,
        Resistance_NegateNumbers,
        OnEat_ConsumeScorePercent,
        Additional_PositiveNumbers,
        Additional_PositiveNumbersBonus,
    }

    [System.Serializable]
    public class Stat
    {
        [SerializeField] private StatName name;
        [SerializeField] private float _baseValue, _multipiler, _min;

        public StatName Name => name;

        public event Action<Stat> OnChangeValue;

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                if (_baseValue != value)
                {
                    _baseValue = value;
                    OnChangeValue?.Invoke(this);
                }
            }
        }

        public float Multipiler
        {
            get => _multipiler;
            set
            {
                if(_multipiler != value)
                {
                    _multipiler = value;
                    OnChangeValue?.Invoke(this);
                }
            }
        }

        public float Minimum
        {
            get => _min;
            set
            {
                if(_min != value)
                {
                    _min = value;
                    OnChangeValue?.Invoke(this);
                }
            }
        }

        public float Value
        {
            get
            {
                var val = _baseValue * _multipiler;
                if (val < _min)
                    val = _min;
                return val;
            }
        }

        public int ValueAsInt()
        {
            var val = Value;
            return Mathf.RoundToInt(val);
        }
        public bool ValueAsBool()
        {
            var val = Value;
            return val != _min;
        }

        public static Stat Copy(Stat source)
        {
            return new Stat()
            {
                name = source.name,
                _baseValue = source._baseValue,
                _min = source._min,
                _multipiler = source._multipiler,
            };
        }
    }

    public class StatsContainer : IEnumerable<Stat>
    {
        private Dictionary<StatName, Stat> map;
        
        public StatsContainer(IEnumerable<Stat> list)
        {
            map = new Dictionary<StatName, Stat>(list.Count());

            foreach (var itm in list)
            {
                var copy = Stat.Copy(itm);
                map.Add(copy.Name, copy);
            }
        }

        public Stat this[StatName key]
        {
            get
            {
                if (map.TryGetValue(key, out Stat result))
                    return result;
                else
                    return null;
            }
        }

        public IEnumerator<Stat> GetEnumerator()
        {
            return map.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
