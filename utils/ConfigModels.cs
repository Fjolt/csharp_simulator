using System;
using System.Collections.Generic;

namespace Utils
{

    public sealed class Vector3
    {
        public double X, Y, Z;
        public Vector3(double x, double y, double z) { X = x; Y = y; Z = z; }
    }

    public sealed class EarthOri
    {
        public double rotation_x { get; set; }
        public double rotation_y { get; set; }
        public double rotation_z { get; set; }
    }

    public sealed class RootCfg
    {
        public string type { get; set; } = string.Empty;
        public DateTime epoch { get; set; }
        public Dictionary<string, SatTLE> satellites { get; set; } = new();
    }

    public sealed class SatTLE
    {
        public string tle_line1 { get; set; } = string.Empty;
        public string tle_line2 { get; set; } = string.Empty;
        public string path { get; set; } = string.Empty;
        public double mass { get; set; }
        public double size_x { get; set; }
        public double size_y { get; set; }
        public double size_z { get; set; }
        public double thruster_power { get; set; }
        public double impulse { get; set; }
    }

    public sealed class SatState
    {
        public double PositionX, PositionY, PositionZ;   // meters
        public double VelocityX, VelocityY, VelocityZ;   // m/s
        public DateTime EpochUtc;
    }

    public sealed class KeplerElements
    {
        public double SemiMajorAxisKm;   // a
        public double Eccentricity;      // e
        public double InclinationRad;    // i
        public double RAANRad;           // Ω
        public double ArgPerigeeRad;     // ω
        public double MeanAnomalyRad;    // M
        public double MeanMotion;
    }

}
