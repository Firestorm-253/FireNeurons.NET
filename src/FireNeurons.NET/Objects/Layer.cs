using FireNeurons.NET.Dto;
using FireNeurons.NET.Indexes;

namespace FireNeurons.NET.Objects;

public class Layer
{
    public Options Options { get; init; }
    public LayerIndex LayerIndex { get; init; }
    public List<Neuron> Neurons { get; init; } = new();


    public Layer(LayerIndex layerIndex, int neurons, Options options)
    {
        this.LayerIndex = layerIndex;
        this.Options = options;

        this.AddNeurons(neurons);
    }
    /// <summary>Deserialization</summary>
    public Layer(LayerDto layerDto, NeuralNetwork network)
    {
        this.LayerIndex = layerDto.LayerIndex;
        this.Options = layerDto.Options;

        foreach (var neuronDto in layerDto.Neurons)
        {
            this.Neurons.Add(new Neuron(neuronDto, this, network));
        }
    }

    private void AddNeurons(int amount)
    {
        for (int n = 0; n < amount; n++)
        {
            this.Neurons.Add(new Neuron((n, this.LayerIndex), this.Options, this));
        }
    }

    public void Connect(Layer inputLayer)
    {
        foreach (var neuron in this.Neurons)
        {
            foreach (var inputNeuron in inputLayer.Neurons)
            {
                neuron.Connect(inputNeuron);
            }
        }
    }

    public void Randomize()
    {
        foreach (var neuron in this.Neurons)
        {
            neuron.Randomize();
        }
    }

    public void Calculate(bool isTraining)
    {
        foreach (var neuron in this.Neurons)
        {
            neuron.GetValue(isTraining);
        }
    }

    public void Invalidate()
    {
        foreach (var neuron in this.Neurons)
        {
            neuron.Invalidate();
        }
    }

    public Dictionary<NeuronIndex, double[]> GetVisualization()
    {
        var dict = new Dictionary<NeuronIndex, double[]>();
        foreach (var neuron in this.Neurons)
        {
            dict.Add(neuron.NeuronIndex, neuron.GetVisualization());
        }
        return dict;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Layer layer)
        {
            return false;
        }

        return this.LayerIndex.Equals(layer.LayerIndex);
    }
    public override int GetHashCode()
    {
        return this.LayerIndex.GetHashCode();
    }
}
