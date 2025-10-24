namespace csharp_simulator.Bodies;

public class Satellite
{
    public double PositionX { get; set; }
    public double PositionY { get; set; }
    public double PositionZ { get; set; }

    public double VelocityX { get; set; }
    public double VelocityY { get; set; }
    public double VelocityZ { get; set; }

    public string Path { get; set; }

    public double Mass { get; set; }
    public double SizeX { get; set; }
    public double SizeY { get; set; }
    public double SizeZ { get; set; }

    public double ThrusterPower { get; set; }
    public double Impulse { get; set; }
}
