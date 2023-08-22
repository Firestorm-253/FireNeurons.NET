namespace FireNeurons.NET.Objects;
using Dto;
using Indexes;

public class Connection
{
    public ConnectionIndex Index { get; init; }

    public Neuron InputNeuron { get; init; }
    public Neuron OutputNeuron { get; init; }

    public double Weight { get; set; }

    public Connection(ConnectionIndex index,
                      Neuron inputNeuron,
                      Neuron outputNeuron)
    {
        this.Index = index;
        this.OutputNeuron = outputNeuron;
        this.InputNeuron = inputNeuron;

        this.InputNeuron.OutgoingConnections.Add(this);
    }
    /// <summary>Deserialization</summary>
    public Connection(ConnectionDto connectionDto, Neuron outputNeuron, NeuralNetwork network)
    {
        this.Index = connectionDto.Index;
        this.Weight = connectionDto.Weight;
        this.OutputNeuron = outputNeuron;
        this.InputNeuron = network.Get(connectionDto.InputNeuron);

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
