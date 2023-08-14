using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET.Dto;

public record LayerDto
{
    public LayerIndex LayerIndex { get; init; }
    public Options Options { get; init; }
    public List<NeuronDto> Neurons { get; init; } = new();

    public LayerDto() { }
    public LayerDto(Layer layer)
    {
        this.LayerIndex = layer.LayerIndex;
        this.Options = layer.Options;

        foreach (var neuron in layer.Neurons)
        {
            this.Neurons.Add(new NeuronDto(neuron));
        }
    }
}
