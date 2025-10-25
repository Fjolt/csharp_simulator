using System;
using System.Collections.Generic;
using One_Sgp4;

namespace Utils;

public static class TLEtoSat
{
    public sealed class SatState
    {
        public double PositionX, PositionY, PositionZ;   // meters
        public double VelocityX, VelocityY, VelocityZ;   // m/s
        public DateTime EpochUtc;
    }

    public static SatState FromTLEAtEpoch(SatTLE tle)
    {
        var epochUtc = ParseTleEpochUtc(tle.tle_line1);
        return FromTLEAtUtc(tle, epochUtc);
    }

    public static SatState FromTLEAtUtc(SatTLE tle, DateTime utc)
    {
        if (tle == null) throw new ArgumentNullException(nameof(tle));
        if (string.IsNullOrWhiteSpace(tle.tle_line1) || string.IsNullOrWhiteSpace(tle.tle_line2))
            throw new ArgumentException("TLE lines must be non-empty.", nameof(tle));

        // Parse TLE and build propagator
        Tle tleItem = ParserTLE.parseTle(tle.tle_line1, tle.tle_line2, "sat");
        var sgp4 = new Sgp4(tleItem, Sgp4.wgsConstant.WGS_84);

        // Build [start, stop] times (UTC). Ensure at least one sample by making stop = start+1s.
        var start = new EpochTime(utc.ToUniversalTime());
        var stop  = new EpochTime(utc.ToUniversalTime());
        stop.addTick(1); // +1 second

        // Run and fetch first result
        double stepMinutes = 1.0 / 60.0; // 1 second steps
        sgp4.runSgp4Cal(start, stop, stepMinutes);
        List<Sgp4Data> results = sgp4.getResults();
        if (results == null || results.Count == 0)
            throw new InvalidOperationException("SGP4 returned no samples.");

        var d = results[0]; // km and km/s per README

        const double km2m = 1000.0;
        return new SatState
        {
            PositionX = d.getX()    * km2m,
            PositionY = d.getY()    * km2m,
            PositionZ = d.getZ()    * km2m,
            VelocityX = d.getXDot() * km2m,
            VelocityY = d.getYDot() * km2m,
            VelocityZ = d.getZDot() * km2m,
            EpochUtc  = utc.ToUniversalTime()
        };
    }

    // utils/TLEtoSat.cs  (replace the old ParseTleEpochUtc with this)
    private static DateTime ParseTleEpochUtc(string l1)
    {
        if (string.IsNullOrWhiteSpace(l1))
            throw new ArgumentException("TLE line 1 is empty.", nameof(l1));

        // 1) Try robust token-based parse: the 4th token is usually YYDDD.DDD...
        //    Example: "1 43013U 17073E   25085.93263889  .00000120  00000-0  20432-4 0  9994"
        //             tokens => [1] [43013U] [17073E] [25085.93263889] [...]
        var tokens = l1.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length >= 4 && tokens[3].Length >= 5) // "YYDDD..." minimum 5 chars
        {
            var yyddd = tokens[3];

            // Split into YY and DDD.fraction
            var yyStr = yyddd.Substring(0, 2);
            var doyStr = yyddd.Substring(2); // rest is DDD[.fraction]

            if (int.TryParse(yyStr, System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture, out int yy) &&
                double.TryParse(doyStr, System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out double dayOfYear))
            {
                int year = (yy >= 57) ? 1900 + yy : 2000 + yy;

                var jan1 = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                double whole = Math.Floor(dayOfYear) - 1.0; // DOY is 1-based
                double frac  = dayOfYear - Math.Floor(dayOfYear);
                return jan1.AddDays(whole).AddSeconds(frac * 86400.0);
            }
        }

        // 2) Fallback: if we *do* have fixed-width columns, parse those (cols 19–32).
        if (l1.Length >= 32)
        {
            var yyStr  = l1.Substring(18, 2);
            var doyStr = l1.Substring(20, 12).Trim();
            if (int.TryParse(yyStr, System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture, out int yy2) &&
                double.TryParse(doyStr, System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out double dayOfYear2))
            {
                int year = (yy2 >= 57) ? 1900 + yy2 : 2000 + yy2;

                var jan1 = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                double whole = Math.Floor(dayOfYear2) - 1.0;
                double frac  = dayOfYear2 - Math.Floor(dayOfYear2);
                return jan1.AddDays(whole).AddSeconds(frac * 86400.0);
            }
        }

        throw new ArgumentException("Could not parse TLE epoch from line 1.", nameof(l1));
    }

}
