global using static Globals;

public static class Globals
{
    public static Random GlobalRandom { get; set; } = new();

    public static double RandomDouble(double min, double max)
    {
        double range = max - min;
        if (range <= 0)
        {
            throw new Exception("ERROR: max must be greater than min!");
        }

        return (GlobalRandom.NextDouble() * range) - min;
    }
}
