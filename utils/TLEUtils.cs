using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using One_Sgp4;

namespace Utils
{
    public static class TLEUtils
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        public static SatTLE updateTLE(SatState stateNew, SatTLE satOrig)
        {
            var keplerElems = OrbitalUtils.NumericalToKepler(stateNew);
            satOrig.tle_line2 = buildLine2(keplerElems, satOrig.tle_line2);
            return satOrig;
        }

        public static string buildLine2(KeplerElements keplerElements, string line2Orig)
        {
            double inclinationRad = keplerElements.InclinationRad;
            double raanRad = keplerElements.RAANRad;
            double eccentricity = keplerElements.Eccentricity;
            double argPerigeeRad = keplerElements.ArgPerigeeRad;
            double meanAnomalyRad = keplerElements.MeanAnomalyRad;
            double meanMotionRevPerDay = keplerElements.MeanMotion;

            Console.WriteLine($" MEAN Motiooom: {meanMotionRevPerDay}");

            var buffer = new StringBuilder();
            string FormatF34(double value) =>
                value.ToString("0.0000", Invariant);

            string FormatF211(double value) =>
                value.ToString("0.00000000", Invariant);

            buffer.Append(line2Orig.Substring(0, 7));

            // 3) Inclination [deg], width 8, right-justified
            double incDeg = inclinationRad * 180.0 / Math.PI;
            buffer.Append(' ');
            buffer.Append(Pad(FormatF34(incDeg), ' ', 8, rightJustify: true));

            // 4) RAAN [deg], width 8
            double raanDeg = raanRad * 180.0 / Math.PI;
            buffer.Append(' ');
            buffer.Append(Pad(FormatF34(raanDeg), ' ', 8, rightJustify: true));

            // 5) Eccentricity * 1e7 (integer, no decimal point), width 7, pad with zeros
            int eccScaled = (int)Math.Round(eccentricity * 1.0e7);
            buffer.Append(' ');
            buffer.Append(Pad(eccScaled.ToString(Invariant), '0', 7, rightJustify: true));

            // 6) Argument of perigee [deg], width 8
            double argPerigeeDeg = argPerigeeRad * 180.0 / Math.PI;
            buffer.Append(' ');
            buffer.Append(Pad(FormatF34(argPerigeeDeg), ' ', 8, rightJustify: true));

            // 7) Mean anomaly [deg], width 8
            double meanAnomalyDeg = meanAnomalyRad * 180.0 / Math.PI;
            buffer.Append(' ');
            buffer.Append(Pad(FormatF34(meanAnomalyDeg), ' ', 8, rightJustify: true));

            buffer.Append(' ');
            buffer.Append(Pad(FormatF211(meanMotionRevPerDay), ' ', 11, rightJustify: true));
            Console.WriteLine($" LENGTH: {line2Orig.Length}");

            buffer.Append(line2Orig.Substring(64, 5));

            // 10) Checksum (last column)
            char cksum = computeTleChecksum(buffer.ToString());
            buffer.Append(cksum);

            return buffer.ToString();
        }


        private static string Pad(string value, char padChar, int width, bool rightJustify)
        {
            if (value.Length > width)
            {
                return value.Substring(value.Length - width, width);
            }

            return rightJustify
                ? value.PadLeft(width, padChar)
                : value.PadRight(width, padChar);
        }

        /// <summary>
        /// TLE checksum: sum of all digits + 1 per '-' sign, mod 10.
        /// </summary>
        private static char computeTleChecksum(string lineWithoutChecksum)
        {
            int sum = 0;
            foreach (char c in lineWithoutChecksum)
            {
                if (c >= '0' && c <= '9')
                {
                    sum += c - '0';
                }
                else if (c == '-')
                {
                    sum += 1;
                }
            }

            int digit = sum % 10;
            return (char)('0' + digit);
        }
        
    }
}