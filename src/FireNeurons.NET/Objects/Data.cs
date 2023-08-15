using FireNeurons.NET.Indexes;

namespace FireNeurons.NET.Objects;

public record Data : Data<double[]>
{
    public Data() : base()
    { }
    public Data(params KeyValuePair<LayerIndex, double[]>[] dataLayers)
        : base(dataLayers)
    { }

    public override Data Add(LayerIndex layerIndex, params double[] values)
    {
        if (this.DataLayers.ContainsKey(layerIndex))
        {
            throw new ArgumentException("ERROR: DataLayer already exists!");
        }

        this.DataLayers.Add(layerIndex, values);
        return this;
    }
}
public record Data<T>
{
    public Dictionary<LayerIndex, T> DataLayers { get; init; } = new();

    public Data()
    { }
    public Data(params KeyValuePair<LayerIndex, T>[] dataLayers)
    {
        this.DataLayers = new Dictionary<LayerIndex, T>(dataLayers);
    }

    public virtual Data<T> Add(LayerIndex layerIndex, T value)
    {
        if (this.DataLayers.ContainsKey(layerIndex))
        {
            throw new ArgumentException("ERROR: DataLayer already exists!");
        }

        this.DataLayers.Add(layerIndex, value);
        return this;
    }

    public T this[LayerIndex layerIndex] => this.DataLayers[layerIndex];

    public static implicit operator Dictionary<LayerIndex, T>(Data<T> data)
        => data.DataLayers;

    //public static implicit operator Data<T>(Data data) => new(data);
    //public static implicit operator Data(Data<T> data) => new Data(data);
}