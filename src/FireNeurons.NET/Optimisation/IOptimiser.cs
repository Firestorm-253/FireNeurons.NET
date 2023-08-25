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

    public virtual void ApplyGradient(IOptimiserData optimiserData, double entValue, Options options, int miniBatchSize)
    {
        optimiserData.FinalGradient = optimiserData.SummedGradient / miniBatchSize;

        if (entValue != 0)
        {
            double weightDecay_L1 = entValue / entValue.Abs();
            double weightDecay_L2 = entValue;

            optimiserData.FinalGradient -= (options.L1_Reg * weightDecay_L1);
            optimiserData.FinalGradient -= (options.L2_Reg * weightDecay_L2);
        }

        //optimiserData.PartialGradient = 0;
        optimiserData.SummedGradient = 0;
    }

    public void AppendGradient(Neuron neuron)
    {
        double sum = 0;
        foreach (var connection in neuron.OutgoingConnections)
        {
            sum += connection.Weight * this.OptimiserDatas[connection.OutputNeuron.Index].PartialGradient;
        }

        this.AppendGradient(neuron, sum);
    }

    public void AppendGradient(Neuron neuron, double loss)
    {
        var derivation = neuron.Blank.Derivate(neuron.Options.Activation);
        double gradient = derivation * loss;

        this.OptimiserDatas[neuron.Index].PartialGradient = gradient;
        this.OptimiserDatas[neuron.Index].SummedGradient += this.OptimiserDatas[neuron.Index].PartialGradient;

        foreach (var connection in neuron.Connections)
        {
            double gradient_conn = gradient * connection.InputNeuron.GetValue(true);

            this.OptimiserDatas[connection.Index].PartialGradient = gradient_conn;
            this.OptimiserDatas[connection.Index].SummedGradient += gradient_conn;
        }
    }
}

public record IOptimiserData
{
    public double Delta { get; set; }
    public double PartialGradient { get; set; }
    public double SummedGradient { get; set; }
    public double FinalGradient { get; set; }
}
