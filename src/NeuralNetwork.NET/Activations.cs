namespace NeuralNetwork.NET;

public enum Activation
{
    Identity,

    Sigmoid,
    TanH,
    Relu,
    LeakyRelu,
}

public static class Activations
{
    public static double Activate(this double value, Activation activation)
    {
        return activation switch
        {
            Activation.Identity => Identity(value),
            Activation.Sigmoid => Sigmoid(value),
            Activation.TanH => TanH(value),
            Activation.Relu => Relu(value),
            Activation.LeakyRelu => LeakyRelu(value),
            _ => throw new NotImplementedException(),
        };
    }
    public static double Derivate(this double value, Activation activation)
    {
        return activation switch
        {
            Activation.Identity => Identity_d(value),
            Activation.Sigmoid => Sigmoid_d(value),
            Activation.TanH => TanH_d(value),
            Activation.Relu => Relu_d(value),
            Activation.LeakyRelu => LeakyRelu_d(value),
            _ => throw new NotImplementedException(),
        };
    }

    public static double Identity(double value)
    {
        return value;
    }
    public static double Identity_d(double _)
    {
        return 1;
    }

    public static double Sigmoid(double value)
    {
        return 1 / (1 + Math.Pow(Math.E, -value));
    }
    public static double Sigmoid_d(double value)
    {
        var ePowX = Math.Pow(Math.E, value);
        var ePowXp1 = ePowX + 1;
        return ePowX / (ePowXp1 * ePowXp1);
    }

    public static double TanH(double value)
    {
        return (2 / (1 + Math.Pow(Math.E, (-2 * value)))) - 1;
    }
    public static double TanH_d(double value)
    {
        var tanH = TanH(value);
        return 1 - (tanH * tanH);
    }

    public static double Relu(double value)
    {
        return (value > 0) ? value : 0;
    }
    public static double Relu_d(double value)
    {
        return (value > 0) ? 1 : 0;
    }

    public static double LeakyRelu(double value, double alpha = 0.3)
    {
        return (value > 0) ? value : (value * alpha);
    }
    public static double LeakyRelu_d(double value, double alpha = 0.3)
    {
        return (value > 0) ? 1 : alpha;
    }
}