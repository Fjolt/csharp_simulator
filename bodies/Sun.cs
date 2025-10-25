using System;
using Interop;

namespace Bodies;

public sealed class Sun
{
    private readonly SpiceContext _ctx;

    public Sun(SpiceContext ctx) => _ctx = ctx;

    public double PositionX { get; private set; }
    public double PositionY { get; private set; }
    public double PositionZ { get; private set; }

    /// <summary>Updates the Sun's position (J2000, w.r.t. Earth) at the given UTC time.</summary>
    public void UpdatePosition(DateTime utc)
    {
        var iso = SpiceContext.ToIsoUtc(utc);
        var v = new double[3];
        int rc = Native.body_position_iso8601(iso, v, "SUN");
        if (rc != 0) throw new InvalidOperationException("Sun query failed: " + Native.LastError);

        PositionX = v[0];
        PositionY = v[1];
        PositionZ = v[2];
    }
}
