using FireNeurons.NET.Dto;
using FireNeurons.NET.Indexes;

namespace FireNeurons.NET.Objects;

public class Neuron
{
    public NeuronIndex Index { get; init; }
    public Options Options { get; init; }
    public Layer Layer { get; init; }
    public List<Connection> Connections { get; init; } = new();
    public List<Connection> OutgoingConnections { get; init; } = new();

    public double Bias { get; set; }

    public double Blank { get; set; }

    public bool CalculationNeeded { get; set; } = true;
    private double _value;
    //public double Value
    //{
    //    get => this.CalculationNeeded && this.IsWorking ? this.CalculateValue() : this._value;
    //    set => this._value = value;
    //}

    public bool IsWorking => (this.Connections.Count != 0) && !this.DroppedOut;
    public bool DroppedOut { get; private set; } = false;

    public Neuron(NeuronIndex index,
                  Options options,
                  Layer layer)
    {
        this.Index = index;
        this.Options = options;
        this.Layer = layer;
    }
    /// <summary>Deserialization</summary>
    public Neuron(NeuronDto neuronDto, Layer layer, NeuralNetwork network)
    {
        this.Index = neuronDto.Index;
        this.Options = neuronDto.Options;
        this.Bias = neuronDto.Bias;
        this.Layer = layer;

        foreach (var connectionDto in neuronDto.Connections)
        {
            this.Connections.Add(new Connection(connectionDto, this, network));
        }
    }

    public void Connect(Neuron input)
    {
        this.Connections.Add(new Connection(new ConnectionIndex(this.Connections.Count, this.Index), input, this));
    }

    public void Randomize()
    {
        foreach (var connection in this.Connections)
        {
            connection.Randomize(this.Options.Activation);
        }

        if (this.Options.UseBias && this.Connections.Count > 0)
        {
            this.Bias = GetRandom(this.Options.Activation, this.Connections.Count, this.Layer.Neurons.Count);
        }
    }

    public double GetValue(bool isTraining)
    {
        if (!this.CalculationNeeded)
        {
            return this._value;
        }
        this.CalculationNeeded = false;

        if (isTraining)
        {
            this.DroppedOut = (GlobalRandom.NextDouble() < this.Options.Dropout);
            if (this.DroppedOut)
            {
                return this._value = 0;
            }
        }

        if (!this.IsWorking)
        {
            return this._value;
        }

        double sum = this.Bias;
        foreach (var connection in this.Connections)
        {
            var value = connection.GetValue(isTraining);
            if (!isTraining)
            {
                value *= (1 - connection.InputNeuron.Options.Dropout);
            }
            sum += value;
        }

        return this.Set(sum);
    }

    private double Set(double blank)
    {
        this.Blank = blank;
        return this._value = this.Blank.Activate(this.Options.Activation);
    }

    public void Feed(double blank, bool isTraining)
    {
        if (this.IsWorking)
        {
            throw new Exception("ERROR: Can't feed a working neuron!");
        }

        this.GetValue(isTraining);

        this.Set(blank);
    }

    public void Invalidate()
    {
        this.Blank = 0;
        this._value = 0;
        this.CalculationNeeded = true;
        this.DroppedOut = false;
    }

    public double[] GetVisualization()
    {
        var offset = -this.Connections.Min(x => x.Weight);
        var max = this.Connections.Max(x => x.Weight) + offset;

        return this.Connections.Select(x => (x.Weight + offset) / max).ToArray();
    }
    public double[] GetVisualization(double clipMin, double clipMax)
    {
        return this.Connections.Select(x => Math.Max(clipMin, Math.Min(clipMax, x.Weight))).ToArray();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Neuron neuron)
        {
            return false;
        }

        return this.Index.Equals(neuron.Index);
    }
    public override int GetHashCode()
    {
        return this.Index.GetHashCode();
    }
}
