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
            canvas.Clear();
            Item(canvas, bounds);
        }

        private void Item(SKCanvas canvas, SKRect bounds)
        {
            float width = (float)_view.WidthRequest;
            float height = (float)_view.HeightRequest;
            var center = new SKPoint(width / 2, height / 2);
            var radius = 100.0f;
            var strokeWidth = 40.0f;

            // Desenhe o círculo de fundo
            using (var backgroundPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.LightGray,
                StrokeWidth = 40f
            })
            {
                canvas.DrawCircle(center, radius+(strokeWidth/2), backgroundPaint);
            }

            // Simulando o ângulo do loading
            var startAngle = -90;
            var sweepAngle = 135;

            // Desenhe o arco de carregamento
            using (var loadingPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue,
            })
            {
                float radiusA = radius;
                var lineA = GetLine(center, radiusA);
                var lineApos = GetPosition(center, startAngle, sweepAngle, radiusA);
                
                var radiusB = radius + strokeWidth;
                var lineB = GetLine(center, radiusB);
                var lineBpos = GetPosition(center, startAngle, sweepAngle, radiusB);


                var path2 = new SKPath();
                path2.AddArc(lineB, startAngle, sweepAngle);

                var path = new SKPath();
                path.AddArc(lineA, startAngle, sweepAngle);

                //path.MoveTo(lineApos.End.X, lineApos.End.Y);
                path.LineTo(lineBpos.End.X, lineBpos.End.Y);

                path.AddPathReverse(path2);
                
                
                //path.MoveTo(lineBpos.Start.X, lineBpos.Start.Y);
                path.LineTo(lineApos.Start.X, lineApos.Start.Y);
                path.Close();
                


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

