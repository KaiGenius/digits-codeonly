using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsPresenter : UnitySingleton<ItemsPresenter>
{
    private List<ItemTracker>[] _itemsCollection = new List<ItemTracker>[10];
    private readonly Operation[] defaultFilter = new Operation[0];

    protected override void SingletonAwake()
    {
        for(int i =0;i<_itemsCollection.Length;i++)
        {
            _itemsCollection[i] = new List<ItemTracker>(64);
        }
    }

    public void AddItem(ItemTracker reference)
    {
        var number = reference.Number;
        var list = SelectList(number);
        list.Add(reference);

        reference.OnDestroyEvent += Reference_OnDestroyEvent;
    }

    public int GetItemsNoAlloc(Vector2Int avalibleRange, Operation[] filter, ItemTracker[] container)
    {
        if (filter == null)
            filter = defaultFilter;

        int iterator = 0;

        int indexFrom = SelectIndex(avalibleRange.x);
        int indexTo = SelectIndex(avalibleRange.y);

        foreach(var item in EnumerateAll(indexFrom, indexTo))
        {
            if(ItemInRange(item) && ItemInFilter(item))
            {
                if (Add(item))
                    continue;
                else
                    break;
            }    
        }

        return iterator;

        bool Add(ItemTracker item)
        {
            container[iterator++] = item;
            return iterator < container.Length;
        }

        bool ItemInRange(ItemTracker item)
        {
            var number = item.Number;
            return avalibleRange.x <= number && avalibleRange.y >= number;
        }

        bool ItemInFilter(ItemTracker item)
        {
            var op = item.Operation;
            for(int i =0; i<filter.Length; i++)
            {
                if (op == filter[i])
                    return true;
            }

            return false;
        }
    }

    private void Reference_OnDestroyEvent(ItemTracker reference)
    {
        var number = reference.Number;
        var list = SelectList(number);
        list.Remove(reference);
    }

    private List<ItemTracker> SelectList(int number)
    {
        var index = SelectIndex(number);
        if (_itemsCollection.Length <= index)
            return _itemsCollection[^1];
        else
            return _itemsCollection[index];
    }

    private int SelectIndex(int number) => number / 10;

    private IEnumerable<ItemTracker> EnumerateAll(int indexFrom, int indexTo)
    {
        if(indexTo >= _itemsCollection.Length)
            indexTo = _itemsCollection.Length-1;

        for(int i = indexFrom; i <= indexTo; i++)
        {
            foreach (var item in _itemsCollection[i])
                yield return item;
        }
    }
}
