using System;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace Pie
{
    public class Pie : Grid
    {
        public Pie()
        {
            var cancasView = new PieSkia(this);
            this.Children.Add(cancasView);
        }
    }

	public class PieSkia : SKCanvasView
    {
        private readonly Pie _view;
        private float _strokeWidth = 40.0f;
        private int _round = 10;
        private Color _color = Color.FromArgb("#84CEB2");
        private Color _colorZero = Color.FromArgb("#E1E2E4");
        private double _minOpacity = .2;
        private double _spacing = 10;
        private List<double> _values = new List<double>() { 300, 100, 50, 20};

        #region privates
        private double _total;
        private float _radius;
        #endregion

        public PieSkia(Pie view)
        {
            _view = view;
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
            _radius = ((float)_view.WidthRequest / 2) - _strokeWidth;
            _total = _values.Sum();


            canvas.Clear();
            //Zero(canvas, bounds);

            Item(0, 60, canvas, bounds);
            
        }

        private void Zero(SKCanvas canvas, SKRect bounds)
        {
            float width = (float)_view.WidthRequest;
            float height = (float)_view.HeightRequest;
            var center = new SKPoint(width / 2, height / 2);

            // Desenhe o círculo de fundo
            using (var backgroundPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = _colorZero.ToSKColor(),
                StrokeWidth = _strokeWidth
            })
            {
                canvas.DrawCircle(center, _radius + (_strokeWidth / 2), backgroundPaint);
            }
        }

        private void Item(int startAngle, int sweepAngle, SKCanvas canvas, SKRect bounds)
        {
            float width = (float)_view.WidthRequest;
            float height = (float)_view.HeightRequest;
            var center = new SKPoint(width / 2, height / 2);


            // Desenhe o arco de carregamento
            using (var loadingPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = _color.ToSKColor().WithAlpha((byte)(0xFF * 1))
            })
            {
                float radiusA = _radius;
                var lineA = GetLine(center, radiusA);
                var lineApos = GetPosition(center, startAngle, sweepAngle, radiusA + _round);
                var lineAposOri = GetPosition(center, startAngle, sweepAngle, radiusA);
                var lineAposSec = GetPosition(center, startAngle + _round / 2, sweepAngle - _round, radiusA);

                var radiusB = _radius + _strokeWidth;
                var lineB = GetLine(center, radiusB);
                var lineBpos = GetPosition(center, startAngle, sweepAngle, radiusB - _round);
                var lineBposOri = GetPosition(center, startAngle, sweepAngle, radiusB);
                var lineBposSec = GetPosition(center, startAngle + _round / 2, sweepAngle - _round, radiusB);

                var pathB = new SKPath();
                pathB.AddArc(lineB, startAngle + _round/2, sweepAngle - _round);


                var path = new SKPath();
                path.AddArc(lineA, startAngle + _round/2, sweepAngle - _round);
               
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

