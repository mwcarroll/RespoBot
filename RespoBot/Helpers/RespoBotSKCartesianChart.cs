using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace RespoBot.Helpers
{
    internal class RespoBotSKCartesianChart : SKCartesianChart
    {
        public RespoBotSKCartesianChart()
        {
            Title = new LabelVisual
            {
                Paint = new SolidColorPaint
                {
                    Color = SKColors.White,
                    StrokeThickness = 2
                },
                TextSize = 18
            };
            Width = 900;
            Height = 350;
            Background = SKColors.Black;
            Legend = new SKDefaultLegend();
            LegendPosition = LiveChartsCore.Measure.LegendPosition.Right;
            LegendTextSize = 12;
            LegendTextPaint = new SolidColorPaint
            {
                Color = SKColors.White
            };
            DrawMarginFrame = new DrawMarginFrame
            {
                Fill = new SolidColorPaint
                {
                    Color = SKColors.Black
                },
                Stroke = new SolidColorPaint
                {
                    Color = SKColors.Gray,
                    StrokeThickness = 2
                }
            };
            XAxes = new[]
            {
                new Axis()
                {
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SKColors.White,
                        StrokeThickness = 1
                    },
                    Padding = new Padding
                    {
                        Bottom = 10,
                        Left = 0,
                        Right = 0,
                        Top = 5
                    },
                    SeparatorsPaint = new SolidColorPaint
                    {
                        Color = SKColors.Gray,
                        StrokeThickness = 2,
                        PathEffect = new DashEffect(new float[] { 3, 3 })
                    },
                    TextSize = 12
                }
            };
            YAxes = new[]
            {
                new Axis()
                {
                    LabelsPaint = new SolidColorPaint
                    {
                        Color = SKColors.White,
                        StrokeThickness = 1
                    },
                    Padding = new Padding
                    {
                        Bottom = 0,
                        Left = 50,
                        Right = 5,
                        Top = 0
                    },
                    SeparatorsPaint = new SolidColorPaint
                    {
                        Color = SKColors.Gray,
                        StrokeThickness = 1
                    },
                    TextSize = 12
                }
            };
            Series = Array.Empty<ISeries>();
        }

        public void AddSeries(ISeries series)
        {
            Series = Series.Append<ISeries>(series).ToArray();
        }

        public void SetTitleText(string titleText)
        {
            ((LabelVisual)Title).Text = titleText;
        }

        public void SetXAxesCustomSeparators(IList<double> customSeparators)
        {
            foreach (ICartesianAxis axis in XAxes.ToList())
            {
                axis.CustomSeparators = customSeparators;
            }
        }
        
        public void SetYAxesCustomSeparators(IList<double> customSeparators)
        {
            foreach (ICartesianAxis axis in YAxes.ToList())
            {
                axis.CustomSeparators = customSeparators;
            }
        }
        
        public void SetXAxesLabeler(Func<double, string> func)
        {
            foreach (ICartesianAxis axis in XAxes.ToList())
            {
                axis.Labeler = func;
            }
        }
        
        public void SetYAxesLabeler(Func<double, string> func)
        {
            foreach (ICartesianAxis axis in YAxes.ToList())
            {
                axis.Labeler = func;
            }
        }

        public void SetXAxesUnitWidth(double unitWidth)
        {
            foreach (ICartesianAxis axis in XAxes.ToList())
            {
                axis.UnitWidth = unitWidth;
            }
        }
        
        public void SetYAxesUnitWidth(double unitWidth)
        {
            foreach (ICartesianAxis axis in YAxes.ToList())
            {
                axis.UnitWidth = unitWidth;
            }
        }

        public void SetXAxesMinStep(double minStep)
        {
            foreach (ICartesianAxis axis in XAxes.ToList())
            {
                axis.MinStep = minStep;
            }
        }
        
        public void SetYAxesMinStep(double minStep)
        {
            foreach (ICartesianAxis axis in YAxes.ToList())
            {
                axis.MinStep = minStep;
            }
        }

        public void SetXAxesMaxLimit(double maxLimit)
        {
            foreach (ICartesianAxis axis in XAxes.ToList())
            {
                axis.MaxLimit = maxLimit;
            }
        }
        
        public void SetYAxesMaxLimit(double maxLimit)
        {
            foreach (ICartesianAxis axis in YAxes.ToList())
            {
                axis.MaxLimit = maxLimit;
            }
        }

        public void SetXAxesMinLimit(double minLimit)
        {
            foreach (ICartesianAxis axis in XAxes.ToList())
            {
                axis.MinLimit = minLimit;
            }
        }
        
        public void SetYAxesMinLimit(double minLimit)
        {
            foreach (ICartesianAxis axis in YAxes.ToList())
            {
                axis.MinLimit = minLimit;
            }
        }
    }
}
