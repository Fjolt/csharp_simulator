
// Program.cs (snippet)
using System;
using Bodies;
using Utils;
using Burns;
using 
System.Diagnostics;

class Program
{
    public static void Propagate(int samples, double step, SatTLE satt, RootCfg cfg)
    {
        for (int i = 2; i < samples + 2; i++)
        {
            var stateBefore = Propagation.FromTLEAtEpoch(satt, cfg.epoch, (i - 1) * step); // or FromTLEAtUtc(sat, cfg.epoch)
            var stateAfterr = Propagation.FromTLEAtEpoch(satt, cfg.epoch, i * step); // or FromTLEAtUtc(sat, cfg.epoch)

            for (double offset = 0.5; offset < step; offset += 0.5)
            {
                Vector3 pos = Propagation.LerpPositionByOffset(stateBefore, stateAfterr, offset, step);
                Console.WriteLine($"[  ] epoch: {stateBefore.EpochUtc.ToUniversalTime().AddSeconds(offset):O}");
                Console.WriteLine($"  r (m):  ({pos.X:F3}, {pos.Y:F3}, {pos.Z:F3})");
                Console.WriteLine($"  v (m/s):({stateBefore.VelocityX:F6}, {stateBefore.VelocityY:F6}, {stateBefore.VelocityZ:F6})");
                // Use pos.X, pos.Y, pos.Z (meters) for your object at t0 + offset
            }

        }
        
    }
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
        /* foreach (var kv in cfg.satellites)
        {
            string name = kv.Key;
            var sat = kv.Value;

            var state = Propagation.FromTLEAtEpoch(sat, cfg.epoch, 3.0); // or FromTLEAtUtc(sat, cfg.epoch)

            Console.WriteLine($"[{name}] epoch: {state.EpochUtc:O}");
            Console.WriteLine($"  r (m):  ({state.PositionX:F3}, {state.PositionY:F3}, {state.PositionZ:F3})");
            Console.WriteLine($"  v (m/s):({state.VelocityX:F6}, {state.VelocityY:F6}, {state.VelocityZ:F6})");
        } */

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
        int samples = 2;   // 1201
        double step = 1.0;            // seconds

        var satt = cfg.satellites["sat1"];
        
        string L1 = satt.tle_line1;
        string L2 = satt.tle_line2;

        Console.WriteLine(L1);
        Console.WriteLine(L2);

        var stateBeforeeee = Propagation.FromTLEAtEpoch(satt, cfg.epoch, 2 * step);

        Console.WriteLine($" Position X: {stateBeforeeee.PositionX} ");
        Console.WriteLine($" Position Y: {stateBeforeeee.PositionY}");
        Console.WriteLine($" Position Z: {stateBeforeeee.PositionZ}");
        Console.WriteLine($" Velocity X: {stateBeforeeee.VelocityX}");
        Console.WriteLine($" Velocity X: {stateBeforeeee.VelocityY}");
        Console.WriteLine($" Velocity X: {stateBeforeeee.VelocityZ}");
        Console.WriteLine($" Epoch: {stateBeforeeee.EpochUtc}");

        var stateBurn = ImpulseBurn.burnWithDeltaV(stateBeforeeee, 0.0, 0.0, 0.0);
        Debug.Assert(stateBeforeeee.PositionX == stateBurn.PositionX);
        Debug.Assert(stateBeforeeee.PositionY == stateBurn.PositionY);
        Debug.Assert(stateBeforeeee.PositionZ == stateBurn.PositionZ);
        Debug.Assert(stateBeforeeee.VelocityX == stateBurn.VelocityX);
        Debug.Assert(stateBeforeeee.VelocityY == stateBurn.VelocityY);
        Debug.Assert(stateBeforeeee.VelocityZ == stateBurn.VelocityZ);
        Debug.Assert(stateBeforeeee.EpochUtc == stateBurn.EpochUtc);
        
        var keplerElems = OrbitalUtils.NumericalToKepler(stateBeforeeee);

        Console.WriteLine($"  SemiMajorAxisKm: {keplerElems. SemiMajorAxisKm} ");
        Console.WriteLine($" Eccentricity: {keplerElems.Eccentricity}");
        Console.WriteLine($" InclinationRad: {keplerElems.InclinationRad}");
        Console.WriteLine($" RAANRad: {keplerElems.RAANRad}");
        Console.WriteLine($" ArgPerigeeRad: {keplerElems.ArgPerigeeRad}");
        Console.WriteLine($" MeanAnomalyRad: {keplerElems.MeanAnomalyRad}");

        Console.WriteLine($" BEFORE: {satt.tle_line1}");
        Console.WriteLine($" BEFORE: {satt.tle_line2}");
        
        Propagate(samples, step, satt, cfg);

        satt = TLEUtils.updateTLE(stateBeforeeee, satt);

        Console.WriteLine($" AFTER: {satt.tle_line1}");
        Console.WriteLine($" AFTER: {satt.tle_line2}");

        Propagate(samples, step, satt,cfg);

        stateBurn = ImpulseBurn.burnWithDeltaV(stateBeforeeee, 0.001, 0.0, 0.0);

        Console.WriteLine($" BEFORE: {satt.tle_line1}");
        Console.WriteLine($" BEFORE: {satt.tle_line2}");

        satt = TLEUtils.updateTLE(stateBurn, satt);

        Console.WriteLine($" AFTER: {satt.tle_line1}");
        Console.WriteLine($" AFTER: {satt.tle_line2}");

        Propagate(samples, step, satt,cfg);

    }
}
