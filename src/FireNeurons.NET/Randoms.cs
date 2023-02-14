using Math.NET;
using Math.NET.Distributions;

namespace FireNeurons.NET;

public static class Randoms
{
    /// <param name="incomingAmount">the amount of Neurons in the layer before the connection</param>
    /// <param name="outgoigAmount">the amount of Neurons in the layer after the connection</param>
    public static double GetRandom(Activation activation, int incomingAmout = 1, int outgoingAmount = 1)
    {
        return activation switch
        {
            Activation.Identity => UniformRandom(-1, +1),
            Activation.Sigmoid => XavierRandom(incomingAmout + 1, outgoingAmount + 1),
            Activation.TanH => XavierRandom(incomingAmout + 1, outgoingAmount + 1),
            Activation.Relu => HeRandom(incomingAmout + 1),
            Activation.LeakyRelu => HeRandom(incomingAmout + 1),
            _ => throw new ArgumentException("ERROR: Invalid activation!"),
        };
    }

    /// <param name="incomingAmount">the amount of Neurons in the layer before the connection</param>
    /// <param name="outgoigAmount">the amount of Neurons in the layer after the connection</param>
    public static double XavierRandom(int incomingAmount, int outgoigAmount)
    {
        double xavierFactor = 6.0.Sqrt() / (incomingAmount + outgoigAmount);
        return UniformRandom(-xavierFactor, +xavierFactor);
    }
    /// <param name="incomingAmount">the amount of Neurons in the layer before the connection</param>
    public static double HeRandom(int incomingAmount)
    {
        var stdDev = (2.0 / incomingAmount).Sqrt();
        var gaussian = Gaussian.ByMeanDeviation(0, stdDev);
        return gaussian.GetRandom();
    }

    public static double UniformRandom(double min, double max)
    {
        double range = max - min;
        if (range <= 0)
        {
            throw new Exception("ERROR: max must be greater than min!");
        }

        return (GlobalRandom.NextDouble() * range) - min;
    }
}
