global using static Globals;
global using static NeuralNetwork.NET.Randoms;

public static class Globals
{
    public static Random GlobalRandom { get; set; } = new();

    public static double Discount(this double oldValue, double newValue, double discountFactor)
    {
        return (oldValue * discountFactor) + ((1 - discountFactor) * newValue);
    }
}
