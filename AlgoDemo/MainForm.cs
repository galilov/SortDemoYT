using System;
using System.Threading;
using System.Windows.Forms;

namespace AlgoDemo
{
    public partial class MainForm : Form, IAlgoDemoService
    {
        private readonly DataPresenter _dataPresenter;
        private readonly EventWaitHandle _wh = new AutoResetEvent(false);
        private int _counter = 0;

        public MainForm()
        {
            InitializeComponent();
            _dataPresenter = new DataPresenter(20);
            Dispatcher.Instance.SetSubscriber(this);
            Regenerate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            _dataPresenter.DisplayTo(e.Graphics, pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _dataPresenter.Update(pictureBox1);
        }

        public int GetItemCount()
        {
            return _dataPresenter.Data.Length;
        }

        private delegate void dvoid();
        public int GetItem(int index)
        {
            pictureBox1.Invoke(new dvoid(() => { 
                lbCounter.Text = (++_counter).ToString();
            }));
            return _dataPresenter.Data[index];
        }

        public void SetItem(int index, byte val)
        {
            _dataPresenter.Data[index] = val;
            pictureBox1.Invoke(new dvoid(() =>
            {
                lbCounter.Text = (++_counter).ToString();
                pictureBox1.Invalidate();
            }));
        }

        public void SwapItems(int index1, int index2)
        {
            pictureBox1.Invoke(new dvoid(() =>
            {
                lbCounter.Text = (++_counter).ToString();
            }));
            _dataPresenter.Swap(index1, index2, _wh);
            _wh.WaitOne();
        }

        public void Select(int index, int argb)
        {
            pictureBox1.Invoke(new dvoid(() =>
            {
                _dataPresenter.Select(index, argb);
                pictureBox1.Invalidate();
            }));
        }

        public void Unselect(int index)
        {
            pictureBox1.Invoke(new dvoid(() =>
            {
                _dataPresenter.Unselect(index);
                pictureBox1.Invalidate();
            }));
        }

        public void UnselectAll()
        {
            pictureBox1.Invoke(new dvoid(() =>
            {
                _dataPresenter.UnselectAll();
                pictureBox1.Invalidate();
            }));
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
            pictureBox1.Invalidate();
        }

        private void Regenerate()
        {
            var rand = new Random();
            rand.NextBytes(_dataPresenter.Data);
            _dataPresenter.UnselectAll();
            _dataPresenter.Save();
            _counter = 0;
            lbCounter.Text = _counter.ToString();
        }

        private void Reset()
        {
            _dataPresenter.UnselectAll();
            _dataPresenter.Load();
            _counter = 0;
            lbCounter.Text = _counter.ToString();
        }

        private void btnRegenerate_Click(object sender, EventArgs e)
        {
            Regenerate();
        }
    }
}
