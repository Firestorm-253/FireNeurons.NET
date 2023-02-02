using FireNeurons.NET.Indexes;
using FireNeurons.NET.Objects;
using FireNeurons.NET.Optimizers;

namespace FireNeurons.NET;

public partial class NeuralNetwork
{
    public HashSet<Layer> Layers { get; init; }
    public IOptimizer Optimizer { get; init; }

    public NeuralNetwork(IOptimizer optimizer)
    {
        this.Layers = new HashSet<Layer>(new LayerEqualityComparer());
        this.Optimizer = optimizer;
    }

    public void Add(int neurons,
                    LayerIndex index,
                    Activation activation,
                    params LayerIndex[] inputs)
    {
        var layer = new Layer(index, neurons, activation, this.Optimizer);

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
        var layer = new Layer(index, neurons, activation, this.Optimizer);

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

    private void Feed(Data data)
    {
        foreach (var ent in data.Layers)
        {
            var layer = this.Get(ent.Item1);

            for (int n = 0; n < layer.Neurons.Count; n++)
            {
                layer.Neurons[n].Feed(ent.Item2[n]);
            }
        }
    }

    public Data Evaluate(Data data, params LayerIndex[] outputLayers)
    {
        this.Feed(data);

        foreach (var layer in this.Layers)
        {
            layer.Invalidate();
        }

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

        return new Data(outputs);
    }

    public void Train(List<(Data, Data)> dataTargetSet, int iterations)
    {
        for (int iteration = 0; iteration < iterations; iteration++)
        {
            //dataSet = dataSet.OrderBy(x => GlobalRandom.Next()).ToList(); // shuffle dataset
            // Find a way to shuffle dataSet together with targetSet!

            for (int d = 0; d < dataTargetSet.Count; d++)
            {
                var data = dataTargetSet[d].Item1;
                var target = dataTargetSet[d].Item2;

                var outputLayersIndexes = target.Layers.Select(x => x.Item1).ToArray();

                this.Evaluate(data, outputLayersIndexes);

                for (int ol = 0; ol < target.Layers.Length; ol++)
                {
                    var outputLayer = this.Get(outputLayersIndexes[ol]);
                    var layerTarget = target.Layers[ol];

                    for (int on = 0; on < outputLayer.Neurons.Count; on++)
                    {
                        var outputNeuron = outputLayer.Neurons[on];

                        this.Optimizer.CalculateGradient(outputNeuron, (layerTarget.Item2[on] - outputNeuron.Value));
                        this.Optimizer.CalculateDelta(outputNeuron.OptimizerData);
                    }
                }

                for (int l = this.Layers.Count - 1; l >= 0; l--)
                {
                    if (outputLayersIndexes.Contains(l))
                    {
                        continue;
                    }

                    foreach (var neuron in this.Layers.ElementAt(l).Neurons)
                    {
                        this.Optimizer.CalculateGradient(neuron);

                        this.Optimizer.CalculateDelta(neuron.OptimizerData);
                        neuron.Bias += neuron.OptimizerData.Delta;

                        foreach (var outgoingConnection in neuron.OutgoingConnections)
                        {
                            this.Optimizer.CalculateDelta(outgoingConnection.OptimizerData);
                            outgoingConnection.Weight += outgoingConnection.OptimizerData.Delta;
                        }
                    }
                }
            }
        }
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

    public override bool Equals(object? obj)
    {
#pragma warning disable IDE0011 // Geschweifte Klammern hinzufügen
        if (obj is not NeuralNetwork check) return false;
        if (this.Layers.Count != check.Layers.Count) return false;

        for (int l = 0; l < this.Layers.Count; l++)
        {
            var layer = this.Layers.ElementAt(l);
            var checkLayer = check.Layers.ElementAt(l);
            if (!layer.Equals(checkLayer)) return false;
            if (layer.Neurons.Count != checkLayer.Neurons.Count) return false;

            for (int n = 0; n < layer.Neurons.Count; n++)
            {
                var neuron = layer.Neurons[n];
                var checkNeuron = checkLayer.Neurons[n];
                if (!neuron.Equals(checkNeuron)) return false;
                if (neuron.Activation != checkNeuron.Activation) return false;
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