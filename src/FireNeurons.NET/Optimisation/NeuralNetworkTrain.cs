using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Optimisation;

public static class NeuralNetworkTrain
{
    public static void Train(this NeuralNetwork network, List<TrainingData> trainingDataSet, int iterations = 1)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            trainingDataSet = trainingDataSet.OrderBy(x => GlobalRandom.Next()).ToList(); // shuffle dataset

            foreach (var trainingData in trainingDataSet)
            {
                network.Train(trainingData);
            }
        }
    }

    public static void Train(this NeuralNetwork network, TrainingData trainingData)
    {
        var targetIndexes = trainingData.LossDerivativeArgs.DataLayers.Keys.ToArray();

        network.Evaluate(trainingData.InputData, true, targetIndexes);

        network.TrainTargetLayers(trainingData.LossDerivativeArgs, trainingData.IgnoreNeurons);

        network.TrainHiddenLayers(targetIndexes);
    }

    private static void TrainTargetLayers(this NeuralNetwork network, Data lossDerivativeArgs, NeuronIndex[] ignoreNeurons)
    {
        foreach (var lossArgsLayer in lossDerivativeArgs.DataLayers)
        {
            network.TrainTargetLayer(network.Get(lossArgsLayer.Key), lossArgsLayer.Value, ignoreNeurons);
        }
    }

    private static void TrainTargetLayer(this NeuralNetwork network, Layer targetLayer, double[] lossArgs, NeuronIndex[] ignoreNeurons)
    {
        for (int n = 0; n < targetLayer.Neurons.Count; n++)
        {
            var targetNeuron = targetLayer.Neurons[n];

            if (ignoreNeurons.Contains(targetNeuron.NeuronIndex))
            {
                continue;
            }

            if (targetNeuron.DroppedOut)
            {
                targetNeuron.OptimiserData.Gradient = 0;
                targetNeuron.OptimiserData.Delta = 0;
                continue;
            }

            network.Optimiser.CalculateGradient(targetNeuron, network.Optimiser.LossDerivative(targetNeuron, lossArgs[n]));
            network.Optimiser.CalculateDelta(targetNeuron.OptimiserData);
        }
    }

    private static void TrainHiddenLayers(this NeuralNetwork network, LayerIndex[] targetIndexes)
    {
        foreach (var layer in network.Layers.OrderByDescending(x => x.LayerIndex.Index))
        {
            if (targetIndexes.Contains(layer.LayerIndex))
            {
                continue;
            }

            network.TrainHiddenLayer(layer);
        }
    }

    private static void TrainHiddenLayer(this NeuralNetwork network, Layer layer)
    {
        foreach (var neuron in layer.Neurons)
        {
            if (neuron.DroppedOut)
            {
                neuron.OptimiserData.Gradient = 0;
                neuron.OptimiserData.Delta = 0;
                continue;
            }

            network.TrainHiddenNeuron(neuron);
        }
    }

    private static void TrainHiddenNeuron(this NeuralNetwork network, Neuron neuron)
    {
        network.Optimiser.CalculateGradient(neuron);

        network.Optimiser.CalculateDelta(neuron.OptimiserData);
        neuron.Bias += neuron.OptimiserData.Delta;

        foreach (var outgoingConnection in neuron.OutgoingConnections)
        {
            network.Optimiser.CalculateDelta(outgoingConnection.OptimiserData);
            outgoingConnection.Weight += outgoingConnection.OptimiserData.Delta;
        }
    }
}
