
using System;
using Bodies;
using Interop;

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