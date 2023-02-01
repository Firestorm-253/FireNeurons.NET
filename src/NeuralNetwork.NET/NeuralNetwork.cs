using NeuralNetwork.NET.Indexes;

namespace NeuralNetwork.NET;

public class NeuralNetwork
{
    private HashSet<Layer> Layers { get; init; } = null!;

    public NeuralNetwork()
    {
        this.Layers = new HashSet<Layer>(new LayerEqualityComparer());
    }

    public void Add(int neurons,
                    Activation activation,
                    LayerIndex index,
                    params LayerIndex[] inputs)
    {
        var layer = new Layer(index, neurons, activation);

        foreach (var inputLayerIndex in inputs)
        {
            var inputLayer = this.Get(inputLayerIndex);
            layer.Connect(inputLayer);
        }
        
        if (this.Layers.Contains(layer))
        {
            throw new Exception("ERROR: LayerIndex must be unique!");
        }
        this.Layers.Add(layer);
    }

    public Layer Get(LayerIndex layerIndex)
    {
        return this.Layers.First(x => x.LayerIndex.Equals(layerIndex));
    }
    public Neuron Get(NeuronIndex neuronIndex)
    {
        var layer = this.Get(neuronIndex.LayerIndex);
        return layer.Neurons.First(x => x.NeuronIndex.Equals(neuronIndex));
    }
}