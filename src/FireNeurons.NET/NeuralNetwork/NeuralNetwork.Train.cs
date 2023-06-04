namespace FireNeurons.NET;

using Indexes;
using Objects;
using Optimizers;

public partial class NeuralNetwork
{
    public void Train(List<TrainingData> trainingDataSet, int iterations = 1, bool apply = true)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            trainingDataSet = trainingDataSet.OrderBy(x => GlobalRandom.Next()).ToList(); // shuffle dataset

            foreach (var trainingData in trainingDataSet)
            {
                this.Train(trainingData, apply);
            }
        }
    }

    public void Train(TrainingData trainingData, bool apply = true)
    {
        var targetIndexes = trainingData.LossDerivativeArgs.DataLayers.Keys.ToArray();

        this.Evaluate(trainingData.InputData, targetIndexes);

        this.TrainTargetLayers(trainingData.LossDerivativeArgs);
        
        this.TrainHiddenLayers(targetIndexes);
    }

    private void TrainTargetLayers(Data lossDerivativeArgs)
    {
        foreach (var lossArgsLayer in lossDerivativeArgs.DataLayers)
        {
            this.TrainTargetLayer(this.Get(lossArgsLayer.Key), lossArgsLayer.Value);
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
