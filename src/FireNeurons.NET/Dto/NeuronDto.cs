namespace FireNeurons.NET.Dto;
using Indexes;
using Objects;

public record NeuronDto
{
    public NeuronIndex Index { get; init; }
    public Options Options { get; init; }
    public List<ConnectionDto> Connections { get; init; } = new();

    public NeuronDto() { }
    public NeuronDto(Neuron neuron)
    {
        this.Index = neuron.Index;
        this.Options = neuron.Options;

        foreach (var connection in neuron.Connections)
        {
            this.Connections.Add(new ConnectionDto(connection));
        }
    }
}
