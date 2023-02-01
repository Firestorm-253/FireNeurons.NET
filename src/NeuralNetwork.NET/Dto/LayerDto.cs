using NeuralNetwork.NET.Indexes;

namespace NeuralNetwork.NET.Dto;

public record LayerDto
{
    public LayerIndex LayerIndex { get; init; }
    public Activation Activation { get; init; }
    public List<NeuronDto> Neurons { get; init; } = new();

    public LayerDto(Layer layer)
    {
        this.LayerIndex = layer.LayerIndex;
        this.Activation = layer.Activation;

        foreach (var neuron in layer.Neurons)
        {
            this.Neurons.Add(new NeuronDto(neuron));
        }
    }
}
