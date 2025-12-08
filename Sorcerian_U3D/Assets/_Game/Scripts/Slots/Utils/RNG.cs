using System;
using System.Collections.Generic;

namespace Kapibara.ConnectSlots
{
    /// <summary>
    /// Utility class for random numbers, probabilities, and random selection.
    /// Example usage:
    /// int n = RNG.Range(0, 10);
    /// float f = RNG.RangeFloat(0f, 1f);
    /// bool success = RNG.Chance(0.5f);
    /// string item = RNG.Pick(new string[] { "apple", "banana", "cherry" });
    /// </summary>
    public static class RNG
    {
        #region Fields

        private static System.Random _random;

        #endregion

        #region Constructor

        static RNG()
        {
            // Deterministic example:
            // _random = new System.Random(12345);

            // Truly random:
            _random = new System.Random();
        }

        #endregion

        #region Integers

        /// <summary>
        /// Returns a random integer in the range [0, Int32.MaxValue).
        /// </summary>
        public static int PickInt()
        {
            return _random.Next(); 
        }
        
        /// <summary>
        /// Returns a random integer in the range [min, max).
        /// </summary>
        public static int PickInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        #endregion

        #region Floats

        /// <summary>
        /// Returns a random float in the range [0, 1).
        /// </summary>
        public static float PickFloat()
        {
            return (float)_random.NextDouble();
        }

        /// <summary>
        /// Returns a random float in the range [min, max).
        /// </summary>
        public static float PickFloat(float min, float max)
        {
            return min + PickFloat() * (max - min);
        }

        #endregion

        #region Probability

        /// <summary>
        /// Returns a random boolean value (true or false) with equal probability.
        /// Example: bool result = RNG.PickBool();
        /// </summary>
        public static bool PickBool()
        {
            return _random.Next(0, 2) == 0;
        }
        
        /// <summary>
        /// Returns true with the given probability [0, 1].
        /// </summary>
        public static bool Chance(float probability)
        {
            return PickFloat() < probability;
        }

        #endregion

        #region Collections

        /// <summary>
        /// Picks a random element from a read-only list.
        /// </summary>
        public static T PickOne<T>(IReadOnlyList<T> list)
        {
            return list[_random.Next(0, list.Count)];
        }

        /// <summary>
        /// Picks a random element from an array.
        /// </summary>
        public static T PickOne<T>(T[] array)
        {
            return array[_random.Next(0, array.Length)];
        }

        /// <summary>
        /// Picks a random value from an enum type.
        /// </summary>
        public static T PickOne<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(_random.Next(values.Length));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Shuffles a list in place.
        /// </summary>
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            for (int i = n - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);
                // T temp = list[i];
                // list[i] = list[j];
                // list[j] = temp;
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        #endregion
    }
}