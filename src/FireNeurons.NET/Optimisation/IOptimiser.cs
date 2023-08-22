using FireMath.NET;

namespace FireNeurons.NET.Optimisation;
using Indexes;
using Objects;

public abstract class IOptimiser
{
    public Func<Neuron, object?, object, double> LossDerivative { get; init; }
    public double LearningRate { get; init; }

    public abstract IOptimiserData DataInstance { get; }
    public Dictionary<IIndex, IOptimiserData> OptimiserDatas { get; }


    public IOptimiser(Func<Neuron, object?, object, double> lossDerivative, double learningRate)
    {
        this.LearningRate = learningRate;
        this.LossDerivative = lossDerivative;

        this.OptimiserDatas = new();
    }

    public void CreateData(NeuralNetwork network)
    {
        this.OptimiserDatas.Clear();

        foreach (var (layerIndex, layer) in network.Layers)
        {
            this.OptimiserDatas.Add(layerIndex, this.DataInstance);

            foreach (var neuron in layer.Neurons)
            {
                this.OptimiserDatas.Add(neuron.Index, this.DataInstance);

                foreach (var connection in neuron.Connections)
                {
                    this.OptimiserDatas.Add(connection.Index, this.DataInstance);
                }
            }
        }
    }

    public abstract void CalculateDelta(IOptimiserData optimiserData);

    public void CalculateGradient(Neuron neuron, double loss)
    {
        if (neuron.OutgoingConnections.Count != 0)
        {
            throw new Exception("ERROR: Can't set a loss-gradient for a none-output neuron!");
        }

        this.SetAllGradients(neuron, loss);
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
            sum += connection.Weight * this.OptimiserDatas[connection.OutputNeuron.Index].Gradient;
        }

        this.SetAllGradients(neuron, sum);
    }

    private const double L1_RATIO = 0.50;
    private void SetAllGradients(Neuron neuron, double loss)
    {
        var derivation = neuron.Blank.Derivate(neuron.Options.Activation);
        double gradient = derivation * loss;

        this.OptimiserDatas[neuron.Index].Gradient = gradient;

        foreach (var connection in neuron.Connections)
        {
            double weightDecay_L1 = connection.Weight / connection.Weight.Abs();
            double weightDecay_L2 = 2 * connection.Weight;
            double weightDecay = (L1_RATIO * weightDecay_L1) + ((1 - L1_RATIO) * weightDecay_L2);

            double adaptedGradient = gradient - (neuron.Options.WeightDecay * weightDecay);
            this.OptimiserDatas[connection.Index].Gradient = adaptedGradient * connection.InputNeuron.GetValue(true);
        }
    }
}

public record IOptimiserData
{
    public double Delta { get; set; }
    public double Gradient { get; set; }
}
