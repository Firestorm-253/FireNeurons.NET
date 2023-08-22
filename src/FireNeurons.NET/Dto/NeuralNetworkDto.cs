namespace FireNeurons.NET.Dto;

public record NeuralNetworkDto
{
    public List<LayerDto> Layers { get; init; } = new();

    public NeuralNetworkDto() { }
    public NeuralNetworkDto(NeuralNetwork network)
    {
        foreach (var (_, layer) in network.Layers)
        {
            this.Layers.Add(new LayerDto(layer));
        }
    }
}
