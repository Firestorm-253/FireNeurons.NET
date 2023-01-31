using NeuralNetwork.NET.Indexes;

namespace NeuralNetwork.NET;

public class Neuron
{
    public NeuronIndex NeuronIndex { get; init; }
    public Activation Activation { get; init; }

    public double Value { get; set; }
    public double Blank { get; set; }
    
    public bool IsWorking => (this.Connections.Count != 0);
    public Neuron(NeuronIndex neuronIndex, Activation activation)
    {
        this.NeuronIndex = neuronIndex;
        this.Activation = activation;
    }

    public void CalculateValue()
    {
        double sum = 0;

        // ToDo: sum all connections

        this.Set(sum);
    }

    private void Set(double blank)
    {
        this.Blank = blank;
        this.Value = this.Blank.Activate(this.Activation);
    }
    public void Feed(double blank)
    {
        if (this.IsWorking)
        {
            throw new Exception("ERROR: Can't feed a working neuron!");
        }

        this.Set(blank);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Neuron neuron)
        {
            return false;
        }

        return this.NeuronIndex.Equals(neuron.NeuronIndex);
    }
}
