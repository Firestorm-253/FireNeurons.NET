using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Dto;

public record LayerDto
{
    public LayerIndex Index { get; init; }
    public Options Options { get; init; }
    public List<NeuronDto> Neurons { get; init; } = new();

    public LayerDto() { }
    public LayerDto(Layer layer)
    {
        this.Index = layer.Index;
        this.Options = layer.Options;

        foreach (var neuron in layer.Neurons)
        {
            this.Neurons.Add(new NeuronDto(neuron));
        }
    }
}
