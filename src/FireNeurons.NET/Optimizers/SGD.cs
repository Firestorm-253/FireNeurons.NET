using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Optimizers;

public class SGD : IOptimizer
{
    public override IOptimizerData DataInstance => new IOptimizerData();

    public SGD(Func<Neuron, double, double> lossDerivative, double learningRate) : base(lossDerivative, learningRate)
    { }

    public override void CalculateDelta(IOptimizerData optimizerData)
    {
        optimizerData.Delta = this.LearningRate * optimizerData.Gradient;
    }
}
