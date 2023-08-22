namespace FireNeurons.NET.Objects;
using Dto;
using Optimisation;

public class Connection
{
    public Neuron InputNeuron { get; init; }
    public Neuron OutputNeuron { get; init; }

    public IOptimiser Optimiser { get; init; }

    public double Weight { get; set; }
    public IOptimiserData OptimiserData { get; set; } = null!; // for Weight

    public Connection(Neuron inputNeuron, Neuron outputNeuron, IOptimiser optimiser)
    {
        this.OutputNeuron = outputNeuron;
        this.InputNeuron = inputNeuron;
        this.Optimiser = optimiser;

        this.OptimiserData = this.Optimiser.DataInstance;
        this.InputNeuron.OutgoingConnections.Add(this);
    }
    /// <summary>Deserialization</summary>
    public Connection(ConnectionDto connectionDto, Neuron outputNeuron, NeuralNetwork network)
    {
        this.Weight = connectionDto.Weight;
        this.OutputNeuron = outputNeuron;
        this.InputNeuron = network.Get(connectionDto.InputNeuron);
        this.Optimiser = network.Optimiser;

        this.OptimiserData = this.Optimiser.DataInstance;
        this.InputNeuron.OutgoingConnections.Add(this);
    }

    public void Randomize(Activation activation)
    {
        this.Weight = GetRandom(activation, this.OutputNeuron.Connections.Count, this.OutputNeuron.Layer.Neurons.Count);
    }

    public double GetValue(bool isTraining)
    {
        return this.InputNeuron.GetValue(isTraining) * this.Weight;
    }
}
