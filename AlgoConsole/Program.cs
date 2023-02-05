using AlgoConsole.SvcRefAlgoDemo;
using System.Drawing;
using System.Threading;

namespace AlgoConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var svc = new AlgoDemoServiceClient();
            QuickSortTest(svc, 0, svc.GetItemCount() - 1);
            //BubbleSortTest();
        }

        static void QuickSortTest(AlgoDemoServiceClient svc, int low, int high)
        {

            if (low < high)
            {
                /* pi is partitioning index, pi is now at right place */
                var pi = PartitionTest(svc, low, high);
                QuickSortTest(svc, low, pi - 1);  // Before pi
                QuickSortTest(svc, pi + 1, high); // After pi
            }
            else
            {
                if (low > -1 && low < svc.GetItemCount())
                {
                    svc.Select(low, Color.Blue.ToArgb());
                    Thread.Sleep(700);
                }
            }
        }

        static int PartitionTest(AlgoDemoServiceClient svc, int low, int high)
        {
            // pivot (Element to be placed at right position)
            svc.Select(high, Color.Yellow.ToArgb());
            var pivot = svc.GetItem(high);

            var i = low;  // Index of smaller element and indicates the 
                          // right position of pivot found so far
            for (var j = low; j < high; j++)
            {
                svc.Select(j, Color.Green.ToArgb());
                var itemj = svc.GetItem(j);
                // If current element is smaller than the pivot
                if (itemj < pivot)
                {
                    svc.Select(i, Color.Red.ToArgb());
                    if (i != j)
                    {
                        svc.SwapItems(i, j);
                    }
                    i++; // increment index of smaller element
                    svc.Select(i, Color.White.ToArgb());
                }
            }
            svc.Select(high, Color.Green.ToArgb());
            if (i != high)
            {
                svc.SwapItems(i, high);
            }
            for (int k = low; k <= high; k++)
            {
                svc.Unselect(k);
            }
            svc.Select(i, Color.Blue.ToArgb());
            return i;
        }

        private static void BubbleSortTest()
        {
            var svc = new AlgoDemoServiceClient();
            var n = svc.GetItemCount();
            bool changed;
            do
            {
                changed = false;
                svc.Select(0, Color.Green.ToArgb());
                var a = svc.GetItem(0);
                Thread.Sleep(300);
                for (int i = 1; i < n; i++)
                {
                    svc.Select(i, Color.Green.ToArgb());
                    var b = svc.GetItem(i);
                    Thread.Sleep(300);
                    if (a > b)
                    {
                        svc.SwapItems(i - 1, i);
                        changed = true;
                        Thread.Sleep(200);
                    }
                    a = b;
                }
                for (int i = 0; i < n; i++)
                {
                    svc.Unselect(i);
                }
            } while (changed);
            for (int i = 0; i < n; i++)
            {
                svc.Select(i, Color.Blue.ToArgb());
            }
        }
    }
}
