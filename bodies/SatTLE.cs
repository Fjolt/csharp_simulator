namespace csharp_simulator.Bodies;

public class SatTLE
{
    public string TleLine1 { get; set; }
    public string TleLine2 { get; set; }
    public string Path { get; set; }
    public double Mass { get; set; }
    public double SizeX { get; set; }
    public double SizeY { get; set; }
    public double SizeZ { get; set; }
    public double ThrusterPower { get; set; }
    public double Impulse { get; set; }
}