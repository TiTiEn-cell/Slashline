using System;
using System.Collections.Generic;

namespace GFramework.Helper {
    public static class XRandom
    {
        private static Random _random;

        static XRandom()
        {
            _random = new Random((int)DateTime.UtcNow.Ticks);
        }

        public static bool NextBool()
        {
            return _random.Next(0, 2) == 1;
        }

        public static int NextInt()
        {
            return _random.Next();
        }

        public static float NextFloat()
        {
            return (float)_random.NextDouble();
        }

        public static int Range(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static float Range(float min, float max)
        {
            return (float)_random.NextDouble() * (max - min) + min;
        }
    }

    public static class RandomHelper
    {
        public static bool Chance(float chance, float total)
        {
            var ratio = XRandom.Range(0, total);
            return chance >= ratio;
        }

        public static bool Chance01(float chance)
        {
            return Chance(chance, 1.0f);
        }

        public static T Choice<T>(IList<T> list)
        {
            return list[XRandom.Range(0, list.Count)];
        }

        public static T WeightedChoice<T>(IList<T> list, IList<float> weights)
        {
            return list[WeightedChoice(weights)];
        }

        public static int WeightedChoice(IList<float> weights)
        {
            if (weights == null || weights.Count < 1) throw new System.InvalidOperationException();
            if (weights.Count == 1) return 0;

            var totalWeight = 0f;
            for (int i = 0, c = weights.Count; i < c; ++i)
            {
                totalWeight += weights[i];
            }

            if (totalWeight <= 0f) return XRandom.Range(0, weights.Count);

            var randomRatio = XRandom.NextFloat() * totalWeight;
            var shuffledIndices = ShuffledIndices(weights.Count);

            for (int i = 0, c = shuffledIndices.Length; i < c; ++i)
            {
                int idx = shuffledIndices[i];
                if (randomRatio < weights[idx])
                {
                    return idx;
                }
                randomRatio -= weights[idx];
            }

            return XRandom.Range(0, weights.Count);
        }

        public static int[] ShuffledIndices(int length)
        {
            var arr = new int[length];
            for (int i = 0; i < length; ++i)
            {
                arr[i] = i;
            }

            for (int i = 0; i < length; ++i)
            {
                var idx = XRandom.Range(0, length);
                var tmp = arr[i];
                arr[i] = arr[idx];
                arr[idx] = tmp;
            }
            return arr;
        }
    }
}
