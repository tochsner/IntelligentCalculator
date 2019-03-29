using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace IntelligentCalculator
{
    public static class IEnumerableExtensions
    {

        public static BaseModule RandomElementByWeight(this IEnumerable<BaseModule> sequence)
        {
            double totalWeight = sequence.Sum(x => x.Fitness);
            // The weight we are after...
            double itemWeightIndex = new Random().NextDouble() * totalWeight;
            double currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence.Distinct() select new { Value = weightedItem, Weight = weightedItem.Fitness })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex > itemWeightIndex)
                    return item.Value;

            }

            return sequence.First();
        }

    }
}