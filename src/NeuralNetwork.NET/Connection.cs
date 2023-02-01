namespace NeuralNetwork.NET;

public class Connection
{
    public Neuron InputNeuron { get; init; }
    public Neuron OutputNeuron { get; init; }

    public double Weight { get; set; }

    public Connection(Neuron inputNeuron, Neuron outputNeuron)
    {
        this.InputNeuron = inputNeuron;
        this.OutputNeuron = outputNeuron;
    }

    public void Randomize(Activation activation)
    {
        this.Weight = activation switch
        {
            Activation.Identity => UniformRandom(-1, +1),
            Activation.Sigmoid => XavierRandom(this.InputNeuron.Layer.Neurons.Count, this.OutputNeuron.Layer.Neurons.Count),
            Activation.TanH => XavierRandom(this.InputNeuron.Layer.Neurons.Count, this.OutputNeuron.Layer.Neurons.Count),
            Activation.Relu => HeRandom(this.InputNeuron.Layer.Neurons.Count),
            Activation.LeakyRelu => HeRandom(this.InputNeuron.Layer.Neurons.Count),
            _ => throw new ArgumentException("ERROR: Invalid activation!"),
        };
    }

    public double GetValue()
    {
        return this.InputNeuron.Value * this.Weight;
    }
}
