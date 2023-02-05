using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sort
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arr1 = new byte[20];
            var arr2 = new byte[20];
            var rand = new Random();
            rand.NextBytes(arr1);

            Array.Copy(arr1, 0, arr2, 0, arr1.Length);
            
            QuickSort(arr1, 0, arr1.Length - 1);
            BubbleSort(arr2);

            for(var i = 0; i < arr1.Length; i++)
            {
                Console.WriteLine(arr1[i] + ", " + arr2[i] + ", " + (arr1[i] == arr2[i] ? "OK" : "FAILED"));
            }
        }

        static void QuickSort(byte[] arr, int low, int high)
        {

            if (low < high)
            {
                /* pi is partitioning index, pi is now at right place */
                var pi = Partition(arr, low, high);
                QuickSort(arr, low, pi - 1);  // Before pi
                QuickSort(arr, pi + 1, high); // After pi
            }
        }

        static int Partition(byte[] arr, int low, int high)
        {
            // pivot (Element to be placed at right position)
            var pivot = arr[high];

            var i = low;  // Index of smaller element and indicates the 
                          // right position of pivot found so far
            for (var j = low; j < high; j++)
            {
                var itemj = arr[j];
                // If current element is smaller than the pivot
                if (itemj < pivot)
                {
                    if (i != j)
                    {
                        (arr[i], arr[j]) = (arr[j], arr[i]); // swap arr[i], arr[j] 
                    }
                    i++; // increment index of smaller element
                }
            }
            if (i != high)
            {
                (arr[i], arr[high]) = (arr[high], arr[i]); // swap arr[i], arr[high]
            }
            return i;
        }

        private static void BubbleSort(byte[] arr)
        {
            bool changed;
            do
            {
                changed = false;
                var a = arr[0];
                for (int i = 1; i < arr.Length; i++)
                {
                    var b = arr[i];
                    if (a > b)
                    {
                        (arr[i - 1], arr[i]) = (arr[i], arr[i - 1]);
                        changed = true;
                    }
                    a = b;
                }
            } while (changed);
        }
    }
}
