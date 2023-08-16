using FireMath.NET;

namespace FireNeurons.NET.Optimisation;

using Objects;


public abstract class IOptimiser
{
    public Func<Neuron, object?, object, double> LossDerivative { get; init; }
    public double LearningRate { get; init; }

    public abstract IOptimiserData DataInstance { get; }

    public IOptimiser(Func<Neuron, object?, object, double> lossDerivative, double learningRate)
    {
        this.LearningRate = learningRate;
        this.LossDerivative = lossDerivative;
    }

    public abstract void CalculateDelta(IOptimiserData optimiserData);

    public void CalculateGradient(Neuron neuron, double loss)
    {
        if (neuron.OutgoingConnections.Count != 0)
        {
            throw new Exception("ERROR: Can't set a loss-gradient for a none-output neuron!");
        }

        SetAllGradients(neuron, loss);
    }
    public void CalculateGradient(Neuron neuron)
    {
        if (neuron.OutgoingConnections.Count == 0)
        {
            throw new Exception("ERROR: Can't set a normal-gradient for an output neuron!");
        }

        double sum = 0;
        foreach (var connection in neuron.OutgoingConnections)
        {
            sum += connection.Weight * connection.OutputNeuron.OptimiserData.Gradient;
        }

        SetAllGradients(neuron, sum);
    }

    private static void SetAllGradients(Neuron neuron, double loss)
    {
        var derivation = neuron.Blank.Derivate(neuron.Options.Activation);
        double gradient = derivation * loss;

        neuron.OptimiserData.Gradient = gradient;

        foreach (var connection in neuron.Connections)
        {
            double weightDecay_L1 = connection.Weight / connection.Weight.Abs();
            double weightDecay_L2 = 2 * connection.Weight;
            double weightDecay = weightDecay_L1 + weightDecay_L2;

            double adaptedGradient = gradient + (neuron.Options.WeightDecay * weightDecay);
            connection.OptimiserData.Gradient = adaptedGradient * connection.InputNeuron.GetValue(true);
        }
    }
}

public record IOptimiserData
{
    public double Delta { get; set; }
    public double Gradient { get; set; }
}