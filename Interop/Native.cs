using System.Runtime.InteropServices;

namespace Interop
{
    internal static class Native
    {
        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int on_init(string kernelsDir);

        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sun_position_iso8601(string utcIso, double[] pos_km);

        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int moon_position_iso8601(string utcIso, double[] pos_km);

        [DllImport("orbit_dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_last_error();

        public static string LastError => Marshal.PtrToStringAnsi(get_last_error()) ?? "";
    }
}