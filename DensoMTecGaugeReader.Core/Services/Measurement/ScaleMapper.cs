namespace DensoMTecGaugeReader.Core.Services.Measurement
{
    /// <summary>
    /// Maps an angle to a physical value based on gauge config.
    /// </summary>
    public static class ScaleMapper
    {
        public static double Map(
            double angle,
            double minValue,
            double maxValue,
            double startAngle,
            double endAngle)
        {
            if (angle <= startAngle) return minValue;
            if (angle >= endAngle) return maxValue;

            double t = (angle - startAngle) / (endAngle - startAngle);
            return minValue + t * (maxValue - minValue);
        }
    }
}