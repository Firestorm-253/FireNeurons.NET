using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET;

public partial class NeuralNetwork
{
    public void Train(List<(Data, Data)> dataLossArgsSet, int iterations)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            dataLossArgsSet = dataLossArgsSet.OrderBy(x => GlobalRandom.Next()).ToList(); // shuffle dataset

            this.Train(dataLossArgsSet);
        }
    }

    public void Train(List<(Data, Data)> dataLossArgsSet)
    {
        foreach (var dataTarget in dataLossArgsSet)
        {
            this.Train(dataTarget.Item1, dataTarget.Item2);
        }
    }

    public void Train(Data data, Data lossArgs)
    {
        var targetIndexes = lossArgs.DataLayers.Keys.ToArray();

        this.Evaluate(data, targetIndexes);

        this.TrainTargetLayers(lossArgs);
        
        this.TrainHiddenLayers(targetIndexes);
    }

    private void TrainTargetLayers(Data lossArgs)
    {
        foreach (var lossArgsLayer in lossArgs.DataLayers)
        {
            var targetLayer = this.Get(lossArgsLayer.Key);

            this.TrainTargetLayer(targetLayer, lossArgsLayer.Value);
        }
    }

    private void TrainTargetLayer(Layer targetLayer, double[] lossArgs)
    {
        for (int n = 0; n < targetLayer.Neurons.Count; n++)
        {
            var targetNeuron = targetLayer.Neurons[n];

            this.Optimizer.CalculateGradient(targetNeuron, this.Optimizer.LossDerivative(targetNeuron, lossArgs[n]));
            this.Optimizer.CalculateDelta(targetNeuron.OptimizerData);
        }
    }

    private void TrainHiddenLayers(LayerIndex[] targetIndexes)
    {
        foreach (var layer in this.Layers.OrderByDescending(x => x.LayerIndex.Index))
        {
            if (targetIndexes.Contains(layer.LayerIndex))
            {
                continue;
            }

            this.TrainHiddenLayer(layer);
        }
    }

    private void TrainHiddenLayer(Layer layer)
    {
        foreach (var neuron in layer.Neurons)
        {
            this.TrainHiddenNeuron(neuron);
        }
    }

    private void TrainHiddenNeuron(Neuron neuron)
    {
        this.Optimizer.CalculateGradient(neuron);

        this.Optimizer.CalculateDelta(neuron.OptimizerData);
        neuron.Bias += neuron.OptimizerData.Delta;

        foreach (var outgoingConnection in neuron.OutgoingConnections)
        {
            this.Optimizer.CalculateDelta(outgoingConnection.OptimizerData);
            outgoingConnection.Weight += outgoingConnection.OptimizerData.Delta;
        }
    }
}
