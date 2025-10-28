
// Program.cs (snippet)
using System;
using Bodies;
using Utils;

class Program
{
    static void Main()
    {
        // Initialize CSPICE once (for Sun/Moon later)
        var spice = new SpiceContext("kernels");

        // Load YAML
        RootCfg cfg =YamlReader.Load("C:/Users/Veronika/Desktop/useful_stuff/wokr/orekit_wrapper/csharp_simulator/configs/config.yaml");
        Console.WriteLine($"Config epoch (UTC): {cfg.epoch:O}");
        Console.WriteLine($"Type: {cfg.type}");
        Console.WriteLine($"Sat count: {cfg.satellites.Count}");

        // For each satellite: parse TLE and get state at its own epoch
        foreach (var kv in cfg.satellites)
        {
            string name = kv.Key;
            var sat = kv.Value;

            var state = TLEtoSat.FromTLEAtEpoch(sat, cfg.epoch, 3.0); // or FromTLEAtUtc(sat, cfg.epoch)

            Console.WriteLine($"[{name}] epoch: {state.EpochUtc:O}");
            Console.WriteLine($"  r (m):  ({state.PositionX:F3}, {state.PositionY:F3}, {state.PositionZ:F3})");
            Console.WriteLine($"  v (m/s):({state.VelocityX:F6}, {state.VelocityY:F6}, {state.VelocityZ:F6})");
        }

        // If you also want Sun/Moon at cfg.epoch:
        var sun = new Sun(spice);
        var moon = new Moon(spice);
        sun.UpdatePosition(cfg.epoch);
        moon.UpdatePosition(cfg.epoch);
        Console.WriteLine($"Sun @ epoch:  X={sun.PositionX:F0} Y={sun.PositionY:F0} Z={sun.PositionZ:F0} km");
        Console.WriteLine($"Moon @ epoch: X={moon.PositionX:F0} Y={moon.PositionY:F0} Z={moon.PositionZ:F0} km");

        Console.WriteLine($"NOW THE REAL FUN BEGINS");

        DateTime t0 = DateTime.UtcNow;

        // Include endpoint:
        int samples = 15;   // 1201
        double step = 3.0;            // seconds

        for (int i = 1; i < samples+1; i++)
        {
            foreach (var kv in cfg.satellites)
            {
                string name = kv.Key;
                var sat = kv.Value;

                var state = TLEtoSat.FromTLEAtEpoch(sat, cfg.epoch, i*step); // or FromTLEAtUtc(sat, cfg.epoch)

                Console.WriteLine($"[{name}] epoch: {state.EpochUtc:O}");
                Console.WriteLine($"  r (m):  ({state.PositionX:F3}, {state.PositionY:F3}, {state.PositionZ:F3})");
                Console.WriteLine($"  v (m/s):({state.VelocityX:F6}, {state.VelocityY:F6}, {state.VelocityZ:F6})");
            }

        }
    }
}
