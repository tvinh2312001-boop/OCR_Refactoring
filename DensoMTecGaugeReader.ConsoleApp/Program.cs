using System;
using System.IO;
using System.Text.Json;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Application;
using DensoMTecGaugeReader.Core.Common.Enums;
using System.Collections.Generic;

namespace DensoMTecGaugeReader.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Load config từ AppConfig.json
                var configText = File.ReadAllText("AppConfig.json");
                var appConfig = JsonSerializer.Deserialize<AppConfig>(configText);

                if (appConfig == null)
                {
                    Console.WriteLine("❌ Failed to load AppConfig.json");
                    return;
                }

                var gaugeConfig = new GaugeConfig
                {
                    GaugeId = appConfig.GaugeId,
                    GaugeType = Enum.Parse<GaugeType>(appConfig.GaugeType, true),
                    FaceType = Enum.Parse<GaugeFaceType>(appConfig.FaceType, true),
                    Unit = appConfig.Unit,
                    StartAngle = appConfig.StartAngle,
                    EndAngle = appConfig.EndAngle,
                    MinValue = appConfig.MinValue,
                    MaxValue = appConfig.MaxValue,
                    NumberToAngleMap = appConfig.NumberToAngleMap
                };

                // Gọi API chính GaugeReader
                var reader = new GaugeReader();
                var result = reader.ReadGauge(appConfig.ImagePath, gaugeConfig, appConfig.GaugeId);

                Console.WriteLine("✅ Gauge Reading Result");
                Console.WriteLine($"Gauge ID : {appConfig.GaugeId}");
                Console.WriteLine($"Type     : {appConfig.GaugeType} ({appConfig.FaceType})");
                Console.WriteLine($"Angle    : {result.RawAngle:F2}°");
                Console.WriteLine($"Value    : {result.Value:F2} {result.Unit}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
    }

    public class AppConfig
    {
        public string ImagePath { get; set; } = "";
        public string GaugeId { get; set; } = "";
        public string GaugeType { get; set; } = "";
        public string FaceType { get; set; } = "";
        public string Unit { get; set; } = "";
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public Dictionary<string, double> NumberToAngleMap { get; set; } = new();
    }
}