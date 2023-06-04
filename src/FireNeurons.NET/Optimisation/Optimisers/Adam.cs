using FireNeurons.NET.Objects;
using Math.NET;

namespace FireNeurons.NET.Optimisation.Optimisers;

public class Adam : SGD
{
    public override IOptimiserData DataInstance => new AdamData();

    public double Beta_1 { get; init; }
    public double Beta_2 { get; init; }

    public Adam(Func<Neuron, double, double> lossDerivative, double learningRate = 0.001, double beta_1 = 0.90, double beta_2 = 0.99) : base(lossDerivative, learningRate)
    {
        this.Beta_1 = beta_1;
        this.Beta_2 = beta_2;
    }

    public override void CalculateDelta(IOptimiserData optimiserData)
    {
        if (optimiserData is not AdamData adamData)
        {
            throw new Exception("ERROR: Wrong optimiserData-type!");
        }

        adamData.Momentum = adamData.Momentum.Discount(adamData.Gradient, this.Beta_1);
        adamData.MomentumSquared = adamData.MomentumSquared.Discount(adamData.Gradient.Pow(2), this.Beta_2);

        var momentum_fixed = adamData.Momentum.FixBias(this.Beta_1, adamData.TimeStep);
        var momentumSquared_fixed = adamData.MomentumSquared.FixBias(this.Beta_2, adamData.TimeStep);

        var rawDelta = momentum_fixed / (momentumSquared_fixed.Sqrt() + 10.0.Pow(-6));
        adamData.Delta = this.LearningRate * rawDelta;

        adamData.TimeStep++;
    }
}

public record AdamData : IOptimiserData
{
    public int TimeStep { get; set; } = 1; // Starts at 1!!!

    public double Momentum { get; set; }
    public double MomentumSquared { get; set; }
}