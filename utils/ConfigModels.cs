using System;
using System.Collections.Generic;

namespace Utils
{

    public sealed class Vector3
    {
        public double position_x { get; set; }
        public double position_y { get; set; }
        public double position_z { get; set; }
    }

    public sealed class EarthOri
    {
        public double rotation_x { get; set; }
        public double rotation_y { get; set; }
        public double rotation_z { get; set; }
    }

    public sealed class RootCfg
    {
        public Vector3 sun { get; set; } = new();
        public EarthOri earth { get; set; } = new();
        public Vector3 moon { get; set; } = new();
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

}
