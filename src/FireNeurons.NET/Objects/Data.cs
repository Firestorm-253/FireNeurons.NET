using FireNeurons.NET.Indexes;

namespace FireNeurons.NET.Objects;

public record Data
{
    public Dictionary<LayerIndex, double[]> DataLayers { get; init; } = new();

    public Data() { }
    public Data(params KeyValuePair<LayerIndex, double[]>[] dataLayers)
    {
        this.DataLayers = new Dictionary<LayerIndex, double[]>(dataLayers);
    }

    public Data Add(LayerIndex layerIndex, double[] values)
    {
        if (this.DataLayers.ContainsKey(layerIndex))
        {
            throw new ArgumentException("ERROR: DataLayer already exists!");
        }

        this.DataLayers.Add(layerIndex, values);
        return this;
    }

    public double[] this[LayerIndex layerIndex] => this.DataLayers[layerIndex];

    public static implicit operator Dictionary<LayerIndex, double[]>(Data data)
        => data.DataLayers;
}