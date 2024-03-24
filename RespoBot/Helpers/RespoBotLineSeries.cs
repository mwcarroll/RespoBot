using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace RespoBot.Helpers
{

    internal class RespoBotLineSeries<TModel> : LineSeries<TModel>
    {
        public RespoBotLineSeries(SKColor strokeColor, float strokeThickness) : base()
        {
            DataPadding = new LvcPoint
            {
                X = 0,
                Y = 0
            };
            Fill = null;
            GeometrySize = 0;
            GeometryFill = new SolidColorPaint
            {
                Color = strokeColor,
                StrokeThickness = 1
            };
            GeometryStroke = new SolidColorPaint
            {
                Color = strokeColor,
                StrokeThickness = 1
            };
            LineSmoothness = 0;
            Stroke = new SolidColorPaint
            {
                Color = strokeColor,
                StrokeThickness = strokeThickness
            };
            MiniatureShapeSize = 8;
        }

        public void SetValues(IEnumerable<TModel> values)
        {
            Values = values;
        }
    }
}