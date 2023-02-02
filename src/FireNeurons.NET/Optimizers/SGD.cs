namespace FireNeurons.NET.Optimizers;

public class SGD : IOptimizer
{
    public override IOptimizerData DataInstance => new IOptimizerData();

    public SGD(double learningRate) : base(learningRate)
    { }

    public override void CalculateDelta(IOptimizerData optimizerData)
    {
        optimizerData.Delta = this.LearningRate * optimizerData.Gradient;
    }
}
