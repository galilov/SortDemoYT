using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgoDemo
{
    internal class DataPresenter
    {
        private readonly byte[] _data;
        private readonly Pen _whitePen;
        private readonly float _selectedPenWidth = 6f;
        private readonly StringFormat _stringFormat;
        private const float _leftRightOffset = 10f;
        private const float _inchPerEm = 72f;
        private const float _mainFontScale = 0.35f;
        private const float _captionFontScale = 0.25f;
        private readonly IDictionary<int, Color> _selectedIndices;
        private int _swapIndex1 = -1, _swapIndex2 = -1;
        private int _animationFrameNo1 = 0, _animationFrameNo2 = 0;
        private DirectBitmap _directBitmap;
        private int _width = -1, _height = -1;
        private float _elementWidth;
        private float _elementHeight;
        private float _y;
        private RectangleF[] _elementRects;
        private SizeF _rectSz;
        private Font _mainFont, _captionFont;
        private Graphics _g;
        private EventWaitHandle _wh;
        public DataPresenter(int nElements)
        {
            _data = new byte[nElements];
            _selectedIndices = new Dictionary<int, Color>(nElements);
            _whitePen = new Pen(Brushes.White, 2f);
            _stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
        }

        public byte[] Data => _data;

        public void Save()
        {
            using (var file = new StreamWriter("data.txt"))
            {
                foreach (byte i in _data)
                {
                    file.WriteLine(i.ToString());
                }
            }
        }

        public void Load()
        {
            using (var file = new StreamReader("data.txt"))
            {
                int i = 0;
                for (; ; )
                {
                    var s = file.ReadLine();
                    if (s == null) break;
                    if (i < _data.Length)
                    {
                        _data[i++] = byte.Parse(s);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void Select(int index, int argb)
        {
            _selectedIndices[index] = Color.FromArgb(argb);
        }

        public void Unselect(int index)
        {
            _selectedIndices.Remove(index);
        }

        public void UnselectAll()
        {
            _selectedIndices.Clear();
        }

        public void Swap(int index1, int index2, EventWaitHandle wh)
        {
            _swapIndex1 = index1;
            _swapIndex2 = index2;
            _animationFrameNo1 = 0;
            _animationFrameNo2 = 0;
            _wh = wh;
        }

        public void Update(Control control)
        {
            if (_swapIndex1 > -1)
            {
                control.Refresh();
            }
            else if (_swapIndex1 < -1)
            {
                _swapIndex1++;
                _swapIndex2++;
                control.Invalidate();
                _wh.Set();
            }
        }

        public void DisplayTo(Graphics graphics, int width, int height)
        {
            if (_directBitmap == null || _width != width || _height != height)
            {
                _width = width;
                _height = height;
                _elementWidth = (width - 2f * _leftRightOffset) / _data.Length;
                _elementHeight = _elementWidth;
                if (_directBitmap != null)
                {
                    _directBitmap.Dispose();
                    _g.Dispose();
                }
                _directBitmap = new DirectBitmap(width, height);

                _g = Graphics.FromImage(_directBitmap.Bitmap);
                _g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                ReCreateFonts(_g, _elementWidth);
            }
            _g.FillRectangle(Brushes.Black, new RectangleF(0, 0, width, height));
            _elementRects = new RectangleF[_data.Length];
            _rectSz = new SizeF(_elementWidth, _elementHeight);
            _y = (height - _elementHeight) / 2;

            for (int i = 0; i < _data.Length; i++)
            {
                var x = _leftRightOffset + i * _elementWidth;
                _elementRects[i] = new RectangleF(new PointF(x, _y), _rectSz);
                _g.DrawString(i.ToString(), _captionFont, Brushes.White,
                    new RectangleF(new PointF(x, _y + 0.75f * _elementHeight), _rectSz), _stringFormat);
            }
            _g.DrawRectangles(_whitePen, _elementRects);
            foreach (var selectedIndex in _selectedIndices)
            {
                using (var pen = new Pen(new SolidBrush(selectedIndex.Value), _selectedPenWidth))
                {
                    var r = _elementRects[selectedIndex.Key];
                    _g.DrawRectangle(pen, r.X + _selectedPenWidth, r.Y + _selectedPenWidth,
                        r.Width - _selectedPenWidth * 2, r.Height - _selectedPenWidth * 2);
                }
            }


            for (int i = 0; i < _data.Length; i++)
            {
                if (i != _swapIndex1 && i != _swapIndex2)
                {
                    DrawElement(i);
                }
            }
            if (_swapIndex1 > -1)
            {
                if (_swapIndex1 > _swapIndex2)
                {
                    (_swapIndex1, _swapIndex2) = (_swapIndex2, _swapIndex1);
                }
                var moveStep = width / 300f;
                var baseX1 = _leftRightOffset + _swapIndex1 * _elementWidth;
                var baseX2 = _leftRightOffset + _swapIndex2 * _elementWidth;
                var x1 = baseX1 + (++_animationFrameNo1 * moveStep);
                var x2 = baseX2 + (--_animationFrameNo2 * moveStep);
                DrawElement(_swapIndex1, x1);
                DrawElement(_swapIndex2, x2);
                if (x2 - baseX1 < moveStep)
                {
                    (_data[_swapIndex2], _data[_swapIndex1]) = (_data[_swapIndex1], _data[_swapIndex2]);
                    DrawElement(_swapIndex1);
                    DrawElement(_swapIndex2);
                    _swapIndex1 = -2;
                    _swapIndex2 = -2;
                }
            }
            graphics.DrawImage(_directBitmap.Bitmap, 0, 0);
        }

        private void DrawElement(int elementIndex)
        {
            var s = _data[elementIndex].ToString();
            var x = _leftRightOffset + elementIndex * _elementWidth;
            _g.DrawString(s, _mainFont, Brushes.Yellow,
                new RectangleF(new PointF(x, _y), _rectSz), _stringFormat);
        }

        private void DrawElement(int elementIndex, float x)
        {
            var r = new RectangleF(new PointF(x, _y - 20), _rectSz);
            var s = _data[elementIndex].ToString();
            _g.FillRectangle(Brushes.Gray, r);
            _g.DrawString(s, _mainFont, Brushes.Yellow, r, _stringFormat);
        }

        private void ReCreateFonts(Graphics g, float elementWidth)
        {
            if (_mainFont != null) _mainFont.Dispose();
            if (_captionFont != null) _captionFont.Dispose();
            _mainFont = new Font("Consolas", elementWidth * _inchPerEm / g.DpiX * _mainFontScale, FontStyle.Bold);
            _captionFont = new Font("Consolas", elementWidth * _inchPerEm / g.DpiX * _captionFontScale, FontStyle.Bold);
        }
    }
}
