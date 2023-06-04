using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Optimisation.Optimisers;

public class SGD : IOptimiser
{
    public override IOptimiserData DataInstance => new IOptimiserData();

    public SGD(Func<Neuron, double, double> lossDerivative, double learningRate) : base(lossDerivative, learningRate)
    { }

    public override void CalculateDelta(IOptimiserData optimiserData)
    {
        optimiserData.Delta = this.LearningRate * optimiserData.Gradient;
    }
}
