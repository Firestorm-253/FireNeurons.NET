using NeuralNetwork.NET.Optimizers;

namespace NeuralNetwork.NET.Tests;

[TestClass]
public class OptimizerTests
{
    [TestMethod]
    public void AdamTest()
    {
        var optimizer = new Adam();

        var adamData = new AdamData() { Momentum = 0, MomentumSquared = 0, TimeStep = 1, Gradient = +0.6 };
        optimizer.Calculate(adamData);

        Assert.AreNotEqual(0, adamData.Delta);
    }
}