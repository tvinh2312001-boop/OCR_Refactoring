namespace DensoMTecGaugeReader.Core.Services.Measurement
{
    /// <summary>
    /// Maps normalized angle into gauge value using config.
    /// </summary>
    public static class ScaleMapper
    {
        public static double Map(double angle, double minValue, double maxValue, double startAngle, double endAngle)
        {
            if (angle <= startAngle) return minValue;
            if (angle >= endAngle) return maxValue;

            double ratio = (angle - startAngle) / (endAngle - startAngle);
            return minValue + ratio * (maxValue - minValue);
        }
    }
}