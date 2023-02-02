global using static Globals;
global using static NeuralNetwork.NET.Randoms;

public static class Globals
{
    public static Random GlobalRandom { get; set; } = new();

    public static double Discount(this double oldValue, double newValue, double discountFactor)
    {
        return (oldValue * discountFactor) + ((1 - discountFactor) * newValue);
    }

    /// <param name="timeStep">starts at 1 !!!</param>
    public static double FixBias(this double value, double discountFactor, int timeStep)
    {
        return value / (1 - Math.Pow(discountFactor, timeStep));
    }
}
