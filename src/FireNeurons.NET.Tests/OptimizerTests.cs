using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;
using FireNeurons.NET.Optimizers;

namespace FireNeurons.NET.Tests;

[TestClass]
public class OptimizerTests
{
    const int randomSeed = 605013250;

    [TestMethod]
    public void SGDTest()
    {
        //# Seed Randomizer
        GlobalRandom = new Random(randomSeed);

        //# Initialize
        const double learningRate = 0.001;
        var optimizer = new Adam(learningRate);
        var model = new NeuralNetwork(optimizer);

        //# InputLayers
        model.Add(1, 0, Activation.Sigmoid);

        //# HiddenLayers
        model.Add(100, 1, Activation.LeakyRelu, 0);
        model.Add(100, 2, Activation.LeakyRelu, 1);
        model.Add(100, 3, Activation.LeakyRelu, 2);

        //# OutputLayers
        model.Add(1, 4, Activation.Sigmoid, 3);

        //# Compile
        model.Randomize();


        //# Test
        double target = 0;
        var outputNeuron = model.Layers.Last().Neurons.First();

        var data = new Data();
        data.Add(0, new double[] { 2.6 });

        var resultsBefore = model.Evaluate(data, outputNeuron.NeuronIndex.LayerIndex);

        var targets = new Data();
        targets.Add(4, new double[] { target });

        model.Train(new List<(Data, Data)>() { (data, targets) }, 10);

        //for (int iterations = 0; iterations < 10; iterations++)
        //{
        //    model.Evaluate(data, outputNeuron.NeuronIndex.LayerIndex);

        //    optimizer.CalculateGradient(outputNeuron, (target - outputNeuron.Value));
        //    optimizer.CalculateDelta(outputNeuron.OptimizerData);

        //    for (int l = model.Layers.Count - 2; l >= 0; l--)
        //    {
        //        foreach (var neuron in model.Layers.ElementAt(l).Neurons)
        //        {
        //            optimizer.CalculateGradient(neuron);
        //            Assert.AreNotEqual(0, neuron.OptimizerData.Gradient);

        //            optimizer.CalculateDelta(neuron.OptimizerData);
        //            neuron.Bias += neuron.OptimizerData.Delta;
        //            Assert.AreNotEqual(0, neuron.OptimizerData.Delta);

        //            foreach (var outgoingConnection in neuron.OutgoingConnections)
        //            {
        //                optimizer.CalculateDelta(outgoingConnection.OptimizerData);
        //                outgoingConnection.Weight += outgoingConnection.OptimizerData.Delta;
        //                Assert.AreNotEqual(0, outgoingConnection.OptimizerData.Delta);
        //            }
        //        }
        //    }

        //    //var results = model.Evaluate(data, outputNeuron.NeuronIndex.LayerIndex);
        //    //var diff = results[0].Item2[0] - resultsBefore[0].Item2[0];
        //}

        var resultsEnd = model.Evaluate(data, outputNeuron.NeuronIndex.LayerIndex);
        var diffEnd = resultsEnd[0][0] - resultsBefore[0][0];
    }
}
