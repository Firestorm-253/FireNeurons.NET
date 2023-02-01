using NeuralNetwork.NET.Indexes;

namespace NeuralNetwork.NET;

public class Neuron
{
    public NeuronIndex NeuronIndex { get; init; }
    public Activation Activation { get; init; }
    public Layer Layer { get; init; }
    public List<Connection> Connections { get; init; } = new();

    public double Blank { get; set; }

    public bool CalculationNeeded { get; set; }
    private double _value;
    public double Value
    {
        get => this.CalculationNeeded ? this.CalculateValue() : this._value;
        set => this._value = value;
    }

    public bool IsWorking => (this.Connections.Count != 0);

    public Neuron(NeuronIndex neuronIndex,
                  Activation activation,
                  Layer layer)
    {
        this.NeuronIndex = neuronIndex;
        this.Activation = activation;
        this.Layer = layer;
    }

    public void Connect(Neuron input)
    {
        this.Connections.Add(new Connection(input, this));
    }

    public double CalculateValue()
    {
        this.CalculationNeeded = false;

        double sum = 0;
        foreach (var connection in this.Connections)
        {
            sum += connection.GetValue();
        }

        return this.Set(sum);
    }

    private double Set(double blank)
    {
        this.Blank = blank;
        return this.Value = this.Blank.Activate(this.Activation);
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
    public override int GetHashCode()
    {
        return this.NeuronIndex.GetHashCode();
    }
}
