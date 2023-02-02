using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET;

public partial class NeuralNetwork
{
    public void Train(List<(Data, Data)> dataTargetSet, int iterations)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            dataTargetSet = dataTargetSet.OrderBy(x => GlobalRandom.Next()).ToList(); // shuffle dataset

            this.Train(dataTargetSet);
        }
    }

    private void Train(List<(Data, Data)> dataTargetSet)
    {
        foreach (var dataTarget in dataTargetSet)
        {
            this.Train(dataTarget.Item1, dataTarget.Item2);
        }
    }

    private void Train(Data data, Data target)
    {
        var targetIndexes = target.DataLayers.Keys.ToArray();

        this.Evaluate(data, targetIndexes);

        this.TrainTargetLayers(target);
        
        this.TrainHiddenLayers(targetIndexes);
    }

    private void TrainTargetLayers(Data target)
    {
        foreach (var dataLayer in target.DataLayers)
        {
            var targetLayer = this.Get(dataLayer.Key);

            this.TrainTargetLayer(targetLayer, dataLayer.Value);
        }
    }

    private void TrainTargetLayer(Layer targetLayer, double[] target)
    {
        for (int n = 0; n < targetLayer.Neurons.Count; n++)
        {
            var targetNeuron = targetLayer.Neurons[n];

            this.Optimizer.CalculateGradient(targetNeuron, (target[n] - targetNeuron.Value));
            this.Optimizer.CalculateDelta(targetNeuron.OptimizerData);
        }
    }

    private void TrainHiddenLayers(LayerIndex[] targetIndexes)
    {
        foreach (var layer in this.Layers.OrderByDescending(x => x.LayerIndex))
        {
            if (targetIndexes.Contains(layer.LayerIndex))
            {
                return;
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
