using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GenerationNumbersDataSO : ScriptableObject
{
    [SerializeField] private NumbersData[] data = new NumbersData[1];
    private float sumWeight;

    public void Prewarm(bool avalibleOnlyPositive = false)
    {
        sumWeight = 0;
        foreach(var d in data)
        {
            if (avalibleOnlyPositive)
            {
                if (isPositive(d))
                    sumWeight += d.weight;
            }
            else
            {
                sumWeight += d.weight;
            }
        }

        bool isPositive(NumbersData data)
        {
            foreach(var op in data.avalibleOperations)
            {
                if (op == Operation.Summary || op == Operation.Multiplicate)
                    return true;
            }
            return false;
        }
    }

    public (int number, Operation op, float sizeMultipiler) GetRandomData(bool? avalibleOnlyPositive = null)
    {
        if(avalibleOnlyPositive.HasValue)
        {
            Prewarm(avalibleOnlyPositive.Value);
        }
        Prewarm(false);

        var requireWeight = Random.Range(0f, sumWeight);
        float currentWeight = 0;
        NumbersData selectedData = null;
        int iterator = 0;

        do
        {
            if (avalibleOnlyPositive.HasValue && avalibleOnlyPositive.Value && isPositive(data[iterator]))
            {
                DoIteration();
            }
            else if (avalibleOnlyPositive.HasValue && !avalibleOnlyPositive.Value)
            {
                DoIteration();
            }
            else if(!avalibleOnlyPositive.HasValue)
            {
                DoIteration();
            }

            void DoIteration()
            {
                selectedData = data[iterator++];
                currentWeight += selectedData.weight;
            }

        } while(currentWeight < requireWeight && iterator < data.Length);

        //selectedData ??= data[^1];

        var rndValEval = Random.value;
        int number = (int)selectedData.numberCurve.Evaluate(rndValEval);
        var op = selectedData.avalibleOperations[Random.Range(0, selectedData.avalibleOperations.Length)];
        if(avalibleOnlyPositive.HasValue && avalibleOnlyPositive.Value)
        {
            if (op == Operation.Divided)
                op = Operation.Multiplicate;
            else if (op == Operation.Substract)
                op = Operation.Summary;
        }

        if(selectedData.printLogIfSelected)
        {
            Debug.Log($"Select weight: {selectedData.weight} with number: {number} and op: {op}");
        }

        return (number, op, selectedData.sizeMult);

        bool isPositive(NumbersData data)
        {
            foreach (var op in data.avalibleOperations)
            {
                if (op == Operation.Summary || op == Operation.Multiplicate)
                    return true;
            }
            return false;
        }
    }
}

[System.Serializable]
public class NumbersData
{
    public float sizeMult = 1f;
    public bool printLogIfSelected = false;
    public float weight;
    public AnimationCurve numberCurve;
    public Operation[] avalibleOperations;
}
