using System.IO;
using YamlDotNet.Serialization;
using Interop;

namespace Utils;

public static class OrbitalUtils
{
    public static KeplerElements StateToKepler(SatState s)
    {
        // Earth μ in km^3/s^2
        const double muEarthKm3s2 = 3.986004418e5;

        double[] kepler = new double[6];

        string utcIso = s.EpochUtc.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss'Z'");

        int rc = Interop.Native.kepler_from_state_iso8601(
            utcIso,
            s.PositionX, s.PositionY, s.PositionZ,
            s.VelocityX, s.VelocityY, s.VelocityZ,
            muEarthKm3s2,
            kepler
        );

        if (rc != 0)
            throw new Exception($"CSPICE error {rc}: {Interop.Native.LastError}");

        double a = kepler[0];
        double nRadPerSec = Math.Sqrt(muEarthKm3s2 / (a * a * a)); // rad/s
        double meanMotionRevPerDay = nRadPerSec * 43200.0 / Math.PI; 

        Console.WriteLine($" a: {a}");
        Console.WriteLine($" MEAN MotAAAAAAAAA: {meanMotionRevPerDay}");

        return new KeplerElements
        {
            SemiMajorAxisKm = kepler[0],
            Eccentricity    = kepler[1],
            InclinationRad  = kepler[2],
            RAANRad         = kepler[3],
            ArgPerigeeRad   = kepler[4],
            MeanAnomalyRad  = kepler[5],
            MeanMotion      = meanMotionRevPerDay
        };
    }
}