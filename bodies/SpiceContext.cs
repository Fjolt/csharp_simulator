using System;
using System.IO;
using Interop;

namespace csharp_simulator.Bodies
{
    public sealed class SpiceContext
    {
        public string KernelsDir { get; }

        public SpiceContext(string kernelsDir)
        {
            KernelsDir = Path.GetFullPath(kernelsDir);
            int rc = Native.on_init(KernelsDir);
            if (rc != 0)
                throw new InvalidOperationException("CSPICE init failed: " + Native.LastError);
        }

        public static string ToIsoUtc(DateTime utc) =>
            utc.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
    }
}