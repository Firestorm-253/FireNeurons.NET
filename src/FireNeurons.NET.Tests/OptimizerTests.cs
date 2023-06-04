using FireMath.NET;

namespace FireNeurons.NET.Tests;

using Objects;
using Optimisation;
using Optimisation.Optimisers;

[TestClass]
public class OptimiserTests
{
    const int randomSeed = 605013250;

    [TestMethod]
    public void AdamVsSGD_Test()
    {
        const double learningRate = 0.0001;

        var sgd_percentage = XOR_LossDecreasePercentage(new SGD(new Func<Neuron, double, double>((neuron, arg) =>
        {
            return (arg - neuron.Value); // MSE-Derivative
        }), learningRate));
        var adam_percentage = XOR_LossDecreasePercentage(new Adam(new Func<Neuron, double, double>((neuron, arg) =>
        {
            return (arg - neuron.Value); // MSE-Derivative
        })));

        Assert.IsTrue(adam_percentage > sgd_percentage);
    }

    private static double XOR_LossDecreasePercentage(IOptimiser optimiser)
    {
        //# Seed Randomizer
        GlobalRandom = new Random(randomSeed);

        //# Initialize
        var model = new NeuralNetwork(optimiser);

        //# InputLayers
        model.Add(2, 0, Activation.Sigmoid);

        //# HiddenLayers
        model.Add(100, 1, Activation.LeakyRelu, 0);
        model.Add(100, 2, Activation.LeakyRelu, 1);
        model.Add(100, 3, Activation.LeakyRelu, 2);

        //# OutputLayers
        model.Add(1, 4, Activation.Sigmoid, 3);

        //# Compile
        model.Randomize();


        //# Test
        var outputNeuron = model.Layers.Last().Neurons.First();

        var trainingDataSet = new List<TrainingData>()
        {
            new(new Data().Add(0, new double[] { 0, 0 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 0 })),
            new(new Data().Add(0, new double[] { 1, 1 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 0 })),
            new(new Data().Add(0, new double[] { 0, 1 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 1 })),
            new(new Data().Add(0, new double[] { 1, 0 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 1 })),
        };

        var resultsBefore = new Data[]
        {
            model.Evaluate(trainingDataSet[0].InputData, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(trainingDataSet[1].InputData, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(trainingDataSet[2].InputData, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(trainingDataSet[3].InputData, outputNeuron.NeuronIndex.LayerIndex),
        };

        model.Train(trainingDataSet, iterations: 200);

        var resultsAfter = new Data[]
        {
            model.Evaluate(trainingDataSet[0].InputData, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(trainingDataSet[1].InputData, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(trainingDataSet[2].InputData, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(trainingDataSet[3].InputData, outputNeuron.NeuronIndex.LayerIndex),
        };

        double mseBefore = MSE(resultsBefore, trainingDataSet.Select(x => x.LossDerivativeArgs).ToArray());
        double mseAfter = MSE(resultsAfter, trainingDataSet.Select(x => x.LossDerivativeArgs).ToArray());
        return 1 - (mseAfter / mseBefore);
    }

    private static double MSE(Data[] results, Data[] targets)
    {
        double loss = 0;
        for (int d = 0; d < results.Length; d++)
        {
            for (int l = 0; l < results[d].DataLayers.Count; l++)
            {
                var values = results[d].DataLayers.ElementAt(l).Value;
                var targetValues = targets[d].DataLayers.ElementAt(l).Value;

                for (int n = 0; n < values.Length; n++)
                {
                    loss += (targetValues[n] - values[n]).Pow(2);
                }
            }
        }
        return loss;
    }
}
