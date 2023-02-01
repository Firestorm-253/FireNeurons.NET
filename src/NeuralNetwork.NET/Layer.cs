

using NeuralNetwork.NET.Indexes;

namespace NeuralNetwork.NET;

public class Layer
{
    public Activation Activation { get; init; }
    public LayerIndex LayerIndex { get; init; }
    public List<Neuron> Neurons { get; init; } = new();

    public Layer(LayerIndex layerIndex, int neurons, Activation activation)
    {
        this.LayerIndex = layerIndex;
        this.Activation = activation;
        
        this.AddNeurons(neurons);
    }

    private void AddNeurons(int amount)
    {
        for (int n = 0; n < amount; n++)
        {
            this.Neurons.Add(new Neuron((n, this.LayerIndex), this.Activation, this));
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

    public void Calculate()
    {
        foreach (var neuron in this.Neurons)
        {
            neuron.CalculateValue();
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
