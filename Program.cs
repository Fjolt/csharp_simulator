
using System;
using System.Runtime.InteropServices;
using System;
using csharp_simulator.Bodies;


static class Native
{
    [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int on_init(string kernelsDir);

    [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int sun_position_iso8601(string utcIso, double[] pos_km);

    [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr get_last_error();

    [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int moon_position_iso8601(string utcIso, double[] pos_km);

    public static string LastError => Marshal.PtrToStringAnsi(get_last_error()) ?? "";
}


class Program
{
    static void Main()
    {
        // init SPICE once
        var spice = new SpiceContext("kernels");

        var sun  = new Sun(spice);
        var moon = new Moon(spice);

        DateTime start = DateTime.UtcNow.Date; // or your YAML epoch

        for (int d = 0; d < 30; d++)
        {
            var t = start.AddDays(d);

            sun.UpdatePosition(t);
            moon.UpdatePosition(t);

            Console.WriteLine(
                $"{t:yyyy-MM-dd}  " +
                $"Sun:  X={sun.PositionX:F0} km  Y={sun.PositionY:F0} km  Z={sun.PositionZ:F0} km   " +
                $"Moon: X={moon.PositionX:F0} km  Y={moon.PositionY:F0} km  Z={moon.PositionZ:F0} km");
        }
    }
}