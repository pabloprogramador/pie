﻿using System;
using System.Linq;
using System.Security.Cryptography;
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

        public static readonly BindableProperty ValuesProperty = BindableProperty.Create(
            nameof(Values),
            typeof(List<double>),
            typeof(Pie),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: ValuesPropertyChanged);

        private static void ValuesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Pie)bindable;
            var oldList = (List<double>)oldValue;
            var newList = (List<double>)newValue;

            if (oldList == null || newList == null || !newList.SequenceEqual(oldList))
            {
                control.ChangeValues();
            }
        }

        public List<double> Values
        {
            get { return (List<double>)base.GetValue(ValuesProperty); }
            set { base.SetValue(ValuesProperty, value); }
        }

        public static readonly BindableProperty PieColorProperty = BindableProperty.Create(
            nameof(PieColor),
            typeof(Color),
            typeof(Pie),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: PieColorPropertyChanged);

        private static void PieColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Pie)bindable;
            if (oldValue != null)
            {
                control.ChangeColor();
            }
            else
            {
                control.PieColorTemp = (Color)newValue;
            }
        }

        public Color PieColor
        {
            get { return (Color)base.GetValue(PieColorProperty); }
            set { base.SetValue(PieColorProperty, value); }
        }

        public static readonly BindableProperty PieColorsProperty = BindableProperty.Create(
            nameof(PieColors),
            typeof(List<Color>),
            typeof(Pie),
            defaultValue: null,
            defaultBindingMode: BindingMode.OneWay);

        public List<Color> PieColors
        {
            get { return (List<Color>)base.GetValue(PieColorsProperty); }
            set { base.SetValue(PieColorsProperty, value); }
        }


        public List<double> ValuesTemp;
        public double SizeCircle { get; set; } = 360;
        public float StrokeWidth { get; set; } = 60;
        public int Round { get; set; } = 20;
        public Color PieColorTemp;
        public Color ColorZero { get; set; } = Color.FromArgb("#E1E2E4");
        public double MinOpacity { get; set; } = .1;
        public int Spacing { get; set; } = 6;
        public int MarginWholeCircle { get; set; } = 30;
        public uint TimeAnimation { get; set; } = 1000;

        private PieSkia _pieSkia;
        private int _wholeCircle = 360;
        private int _halfCircle = 183;
        private List<double> _valuesOld;

        public Pie()
        {
            PieColorTemp = PieColor;
        }

        protected async override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            while(width < 0)
            {
                await Task.Delay(200).ConfigureAwait(true);
            }
            var fix = width - (MarginWholeCircle * 2);
            if(this.Height != fix)
            {
                this.HeightRequest = fix;
                return;
            }
            _pieSkia = new PieSkia(this, fix, height);
            this.Children.Add(_pieSkia);
            ChangeValues();
        }

        private async void ChangeMaxCircle()
        {
            if (_pieSkia == null) return;
            double sizeCircle = IsHalfCircle ? _halfCircle : _wholeCircle;
            ChangeMaxCircleByValue(sizeCircle);
            if (IsHalfCircle)
            {
                this.TranslateTo(-(this.Width / 2), 0, TimeAnimation, Easing.CubicOut);
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

            var animation = new Animation((v) =>
            {
                this.SizeCircle = v;
                _pieSkia.InvalidateSurface();
            }, this.SizeCircle, value, Easing.CubicOut);

            animation.Commit(this, "PieMaxCircle", 16, TimeAnimation);
        }

        private void ChangeValues()
        {

            if (_pieSkia == null) return;

            if (Values == null || !Values.Any())
            {
                ValuesTemp = null;
                _valuesOld = null;
                _pieSkia.InvalidateSurface();
                return;
            }

            ValuesTemp = Values.ToList();

            if (_valuesOld == null)
            {
                _valuesOld = Values.ToList();
                this.SizeCircle = 50;
                _pieSkia.InvalidateSurface();
                ChangeMaxCircleByValue(_wholeCircle);
                return;
            }

            var animation = new Animation((v) =>
            {
                if (Values == null) { return; }
                var temp = Values.Count > _valuesOld.Count ? Values.ToList() : _valuesOld;
                var values = Values.ToList();

                int i = 0;
                foreach (var item in temp)
                {
                    if (_valuesOld.Count <= i)
                    {
                        _valuesOld.Add(0);
                    }
                    if (values.Count <= i)
                    {
                        values.Add(0);
                    }
                    if (ValuesTemp.Count <= i)
                    {
                        ValuesTemp.Add(0);
                    }

                    double old = _valuesOld[i];
                    ValuesTemp[i] = old + ((values[i] - old) * v);

                    i++;
                }
                _pieSkia.InvalidateSurface();
            }, 0, 1);

            animation.Commit(this, "PieValues", 16, TimeAnimation, Easing.CubicInOut,
                 (double v, bool s) =>
                 {
                     if (Values == null) { return; }
                     _valuesOld = Values.Where(x => x > 0).ToList();
                 });
        }

        private void ChangeColor()
        {
            var animation = new Animation((t) =>
            {
                if (PieColor == null) { return; }

                PieColorTemp = Color.FromRgba(PieColorTemp.Red + t * (PieColor.Red - PieColorTemp.Red),
                               PieColorTemp.Green + t * (PieColor.Green - PieColorTemp.Green),
                               PieColorTemp.Blue + t * (PieColor.Blue - PieColorTemp.Blue),
                               PieColorTemp.Alpha + t * (PieColor.Alpha - PieColorTemp.Alpha));

                _pieSkia.InvalidateSurface();
            }, 0, 1);

            animation.Commit(this, "PieColor", 16, TimeAnimation, Easing.Linear);
        }
    }

    /// <summary>
    /// PIESKIA
    /// </summary>
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
            if (_view.ValuesTemp == null || !_view.ValuesTemp.Any())
            {
                Zero(canvas, bounds);
            }
            else
            {
                int pos = -90;
                int i = 1;
                double totalO = 1 - _view.MinOpacity;
                var totalV = _view.Values.Count;
                
                foreach (var item in _view.ValuesTemp.ToList())
                {
                    if (_view.ValuesTemp[i - 1] == 0)
                    {
                        _view.ValuesTemp.RemoveAt(i - 1);
                    }
                    else
                    {
                        _total = _view.ValuesTemp.Sum();
                        int end = (int)Math.Round(item * (_view.SizeCircle - (_view.Spacing * totalV)) / _total);
                        double opacity = i >= totalV ? _view.MinOpacity : 1 - (i * totalO / totalV);
                        Item(i, pos, end, opacity, canvas, bounds);
                        pos += (end + _view.Spacing);
                        i++;
                    }
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

        private void Item(int index, int startAngle, int sweepAngle, double opacity, SKCanvas canvas, SKRect bounds)
        {
            float width = (float)_width;
            float height = (float)_height;
            var center = new SKPoint(_view.MarginWholeCircle + (width / 2), height / 2);
            var round = (_view.Round * 2) > sweepAngle ? (int)Math.Floor((double)sweepAngle / 2) : _view.Round;
            var color = Colors.Black;
            if (_view.PieColors == null)
            {
                color = _view.PieColorTemp ?? color;
            }
            else
            {
                opacity = 1;
                if (_view.PieColors.Count > (index - 1))
                {
                    color = _view.PieColors[index - 1];
                }
                else
                {
                    double t1 = (double)(index) / _view.PieColors.Count;
                    var t2 = (int)Math.Ceiling(t1) - 1;
                    var ind = index - (_view.PieColors.Count * t2) - 1;
                    color = _view.PieColors[ind];
                }

            }
            using (var loadingPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = color.ToSKColor().WithAlpha((byte)(0xFF * opacity))
            })
            {
                float radiusA = _radius;
                var lineA = GetLine(center, radiusA);
                var lineApos = GetPosition(center, startAngle, sweepAngle, radiusA + round);
                var lineAposOri = GetPosition(center, startAngle, sweepAngle, radiusA);
                var lineAposSec = GetPosition(center, startAngle + round / 2, sweepAngle - round, radiusA);

                var radiusB = _radius + _view.StrokeWidth;
                var lineB = GetLine(center, radiusB);
                var lineBpos = GetPosition(center, startAngle, sweepAngle, radiusB - round);
                var lineBposOri = GetPosition(center, startAngle, sweepAngle, radiusB);
                var lineBposSec = GetPosition(center, startAngle + round / 2, sweepAngle - round, radiusB);

                var pathB = new SKPath();
                pathB.AddArc(lineB, startAngle + round / 2, sweepAngle - round);


                var path = new SKPath();
                path.AddArc(lineA, startAngle + round / 2, sweepAngle - round);

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

            return new Position()
            {
                Start = new SKPoint(startX, startY),
                End = new SKPoint(endX, endY)
            };
        }

        class Position
        {
            public SKPoint Start;
            public SKPoint End;
        }
    }
}