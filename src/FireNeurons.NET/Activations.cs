using Math.NET;

namespace FireNeurons.NET;

public enum Activation
{
    Identity,

    Sigmoid,
    TanH,
    Relu,
    LeakyRelu,
    Softplus,
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
            Activation.Softplus => Softplus(value),
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
            Activation.Softplus => Softplus_d(value),
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
        => Math.NET.Activations.Activations.Sigmoid(value);
    public static double Sigmoid_d(double value)
    {
        var ePowX = Base.Exp(value);
        var ePowXp1 = ePowX + 1;
        return ePowX / (ePowXp1 * ePowXp1);
    }

    public static double TanH(double value)
        => Math.NET.Activations.Activations.TanH(value);
    public static double TanH_d(double value)
    {
        var tanH = Math.NET.Activations.Activations.TanH(value);
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
        => Math.NET.Activations.Activations.LeakyRelu(value, alpha);
    public static double LeakyRelu_d(double value, double alpha = 0.3)
    {
        return (value > 0) ? 1 : alpha;
    }

    public static double Softplus(double value)
        => Math.NET.Activations.Activations.Softplus(value);
    public static double Softplus_d(double value)
    {
        return Sigmoid(value);
    }
}