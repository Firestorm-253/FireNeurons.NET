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
                for (int i = 0; i < miniBatch.Length; i++)
                {
                    network.Train(optimiser, miniBatch[i], miniBatchSize, (i == miniBatch.Length - 1));
                }
            }
        }
    }

    public static void Train(
        this NeuralNetwork network,
        IOptimiser optimiser,
        TrainingData trainingData,
        int miniBatchSize,
        bool apply)
    {
        var targetIndexes = trainingData.LossDerivativeArgs.DataLayers.Keys.ToArray();

        network.Evaluate(trainingData.InputData, true, targetIndexes);

        network.TrainTargetLayers(optimiser, trainingData.LossDerivativeArgs, trainingData.IgnoreNeurons, miniBatchSize, apply);

        network.TrainHiddenLayers(optimiser, targetIndexes, miniBatchSize, apply);
    }

    private static void TrainTargetLayers(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Data<(object?, Dictionary<NeuronIndex, object>)> lossDerivativeArgs,
        NeuronIndex[] ignoreNeurons,
        int miniBatchSize,
        bool apply)
    {
        foreach (var (layerIndex, data) in lossDerivativeArgs.DataLayers)
        {
            network.TrainTargetLayer(optimiser, network.Layers[layerIndex], data, ignoreNeurons, miniBatchSize, apply);
        }
    }

    private static void TrainTargetLayer(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Layer targetLayer,
        (object?, Dictionary<NeuronIndex, object>) lossDerivativeArgs,
        NeuronIndex[] ignoreNeurons,
        int miniBatchSize,
        bool apply)
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
                optimiser.OptimiserDatas[targetNeuron.Index].SummedGradient = 0;
                optimiser.OptimiserDatas[targetNeuron.Index].Delta = 0;
                continue;
            }

            optimiser.AppendGradient(targetNeuron, optimiser.LossDerivative(targetNeuron, lossDerivativeArgs.Item1, lossDerivativeArgs.Item2[targetNeuron.Index]));

            if (apply)
            {
                optimiser.ApplyGradient(optimiser.OptimiserDatas[targetNeuron.Index], miniBatchSize);
                targetNeuron.Bias += optimiser.OptimiserDatas[targetNeuron.Index].Delta;
            }
        }
    }

    private static void TrainHiddenLayers(
        this NeuralNetwork network,
        IOptimiser optimiser,
        LayerIndex[] targetIndexes,
        int miniBatchSize,
        bool apply)
    {
        foreach (var (layerIndex, layer) in network.Layers.OrderByDescending(x => x.Key.Index))
        {
            if (targetIndexes.Contains(layerIndex))
            {
                continue;
            }

            network.TrainHiddenLayer(optimiser, layer, miniBatchSize, apply);
        }
    }

    private static void TrainHiddenLayer(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Layer layer,
        int miniBatchSize,
        bool apply)
    {
        foreach (var neuron in layer.Neurons)
        {
            if (neuron.DroppedOut)
            {
                optimiser.OptimiserDatas[neuron.Index].SummedGradient = 0;
                optimiser.OptimiserDatas[neuron.Index].Delta = 0;
                continue;
            }

            network.TrainHiddenNeuron(optimiser, neuron, miniBatchSize, apply);
        }
    }

    private static void TrainHiddenNeuron(
        this NeuralNetwork network,
        IOptimiser optimiser,
        Neuron neuron,
        int miniBatchSize,
        bool apply)
    {
        optimiser.AppendGradient(neuron);

        if (apply)
        {
            optimiser.ApplyGradient(optimiser.OptimiserDatas[neuron.Index], miniBatchSize);
            neuron.Bias += optimiser.OptimiserDatas[neuron.Index].Delta;
        }

        foreach (var outgoingConnection in neuron.OutgoingConnections)
        {
            if (apply)
            {
                optimiser.ApplyGradient(optimiser.OptimiserDatas[outgoingConnection.Index], miniBatchSize);
                outgoingConnection.Weight += optimiser.OptimiserDatas[outgoingConnection.Index].Delta;
            }
        }
    }
}
