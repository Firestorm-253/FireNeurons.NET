using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;

namespace FireNeurons.NET;

public partial class NeuralNetwork
{
    public Dictionary<LayerIndex, Layer> Layers { get; init; }

    public NeuralNetwork()
    {
        this.Layers = new Dictionary<LayerIndex, Layer>();
    }

    public void Add(int neurons,
                    LayerIndex index,
                    Options options,
                    params LayerIndex[] inputs)
    {
        if (this.Layers.ContainsKey(index))
        {
            throw new Exception("ERROR: LayerIndex must be unique!");
        }

        var layer = new Layer(index, neurons, options);
        
        foreach (var inputLayerIndex in inputs)
        {
            var inputLayer = this.Layers[inputLayerIndex];
            layer.Connect(inputLayer);
        }
        
        this.Layers.Add(index, layer);
    }
    public void Add(int neurons,
                    LayerIndex index,
                    Activation activation,
                    params LayerIndex[] inputs)
    {
        this.Add(neurons, index, new Options() { Activation = activation }, inputs);
    }

    public void Randomize()
    {
        foreach (var (_, layer) in this.Layers)
        {
            layer.Randomize();
        }
    }

    private void Feed(Data data, bool isTraining)
    {
        foreach (var (layerIndex, values) in data.DataLayers)
        {
            var layer = this.Layers[layerIndex];

            for (int n = 0; n < layer.Neurons.Count; n++)
            {
                layer.Neurons[n].Feed(values[n], isTraining);
            }
        }
    }

    public Data Evaluate(Data data, bool isTraining = false, params LayerIndex[] outputLayers)
    {
        foreach (var (_, layer) in this.Layers)
        {
            layer.Invalidate();
        }

        this.Feed(data, isTraining);

        foreach (var (_, layer) in this.Layers)
        {
            layer.Calculate(isTraining);
        }

        var outputs = new KeyValuePair<LayerIndex, double[]>[outputLayers.Length];
        for (int l = 0; l < outputLayers.Length; l++)
        {
            var outputLayer = this.Layers[outputLayers[l]];
            var outputData = new double[outputLayer.Neurons.Count];

            for (int n = 0; n < outputLayer.Neurons.Count; n++)
            {
                outputData[n] = outputLayer.Neurons[n].GetValue(isTraining);
            }

            outputs[l] = new(outputLayers[l], outputData);
        }

        return new(outputs);
    }

    public Neuron Get(NeuronIndex neuronIndex)
    {
        var layer = this.Layers[neuronIndex.LayerIndex];
        return layer.Neurons.First(x => x.Index.Equals(neuronIndex));
    }

    public Dictionary<LayerIndex, Dictionary<NeuronIndex, double[]>> GetVisualization()
    {
        var dict = new Dictionary<LayerIndex, Dictionary<NeuronIndex, double[]>>();
        foreach (var (_, layer) in this.Layers)
        {
            if (!layer.Neurons.Any(x => x.Connections.Any()))
            {
                continue;
            }
            dict.Add(layer.Index, layer.GetVisualization());
        }
        return dict;
    }
    public Dictionary<LayerIndex, Dictionary<NeuronIndex, double[]>> GetVisualization(double clipMin, double clipMax)
    {
        var dict = new Dictionary<LayerIndex, Dictionary<NeuronIndex, double[]>>();
        foreach (var (_, layer) in this.Layers)
        {
            if (!layer.Neurons.Any(x => x.Connections.Any()))
            {
                continue;
            }
            dict.Add(layer.Index, layer.GetVisualization(clipMin, clipMax));
        }
        return dict;
    }
    
    public override bool Equals(object? obj)
    {
#pragma warning disable IDE0011 // Geschweifte Klammern hinzufügen
        if (obj is not NeuralNetwork check) return false;
        if (this.Layers.Count != check.Layers.Count) return false;

        for (int l = 0; l < this.Layers.Count; l++)
        {
            var (_, layer) = this.Layers.ElementAt(l);
            var (_, checkLayer) = check.Layers.ElementAt(l);
            if (!layer.Equals(checkLayer)) return false;
            if (layer.Neurons.Count != checkLayer.Neurons.Count) return false;

            for (int n = 0; n < layer.Neurons.Count; n++)
            {
                var neuron = layer.Neurons[n];
                var checkNeuron = checkLayer.Neurons[n];
                if (!neuron.Equals(checkNeuron)) return false;
                if (neuron.Options.Activation != checkNeuron.Options.Activation) return false;
                if (neuron.Connections.Count != checkNeuron.Connections.Count) return false;

                for (int c = 0; c < neuron.Connections.Count; c++)
                {
                    var connection = neuron.Connections[c];
                    var checkConnection = checkNeuron.Connections[c];
                    if (!connection.InputNeuron.Equals(checkConnection.InputNeuron)) return false;
                    if (connection.Weight != checkConnection.Weight) return false;
                }
            }
        }

        return true;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}