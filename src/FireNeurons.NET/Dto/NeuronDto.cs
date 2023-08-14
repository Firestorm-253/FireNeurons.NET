using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Dto;

public record NeuronDto
{
    public NeuronIndex NeuronIndex { get; init; }
    public Options Options { get; init; }
    public List<ConnectionDto> Connections { get; init; } = new();

    public NeuronDto() { }
    public NeuronDto(Neuron neuron)
    {
        this.NeuronIndex = neuron.NeuronIndex;
        this.Options = neuron.Options;

        foreach (var connection in neuron.Connections)
        {
            this.Connections.Add(new ConnectionDto(connection));
        }
    }
}
