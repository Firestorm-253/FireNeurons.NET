namespace NeuralNetwork.NET.Dto;

public record NeuralNetworkDto
{
    public List<LayerDto> Layers { get; init; } = new();

    public NeuralNetworkDto(NeuralNetwork network)
    {
        foreach (var layer in network.Layers)
        {
            this.Layers.Add(new LayerDto(layer));
        }
    }
}
