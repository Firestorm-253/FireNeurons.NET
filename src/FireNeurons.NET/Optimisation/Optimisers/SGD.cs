namespace FireNeurons.NET.Optimisation.Optimisers;
using Objects;

public class SGD : IOptimiser
{
    public override IOptimiserData DataInstance => new IOptimiserData();

    public SGD(Func<Neuron, object?, object, double> lossDerivative, double learningRate)
        : base(lossDerivative, learningRate)
    { }

    public override void CalculateDelta(IOptimiserData optimiserData)
    {
        optimiserData.Delta = this.LearningRate * optimiserData.Gradient;
    }
}
