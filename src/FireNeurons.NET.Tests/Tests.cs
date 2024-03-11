using System.Diagnostics;

namespace FireNeurons.NET.Tests;

using FireNeurons.NET.CNN;
using Objects;
using System.Text;

[TestClass]
public class Tests
{
    const int randomSeed = 605013250;

    [TestMethod]
    public void SerializationComparison()
    {
        //# Seed Randomizer
        GlobalRandom = new Random(randomSeed);

        //# Initialize
        var model = new NeuralNetwork();

        //# InputLayers
        model.Add(400, 0, Activation.Identity);
        model.Add(40, 1, Activation.Sigmoid);

        //# HiddenLayers
        model.Add(40, 2, Activation.LeakyRelu, 0, 1);

        //# OutputLayers
        model.Add(40, 3, Activation.LeakyRelu, 2);
        model.Add(40, 4, Activation.LeakyRelu, 2);

        //# Compile
        model.Randomize();


        var sw = Stopwatch.StartNew();
        for (int i = 0; i < 10; i++)
        {
            var bin = new NeuralNetwork(NeuralNetwork.FromBytes(NeuralNetwork.ToBytes(new(model))));
        }
        sw.Stop();
        var bin_time = sw.ElapsedMilliseconds / 10;

        sw.Restart();
        for (int i = 0; i < 10; i++)
        {
            var json = new NeuralNetwork(NeuralNetwork.FromJson(NeuralNetwork.ToJson(new(model))));
        }
        sw.Stop();
        var json_time = sw.ElapsedMilliseconds / 10;



        var bin_size = NeuralNetwork.ToBytes(new(model)).Length;
        var json_size = Encoding.Default.GetBytes(NeuralNetwork.ToJson(new(model))).Length;
    }

    [TestMethod]
    public void SaveLoad()
    {
        var lossDerivative = new Func<Neuron, object?, object, double>((neuron, obj_global, obj_local) =>
        {
            return ((double)obj_local - neuron.GetValue(true)); // MSE-Derivative
        });
        var model = new NeuralNetwork();
        model.Add(1, 0, Activation.Identity);
        model.Save("models\\m1.abgsdfgr", SaveType.Binary);

        var loaded = new NeuralNetwork("models\\m1.nn");
    }

    [TestMethod]
    public void Clone()
    {
        var lossDerivative = new Func<Neuron, object?, object, double>((neuron, obj_global, obj_local) =>
        {
            return ((double)obj_local - neuron.GetValue(true)); // MSE-Derivative
        });
        var model = new NeuralNetwork();
        model.Add(1, 0, Activation.Identity);
        
        var clone = model.Clone();
    }

    [TestMethod]
    public void Test1()
    {
        //# Seed Randomizer
        GlobalRandom = new Random(randomSeed);

        //# Initialize
        var model = new NeuralNetwork();

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
        var data = new Data();
        data.Add(0, new double[] { 2.6 });
        data.Add(1, new double[] { 21, -30 });

        var results = model.Evaluate(data, false, 3, 4);
    }

    [TestMethod]
    public void CNN()
    {
        var data = new double[3][][];
        for (int z = 0; z < data.Length; z++)
        {
            data[z] = new double[28][];
            for (int y = 0; y < data[z].Length; y++)
            {
                data[z][y] = new double[28];
            }
        }
        var input = new Data3D(data);

        var cnn_layer_0 = new ConvolutionLayer(5, 3, input.Depth, 1);
        var conv_0 = cnn_layer_0.Execute(input);

        var cnn_layer_1 = new ConvolutionLayer(10, 3, conv_0.Depth, 1);
        var conv_1 = cnn_layer_1.Execute(conv_0);
    }

    //[TestMethod]
    //public void SwitchSpeedTest()
    //{
    //    const int iterations = 100_000_000;

    //    Stopwatch sw = Stopwatch.StartNew();
    //    Activation activation = Activation.TanH;
    //    for (int i = 0; i < iterations; i++)
    //    {
    //        (2.0).Activate(activation);
    //    }
    //    sw.Stop();
    //    TimeSpan result_1 = sw.Elapsed;

    //    sw.Restart();
    //    for (int i = 0; i < iterations; i++)
    //    {
    //        Activations.TanH(2.0);
    //    }
    //    sw.Stop();
    //    TimeSpan result_2 = sw.Elapsed;

    //    var percentage = result_1 / result_2;

    //    for (int i = 200; i >= 100; i--)
    //    {
    //        Assert.IsTrue(percentage <= (i / 100.0), $"{i}%");
    //    }
    //}
}