using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace AlgoDemo
{
    internal class Dispatcher : IAlgoDemoService
    {
        private static readonly Dispatcher _instance = new Dispatcher();

        public static Dispatcher Instance => _instance;

        private readonly ReaderWriterLock _rwLock = new ReaderWriterLock();

        private IAlgoDemoService Subscriber
        {
            get
            {
                try
                {
                    _rwLock.AcquireReaderLock(int.MaxValue);
                    return _subscriber;
                }
                finally
                {
                    _rwLock.ReleaseReaderLock();
                }
            }
            set
            {
                try
                {
                    _rwLock.AcquireWriterLock(int.MaxValue);
                    _subscriber = value;
                }
                finally
                {
                    _rwLock.ReleaseWriterLock();
                }
            }
        }

        private volatile IAlgoDemoService _subscriber;

        private Dispatcher()
        {

        }

        public void SetSubscriber(IAlgoDemoService subscriber)
        {
            Subscriber = subscriber;
        }

        public void RemoveSubscriber()
        {
            Subscriber = null;
        }

        public int GetItemCount()
        {
            var subscriber = Subscriber;
            return subscriber != null ? subscriber.GetItemCount() : 0;
        }

        public int GetItem(int index)
        {
            return Subscriber.GetItem(index);
        }

        public void SwapItems(int index1, int index2)
        {
            Subscriber.SwapItems(index1, index2);
        }

        public void SetItem(int index, byte val)
        {
            Subscriber.SetItem(index, val);
        }

        public void Select(int index, int argb)
        {
            Subscriber.Select(index, argb);
        }

        public void Unselect(int index)
        {
            Subscriber.Unselect(index);
        }

        public void UnselectAll()
        {
            Subscriber.UnselectAll();
        }
    }
}
