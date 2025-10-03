namespace DensoMTecGaugeReader.Core.Common.Enums
{
    public enum GaugeType
    {
        Pressure,
        Thermo,
        Compound,
        Digital
    }

    public enum GaugeFaceType
    {
        Round,
        Square,
        Digital
    }

    public enum ScaleType
    {
        Pressure,
        Thermo50,
        Thermo100
    }

    public enum ResultStatus
    {
        Success,
        Failure,
        ValidationError,
        NotFound,
        Warning
    }
}