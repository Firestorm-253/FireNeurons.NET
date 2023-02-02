using NeuralNetwork.NET.Indexes;
using NeuralNetwork.NET.Objects;
using System.Diagnostics;

namespace NeuralNetwork.NET.Tests;

[TestClass]
public class Tests
{
    const int randomSeed = 605013250;

    [TestMethod]
    public void Test1()
    {
        //# Seed Randomizer
        GlobalRandom = new Random(randomSeed);

        //# Initialize
        var optimizer = new Optimizers.SGD(0.02);
        var model = new NeuralNetwork(optimizer);

        //# InputLayers
        model.Add(1, 0, Activation.Identity);
        model.Add(2, 1, Activation.Sigmoid);

        //# HiddenLayers
        model.Add(1, 2, Activation.LeakyRelu, 0, 1);

        //# OutputLayers
        model.Add(1, 3, Activation.LeakyRelu, 2);
        model.Add(2, 4, Activation.LeakyRelu, 2);

        //# Compile
        model.Randomize();

        //# Test
        var data = new Data(new(LayerIndex, double[])[]
        {
            (0, new double[] { 2.6 }),
            (1, new double[] { 21, -30 }),
        });

        var results = model.Evaluate(data, 3, 4);
    }

    [TestMethod]
    public void SaveLoad()
    {
        //# Seed Randomizer
        GlobalRandom = new Random(randomSeed);

        //# Initialize
        var optimizer = new Optimizers.SGD(0.02);
        var model = new NeuralNetwork(optimizer);

        //# InputLayers
        model.Add(6, 0, Activation.Sigmoid);
        model.Add(6, 1, Activation.Sigmoid);

        //# HiddenLayers
        model.Add(20, 2, Activation.LeakyRelu, 0, 1);
        model.Add(20, 3, Activation.LeakyRelu, 2);

        //# OutputLayers
        model.Add(10, 4, Activation.Sigmoid, 3);

        //# Compile
        model.Randomize();

        //# Test
        const string name = "test-network";
        model.Save(name, SaveType.Binary);
        model.Save(name, SaveType.Json);

        var binaryLoaded = new NeuralNetwork($"{name}.nn", optimizer);
        var jsonLoaded = new NeuralNetwork($"{name}.json", optimizer);

        Assert.AreEqual(binaryLoaded, jsonLoaded);
        Assert.AreEqual(model, binaryLoaded);
    }

    [TestMethod]
    public void SwitchSpeedTest()
    {
        const int iterations = 100_000_000;

        Stopwatch sw = Stopwatch.StartNew();
        Activation activation = Activation.TanH;
        for (int i = 0; i < iterations; i++)
        {
            (2.0).Activate(activation);
        }
        sw.Stop();
        TimeSpan result_1 = sw.Elapsed;

        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            Activations.TanH(2.0);
        }
        sw.Stop();
        TimeSpan result_2 = sw.Elapsed;

        var percentage = result_1 / result_2;

        for (int i = 200; i >= 100; i--)
        {
            Assert.IsTrue(percentage <= (i / 100.0), $"{i}%");
        }
    }
}