using UnityEngine;
using Random = UnityEngine.Random;

public sealed class CommonRandomizer : BaseRandomizer
{
    public override T RandomValue<T>(T[] collection)
    {
        if (collection.Length == 0) return default(T);
        
        var index = Random.Range(0, collection.Length);
        return collection[index];
    }

    public override T RandomValueWithProbability<T>(T[] collection)
    {
        if (collection.Length == 0) return default(T);
        
        float sum = 0;
        var random = Random.Range(0, 1f);

        foreach (var item in collection)
        {
            sum += item.Probability;
            if (random <= sum) return item;
        }
        
        Debug.LogError("Items have incorrect probabilities! Sum probability doesn't equals '1'!");
        return collection[collection.Length - 1];
    }
}