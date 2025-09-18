using DensoMTecGaugeReader.Core.Common.Enums;

namespace DensoMTecGaugeReader.Core.Contracts
{
    /// <summary>
    /// Base interface for all gauge faces (round, square, digital).
    /// </summary>
    public interface IGaugeFace
    {
        /// <summary>
        /// Strongly typed face type (enum).
        /// </summary>
        GaugeFaceType FaceType { get; }
    }
}