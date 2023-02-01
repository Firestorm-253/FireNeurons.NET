using NeuralNetwork.NET.Indexes;

namespace NeuralNetwork.NET.Dto;

public record NeuronDto
{
    public NeuronIndex NeuronIndex { get; init; }
    public Activation Activation { get; init; }
    public List<ConnectionDto> Connections { get; init; } = new();

    public NeuronDto() { }
    public NeuronDto(Neuron neuron)
    {
        this.NeuronIndex = neuron.NeuronIndex;
        this.Activation = neuron.Activation;

        foreach (var connection in neuron.Connections)
        {
            this.Connections.Add(new ConnectionDto(connection));
        }
    }
}
