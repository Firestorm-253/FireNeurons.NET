using FireNeurons.NET.Dto;
using FireNeurons.NET.Indexes;
using FireNeurons.NET.Optimizers;

namespace FireNeurons.NET.Objects;

public class Layer
{
    public Activation Activation { get; init; }
    public LayerIndex LayerIndex { get; init; }
    public List<Neuron> Neurons { get; init; } = new();
    public IOptimizer Optimizer { get; init; } = null!;


    public Layer(LayerIndex layerIndex, int neurons, Activation activation, IOptimizer optimizer)
    {
        this.LayerIndex = layerIndex;
        this.Activation = activation;
        this.Optimizer = optimizer;

        this.AddNeurons(neurons);
    }
    /// <summary>Deserialization</summary>
    public Layer(LayerDto layerDto, NeuralNetwork network)
    {
        this.LayerIndex = layerDto.LayerIndex;
        this.Activation = layerDto.Activation;
        this.Optimizer = network.Optimizer;

        foreach (var neuronDto in layerDto.Neurons)
        {
            this.Neurons.Add(new Neuron(neuronDto, this, network));
        }
    }

    private void AddNeurons(int amount)
    {
        for (int n = 0; n < amount; n++)
        {
            this.Neurons.Add(new Neuron((n, this.LayerIndex), this.Activation, this, this.Optimizer));
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

    public void Randomize(bool withBias)
    {
        foreach (var neuron in this.Neurons)
        {
            neuron.Randomize(withBias);
        }
    }

    public void Calculate()
    {
        foreach (var neuron in this.Neurons)
        {
            neuron.CalculateValue();
        }
    }

    public void Invalidate()
    {
        foreach (var neuron in this.Neurons)
        {
            neuron.Invalidate();
        }
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
