using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PositionSmoothing {

    private readonly Queue<float> _weightedX = new Queue<float>();
    private readonly Queue<float> _weightedY = new Queue<float>();
    private int nmbrOfValsUsedForSmoothing = 5;

    private float ExponentialMovingAverage(List<float> data, float baseValue)
    {
        float numerator = 0;
        float denominator = 0;

        float average = data.Sum();
        average /= data.Count;

        for (int i = 0; i < data.Count; ++i)
        {
            numerator += data[i] * Mathf.Pow(baseValue, data.Count - i - 1);
            denominator += Mathf.Pow(baseValue, data.Count - i - 1);
        }

        numerator += average * Mathf.Pow(baseValue, data.Count);
        denominator += Mathf.Pow(baseValue, data.Count);

        return numerator / denominator;
    }

    private float WeightedAverage(List<float> data, List<float> weights)
    {
        if (data.Count != weights.Count)
        {
            return float.MinValue;
        }

        float weightedAverage = data.Select((t, i) => t * weights[i]).Sum();

        return weightedAverage / weights.Sum();
    }

    public Vector2 ExponentialWeightedAvg(Vector2 pos)
    {
        _weightedX.Enqueue(pos.x);
        _weightedY.Enqueue(pos.y);

        if (_weightedX.Count > nmbrOfValsUsedForSmoothing)
        {
            _weightedX.Dequeue();
            _weightedY.Dequeue();
        }

        var x = ExponentialMovingAverage(_weightedX.ToList(), 0.9f);
        var y = ExponentialMovingAverage(_weightedY.ToList(), 0.9f);

        return new Vector2(x, y);
    }
}
