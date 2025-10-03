using DensoMTecGaugeReader.Core.Common.Enums;
using DensoMTecGaugeReader.Core.Models;
using System;
using System.Linq;

namespace DensoMTecGaugeReader.Core.Services.Measurement
{
    /// <summary>
    /// Maps an angle to a real-world value based on gauge configuration.
    /// </summary>
    public class ScaleMapper
    {
        public double MapAngleToValue(GaugeConfig config, double angle)
        {
            if (config.NumberToAngleMap.Any())
            {
                return MapUsingTickMarks(config, angle);
            }

            return MapLinear(config, angle);
        }

        private double MapLinear(GaugeConfig config, double angle)
        {
            double angleRange = config.EndAngle - config.StartAngle;
            if (angleRange < 0) angleRange += 360.0;

            double relative = angle - config.StartAngle;
            if (relative < 0) relative += 360.0;

            double ratio = angleRange == 0 ? 0 : (relative / angleRange);
            return config.MinValue + ratio * (config.MaxValue - config.MinValue);
        }

        private double MapUsingTickMarks(GaugeConfig config, double angle)
        {

            var sorted = config.NumberToAngleMap
                .Select(kvp => new { Value = double.Parse(kvp.Key), Angle = kvp.Value })
                .OrderBy(x => x.Angle)
                .ToList();

            if (angle <= sorted.First().Angle)
                return sorted.First().Value;
            if (angle >= sorted.Last().Angle)
                return sorted.Last().Value;

            for (int i = 0; i < sorted.Count - 1; i++)
            {
                var left = sorted[i];
                var right = sorted[i + 1];

                if (angle >= left.Angle && angle <= right.Angle)
                {
                    double ratio = (angle - left.Angle) / (right.Angle - left.Angle);
                    return left.Value + ratio * (right.Value - left.Value);
                }
            }

            return config.MinValue;
        }
    }
}