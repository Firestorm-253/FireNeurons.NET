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
                    LayerIndex index,
                    Activation activation,
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
    public void Add(int neurons,
                    LayerIndex index,
                    Activation activation)
    {
        var layer = new Layer(index, neurons, activation);

        if (this.Layers.Contains(layer))
        {
            throw new Exception("ERROR: LayerIndex must be unique!");
        }
        this.Layers.Add(layer);
    }

    public void Randomize(bool withBias = true)
    {
        foreach (var layer in this.Layers)
        {
            layer.Randomize(withBias);
        }
    }

    private void Feed((LayerIndex, double[])[] data)
    {
        foreach (var ent in data)
        {
            var layer = this.Get(ent.Item1);

            for (int n = 0; n < layer.Neurons.Count; n++)
            {
                layer.Neurons[n].Feed(ent.Item2[n]);
            }
        }
    }

    public (LayerIndex, double[])[] Evaluate((LayerIndex, double[])[] data, params LayerIndex[] outputLayers)
    {
        this.Feed(data);

        foreach (var layer in this.Layers)
        {
            layer.Calculate();
        }

        var outputs = new (LayerIndex, double[])[outputLayers.Length];
        for (int l = 0; l < outputLayers.Length; l++)
        {
            var outputLayer = this.Get(outputLayers[l]);
            var outputData = new double[outputLayer.Neurons.Count];

            for (int n = 0; n < outputLayer.Neurons.Count; n++)
            {
                outputData[n] = outputLayer.Neurons[n].Value;
            }

            outputs[l] = (outputLayers[l], outputData);
        }

        return outputs;
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