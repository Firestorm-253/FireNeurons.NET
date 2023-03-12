using FireNeurons.NET.Objects;
using FireNeurons.NET.Optimizers;
using FireMath.NET;

namespace FireNeurons.NET.Tests;

[TestClass]
public class OptimizerTests
{
    const int randomSeed = 605013250;

    [TestMethod]
    public void AdamVsSGD_Test()
    {
        const double learningRate = 0.0001;

        var sgd_percentage = XOR_LossDecreasePercentage(new SGD(learningRate));
        var adam_percentage = XOR_LossDecreasePercentage(new Adam(learningRate));

        Assert.IsTrue(adam_percentage > sgd_percentage);
    }

    private static double XOR_LossDecreasePercentage(IOptimizer optimizer)
    {
        //# Seed Randomizer
        GlobalRandom = new Random(randomSeed);

        //# Initialize
        var model = new NeuralNetwork(optimizer);

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

        var dataTargetSet = new List<(Data, Data)>()
        {
            (new Data().Add(0, new double[] { 0, 0 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 0 })),
            (new Data().Add(0, new double[] { 1, 1 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 0 })),
            (new Data().Add(0, new double[] { 0, 1 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 1 })),
            (new Data().Add(0, new double[] { 1, 0 }), new Data().Add(outputNeuron.NeuronIndex.LayerIndex, new double[] { 1 })),
        };

        var resultsBefore = new Data[]
        {
            model.Evaluate(dataTargetSet[0].Item1, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(dataTargetSet[1].Item1, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(dataTargetSet[2].Item1, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(dataTargetSet[3].Item1, outputNeuron.NeuronIndex.LayerIndex),
        };

        model.Train(dataTargetSet, iterations: 200);

        var resultsAfter = new Data[]
        {
            model.Evaluate(dataTargetSet[0].Item1, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(dataTargetSet[1].Item1, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(dataTargetSet[2].Item1, outputNeuron.NeuronIndex.LayerIndex),
            model.Evaluate(dataTargetSet[3].Item1, outputNeuron.NeuronIndex.LayerIndex),
        };

        double mseBefore = MSE(resultsBefore, dataTargetSet.Select(x => x.Item2).ToArray());
        double mseAfter = MSE(resultsAfter, dataTargetSet.Select(x => x.Item2).ToArray());
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
