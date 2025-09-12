namespace DensoMTecGaugeReader.Core.Contracts
{
    /// <summary>
    /// Base interface for all gauge faces (Round, Square, Digital).
    /// Each face type has its own way of normalizing and extracting hand info.
    /// </summary>
    public interface IGaugeFace
    {
        GaugeFaceType FaceType { get; }
        Mat Normalize(Mat raw);
        GaugeHandInfo? ExtractHand(Mat normalizedImage);
    }
}