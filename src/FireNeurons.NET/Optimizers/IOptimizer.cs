using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Optimizers;

public abstract class IOptimizer
{
    public Func<Neuron, double, double> LossDerivative { get; init; }
    public double LearningRate { get; init; }

    public abstract IOptimizerData DataInstance { get; }

    public IOptimizer(Func<Neuron, double, double> lossDerivative, double learningRate)
    {
        this.LearningRate = learningRate;
        this.LossDerivative = lossDerivative;
    }

    public abstract void CalculateDelta(IOptimizerData optimizerData);

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
            sum += connection.Weight * connection.OutputNeuron.OptimizerData.Gradient;
        }

        SetAllGradients(neuron, sum);
    }

    private static void SetAllGradients(Neuron neuron, double loss)
    {
        var derivation = neuron.Blank.Derivate(neuron.Activation);
        double gradient = derivation * loss;

        neuron.OptimizerData.Gradient = gradient;

        foreach (var connection in neuron.Connections)
        {
            connection.OptimizerData.Gradient = gradient * connection.InputNeuron.Value;
        }
    }
}

public record IOptimizerData
{
    public double Delta { get; set; }
    public double Gradient { get; set; }
}