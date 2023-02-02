using NeuralNetwork.NET.Indexes;
using NeuralNetwork.NET.Objects;

namespace NeuralNetwork.NET.Dto;

public record ConnectionDto
{
    public double Weight { get; init; }
    public NeuronIndex InputNeuron { get; init; }
    public NeuronIndex OutputNeuron { get; init; }

    public ConnectionDto() { }
    public ConnectionDto(Connection connection)
    {
        this.Weight = connection.Weight;
        this.InputNeuron = connection.InputNeuron.NeuronIndex;
        this.OutputNeuron = connection.OutputNeuron.NeuronIndex;
    }
}
