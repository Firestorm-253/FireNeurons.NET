namespace NeuralNetwork.NET;

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
        double xavierFactor = Math.Sqrt(6) / (incomingAmount + outgoigAmount);
        return UniformRandom(-xavierFactor, +xavierFactor);
    }
    /// <param name="incomingAmount">the amount of Neurons in the layer before the connection</param>
    public static double HeRandom(int incomingAmount)
    {
        var stdDev = Math.Sqrt(2.0 / incomingAmount);
        return GaussianRandom(mean: 0, stdDev: stdDev);
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
    public static double GaussianRandom(double mean = 0, double stdDev = 1)
    {
        double u1 = 1.0 - GlobalRandom.NextDouble();
        double u2 = 1.0 - GlobalRandom.NextDouble();

        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
    }
}
