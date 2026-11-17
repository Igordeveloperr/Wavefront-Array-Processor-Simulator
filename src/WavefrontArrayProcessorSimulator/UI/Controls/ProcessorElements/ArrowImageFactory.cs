using System;
using System.Drawing;

namespace WaveVS.VSGraphics
{
    internal sealed class ArrowImageFactory
    {
        private readonly int _arrowWidth;
        private readonly int _arrowHeight;

        public ArrowImageFactory(int width, int height)
        {
            _arrowWidth = width;
            _arrowHeight = height;
        }

        public Image Load(string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img", fileName);
                if (System.IO.File.Exists(path))
                {
                    return Image.FromFile(path);
                }
                else
                {
                    return CreateSimpleArrow(fileName);
                }
            }
            catch
            {
                return CreateSimpleArrow(fileName);
            }
        }

        private Image CreateSimpleArrow(string arrowType)
        {
            arrowType = arrowType.ToLowerInvariant();
            bool isHorizontal = arrowType.Contains("right") || arrowType.Contains("left");
            int width = isHorizontal ? _arrowWidth : _arrowHeight;
            int height = isHorizontal ? _arrowHeight : _arrowWidth;

            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);

                Brush brush = Brushes.Black;

                if (arrowType.Contains("right"))
                {
                    // Горизонтальная стрелка вправо
                    g.FillRectangle(brush, 0, height / 2 - 2, width - 10, 4);
                    Point[] arrowHead = new Point[]
                    {
                        new Point(width - 10, 0),
                        new Point(width, height/2),
                        new Point(width - 10, height)
                    };
                    g.FillPolygon(brush, arrowHead);
                }
                else if (arrowType.Contains("left"))
                {
                    // Горизонтальная стрелка влево
                    g.FillRectangle(brush, 10, height / 2 - 2, width - 10, 4);
                    Point[] arrowHead = new Point[]
                    {
                        new Point(10, 0),
                        new Point(0, height/2),
                        new Point(10, height)
                    };
                    g.FillPolygon(brush, arrowHead);
                }
                else if (arrowType.Contains("down"))
                {
                    // Вертикальная стрелка вниз
                    g.FillRectangle(brush, width / 2 - 2, 0, 4, height - 10);
                    Point[] arrowHead = new Point[]
                    {
                        new Point(0, height - 10),
                        new Point(width/2, height),
                        new Point(width, height - 10)
                    };
                    g.FillPolygon(brush, arrowHead);
                }
                else if (arrowType.Contains("up"))
                {
                    // Вертикальная стрелка вверх
                    g.FillRectangle(brush, width / 2 - 2, 10, 4, height - 10);
                    Point[] arrowHead = new Point[]
                    {
                        new Point(0, 10),
                        new Point(width/2, 0),
                        new Point(width, 10)
                    };
                    g.FillPolygon(brush, arrowHead);
                }
            }

            return bmp;
        }
    }
}
