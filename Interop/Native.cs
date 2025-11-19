using System.Runtime.InteropServices;

namespace Interop
{
    internal static class Native
    {
        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int on_init(string kernelsDir);

        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int body_position_iso8601(string utcIso, double[] pos_km, string body);

        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_last_error();

        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int kepler_from_state_iso8601(
            string utcIso,
            double rx, double ry, double rz,
            double vx, double vy, double vz,
            double mu,
            double[] outKepler // length >= 6
        );
        public static string LastError => Marshal.PtrToStringAnsi(get_last_error()) ?? "";
    }
}