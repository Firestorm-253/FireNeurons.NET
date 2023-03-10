using FireNeurons.NET.Dto;
using FireNeurons.NET.Optimizers;

namespace FireNeurons.NET.Objects;

public class Connection
{
    public Neuron InputNeuron { get; init; }
    public Neuron OutputNeuron { get; init; }

    public IOptimizer Optimizer { get; init; }

    public double Weight { get; set; }
    public IOptimizerData OptimizerData { get; set; } = null!; // for Weight

    public Connection(Neuron inputNeuron, Neuron outputNeuron, IOptimizer optimizer)
    {
        this.OutputNeuron = outputNeuron;
        this.InputNeuron = inputNeuron;
        this.Optimizer = optimizer;

        this.OptimizerData = this.Optimizer.DataInstance;
        this.InputNeuron.OutgoingConnections.Add(this);
    }
    /// <summary>Deserialization</summary>
    public Connection(ConnectionDto connectionDto, Neuron outputNeuron, NeuralNetwork network)
    {
        this.Weight = connectionDto.Weight;
        this.OutputNeuron = outputNeuron;
        this.InputNeuron = network.Get(connectionDto.InputNeuron);
        this.Optimizer = network.Optimizer;

        this.OptimizerData = this.Optimizer.DataInstance;
        this.InputNeuron.OutgoingConnections.Add(this);
    }

    public void Randomize(Activation activation)
    {
        this.Weight = GetRandom(activation, this.OutputNeuron.Connections.Count, this.OutputNeuron.Layer.Neurons.Count);
    }

    public double GetValue()
    {
        return this.InputNeuron.Value * this.Weight;
    }
}
