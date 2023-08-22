﻿using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Optimisation;

public static class NeuralNetworkTrain
{
    public static void Train(this NeuralNetwork network, List<TrainingData> trainingDataSet, int miniBatchSize = 1, int epochs = 1)
    {
        for (int epoch = 0; epoch < epochs; epoch++)
        {
            var miniBatches = trainingDataSet.OrderBy(x => GlobalRandom.Next()).Chunk(miniBatchSize);

            foreach (var miniBatch in miniBatches)
            {
                if (miniBatch.Length > 1)
                {
                    throw new NotImplementedException();
                }

                network.Train(miniBatch[0]);
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

    private static void TrainTargetLayers(this NeuralNetwork network, Data<(object?, Dictionary<NeuronIndex, object>)> lossDerivativeArgs, NeuronIndex[] ignoreNeurons)
    {
        foreach (var (layerIndex, data) in lossDerivativeArgs.DataLayers)
        {
            network.TrainTargetLayer(network.Layers[layerIndex], data, ignoreNeurons);
        }
    }

    private static void TrainTargetLayer(this NeuralNetwork network, Layer targetLayer, (object?, Dictionary<NeuronIndex, object>) lossDerivativeArgs, NeuronIndex[] ignoreNeurons)
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

            network.Optimiser.CalculateGradient(targetNeuron, network.Optimiser.LossDerivative(targetNeuron, lossDerivativeArgs.Item1, lossDerivativeArgs.Item2[targetNeuron.NeuronIndex]));
            network.Optimiser.CalculateDelta(targetNeuron.OptimiserData);
        }
    }

    private static void TrainHiddenLayers(this NeuralNetwork network, LayerIndex[] targetIndexes)
    {
        foreach (var (layerIndex, layer) in network.Layers.OrderByDescending(x => x.Key.Index))
        {
            if (targetIndexes.Contains(layerIndex))
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
