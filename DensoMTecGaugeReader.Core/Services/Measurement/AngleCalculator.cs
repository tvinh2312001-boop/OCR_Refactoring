using DensoMTecGaugeReader.Core.Models;
using System;

namespace DensoMTecGaugeReader.Core.Services.Measurement
{
    /// <summary>
    /// Provides methods to calculate angles from gauge hand positions.
    /// </summary>
    public class AngleCalculator
    {
        /// <summary>
        /// Calculates the angle (in degrees) of the hand relative to the gauge face center.
        /// 0Åã is along the positive X-axis, increasing counter-clockwise.
        /// </summary>
        public double CalculateAngle(GaugeFaceInfo face, GaugeHandInfo hand)
        {
            double dx = hand.EndX - face.CenterX;
            double dy = hand.EndY - face.CenterY;

            double angleRad = Math.Atan2(dy, dx); // range: -ÉŒ to +ÉŒ
            double angleDeg = angleRad * (180.0 / Math.PI);

            if (angleDeg < 0)
            {
                angleDeg += 360.0;
            }

            return angleDeg;
        }
    }
}