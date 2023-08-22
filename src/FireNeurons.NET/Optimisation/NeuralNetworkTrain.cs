namespace FireNeurons.NET.Optimisation;
using Indexes;
using Objects;

public static class NeuralNetworkTrain
{
    public static void Train(
        this NeuralNetwork network,
        IOptimiser optimiser,
        List<TrainingData> trainingDataSet,
        int miniBatchSize = 1,
        int epochs = 1)
    {
        if (!optimiser.OptimiserDatas.Any())
        {
            optimiser.CreateData(network);
        }

        for (int epoch = 0; epoch < epochs; epoch++)
        {
            var miniBatches = trainingDataSet.OrderBy(x => GlobalRandom.Next()).Chunk(miniBatchSize);

            foreach (var miniBatch in miniBatches)
            {
                if (miniBatch.Length > 1)
                {
                    throw new NotImplementedException();
                }

                network.Train(optimiser, miniBatch[0]);
            }
        }
    }

    public static void Train(
        this NeuralNetwork network,
        IOptimiser optimiser,
        TrainingData trainingData)
    {
        var targetIndexes = trainingData.LossDerivativeArgs.DataLayers.Keys.ToArray();

        network.Evaluate(trainingData.InputData, true, targetIndexes);

        network.TrainTargetLayers(optimiser, trainingData.LossDerivativeArgs, trainingData.IgnoreNeurons);

        network.TrainHiddenLayers(optimiser, targetIndexes);
    }

    private static void TrainTargetLayers(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Data<(object?, Dictionary<NeuronIndex, object>)> lossDerivativeArgs,
        NeuronIndex[] ignoreNeurons)
    {
        foreach (var (layerIndex, data) in lossDerivativeArgs.DataLayers)
        {
            network.TrainTargetLayer(optimiser, network.Layers[layerIndex], data, ignoreNeurons);
        }
    }

    private static void TrainTargetLayer(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Layer targetLayer,
        (object?, Dictionary<NeuronIndex, object>) lossDerivativeArgs,
        NeuronIndex[] ignoreNeurons)
    {
        for (int n = 0; n < targetLayer.Neurons.Count; n++)
        {
            var targetNeuron = targetLayer.Neurons[n];

            if (ignoreNeurons.Contains(targetNeuron.Index))
            {
                continue;
            }

            if (targetNeuron.DroppedOut)
            {
                optimiser.OptimiserDatas[targetNeuron.Index].Gradient = 0;
                optimiser.OptimiserDatas[targetNeuron.Index].Delta = 0;
                continue;
            }

            optimiser.CalculateGradient(targetNeuron, optimiser.LossDerivative(targetNeuron, lossDerivativeArgs.Item1, lossDerivativeArgs.Item2[targetNeuron.Index]));
            optimiser.CalculateDelta(optimiser.OptimiserDatas[targetNeuron.Index]);
        }
    }

    private static void TrainHiddenLayers(
        this NeuralNetwork network,
        IOptimiser optimiser,
        LayerIndex[] targetIndexes)
    {
        foreach (var (layerIndex, layer) in network.Layers.OrderByDescending(x => x.Key.Index))
        {
            if (targetIndexes.Contains(layerIndex))
            {
                continue;
            }

            network.TrainHiddenLayer(optimiser, layer);
        }
    }

    private static void TrainHiddenLayer(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Layer layer)
    {
        foreach (var neuron in layer.Neurons)
        {
            if (neuron.DroppedOut)
            {
                optimiser.OptimiserDatas[neuron.Index].Gradient = 0;
                optimiser.OptimiserDatas[neuron.Index].Delta = 0;
                continue;
            }

            network.TrainHiddenNeuron(optimiser, neuron);
        }
    }

    private static void TrainHiddenNeuron(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Neuron neuron)
    {
        optimiser.CalculateGradient(neuron);

        optimiser.CalculateDelta(optimiser.OptimiserDatas[neuron.Index]);
        neuron.Bias += optimiser.OptimiserDatas[neuron.Index].Delta;

        foreach (var outgoingConnection in neuron.OutgoingConnections)
        {
            optimiser.CalculateDelta(optimiser.OptimiserDatas[outgoingConnection.Index]);
            outgoingConnection.Weight += optimiser.OptimiserDatas[outgoingConnection.Index].Delta;
        }
    }
}
