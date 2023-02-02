using NeuralNetwork.NET.Dto;
using NeuralNetwork.NET.Optimizers;

namespace NeuralNetwork.NET.Objects;

public class Connection
{
    public Neuron InputNeuron { get; init; }
    public Neuron OutputNeuron { get; init; }

    public double Weight { get; set; }
    public IOptimizerData OptimizerData { get; set; } = null!; // for Weight

    public Connection(Neuron inputNeuron, Neuron outputNeuron)
    {
        this.InputNeuron = inputNeuron;
        this.OutputNeuron = outputNeuron;
    }
    /// <summary>Deserialization</summary>
    public Connection(ConnectionDto connectionDto, Neuron outputNeuron, NeuralNetwork network)
    {
        this.Weight = connectionDto.Weight;
        this.InputNeuron = network.Get(connectionDto.InputNeuron);
        this.OutputNeuron = outputNeuron;
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
