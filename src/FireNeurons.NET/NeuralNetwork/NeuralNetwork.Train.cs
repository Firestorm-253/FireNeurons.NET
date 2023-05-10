using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET;

public partial class NeuralNetwork
{
    public void Train(List<(Data, Data)> dataLossArgsSet, int iterations, bool apply = true)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            dataLossArgsSet = dataLossArgsSet.OrderBy(x => GlobalRandom.Next()).ToList(); // shuffle dataset

            this.Train(dataLossArgsSet, apply);
        }
    }

    public void Train(List<(Data, Data)> dataLossArgsSet, bool apply = true)
    {
        foreach (var dataTarget in dataLossArgsSet)
        {
            this.Train(dataTarget.Item1, dataTarget.Item2, apply);
        }
    }

    public void Train(Data data, Data lossArgs, bool apply = true)
    {
        var targetIndexes = lossArgs.DataLayers.Keys.ToArray();

        this.Evaluate(data, targetIndexes);

        this.TrainTargetLayers(lossArgs);
        
        this.TrainHiddenLayers(targetIndexes, apply);
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

    private void TrainHiddenLayers(LayerIndex[] targetIndexes, bool apply)
    {
        foreach (var layer in this.Layers.OrderByDescending(x => x.LayerIndex.Index))
        {
            if (targetIndexes.Contains(layer.LayerIndex))
            {
                continue;
            }

            this.TrainHiddenLayer(layer, apply);
        }
    }

    private void TrainHiddenLayer(Layer layer, bool apply)
    {
        foreach (var neuron in layer.Neurons)
        {
            this.TrainHiddenNeuron(neuron, apply);
        }
    }

    private void TrainHiddenNeuron(Neuron neuron, bool apply)
    {
        this.Optimizer.CalculateGradient(neuron);

        this.Optimizer.CalculateDelta(neuron.OptimizerData);
        if (apply)
        {
            neuron.Bias += neuron.OptimizerData.Delta;
        }

        foreach (var outgoingConnection in neuron.OutgoingConnections)
        {
            this.Optimizer.CalculateDelta(outgoingConnection.OptimizerData);
            if (apply)
            {
                outgoingConnection.Weight += outgoingConnection.OptimizerData.Delta;
            }
        }
    }
}
