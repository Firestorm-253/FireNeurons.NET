using FireNeurons.NET.Indexes;

namespace FireNeurons.NET.Objects;

public record Data
{
    public (LayerIndex, double[])[] Layers { get; init; }

    public Data(params (LayerIndex, double[])[] layers)
    {
        this.Layers = layers;
    }
}