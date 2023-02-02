namespace NeuralNetwork.NET.Optimizers;

public abstract class IOptimizer
{
    public double LearningRate { get; init; }

    public IOptimizer(double learningRate)
    {
        this.LearningRate = learningRate;
    }
}

public abstract record IOptimizerData
{
    public double Delta { get; set; }
    public double Gradient { get; set; }
}