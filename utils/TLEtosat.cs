using System;
using System.Collections.Generic;
using One_Sgp4;

namespace Utils;

public static class Propagation
{

    public static SatState FromTLEAtEpoch(SatTLE tle, DateTime epochUtc, double stepSeconds)
    {
        // epochUtc can be set so that we can have sats initialized at the same time
        // not based on TLE
        return FromTLEAtUtc(tle, epochUtc, stepSeconds);
    }

    public static SatState FromTLEAtUtc(SatTLE tle, DateTime utc, double stepSeconds)
    {
        if (tle == null) throw new ArgumentNullException(nameof(tle));
        if (string.IsNullOrWhiteSpace(tle.tle_line1) || string.IsNullOrWhiteSpace(tle.tle_line2))
            throw new ArgumentException("TLE lines must be non-empty.", nameof(tle));

        // Parse TLE and build propagator
        Tle tleItem = ParserTLE.parseTle(tle.tle_line1, tle.tle_line2, "sat");
        var sgp4 = new Sgp4(tleItem, Sgp4.wgsConstant.WGS_84);

        // Build [start, stop] times (UTC). Ensure at least one sample by making stop = start+1s.
        var start = new EpochTime(utc.ToUniversalTime());
        var stop = utc.ToUniversalTime().AddSeconds(stepSeconds);
        var stopEpoch = new EpochTime(stop);
        double stepMinutes = stepSeconds / 60.0;
        sgp4.runSgp4Cal(start, stopEpoch, stepMinutes);
        List<Sgp4Data> results = sgp4.getResults();

        if (results == null || results.Count == 0)
            throw new InvalidOperationException("SGP4 returned no samples.");

        var d = results[1]; // km and km/s per README

        const double km2m = 1000.0;
        return new SatState
        {
            PositionX = d.getX() * km2m,
            PositionY = d.getY() * km2m,
            PositionZ = d.getZ() * km2m,
            VelocityX = d.getXDot() * km2m,
            VelocityY = d.getYDot() * km2m,
            VelocityZ = d.getZDot() * km2m,
            EpochUtc = utc.ToUniversalTime().AddSeconds(stepSeconds)
        };
    }
    /// <summary>
    /// Linearly interpolate position between two SatState endpoints using fraction t in [0,1].
    /// Returns position in meters (to match your SatState units).
    /// </summary>
    public static Vector3 LerpPosition(SatState a, SatState b, double t)
    {
        // clamp t
        if (t < 0) t = 0;
        if (t > 1) t = 1;

        double x = a.PositionX + (b.PositionX - a.PositionX) * t;
        double y = a.PositionY + (b.PositionY - a.PositionY) * t;
        double z = a.PositionZ + (b.PositionZ - a.PositionZ) * t;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Interpolate position by time offset within a step.
    /// Example: stepSeconds = 3.0, offsetSeconds = 0.5, 1.0, 1.5, ...
    /// </summary>
    public static Vector3 LerpPositionByOffset(SatState a, SatState b, double offsetSeconds, double stepSeconds)
    {
        if (stepSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(stepSeconds));
        // Convert offset to fraction; clamp inside LerpPosition
        double t = offsetSeconds / stepSeconds;
        return LerpPosition(a, b, t);
    }

    /// <summary>
    /// Interpolate by actual timestamp between a.EpochUtc and b.EpochUtc.
    /// </summary>
    public static Vector3 LerpPositionByTime(SatState a, SatState b, DateTime atUtc)
    {
        var t0 = a.EpochUtc.ToUniversalTime();
        var t1 = b.EpochUtc.ToUniversalTime();
        if (t1 <= t0) return new Vector3(a.PositionX, a.PositionY, a.PositionZ); // guard

        double stepSeconds = (t1 - t0).TotalSeconds;
        double offset = (atUtc.ToUniversalTime() - t0).TotalSeconds;
        return LerpPositionByOffset(a, b, offset, stepSeconds);
    }

}
