using NeuralNetwork.NET.Indexes;

namespace NeuralNetwork.NET;

public class Neuron
{
    public NeuronIndex NeuronIndex { get; init; }
    public Activation Activation { get; init; }

    public double Value { get; set; }
    public double Blank { get; set; }
    
    public Neuron(NeuronIndex neuronIndex, Activation activation)
    {
        this.NeuronIndex = neuronIndex;
        this.Activation = activation;
    }

    public void CalculateValue()
    {
        double sum = 0;

        // ToDo: sum all connections

        this.Feed(sum);
    }

    public void Feed(double blank)
    {
        this.Blank = blank;
        this.Value = this.Blank.Activate(this.Activation);
    }
}
