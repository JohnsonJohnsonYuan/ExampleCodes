using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SearchAlg
{
    public static class CommonHelper
    {
        public static T GetArrayRandomItem<T>(T[] array)
        {
            Random rand = new Random();
            return array[rand.Next(array.Length)];
        }

        public static int[] GenerateRandomArray(int arraySize, int min = 0, int max = int.MaxValue)
        {
            int[] array = new int[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                array[i] = i == 0 ? GenerateRandomInteger(min, max) : array[i - 1] + GenerateRandomInteger(0, 1000);
            }

            return array;
        }

        /// <summary>
        /// Returns an random interger number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }
    }
}
