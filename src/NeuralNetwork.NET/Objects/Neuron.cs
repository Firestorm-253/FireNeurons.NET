using NeuralNetwork.NET.Dto;
using NeuralNetwork.NET.Indexes;
using NeuralNetwork.NET.Optimizers;

namespace NeuralNetwork.NET.Objects;

public class Neuron
{
    public NeuronIndex NeuronIndex { get; init; }
    public Activation Activation { get; init; }
    public Layer Layer { get; init; }
    public List<Connection> Connections { get; init; } = new();
    public List<Connection> OutgoingConnections { get; init; } = new();

    public IOptimizer Optimizer { get; init; }

    public double Bias { get; set; }

    public IOptimizerData OptimizerData { get; set; } = null!; // for Neuron & Bias
    public double Blank { get; set; }

    public bool CalculationNeeded { get; set; } = true;
    private double _value;
    public double Value
    {
        get => this.CalculationNeeded ? this.CalculateValue() : this._value;
        set => this._value = value;
    }

    public bool IsWorking => this.Connections.Count != 0;

    public Neuron(NeuronIndex neuronIndex,
                  Activation activation,
                  Layer layer,
                  IOptimizer optimizer)
    {
        this.NeuronIndex = neuronIndex;
        this.Activation = activation;
        this.Layer = layer;
        this.Optimizer = optimizer;

        this.OptimizerData = this.Optimizer.DataInstance;
    }
    /// <summary>Deserialization</summary>
    public Neuron(NeuronDto neuronDto, Layer layer, NeuralNetwork network)
    {
        this.NeuronIndex = neuronDto.NeuronIndex;
        this.Activation = neuronDto.Activation;
        this.Layer = layer;
        this.Optimizer = network.Optimizer;

        foreach (var connectionDto in neuronDto.Connections)
        {
            this.Connections.Add(new Connection(connectionDto, this, network));
        }

        this.OptimizerData = this.Optimizer.DataInstance;
    }

    public void Connect(Neuron input)
    {
        this.Connections.Add(new Connection(input, this, this.Optimizer));
    }

    public void Randomize(bool withBias)
    {
        foreach (var connection in this.Connections)
        {
            connection.Randomize(this.Activation);
        }

        if (withBias)
        {
            this.Bias = GetRandom(this.Activation, this.Connections.Count, this.Layer.Neurons.Count);
        }
    }

    public double CalculateValue()
    {
        this.CalculationNeeded = false;

        double sum = this.Bias;
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

        this.CalculationNeeded = false;

        this.Set(blank);
    }

    public void Invalidate()
    {
        this.Blank = 0;
        this.Value = 0;
        this.CalculationNeeded = true;
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
