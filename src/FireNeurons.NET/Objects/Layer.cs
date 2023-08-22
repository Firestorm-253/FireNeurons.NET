using FireNeurons.NET.Dto;
using FireNeurons.NET.Indexes;

namespace FireNeurons.NET.Objects;

public class Layer
{
    public Options Options { get; init; }
    public LayerIndex Index { get; init; }
    public List<Neuron> Neurons { get; init; } = new();


    public Layer(LayerIndex index, int neurons, Options options)
    {
        this.Index = index;
        this.Options = options;

        this.AddNeurons(neurons);
    }
    /// <summary>Deserialization</summary>
    public Layer(LayerDto layerDto, NeuralNetwork network)
    {
        this.Index = layerDto.Index;
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
            this.Neurons.Add(new Neuron((n, this.Index), this.Options, this));
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
            dict.Add(neuron.Index, neuron.GetVisualization());
        }
        return dict;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Layer layer)
        {
            return false;
        }

        return this.Index.Equals(layer.Index);
    }
    public override int GetHashCode()
    {
        return this.Index.GetHashCode();
    }
}
