using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoDemo
{
    public class AlgoDemoService : IAlgoDemoService
    {
        public int GetItemCount()
        {
            return Dispatcher.Instance.GetItemCount();
        }

        public int GetItem(int index)
        {
            return Dispatcher.Instance.GetItem(index);
        }

        public void SwapItems(int index1, int index2)
        {
            Dispatcher.Instance.SwapItems(index1, index2);
        }

        public void SetItem(int index, byte val)
        {
            Dispatcher.Instance.SetItem(index, val);
        }

        public void Select(int index, int argb)
        {
            Dispatcher.Instance.Select(index, argb);
        }

        public void Unselect(int index)
        {
            Dispatcher.Instance.Unselect(index);
        }

        public void UnselectAll()
        {
            Dispatcher.Instance.UnselectAll();
        }
    }
}
