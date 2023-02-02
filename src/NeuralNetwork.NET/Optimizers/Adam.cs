using NeuralNetwork.NET.Objects;
using System.Runtime.CompilerServices;

namespace NeuralNetwork.NET.Optimizers;

public class Adam : IOptimizer
{
    public override IOptimizerData DataInstance => new AdamData();

    public double Beta_1 { get; init; }
    public double Beta_2 { get; init; }

    public Adam(double learningRate = 0.001, double beta_1 = 0.90, double beta_2 = 0.99) : base(learningRate)
    {
        this.Beta_1 = beta_1;
        this.Beta_2 = beta_2;
    }

    public void Calculate(IOptimizerData optimizerData)
    {
        if (optimizerData is not AdamData adamData)
        {
            throw new Exception("ERROR: Wrong optimizerData-type!");
        }

        adamData.Momentum = adamData.Momentum.Discount(adamData.Gradient, this.Beta_1);
        adamData.MomentumSquared = adamData.MomentumSquared.Discount(Math.Pow(adamData.Gradient, 2), this.Beta_2);

        var momentum_fixed = adamData.Momentum.FixBias(this.Beta_1, adamData.TimeStep);
        var momentumSquared_fixed = adamData.MomentumSquared.FixBias(this.Beta_2, adamData.TimeStep);

        var rawDelta = (momentum_fixed / (Math.Sqrt(momentumSquared_fixed) + Math.Pow(10, -6)));
        adamData.Delta = -1 * this.LearningRate * rawDelta;
    }
}

public record AdamData : IOptimizerData
{
    public int TimeStep { get; set; } = 1; // Starts at 1!!!

    public double Momentum { get; set; }
    public double MomentumSquared { get; set; }
}