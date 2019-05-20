using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading.Tasks;

namespace Search_element_in_collection_using_SIMD
{
    public class ArrayProcessor
    {
        private static int[] _arr1;
        private static int[] _arr2;

        public static void Main()
        {
            FillArray(ref _arr1);
            FillArray(ref _arr2);
            var classInstance = new ArrayProcessor(_arr1, _arr2);
           foreach(var res in classInstance.AmountOfRepetitionsInArrays(10))
            {
                Console.WriteLine(res);
            }

        }

        public ArrayProcessor(int[] arr1, int[] arr2)
        {
            _arr1 = arr1;
            _arr2 = arr2;
        }

        public List<int> AmountOfRepetitionsInArrays(int item)
        {
            var repetitionsInFirstArr = Task.Run(() => AmountOfRepetition(item, _arr1));
            var repetitionsInSecondArr = Task.Run(() => AmountOfRepetition(item, _arr2));
            Task.WhenAll(repetitionsInFirstArr, repetitionsInSecondArr);

            repetitionsInFirstArr.Result.AddRange(repetitionsInSecondArr.Result);

            return repetitionsInFirstArr.Result;
        }

        private List<int> AmountOfRepetition(int targetElement, int[] targetArray)
        {
            var mask = new Vector<int>(targetElement);
            int vectorSize = Vector<int>.Count;
            var accResult = new Vector<int>();
            int i;
            var array = targetArray;
            for (i = 0; i < array.Length - vectorSize; i += vectorSize)
            {
                var v = new Vector<int>(array, i);
                var areEqual = Vector.Equals(v, mask);
                //ToDo: find indexes of elements in array using vector
                accResult = Vector.Subtract(accResult, areEqual);
            }
            var result = new List<int>();
            for (; i < array.Length; i++)
            {
                if (array[i] == targetElement)
                {
                    result.Add(i);
                }
            }
            //result += Vector.Dot(accResult, Vector<int>.One);
            return result;
        }

        private static void FillArray(ref int[] arr)
        {
            int Min = 8;
            int Max = 11; // max value of number in array
            Random randNum = new Random();
            arr = Enumerable
                .Repeat(0, 64) // fill Max 64 numbers in array
                .Select(i => randNum.Next(Min, Max))
                .ToArray();
        }
    }
}
