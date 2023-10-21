using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Pie
{
    public class Pie : Grid
    {

        public static readonly BindableProperty IsHalfCircleProperty = BindableProperty.Create(
            nameof(IsHalfCircle),
            typeof(bool),
            typeof(Pie),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: IsHalfCirclePropertyChanged);

        private static void IsHalfCirclePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Pie)bindable;
            control.ChangeMaxCircle();
        }

        public bool IsHalfCircle
        {
            get { return (bool)base.GetValue(IsHalfCircleProperty); }
            set { base.SetValue(IsHalfCircleProperty, value); }
        }

        public List<double> Values { get; set; } = new List<double>() { 300, 100, 50, 20 };
        public double SizeCircle { get; set; } = 360;
        public float StrokeWidth { get; set; } = 50;
        public int Round { get; set; } = 5;
        public Color Color { get; set; } = Color.FromArgb("#84CEB2");
        public Color ColorZero { get; set; } = Color.FromArgb("#E1E2E4");
        public double MinOpacity { get; set; } = .1;
        public int Spacing { get; set; } = 3;
        public int MarginWholeCircle { get; set; } = 30;
        public uint TimeAnimation { get; set; } = 1000;

        private PieSkia _pieSkia;
        private int _wholeCircle = 360;
        private int _halfCircle = 183;
        private List<double> _valuesOld;

        public Pie()
        {
            
        }

        protected async override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            _pieSkia = new PieSkia(this, width - (MarginWholeCircle * 2), height);
            this.Children.Add(_pieSkia);
            //ChangeMaxCircleByValue(_wholeCircle);
        }

        private void ChangeMaxCircle()
        {
            if (_pieSkia == null) return;
            double sizeCircle = IsHalfCircle ? _halfCircle : _wholeCircle;
            ChangeMaxCircleByValue(sizeCircle);
            if (IsHalfCircle)
            {
                this.TranslateTo(-(this.Width/2), 0, TimeAnimation, Easing.CubicOut);
                this.ScaleTo(.8, TimeAnimation, Easing.CubicInOut);
            }
            else
            {
                this.TranslateTo(0, 0, TimeAnimation, Easing.CubicOut);
                this.ScaleTo(1, TimeAnimation, Easing.CubicOut);
            }
        }

        private void ChangeMaxCircleByValue(double value)
        {
            if (_pieSkia == null) return;

            var animation = new Animation((v) => {
                this.SizeCircle = v;
                _pieSkia.InvalidateSurface();
            }, this.SizeCircle, value, Easing.CubicOut);

            animation.Commit(this, "MaxCircle", 16, TimeAnimation);
        }

    }

	public class PieSkia : SKCanvasView
    {
        private Pie _view;
        private double _width;
        private double _height;
        private double _total;
        private float _radius;

        public PieSkia(Pie view, double width, double height)
        {
            _view = view;
            _width = width;
            _height = height;
            this.IgnorePixelScaling = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var bounds = new SKRect(
                e.Info.Rect.Left,
                e.Info.Rect.Top,
                e.Info.Rect.Right,
                e.Info.Rect.Bottom
            );
            SKImageInfo info = e.Info;
            Draw(e.Surface.Canvas, info, bounds);
        }

        private void Draw(SKCanvas canvas, SKImageInfo info, SKRect bounds)
        {
            _radius = ((float)_width / 2) - _view.StrokeWidth;
            canvas.Clear();
            if (_view.Values == null)
            {
                Zero(canvas, bounds);
            }
            else
            {
                _total = _view.Values.Sum();
                int pos = -90;
                int i = 1;
                double totalO = 1 - _view.MinOpacity;
                double totalV = _view.Values.Count;
                foreach (var item in _view.Values)
                {
                    int end = (int)Math.Round(item * (_view.SizeCircle - (_view.Spacing * _view.Values.Count)) / _total);
                    double opacity = 1 - (i * totalO / totalV) + _view.MinOpacity;
                    Item(pos, end, opacity, canvas, bounds);
                    pos += (end + _view.Spacing);
                    i++;
                }
            }
        }

        private void Zero(SKCanvas canvas, SKRect bounds)
        {
            float width = (float)_width;
            float height = (float)_height;
            var center = new SKPoint(_view.MarginWholeCircle + (width / 2), height / 2);

            using (var backgroundPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _view.ColorZero.ToSKColor(),
                StrokeWidth = _view.StrokeWidth
            })
            {
                canvas.DrawCircle(center, _radius + (_view.StrokeWidth / 2), backgroundPaint);
            }
        }

        private void Item(int startAngle, int sweepAngle, double opacity, SKCanvas canvas, SKRect bounds)
        {
            float width = (float)_width;
            float height = (float)_height;
            var center = new SKPoint(_view.MarginWholeCircle + (width / 2), height / 2);

            using (var loadingPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = _view.Color.ToSKColor().WithAlpha((byte)(0xFF * opacity))
            })
            {
                float radiusA = _radius;
                var lineA = GetLine(center, radiusA);
                var lineApos = GetPosition(center, startAngle, sweepAngle, radiusA + _view.Round);
                var lineAposOri = GetPosition(center, startAngle, sweepAngle, radiusA);
                var lineAposSec = GetPosition(center, startAngle + _view.Round / 2, sweepAngle - _view.Round, radiusA);

                var radiusB = _radius + _view.StrokeWidth;
                var lineB = GetLine(center, radiusB);
                var lineBpos = GetPosition(center, startAngle, sweepAngle, radiusB - _view.Round);
                var lineBposOri = GetPosition(center, startAngle, sweepAngle, radiusB);
                var lineBposSec = GetPosition(center, startAngle + _view.Round / 2, sweepAngle - _view.Round, radiusB);

                var pathB = new SKPath();
                pathB.AddArc(lineB, startAngle + _view.Round /2, sweepAngle - _view.Round);


                var path = new SKPath();
                path.AddArc(lineA, startAngle + _view.Round /2, sweepAngle - _view.Round);
               
                path.QuadTo(
                    new SKPoint(lineAposOri.End.X, lineAposOri.End.Y),
                    new SKPoint(lineApos.End.X, lineApos.End.Y));

                path.LineTo(lineBpos.End.X, lineBpos.End.Y);

                path.QuadTo(
                    new SKPoint(lineBposOri.End.X, lineBposOri.End.Y),
                    new SKPoint(lineBposSec.End.X, lineBposSec.End.Y));

                path.AddPathReverse(pathB);
                
                path.QuadTo(
                     new SKPoint(lineBposOri.Start.X, lineBposOri.Start.Y),
                     new SKPoint(lineBpos.Start.X, lineBpos.Start.Y));

                path.LineTo(lineApos.Start.X, lineApos.Start.Y);
                path.QuadTo(
                     new SKPoint(lineAposOri.Start.X, lineAposOri.Start.Y),
                     new SKPoint(lineAposSec.Start.X, lineAposSec.Start.Y));
                //path.Close();
                canvas.DrawPath(path, loadingPaint);
            }
            canvas.Restore();
        }

        private SKRect GetLine(SKPoint center, float radius)
        {
            return new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius);
        }

        private Position GetPosition(SKPoint center, int startAngle, int sweepAngle, float radius)
        {
            var startRadians = Math.PI * startAngle / 180.0;
            var endRadians = Math.PI * (startAngle + sweepAngle) / 180.0;

            var startX = center.X + radius * (float)Math.Cos(startRadians);
            var startY = center.Y + radius * (float)Math.Sin(startRadians);

            var endX = center.X + radius * (float)Math.Cos(endRadians);
            var endY = center.Y + radius * (float)Math.Sin(endRadians);

            return new Position() {
                Start = new SKPoint(startX, startY),
                End = new SKPoint(endX, endY)};

        }

        class Position
        {
            public SKPoint Start;
            public SKPoint End;
        }
    }
}

